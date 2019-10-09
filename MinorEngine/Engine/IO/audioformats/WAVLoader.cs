using System.IO;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.IO.audioformats
{
    public class WAVLoader : IAudioFormatLoader
    {
        public bool TryLoadFile(string file, out byte[] data, out int channel, out int bits, out int bitRate)
        {
            Stream s = File.Open(file, FileMode.Open);

            var ret = TryLoadFile(s, out data, out channel, out bits, out bitRate);
            s.Close();
            return ret;
        }


        public bool TryLoadFile(Stream fileStream, out byte[] data, out int channel, out int bits, out int bitRate)
        {
            if (fileStream == null)
            {
                Logger.Crash(new AudioFileInvalidException("Filestream is null"), true);
                data = null;
                channel = bits = bitRate = 0;
                return false;
            }

            using (var reader = new BinaryReader(fileStream))
            {
                // RIFF header
                var signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                {
                    Logger.Crash(new AudioFileInvalidException("Specified stream is not a wave file."), false);
                }

                /*int riff_chunck_size = */
                reader.ReadInt32();

                var format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                {
                    Logger.Crash(new AudioFileInvalidException("Specified stream is not a wave file."), false);
                }

                // WAVE header
                var format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                {
                    Logger.Crash(new AudioFileInvalidException("Specified wave file is not supported."), false);
                }

                /*int format_chunk_size = */
                reader.ReadInt32();
                /*int audio_format = */
                reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                var sample_rate = reader.ReadInt32();
                /*int byte_rate = */
                reader.ReadInt32();
                /*int block_align = */
                reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                var data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                {
                    Logger.Crash(new AudioFileInvalidException("Specified wave file is not supported."), false);
                }

                /*int data_chunk_size = */
                reader.ReadInt32();

                channel = num_channels;
                bits = bits_per_sample;
                bitRate = sample_rate;
                data = reader.ReadBytes((int)reader.BaseStream.Length);
                return true;
            }
        }
    }
}