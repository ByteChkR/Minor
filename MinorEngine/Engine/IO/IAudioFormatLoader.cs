namespace Engine.IO
{
    public interface IAudioFormatLoader
    {
        bool TryLoadFile(string file, out byte[] data, out int channel, out int bits, out int bitRate);
    }
}