using System.Diagnostics;
using Android.Content;

namespace Plugin.Maui.Audio;

partial class AudioRecorder : IAudioRecorder
{
	public AudioRecorder()
	{
		Init();
	}

	public void Init()
	{
		if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.JellyBean)
		{
			try
			{
				//if the below call to AudioManager is blocking and never returning/taking forever, ensure the emulator has proper access to the system mic input
				var audioManager = (Android.Media.AudioManager)Android.App.Application.Context.GetSystemService(Context.AudioService);
				var property = audioManager.GetProperty(Android.Media.AudioManager.PropertyOutputSampleRate);

				if (!string.IsNullOrEmpty(property) && int.TryParse(property, out int sampleRate))
				{
					Debug.WriteLine($"Setting PreferredSampleRate to {sampleRate} as reported by AudioManager.PropertyOutputSampleRate");
					PreferredSampleRate = sampleRate;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error using AudioManager to get AudioManager.PropertyOutputSampleRate: {0}", ex);
				Debug.WriteLine("PreferredSampleRate will remain at the default");
			}
		}
	}

	public Task<string> GetDefaultFilePath()
	{
		return Task.FromResult(Path.Combine(Path.GetTempPath(), defaultFileName));
	}

	public void OnRecordingStarting()
	{
	}

	public void OnRecordingStopped()
	{
	}
}
