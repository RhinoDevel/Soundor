
// 2026aug28, Marcel Timm, RhinoDevel

using System.Text.Json;

namespace Soundor
{
    /// <summary>
    /// Also see input.json file.
    /// </summary>
    internal class Params
    {
        public ulong DurationMs { get; set; }
        public double SignalStartFreqHz { get; set; }
        public double SignalEndFreqHz { get; set; }
        public double Attack { get; set; }
        public double Decay { get; set; }
        public double Sustain { get; set; }
        public double Release { get; set; }

        internal static Params CreateDefault()
        {
            return new Params
            {
                DurationMs = 400,
                SignalStartFreqHz = 440.0,
                SignalEndFreqHz = 440.0,
                Attack = 0.01,
                Decay = 0.15,
                Sustain = 0.05,
                Release = 0.79
            };
        }

        internal static Params? Load(string filename)
        {
            try
            {
                using var stream = File.OpenRead(filename);

                return JsonSerializer.Deserialize<Params>(
                    stream,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
            }
            catch(Exception /*e*/)
            {
                return null;
            }
        }

        internal static bool Save(string filename, Params p)
        {
            try
            {
                using var stream = File.Create(filename);

                JsonSerializer.Serialize(
                    stream,
                    p,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    });

                return true;
            }
            catch(Exception /*e*/)
            {
                return false;
            }
        }
    }
}
