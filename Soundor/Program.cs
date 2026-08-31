
// 2026aug26, Marcel Timm, RhinoDevel

using System.Diagnostics;

namespace Soundor
{
    internal class Program
    {
        /// <remarks>
        /// Kind of depends on the UI width.
        /// Must leave room for an eventual "." (for floating-point) and/or "-"
        /// (if a minus signs is ever necessary for some values).
        /// </remarks>
        private const int _maxDigits = 15; // Integer PLUS decimal digits!

        private const string _year = "2026";
        private const string _defaultInputJson = "input.json";
        private const string _defaultOutputWav = "output.wav";
        private const ulong _defaultSamplingRateHz = 44100;

        private const int _leftVal = 32;

        private const int _topEntry = 21;

        static string GetStr(double val)
        {
            int intDigits = Math.Abs(val).ToString("F0").Length;
            int decDigits = Math.Max(0, _maxDigits - intDigits);

            return val.ToString("0." + new string('#', decDigits));
        }

        static string ReadEntry(string cur, int? lastLeft)
        {
            var input = cur;

            Console.CursorVisible = true;

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.CursorVisible = false;
                    return input;
                }

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (0 < input.Length)
                    {
                        input = input[..^1];

                        Console.Write("\b \b");
                    }
                    continue;
                }
                
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    if(lastLeft != null && Console.CursorLeft == lastLeft.Value)
                    {
                        continue;
                    }

                    input += keyInfo.KeyChar;

