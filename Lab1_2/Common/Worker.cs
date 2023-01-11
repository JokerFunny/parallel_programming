using System.Text;

namespace Common
{
    public class Worker
    {
        public static int[] DoStuff(string s)
        {
            int[] ia = new int[s.Length];

            byte[] asciiBytes = Encoding.ASCII.GetBytes(s);
            Random rnd = new Random();

            for (int x = 0; x < asciiBytes.Length; x++)
            {
                int asciiSymbol = asciiBytes[x];
                _ = Enumerable.Range(1, 1000).Sum();
                //just doing some bogus mathematical calculations to simulate work
                ia[x] = (int)(Math.Sqrt(Math.Log(asciiSymbol) % Math.Log10(asciiSymbol)) * Math.Log(Math.Log10(asciiSymbol) / Math.Sqrt(asciiSymbol))
                    / (Math.Sqrt(Math.Log(asciiSymbol) % Math.Log10(asciiSymbol)) * Math.Sqrt(Math.Log(asciiSymbol) % Math.Log2(asciiSymbol))) * rnd.Next(-20, 20) * 100);
            }

            return ia;
        }
    }
}
