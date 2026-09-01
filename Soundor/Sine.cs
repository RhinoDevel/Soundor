
// 2026aug26, Marcel Timm, RhinoDevel

using System.Diagnostics;

namespace Soundor
{
    internal sealed class Sine : Oscillator
    {
        private static double[] CreateSamplesInternal(
            ulong durationMs,
            ulong rateHz,
            double signalBegFreqHz,
            double signalEndFreqHz)
        {
            Debug.Assert(0 < durationMs);
            Debug.Assert(0 < rateHz);
            Debug.Assert(0.0 < signalBegFreqHz);
            Debug.Assert(0.0 < signalEndFreqHz);

            var sampleCount = Helper.GetSampleCountByDuration(
                    durationMs, rateHz);
            var retVal = new double[sampleCount];
            double phaseRad;
            var signalDiff = signalEndFreqHz - signalBegFreqHz;
            var sweepDiv = (double)(retVal.Length - 1);
            bool doSweep = 1 < retVal.Length && signalDiff != 0.0;

            phaseRad = 0.0;
            for (int i = 0; i < retVal.Length; ++i)
            {
                double signalFreqHz;

                retVal[i] = Math.Sin(phaseRad);

                // Update phase:

                signalFreqHz = signalBegFreqHz;

                if (doSweep)
                {
                    signalFreqHz += (double)i * signalDiff / sweepDiv;
                }

                phaseRad += Math.Tau * signalFreqHz / rateHz;
                if (Math.Tau <= phaseRad)
                {
                    phaseRad -= Math.Tau;
                }
            }
            return retVal;
        }

        internal override double[] CreateSamples(
            ulong durationMs, ulong rateHz, double signalFreqHz)
                => CreateSamplesInternal(
                    durationMs, rateHz, signalFreqHz, signalFreqHz);

        internal override double[] CreateSamples(
            ulong durationMs,
            ulong rateHz,
            double signalBegFreqHz,
            double signalEndFreqHz)
                => CreateSamplesInternal(
                    durationMs, rateHz, signalBegFreqHz, signalEndFreqHz);
    }
}
