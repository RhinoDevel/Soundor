
// 2026aug26, Marcel Timm, RhinoDevel

using System.Diagnostics;

namespace Soundor
{
    internal static class Adsr
    {
        internal static double[] Create(
            double[] samples,
            double attack,
            double decay,
            double sustain,
            double release)
        {
            Debug.Assert(0.0 <= attack && attack <= 1.0);
            Debug.Assert(0.0 <= decay && decay <= 1.0);
            Debug.Assert(0.0 <= sustain && sustain <= 1.0);
            Debug.Assert(0.0 <= release && release <= 1.0);

            Debug.Assert(attack + decay + release <= 1.0);

            // Seems to be an edge case not handled correct, here:
            Debug.Assert(1 < samples.Length || release == 0.0);

            var retVal = new double[samples.Length];
            int pos;
            double attackEnd;
            int decayBeginPos;
            double decayEnd;
            int sustainBeginPos;
            double sustainEnd;
            int releaseBeginPos;

            pos = 0;

            // *****************************************************************
            // *** ATTACK                                                    ***
            // *****************************************************************

            // f(x) = m * x + b
            // b = 0
            // =>
            // f(x) = m * x
            // m = (y2 - y1) / (x2 - x1)
            // =>
            // f(x) = ((y2 - y1) / (x2 - x1)) * x
            // f(x) = ((1.0 - 0.0) / (attackEnd - 0.0)) * x
            // f(x) = (1 / attackEnd) * x
            // f(x) = x / attackEnd
            // f(x) = x / (attack * samples.Length)
            attackEnd = attack * (double)samples.Length;
            decayBeginPos = (int)attackEnd;

            if(0 < decayBeginPos)
            {
                for(; pos < decayBeginPos; ++pos)
                {
                    retVal[pos] = samples[pos] * (double)pos / attackEnd;
                }
            }

            // *****************************************************************
            // *** DECAY                                                     ***
            // *****************************************************************

            Debug.Assert(pos == decayBeginPos);

            decayEnd = attackEnd + decay * (double)samples.Length;
            sustainBeginPos = (int)decayEnd;

            if (0.0 < decay)
            {
                // x1 = attackEnd
                // x2 = decayEnd
                // y1 = 1.0
                // y2 = sustain
                //
                // => f(x) = ((sustain - 1.0) / (decayEnd - attackEnd)) * x + b
                double decayDeltaX = decayEnd - attackEnd;
                double decayDeltaY = sustain - 1.0;
                double decayM = decayDeltaY / decayDeltaX;
                double decayB = 1.0 - decayM * attackEnd;

                for (; pos < sustainBeginPos; ++pos)
                {
                    retVal[pos] =
                        samples[pos] * (decayM * (double)pos + decayB);
                }
            }

            // *****************************************************************
            // *** SUSTAIN                                                   ***
            // *****************************************************************

            Debug.Assert(pos == sustainBeginPos);

            sustainEnd = (1.0 - release) * (double)samples.Length;
            releaseBeginPos = (int)sustainEnd;

            for(; pos < releaseBeginPos; ++pos)
            {
                retVal[pos] = samples[pos] * sustain;
            }

            // *****************************************************************
            // *** RELEASE                                                   ***
            // *****************************************************************

            Debug.Assert(pos == releaseBeginPos);

            if(0.0 < release)
            {
                double relDeltaX = ((double)samples.Length - 1) - sustainEnd;
                double relDeltaY = 0.0 - sustain;
                double relM = relDeltaY / relDeltaX;
                double relB = sustain - relM * sustainEnd;

                for (; pos < samples.Length; ++pos)
                {
                    retVal[pos] = samples[pos] * (relM * (double)pos + relB);
                }
            }

            return retVal;
        }
    }
}
