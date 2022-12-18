namespace Common
{
    public class Worker
    {
        public static int[] DoStuff(string s)
        {
            string[] sa = s.Split(new char[' ']);
            int[] ia = new int[sa.Length];
            string charSymbol;
            for (int x = 0; x < sa.Length; x++)
            {
                foreach (char c in sa[x])
                {
                    charSymbol = c.ToString();
                    if (int.TryParse(charSymbol, out int num))
                    {
                        //just doing some bogus mathematical calculations to simulate work
                        ia[x] = Enumerable.Range(1, 1000).Sum() + (int)((Math.Sqrt(Math.Log(num) % Math.Log10(num)) * Math.Log(Math.Log10(num) / Math.Sqrt(num)))
                            / (Math.Sqrt(Math.Log(num) % Math.Log10(num)) * Math.Sqrt(Math.Log(num) % Math.Log2(num))));
                    }
                }
            }

            //clean up
            Array.Clear(sa, 0, sa.Length);

            return ia;
        }
    }
}
