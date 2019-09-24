using System;
using System.IO;
using Common;
using Common.Exceptions;

namespace GameEngine.engine.audio.formats
{
    public class WAVAudioFormat : IAudioFormat
    {

        public bool TryLoadFile(string file, out byte[] data, out int channel, out int bits, out int bitRate)
        {
            Stream s = File.Open(file, FileMode.Open);

            bool ret = TryLoadFile(s, out data, out channel, out bits, out bitRate);
            s.Close();
            return ret;
        }


        public bool TryLoadFile(Stream fileStream, out byte[] data, out int channel, out int bits, out int bitRate)
        {
            if (fileStream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    this.Crash(new ME_InvalidAudioFile("Specified stream is not a wave file."));

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    this.Crash(new ME_InvalidAudioFile("Specified stream is not a wave file."));

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    this.Crash(new ME_InvalidAudioFile("Specified wave file is not supported."));

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    this.Crash(new ME_InvalidAudioFile("Specified wave file is not supported."));

                int data_chunk_size = reader.ReadInt32();

                channel = num_channels;
                bits = bits_per_sample;
                bitRate = sample_rate;
                data = reader.ReadBytes((int) reader.BaseStream.Length);
                return true;
            }

        }
    }
}