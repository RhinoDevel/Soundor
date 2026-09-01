
// 2026sep01, Marcel Timm, RhinoDevel

using System.Text.Json;

namespace Soundor
{
    internal class BatchParamsData
    {
        public string InputJson { get; set; } = string.Empty;
        public ulong Count { get; set; }
    }
}
