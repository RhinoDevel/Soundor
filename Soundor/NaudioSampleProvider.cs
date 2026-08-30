
// 2026aug27, Marcel Timm, RhinoDevel

using NAudio.Wave;

namespace Soundor
{
    /// <summary>
    /// Used for playback of internal sample arrays.
    /// </summary>
    public sealed class NaudioSampleProvider : ISampleProvider
    {
        private readonly double[] _samples;
        private int _pos;

        public WaveFormat WaveFormat { get; }

        public NaudioSampleProvider(double[] samples, int sampleRate)
        {
            _samples = samples;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
        }

        public int Read(Span<float> buffer)
        {
            int count = Math.Min(buffer.Length, _samples.Length - _pos);

            for (int i = 0; i < count; i++)
            {
                buffer[i] = (float)_samples[_pos + i];
            }

            _pos += count;
            return count;
        }
    }
}
