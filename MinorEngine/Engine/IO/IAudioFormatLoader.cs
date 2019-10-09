namespace Engine.IO
{
    /// <summary>
    /// Interface defining the Functions that are needed to be used as a Audio Format Loader
    /// </summary>
    public interface IAudioFormatLoader
    {
        /// <summary>
        /// Tries to load the specified file and pass the loaded data through the out parameters
        /// </summary>
        /// <param name="file">Input File</param>
        /// <param name="data">Data that has been loaded</param>
        /// <param name="channel">The channel count of the file</param>
        /// <param name="bits">The bits per sample(or the BitDepth)</param>
        /// <param name="bitRate">The bitrate of the Audio File</param>
        /// <returns></returns>
        bool TryLoadFile(string file, out byte[] data, out int channel, out int bits, out int bitRate);
    }
}