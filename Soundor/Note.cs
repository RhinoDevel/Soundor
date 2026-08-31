
// 2026aug31, Marcel Timm, RhinoDevel

namespace Soundor
{
    internal static class Note
    {
        internal static bool TryParseFreq(string note, out double freqHz)
        {
            int pos;
            int noteIndex;
            int octOffset;
            int noteOct;

            pos = 0;
            if(pos == note.Length)
            {
                freqHz = 0.0;
                return false;
            }

            // 000000000011
            // 012345678901
            // C D EF G A B
            switch (char.ToLowerInvariant(note[pos]))
            {
                case 'c':
                {
                    noteIndex = 0;
                    break;
                }

                case 'd':
                {
                    noteIndex = 2;
                    break;
                }

                case 'e':
                {
                    noteIndex = 4;
                    break;
                }
                case 'f':
                {
                    noteIndex = 5;
                    break;
                }

                case 'g':
                {
                    noteIndex = 7;
                    break;
                }

                case 'a':
                {
                    noteIndex = 9;
                    break;
                }

                case 'b':
                {
                    noteIndex = 11;
                    break;
                }

                default:
                {
                    freqHz = 0.0;
                    return false;
                }
            }

            ++pos;
            if(pos == note.Length)
            {
                freqHz = 0.0;
                return false;
            }

            octOffset = 0;
            switch(note[pos])
            {
                case 'b':
                {
                    --noteIndex;
                    if(noteIndex < 0)
                    {
                        noteIndex = 11;
                        octOffset = -1;
                    }
                    ++pos;
                    if (pos == note.Length)
                    {
                        freqHz = 0.0;
                        return false;
                    }
                    break;
                }
                case '#':
                {
                    ++noteIndex;
                    if(11 < noteIndex)
                    {
                        noteIndex = 0;
                        octOffset = 1;
                    }
                    ++pos;
                    if (pos == note.Length)
                    {
                        freqHz = 0.0;
                        return false;
                    }
                    break;
                }

                default:
                {
                    break;
                }
            }

            if (pos == note.Length)
            {
                freqHz = 0.0;
                return false;
            }

            if(!int.TryParse(note.Substring(pos), out noteOct))
            {
                freqHz = 0.0;
                return false;
            }

            noteOct += octOffset;
            if (noteOct < 0 || 10 < noteOct)
            {
                freqHz = 0.0;
                return false;
            }

            // A4 = 440 Hz, A4 is 57 semitones above C0.
            freqHz = 440.0 * Math.Pow(
                2.0, (double)(12 * noteOct + noteIndex - 57) / 12.0);
            return true;
        }
    }
}
