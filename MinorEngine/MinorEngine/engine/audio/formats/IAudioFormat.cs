namespace MinorEngine.engine.audio.formats
{
    public interface IAudioFormat
    {
        bool TryLoadFile(string file, out byte[] data, out int channel, out int bits, out int bitRate);
    }
}