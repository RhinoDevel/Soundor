
// 2026aug26, Marcel Timm, RhinoDevel

namespace Soundor
{
    internal abstract class Oscillator
    {
        internal abstract double[] CreateSamples(
            ulong durationMs, ulong rateHz, double signalFreqHz);
    }
}
