using Windows.Storage;

namespace Plugin.Maui.Audio;

partial class AudioRecorder : IAudioRecorder
{
	public void Init() { }

	public async Task<string> GetDefaultFilePath()
	{
		var temporaryFolder = ApplicationData.Current.TemporaryFolder;
		var tempFile = await temporaryFolder.CreateFileAsync(defaultFileName, CreationCollisionOption.ReplaceExisting);

		return tempFile.Path;
	}

	public void OnRecordingStarting()
	{
	}

	public void OnRecordingStopped()
	{
	}
}
