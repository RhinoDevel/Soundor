
// 2026sep01, Marcel Timm, RhinoDevel

using System.Text.Json;

namespace Soundor
{
    /// <summary>
    /// Also see batch.json file.
    /// </summary>
    internal class BatchParams
    {
        public ulong SamplingRate { get; set; }
        public List<BatchParamsData> Data { get; set; } = new();
        public string OutputWav { get; set; } = string.Empty;

        internal static BatchParams CreateDefault() => new BatchParams();

        internal static BatchParams? Load(string filename)
        {
            try
            {
                using var stream = File.OpenRead(filename);

                return JsonSerializer.Deserialize<BatchParams>(
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
    }
}
