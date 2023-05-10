using System.Diagnostics;

namespace Plugin.Maui.Audio;

public partial class AudioRecorder : IAudioRecorder
{
	const string defaultFileName = "ARS_recording.wav";
	const float nearZero = .00000000001F;

	WaveRecorder recorder;

	IAudioStream audioStream;

	bool audioDetected;
	DateTime? silenceTime;
	DateTime? startTime;
	TaskCompletionSource<string> recordTask;

	public AudioStreamDetails AudioStreamDetails { get; private set; }

	public string FilePath { get; set; }

	public int PreferredSampleRate { get; set; } = 44100;

	public bool IsRecording => audioStream?.Active ?? false;

	public TimeSpan AudioSilenceTimeout { get; set; } = TimeSpan.FromSeconds(2);

	public TimeSpan TotalAudioTimeout { get; set; } = TimeSpan.FromSeconds(30);

	public bool StopRecordingOnSilence { get; set; } = true;

	public bool StopRecordingAfterTimeout { get; set; } = true;

	public float SilenceThreshold { get; set; } = .15f;

	public event EventHandler<string> AudioInputReceived;

	public async Task<Task<string>> StartRecording()
	{
		if (FilePath == null)
		{
			FilePath = await GetDefaultFilePath();
		}

		ResetAudioDetection();
		OnRecordingStarting();

		InitializeStream(PreferredSampleRate);

		await recorder.StartRecorder(audioStream, FilePath);

		AudioStreamDetails = new AudioStreamDetails
		{
			ChannelCount = audioStream.ChannelCount,
			SampleRate = audioStream.SampleRate,
			BitsPerSample = audioStream.BitsPerSample
		};

		startTime = DateTime.Now;
		recordTask = new TaskCompletionSource<string>();

		Debug.WriteLine("AudioRecorderService.StartRecording() complete.  Audio is being recorded.");

		return recordTask.Task;
	}

	public Stream GetAudioFileStream()
	{
		return recorder.GetAudioFileStream();
	}

	public void ResetAudioDetection()
	{
		audioDetected = false;
		silenceTime = null;
		startTime = null;
	}

	public void AudioStream_OnBroadcast(object sender, byte[] bytes)
	{
		var level = AudioFunctions.CalculateLevel(bytes);

		if (level < nearZero && !audioDetected) // discard any initial 0s so we don't jump the gun on timing out
		{
			Debug.WriteLine("level == {0} && !audioDetected", level);
			return;
		}

		if (level > SilenceThreshold) // did we find a signal?
		{
			audioDetected = true;
			silenceTime = null;

			Debug.WriteLine("AudioStream_OnBroadcast :: {0} :: level > SilenceThreshold :: bytes: {1}; level: {2}", DateTime.Now, bytes.Length, level);
		}
		else // no audio detected
		{
			// see if we've detected 'near' silence for more than <audioTimeout>
			if (StopRecordingOnSilence && silenceTime.HasValue)
			{
				var currentTime = DateTime.Now;

				if (currentTime.Subtract(silenceTime.Value).TotalMilliseconds > AudioSilenceTimeout.TotalMilliseconds)
				{
					Timeout($"AudioStream_OnBroadcast :: {currentTime} :: AudioSilenceTimeout exceeded, stopping recording :: Near-silence detected at: {silenceTime}");
					return;
				}
			}
			else
			{
				silenceTime = DateTime.Now;

				Debug.WriteLine("AudioStream_OnBroadcast :: {0} :: Near-silence detected :: bytes: {1}; level: {2}", silenceTime, bytes.Length, level);
			}
		}

		if (StopRecordingAfterTimeout && DateTime.Now - startTime > TotalAudioTimeout)
		{
			Timeout("AudioStream_OnBroadcast(): TotalAudioTimeout exceeded, stopping recording");
		}
	}

	public void Timeout(string reason)
	{
		Debug.WriteLine(reason);
		audioStream.OnBroadcast -= AudioStream_OnBroadcast; // need this to be immediate or we can try to stop more than once

		// since we're in the middle of handling a broadcast event when an audio timeout occurs, we need to break the StopRecording call on another thread
		//	Otherwise, Bad. Things. Happen.
		_ = Task.Run(() => StopRecording());
	}

	public async Task StopRecording(bool continueProcessing = true)
	{
		audioStream.Flush(); // allow the stream to send any remaining data
		audioStream.OnBroadcast -= AudioStream_OnBroadcast;

		try
		{
			await audioStream.Stop();
			// WaveRecorder will be stopped as result of stream stopping
		}
		catch (Exception ex)
		{
			Debug.WriteLine("Error in StopRecording: {0}", ex);
		}

		OnRecordingStopped();

		var returnedFilePath = GetAudioFilePath();
		// complete the recording Task for anthing waiting on this
		recordTask.TrySetResult(returnedFilePath);

		if (continueProcessing)
		{
			Debug.WriteLine($"AudioRecorderService.StopRecording(): Recording stopped, raising AudioInputReceived event; audioDetected == {audioDetected}; filePath == {returnedFilePath}");

			AudioInputReceived?.Invoke(this, returnedFilePath);
		}
	}

	public void InitializeStream(int sampleRate)
	{
		try
		{
			if (audioStream != null)
			{
				audioStream.OnBroadcast -= AudioStream_OnBroadcast;
			}
			else
			{
				audioStream = new AudioStream(sampleRate);
			}

			audioStream.OnBroadcast += AudioStream_OnBroadcast;

			if (recorder == null)
			{
				recorder = new WaveRecorder();
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine("Error: {0}", ex);
		}
	}

	public string GetAudioFilePath()
	{
		return audioDetected ? FilePath : null;
	}

	~AudioRecorder()
	{

	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}
