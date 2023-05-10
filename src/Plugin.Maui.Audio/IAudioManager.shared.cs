namespace Plugin.Maui.Audio;

/// <summary>
/// Provides the ability to create <see cref="IAudioPlayer" /> instances.
/// </summary>
public interface IAudioManager
{
	/// <summary>
	/// Creates a new <see cref="IAudioPlayer"/> object.
	/// </summary>
	/// <returns>A new <see cref="IAudioPlayer"/> object
	IAudioPlayer CreatePlayer() => new AudioPlayer();

	/// <Summary>
	/// Create a new <see cref="IAudioRecorder"/> object
	/// </Summary>
	/// <returns>A new <see cref="IAudioRecorder"/> object
	IAudioRecorder CreateRecorder() => new AudioRecorder();
}