                    Console.Write(keyInfo.KeyChar);
                }
            }
        }

        static void DrawEmptyLine(int top, bool addNewline)
        {
            Console.SetCursorPosition(0, top);
            Console.Write("*                                                                              *");

            if(addNewline)
            {
                Console.Write(Environment.NewLine);
            }
        }

        static void DrawEmptyVal(int top)
        {
            Console.SetCursorPosition(_leftVal, top);
            Console.Write("                                               ");
        }

        static void DrawVal(int top, object val)
        {
            DrawEmptyVal(top);
            DrawObj(_leftVal, top, val);
        }

        static void DrawMask()
        {
            int line = 0;

            Console.WriteLine("********************************************************************************"); ++line;
            Console.WriteLine($"* RhinoDevel presents: SOUNDOR - Master Of The Effects                    {_year} *"); ++line;
            Console.WriteLine("********************************************************************************"); ++line;
            Console.WriteLine("* [I]nput file (JSON):                                                         *"); ++line;
            DrawEmptyLine(line++, true);
            Console.WriteLine("* [D]uration (ms):                                                             *"); ++line;
            Console.WriteLine("* Signal [f]requency (Hz):                                                     *"); ++line;
            Console.WriteLine("* A[t]tack (part of duration):                                                 *"); ++line;
            Console.WriteLine("* D[e]cay (part of duration):                                                  *"); ++line;
            Console.WriteLine("* S[u]stain (level):                                                           *"); ++line;
            Console.WriteLine("* Rele[a]se (part of duration):                                                *"); ++line;
            DrawEmptyLine(line++, true);
            Console.WriteLine("* Sampling [r]ate (Hz):                                                        *"); ++line;
            Console.WriteLine("* [O]utput file (WAV):                                                         *"); ++line;
            DrawEmptyLine(line++, true);
            Console.WriteLine("* [P]layback effect.                                                           *"); ++line;
            Console.WriteLine("* [L]oad from input file.                                                      *"); ++line;
            Console.WriteLine("* Sa[v]e to input file (JSON).                                                 *"); ++line;
            Console.WriteLine("* [S]ave to output file (WAV).                                                 *"); ++line;
            Console.WriteLine("* E[x]it Soundor.                                                              *"); ++line;
            Console.WriteLine("********************************************************************************"); ++line;
            Console.WriteLine("* >                                                                            *"); ++line;
            Console.WriteLine("********************************************************************************"); ++line;
            DrawEmptyLine(line++, true);
            Console.WriteLine("********************************************************************************"); ++line;

            Debug.Assert(line == 25);
        }

        static void DrawObj(int left, int top, object o)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(o);
        }

        static void DrawStatus(string status)
        {
            DrawEmptyLine(23, false);
            DrawObj(2, 23, status);
        }

        static void DrawValues(
            Params p, string inputJson, string outputWav, ulong samplingRateHz)
        {
            int top;

            top = 3;
            DrawVal(top++, inputJson);

            top += 1;

            DrawVal(top++, p.DurationMs);
            DrawVal(top++, p.SignalFreqHz);
            DrawVal(top++, p.Attack);
            DrawVal(top++, p.Decay);
            DrawVal(top++, p.Sustain);
            DrawVal(top++, p.Release);

            top += 1;

            DrawVal(top++, samplingRateHz);
            DrawVal(top++, outputWav);
        }

        static int DrawEntry(
            object val, double? min = null, double? max = null)
        {
            int offset = 2;

            DrawEmptyLine(_topEntry, false);
            if(min != null)
            {
                Debug.Assert(max != null);
                Debug.Assert(min.Value <= max.Value);

                var minStr = GetStr(min.Value);
                var maxStr = GetStr(max.Value);

                DrawObj(offset++, _topEntry, "[");

                DrawObj(offset, _topEntry, minStr);
                offset += minStr.Length;

                DrawObj(offset, _topEntry, "..");
                offset += 2;

                DrawObj(offset, _topEntry, maxStr);
                offset += maxStr.Length;

                DrawObj(offset, _topEntry, "] ");
                offset += 2;
            }
            DrawObj(offset, _topEntry, ">");
            offset += 2;
            DrawObj(offset, _topEntry, val);
            return offset;
        }

        static double GetNewValDbl(
            string title, double curVal, double min, double max)
        {
            Debug.Assert(min < max);

            string curStr = GetStr(curVal!);
            string newStr;
            double newVal;
            int offset;

            offset = DrawEntry(curVal, min, max);
            DrawStatus($"Selected {title}.");

            Console.SetCursorPosition(offset + curStr.Length, _topEntry);

            newStr = ReadEntry(curStr, 78);

            if (!double.TryParse(newStr, out newVal))
            {
                DrawEntry(curStr); // (because of length limit)
                DrawStatus($"Reused current {title} (invalid input).");
                return curVal;
            }

            // (because of length limit)
            newStr = GetStr(newVal);
            newVal = double.Parse(newStr); 

            if (newVal < min || max < newVal)
            {
                DrawEntry(curStr); // (because of length limit)
                DrawStatus($"Reused current {title} (input out of range).");
                return curVal;
            }
            if (newVal == curVal)
            {
                DrawEntry(curStr); // (because of length limit)
                DrawStatus($"Kept current {title} (no change).");
                return curVal;
            }
            DrawEntry(newStr); // (because of length limit)
            DrawStatus($"Changed {title}.");
            return newVal;
        }

        static double GetNewValFreqOrNote(
            string title, double curVal, double min, double max)
        {
            Debug.Assert(min < max);

            string curStr = GetStr(curVal!);
            string newStr;
            double newVal;
            int offset;

            offset = DrawEntry(curVal, min, max);
            DrawStatus($"Selected {title}.");

            Console.SetCursorPosition(offset + curStr.Length, _topEntry);

            newStr = ReadEntry(curStr, 78);

            if (!double.TryParse(newStr, out newVal))
            {
                // Does not seem to be a floating-point value.

                if (!Note.TryParseFreq(newStr, out newVal))
                {
                    // Also does not seem to be a musical note being entered.

                    DrawEntry(curStr); // (because of length limit)
                    DrawStatus($"Reused current {title} (invalid input).");
                    return curVal;
                }
            }

            // (because of length limit)
            newStr = GetStr(newVal);
            newVal = double.Parse(newStr);

            if (newVal < min || max < newVal)
            {
                DrawEntry(curStr); // (because of length limit)
                DrawStatus($"Reused current {title} (input out of range).");
                return curVal;
            }
            if (newVal == curVal)
            {
                DrawEntry(curStr); // (because of length limit)
                DrawStatus($"Kept current {title} (no change).");
                return curVal;
            }
            DrawEntry(newStr); // (because of length limit)
            DrawStatus($"Changed {title}.");
            return newVal;
        }

        static ulong GetNewValUlong(
            string title, ulong curVal, ulong min, ulong max)
        {
            Debug.Assert(min < max);

            string curStr = curVal!.ToString()!;
            string newStr;
            ulong newVal;
            int offset;

            offset = DrawEntry(curVal, (double)min, (double)max);
            DrawStatus($"Selected {title}.");

            Console.SetCursorPosition(offset + curStr.Length, _topEntry);

            newStr = ReadEntry(curStr, 78);

            if (!ulong.TryParse(newStr, out newVal)
                    || newVal < min
                    || max < newVal)
            {
                DrawEntry(curStr);
                DrawStatus($"Reused current {title} (invalid input).");
                return curVal;
            }
            if (newVal == curVal)
            {
                DrawEntry(curStr); // (because of length limit)
                DrawStatus($"Kept current {title} (no change).");
                return curVal;
            }
            DrawEntry(newStr); // (because of length limit)
            DrawStatus($"Changed {title}.");
            return newVal;
        }

        static string GetNewValStr(string title, string curVal)
        {
            string newVal;
            int offset;

            offset = DrawEntry(curVal);
            DrawStatus($"Selected {title}.");

            Console.SetCursorPosition(offset + curVal.Length, _topEntry);

            newVal = ReadEntry(curVal, 78).Trim();

            if (newVal.Length <= 0)
            {
                DrawEntry(curVal);
                DrawStatus($"Reused current {title} (invalid input).");
                return curVal;
            }
            if (newVal == curVal)
            {
                DrawStatus($"Kept current {title} (no change).");
                return curVal;
            }
            DrawStatus($"Changed {title}.");
            return newVal;
        }

        static void DrawQuestion(string question)
        {
            string options = "[y/n]";
            int offset = 2;

            DrawEmptyLine(_topEntry, false);
            DrawObj(offset, _topEntry, question);
            offset += question.Length + 1;
            DrawObj(offset, _topEntry, options);
        }
        static void ClearQuestion()
        {
            DrawEmptyLine(_topEntry, false);
            DrawObj(2, _topEntry, ">");
        }

        static bool GetIsConfirmed(string question)
        {
            bool retVal;

            DrawQuestion(question);

            retVal = Console.ReadKey(true).Key == ConsoleKey.Y;

            ClearQuestion();

            return retVal;
        }

        static int Main(string[] args)
        {
            var p = Params.CreateDefault();
            var inputJson = _defaultInputJson;
            var outputWav = _defaultOutputWav;
            var samplingRateHz = _defaultSamplingRateHz;
            double[] samples;

            Console.CursorVisible = false;

            Console.Clear();

            DrawMask();
            DrawStatus("Welcome to the magical power of sound effects!");

            while (true)
            {
                DrawValues(p, inputJson, outputWav, samplingRateHz);

                // Not always necessary, but for simplicity:
                samples = new Sine().CreateSamples(
                    p.DurationMs, samplingRateHz, p.SignalFreqHz);
                samples = Adsr.Create(
                    samples, p.Attack, p.Decay, p.Sustain, p.Release);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.A:
                    {
                        p.Release = GetNewValDbl(
                            "release", p.Release, 0.0, 1.0);
                        DrawValues(p, inputJson, outputWav, samplingRateHz);
                        break;
                    }

                    case ConsoleKey.D:
                    {
                        p.DurationMs = GetNewValUlong(
                            "duration",
                            p.DurationMs,
                            1,
                            1 * 60 * 1000);
                        DrawValues(p, inputJson, outputWav, samplingRateHz);
                        break;
                    }
                    case ConsoleKey.E:
                    {
                        p.Decay = GetNewValDbl("decay", p.Decay, 0.0, 1.0);
                        DrawValues(p, inputJson, outputWav, samplingRateHz);
                        break;
                    }
                    case ConsoleKey.F:
                    {
                        p.SignalFreqHz = GetNewValFreqOrNote(
                            "signal frequency",
                            p.SignalFreqHz,
                            Helper.AudioRangeMinHz,
                            Helper.AudioRangeMaxHz);
                        DrawValues(p, inputJson, outputWav, samplingRateHz);
                        break;
                    }

                    case ConsoleKey.I:
                    {
                        inputJson = GetNewValStr("input file", inputJson);
                        break;
                    }

                    case ConsoleKey.L:
                    {
                        Params? newP;

                        if (!GetIsConfirmed(
                                "Really load from input JSON file?"))
                        {
                            DrawStatus("Did not load from input JSON file.");
                            break;
                        }

                        newP = Params.Load(inputJson);
                        if(newP == null)
                        {
                            DrawStatus("Failed to load from input JSON file.");
                            break;
                        }
                        p = newP;
                        DrawStatus("Loaded input JSON file.");
                        break;
                    }

                    case ConsoleKey.O:
                    {
                        outputWav = GetNewValStr("output file", outputWav);
                        break;
                    }
                    case ConsoleKey.P:
                    {
                        Helper.Play(samples, samplingRateHz);
                        DrawStatus("Played effect.");
                        break;
                    }

                    case ConsoleKey.R:
                    {
                        samplingRateHz = GetNewValUlong(
                            "sampling rate",
                            samplingRateHz,
                            8000, // Telephony, speech, etc.
                            48000); // Video, games, etc.
                        DrawValues(p, inputJson, outputWav, samplingRateHz);
                        break;
                    }
                    case ConsoleKey.S:
                    {
                        if (!GetIsConfirmed(
                                "Really save/overwrite output (WAV) file?"))
                        {
                            DrawStatus("Did not save/overwrite output (WAV) file.");
                            break;
                        }
                        Helper.SaveAsWav(
                            outputWav, samples, samplingRateHz);
                        DrawStatus("Saved to output (WAV) file.");
                        break;
                    }
                    case ConsoleKey.T:
                    {
                        p.Attack = GetNewValDbl("attack", p.Attack, 0.0, 1.0);
                        DrawValues(p, inputJson, outputWav, samplingRateHz);
                        break;
                    }
                    case ConsoleKey.U:
                    {
                        p.Sustain = GetNewValDbl(
                            "sustain", p.Sustain, 0.0, 1.0);
                        DrawValues(p, inputJson, outputWav, samplingRateHz);
                        break;
                    }
                    case ConsoleKey.V:
                    {
                        if (!GetIsConfirmed(
                                "Really save/overwrite (input) JSON file?"))
                        {
                            DrawStatus("Did not save/overwrite (input) JSON file.");
                            break;
                        }
                        if (!Params.Save(inputJson, p))
                        {
                            DrawStatus("Failed to save to input (JSON) file.");
                            break;
                        }
                        DrawStatus("Saved to input (JSON) file.");
                        break;
                    }

                    case ConsoleKey.X:
                    {
                        if(!GetIsConfirmed("Really exit Soundor?"))
                        {
                            DrawStatus("Did not exit Soundor (good choice!).");
                            break;
                        }
                        DrawStatus("Exited Soundor. See you soon!");
                        Console.SetCursorPosition(80, 24);
                        return 0;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }
    }
}
