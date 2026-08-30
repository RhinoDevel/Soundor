
// 2026aug26, Marcel Timm, RhinoDevel

using System.Diagnostics;

namespace Soundor
{
    internal sealed class Sine : Oscillator
    {
        internal override double[] CreateSamples(
            ulong durationMs, ulong rateHz, double signalFreqHz)
        {
            Debug.Assert(0 < durationMs);
            Debug.Assert(0 < rateHz);
            Debug.Assert(0.0 < signalFreqHz);

            var sampleCount = Helper.GetSampleCountByDuration(
                    durationMs, rateHz);
            var phaseIncrementRad =
                    Math.Tau * signalFreqHz / (double)rateHz;
            var retVal = new double[sampleCount];
            double phaseRad;

            phaseRad = 0.0;
            for(int i = 0; i < retVal.Length; ++i)
            {
                retVal[i] = Math.Sin(phaseRad);

                // Update phase:
                phaseRad += phaseIncrementRad;
                if (Math.Tau <= phaseRad)
                {
                    phaseRad -= Math.Tau;
                }
            }
            return retVal;
        }
    }
}
