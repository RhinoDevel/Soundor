
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
            BatchParams? p;

            Log("Entered batch mode.");

            if(!File.Exists(fileName))
            {
                LogErr("Input file was not found!");
                return false;
            }

            p = BatchParams.Load(fileName);
            if(p == null)
            {
                LogErr("Failed to load from input file!");
            }

            // TODO: Implement!

            return true;
        }
    }
}
