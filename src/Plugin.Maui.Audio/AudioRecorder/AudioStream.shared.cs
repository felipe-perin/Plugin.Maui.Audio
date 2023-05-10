namespace Plugin.Maui.Audio;

public partial class AudioStream : IAudioStream
{
	~AudioStream()
	{

	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}
