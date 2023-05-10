namespace Plugin.Maui.Audio;

partial class AudioStream : IAudioStream
{
	public AudioStream(int sampleRate)
	{

	}

	public int SampleRate { get; set; }

	public int ChannelCount { get; set; }

	public int BitsPerSample { get; set; }

	public bool Active { get; set; }

	public event EventHandler<byte[]> OnBroadcast;
	public event EventHandler<bool> OnActiveChanged;
	public event EventHandler<Exception> OnException;

	public void Flush()
	{

	}

	public Task Start()
	{
		return Task.CompletedTask;
	}

	public Task Stop()
	{
		return Task.CompletedTask;
	}
}
