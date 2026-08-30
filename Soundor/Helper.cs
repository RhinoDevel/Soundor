
// 2026aug26, Marcel Timm, RhinoDevel

using NAudio.Wave;
using NAudio.Wave.Alsa;
using System.Diagnostics;

namespace Soundor
{
    internal static class Helper
    {
        internal const double AudioRangeMinHz = 20.0; // Hz
        internal const double AudioRangeMaxHz = 20000.0; // Hz

        internal static ulong GetSampleCountByDuration(
            ulong durationMs, ulong rateHz)
        {
            // E.g. for 10 minutes at a sampling rate of 44100 Hz:
            //
            // 10 minutes = 10 x 60 x 1000 ms = 600000 ms
            //
            // 600000 x 41000 / 1000 = 26460000 samples

            return durationMs * rateHz / 1000;
        }

        internal static ulong GetSampleCountBySignalFrequency(
            ulong signalFreqHz, ulong rateHz)
        {
            return rateHz / signalFreqHz;
        }

        internal static short GetPcm16(double sample)
        {
            var normalized = Math.Clamp(sample, -1.0, 1.0); // For safety..

            return (short)Math.Clamp(
                Math.Round(-short.MinValue * normalized),
                short.MinValue,
                short.MaxValue);
        }

        internal static void SaveAsWav(
            string filename, double[] samples, ulong rateHz)
        {
            const ushort channels = 1;
            const ushort bitsPerSample = 16;

            var sampleRate = checked((uint)rateHz);
            var dataSize = checked((uint)(samples.Length * 2));

            using var stream = File.Create(filename);
            using var writer = new BinaryWriter(stream);

            writer.Write("RIFF"u8);
            writer.Write(checked(36u + dataSize));
            writer.Write("WAVE"u8);

            writer.Write("fmt "u8);
            writer.Write(16u); // PCM format chunk size.
            writer.Write((ushort)1); // PCM
            writer.Write(channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * channels * (bitsPerSample / 8));
            writer.Write((ushort)(channels * (bitsPerSample / 8)));
            writer.Write(bitsPerSample);

            writer.Write("data"u8);
            writer.Write(dataSize);

            foreach (var sample in samples)
            {
                // Expects samples from -1 to 1, but GetPcm16() clamps..
                var value = GetPcm16(sample);
                writer.Write(value);
            }
        }

        internal static void SaveAsCsv(string filename, double[] samples)
        {
            var csv = "Index;Value;";

            for (int i = 0; i < samples.Length; ++i)
            {
                csv += Environment.NewLine;
                csv += i;
                csv += ";";
                csv += samples[i];
                csv += ";";
            }

            File.WriteAllText(filename, csv);
        }

        internal static bool Play(double[] samples, ulong samplingRateHz)
        {
            Debug.Assert(samplingRateHz <= (ulong)int.MaxValue);

            IWavePlayer player;

            if (OperatingSystem.IsWindows())
            {
                player = new WasapiPlayerBuilder().Build();
            }
            else
            {
                if (OperatingSystem.IsLinux())
                {
                    player = new AlsaOut();
                }
                else
                {
                    return false; // OS is not supported.
                }
            }

            using var finished = new ManualResetEventSlim(false);

            player.PlaybackStopped += (_, _) => finished.Set();
            player.Init(
                new NaudioSampleProvider(samples, (int)samplingRateHz)
                        .ToWaveProvider());
            player.Play();

            finished.Wait();
            return true;
        }
    }
}
