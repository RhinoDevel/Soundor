
// 2026sep01, Marcel Timm, RhinoDevel

namespace Soundor
{
    internal static class Batch
    {
        private const string _errPrefix = "ERROR: ";

        private static void Log(string msg) => Console.WriteLine(msg);
        private static void LogErr(string msg) => Log($"{_errPrefix}{msg}");

        internal static bool Exec(string fileName)
        {
            BatchParams? bp;
            List<Tuple<double[], ulong>> effectAndCounts;
            int sampleCount;
            double[] samples;
            int pos;

            Log("Entered batch mode.");

            if(!File.Exists(fileName))
            {
                LogErr("Input file was not found!");
                return false;
            }

            bp = BatchParams.Load(fileName);
            if(bp == null)
            {
                LogErr("Failed to load from input file!");
                return false;
            }

            if(bp.Data == null)
            {
                LogErr("Invalid data array read!");
                return false;
            }

            sampleCount = 0;
            effectAndCounts = new();
            for(int i = 0; i < bp.Data.Count; ++i)
            {
                var d = bp.Data[i];
                Params? p;
                double[] dataSamples;

                if(!File.Exists(d.InputJson))
                {
                    LogErr($"Effect input file at index {i} was not found!");
                    return false;
                }

                p = Params.Load(d.InputJson);
                if(p == null)
                {
                    LogErr($"Failed to load from effect input file at index {i}!");
                    return false;
                }

                dataSamples = new Sine().CreateSamples(
                    p.DurationMs,
                    bp.SamplingRate, // TODO: Add min./max. check!
                    p.SignalFreqHz);

                dataSamples = Adsr.Create(
                    dataSamples, p.Attack, p.Decay, p.Sustain, p.Release);

                effectAndCounts.Add(new(dataSamples, d.Count));

                sampleCount += (int)d.Count * dataSamples.Length; // TODO: Check that max. sample count supported is high enough!
            }

            samples = new double[sampleCount];
            pos = 0;
            foreach(var effectAndCount in effectAndCounts)
            {
                for(int i = 0; i < (int)effectAndCount.Item2; ++i)
                {
                    samples[pos++] = effectAndCount.Item1[i];
                }
            }

            if(!Helper.SaveAsWav(bp.OutputWav, samples, bp.SamplingRate))
            {
                LogErr("Failed to save WAV file!");
                return false;
            }

            Log("Done! Enjoy your music!");
            return true;
        }
    }
}
