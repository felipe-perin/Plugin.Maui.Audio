namespace Plugin.Maui.Audio;

public class AudioManager : IAudioManager
{
	static IAudioManager? currentImplementation;

	public static IAudioManager Current => currentImplementation ??= new AudioManager();

	/// <inheritdoc />
	public IAudioPlayer CreatePlayer()
	{
		return new AudioPlayer();
	}

	/// <inheritdoc />
	public IAudioRecorder CreateRecorder()
	{
		return new AudioRecorder();
	}
}
