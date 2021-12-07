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
        private static int SymbolStart = 0; 
        private static int MaxChars = 1; 

        private static readonly char[] SkippedSymbols = { '"', '\'', '\\' }; // We are skipping special symbols that JSON might not handle.  

        private static char[] UsedSymbols;

        public static void SetupIds(int chars)
        {
            UsedSymbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            MaxChars = UsedSymbols.Length - 1;
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
                id[i] = UsedSymbols[Symbols[i]];
            }

            Symbols[0]++;
            for (int i = 0; i < Symbols.Length - 1; i++)
            {
                //foreach(char c in SkippedSymbols)
                //{
                //    if (Symbols[i] == (int)c) Symbols[i]++;
                //}
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
