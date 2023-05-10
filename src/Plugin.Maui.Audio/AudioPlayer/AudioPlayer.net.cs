namespace Plugin.Maui.Audio;

partial class AudioPlayer : IAudioPlayer
{
	protected virtual void Dispose(bool disposing) { }

	public double Duration { get; }

	public double CurrentPosition { get; }

	public double Volume { get; set; }

	public double Balance { get; set; }

	public bool IsPlaying { get; }

	public bool Loop { get; set; }

	public bool CanSeek { get; }

	public void Play() { }

	public void Pause() { }

	public void Stop() { }

	public void Seek(double position) { }

	public void SetData(string fileName, bool fromAssets = true) { }

	public void SetData(Stream audioStream) { }

	public double Speed { get; set; }

	public double MinimumSpeed { get; }

	public double MaximumSpeed { get; }

	public bool CanSetSpeed { get; }
}