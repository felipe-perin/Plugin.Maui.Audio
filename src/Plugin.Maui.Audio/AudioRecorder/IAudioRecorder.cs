namespace Plugin.Maui.Audio;

/// <summary>
/// Interface for AudioRecorder
/// </summary>
public interface IAudioRecorder
{
	/// <summary>
	/// Gets/sets the desired file path. If null it will be set automatically
	/// to a temporary file.
	/// </summary>
	string FilePath { get; set; }

	/// <summary>
	/// Gets/sets the preferred sample rate to be used during recording.
	/// </summary>
	/// <remarks>This value may be overridden by platform-specific implementations, e.g. the Android AudioManager will be asked for its preferred sample rate and may override any user-set value here.</remarks>
	int PreferredSampleRate { get; set; }

	/// <summary>
	/// Returns a value indicating if the <see cref="AudioRecorderService"/> is currently recording audio
	/// </summary>
	bool IsRecording { get; }

	/// <summary>
	/// If <see cref="StopRecordingOnSilence"/> is set to <c>true</c>, this <see cref="TimeSpan"/> indicates the amount of 'silent' time is required before recording is stopped.
	/// </summary>
	/// <remarks>Defaults to 2 seconds.</remarks>
	TimeSpan AudioSilenceTimeout { get; set; }

	/// <summary>
	/// If <see cref="StopRecordingAfterTimeout"/> is set to <c>true</c>, this <see cref="TimeSpan"/> indicates the total amount of time to record audio for before recording is stopped. Defaults to 30 seconds.
	/// </summary>
	/// <seealso cref="StopRecordingAfterTimeout"/>
	TimeSpan TotalAudioTimeout { get; set; }

	/// <summary>
	/// Gets/sets a value indicating if the <see cref="AudioRecorderService"/> should stop recording after silence (low audio signal) is detected.
	/// </summary>
	/// <remarks>Default is `true`</remarks>
	bool StopRecordingOnSilence { get; set; }

	/// <summary>
	/// Gets/sets a value indicating if the <see cref="AudioRecorderService"/> should stop recording after a certain amount of time.
	/// </summary>
	/// <remarks>Defaults to <c>true</c></remarks>
	/// <seealso cref="TotalAudioTimeout"/>
	bool StopRecordingAfterTimeout { get; set; }

	/// <summary>
	/// Gets/sets a value indicating the signal threshold that determines silence.  If the recorder is being over or under aggressive when detecting silence, you can alter this value to achieve different results.
	/// </summary>
	/// <remarks>Defaults to .15.  Value should be between 0 and 1.</remarks>
	float SilenceThreshold { get; set; }

	/// <summary>
	/// This event is raised when audio recording is complete and delivers a full filepath to the recorded audio file.
	/// </summary>
	/// <remarks>This event will be raised on a background thread to allow for any further processing needed.  The audio file will be <c>null</c> in the case that no audio was recorded.</remarks>
	public event EventHandler<string> AudioInputReceived;

	/// <summary>
	/// Gets the full filepath to the recorded audio file.
	/// </summary>
	/// <returns>The full filepath to the recorded audio file, or null if no audio was detected during the last record.</returns>
	string GetAudioFilePath();

	/// <summary>
	/// Stops recording audio.
	/// </summary>
	/// <param name="continueProcessing"><c>true</c> (default) to finish recording and raise the <see cref="AudioInputReceived"/> event. 
	/// Use <c>false</c> here to stop recording but do nothing further (from an error state, etc.).</param>
	Task StopRecording(bool continueProcessing = true);

	/// <summary>
	/// Gets a new <see cref="Stream"/> to the recording audio file in readonly mode.
	/// </summary>
	/// <returns>A <see cref="Stream"/> object that can be used to read the audio file from the beginning.</returns>
	Stream GetAudioFileStream();

	/// <summary>
	/// Starts recording audio.
	/// </summary>
	/// <returns>A <see cref="Task"/> that will complete when recording is finished.  
	/// The task result will be the path to the recorded audio file, or null if no audio was recorded.</returns>
	Task<Task<string>> StartRecording();

	void Init();

	Task<string> GetDefaultFilePath();

	void OnRecordingStarting();

	void OnRecordingStopped();
}
