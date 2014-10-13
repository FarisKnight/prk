using System;
using System.Linq;

namespace Parallel_Rabin_Karp_Application
{
    public class PRabinKarp
    {
        public static int rksearch(string[] pattern, string[] text)
        {
            int patternHash, textHash;
            int found = 0;

            if (pattern == null || text == null) return -1;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            for (int j = 0; j < pattern.Count(); j++)
            {
                patternHash = rollingHash(pattern[j]);

                for (int i = 0; i < text.Length - pattern[j].Length + 1; i++)
                {
                    textHash = rollingHash(text[i]);

                    if (patternHash == textHash && pattern[j] == text[i])
                    {
                        // do logic here:
                        found += 1;
                    }
                }
            }

            Console.WriteLine(watch.ElapsedMilliseconds);

            return found;
        }

        public static int rollingHash(string pattern)
        {
            int hashValue = 0;

            char[] charData = pattern.ToCharArray();

            foreach (char c in charData)
            {
                hashValue += (int)c;
            }

            return hashValue % 101;
        }
    }
}
