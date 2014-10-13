namespace Parallel_Rabin_Karp_Application
{
    public class Kgram
    {
        public static string[] kgramParsing(string[] text, int _kGram)
        {
            string[] textKgram = new string[sizeKgram(text, _kGram)];

            string textToParse = string.Join("", text);
            int textLength = textToParse.Length;

            for (int i = 0; i <= textLength - _kGram; i++)
            {
                textKgram[i] = textToParse.Substring(i, _kGram);
            }

            return textKgram;
        }

        public static int sizeKgram(string[] text, int _kGram)
        {
            int size = 0;

            string textToParse = string.Join("", text);
            int textLength = textToParse.Length;

            while (size <= textLength - _kGram)
            {
                size += _kGram;
            };



            return size;
        }
    }
}
