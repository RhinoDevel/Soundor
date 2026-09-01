
// 2026aug26, Marcel Timm, RhinoDevel

namespace Soundor
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if(args.Length == 0)
            {
                // UI mode.

                Ui.Exec();
                return 0;
            }

            // Batch mode.

            if (!Batch.Exec(args[0]))
            {
                return 1;
            }
            return 0;
        }
    }
}
