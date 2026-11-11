namespace Modsen.Portal.Utilities.Controllers.Fuzzy
{
    public static class DiceCoefficientExtensions
    {
        private const string SinglePercent = "%";
        private const string SinglePound = "#";

        public static double DiceCoefficient(this string? input, string comparedTo)
        {
            var ngrams = (input ?? string.Empty).ToBiGrams();
            var compareToNgrams = comparedTo.ToBiGrams();

            return ngrams.DiceCoefficient(compareToNgrams);
        }

        public static double DiceCoefficient(this string[]? nGrams, string[]? compareToNGrams)
        {
            if (nGrams == null || compareToNGrams == null)
            {
                return 0d;
            }

            var matches = nGrams.Count(nGram => compareToNGrams.Any(x => x == nGram));
            if (matches == 0)
            {
                return 0.0d;
            }

            double totalBigrams = nGrams.Length + compareToNGrams.Length;

            return 2 * matches / totalBigrams;
        }

        public static string[] ToBiGrams(this string input)
        {
            input = SinglePercent + input + SinglePound;

            return ToNGrams(input, 2);
        }

        private static string[] ToNGrams(string input, int nLength)
        {
            var itemsCount = input.Length - 1;
            var ngrams = new string[input.Length - 1];
           
            for (var i = 0; i < itemsCount; i++)
            {
                ngrams[i] = input.Substring(i, nLength);
            }

            return ngrams;
        }
    }
}
