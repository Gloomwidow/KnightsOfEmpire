using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units
{
    public static class UnitIdManager
    {
        private static bool isSet = false;
        private static int[] Symbols;
        private static int SymbolStart = 32; // We are skipping ASCII control chars in case JSON will do sth strange with them.
        private static readonly int MaxChars = 127; // We can only send basic ASCII through the net, so we need to use only 128 first chars.

        private static readonly char[] SkippedSymbols = { '"', '\'', '\\' }; // We are skipping special symbols that JSON might not handle.  
        

        public static void SetupIds(int chars)
        {
            if (isSet) return;
            Symbols = new int[chars];
            isSet = true;
            Console.WriteLine($"Maximum unique ids is {Math.Pow(MaxChars + 1 - SymbolStart, chars)}.");
        }

        public static char[] GetNewId()
        {
            char[] id = new char[Symbols.Length];
            for(int i=0;i<Symbols.Length;i++)
            {
                id[i] = (char)Symbols[i];
            }

            Symbols[0]++;
            for (int i = 0; i < Symbols.Length - 1; i++)
            {
                foreach(char c in SkippedSymbols)
                {
                    if (Symbols[i] == (int)c) Symbols[i]++;
                }
                if (Symbols[i] > MaxChars)
                {
                    Symbols[i] = SymbolStart;
                    Symbols[i + 1]++;
                }
                else break;
            }

            return id;
        }
    }
}
