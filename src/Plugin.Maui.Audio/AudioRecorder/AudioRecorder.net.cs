namespace Plugin.Maui.Audio;

partial class AudioRecorder : IAudioRecorder
{
	public Task<string> GetDefaultFilePath()
	{
		return (Task<string>)Task.CompletedTask;
	}

	public void Init()
	{

	}

	public void OnRecordingStarting()
	{

	}

	public void OnRecordingStopped()
	{

	}
}