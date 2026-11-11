namespace Modsen.Portal.Core.Utils.Extensions;

public static class CollectionExtensions
{
    public static bool AreEquivalentIgnoreOrdering<T>(IEnumerable<T> first, IEnumerable<T> second) where T : struct
    {
        var firstArr = first.ToArray();
        var secondArr = second.ToArray();

        return !firstArr.Except(secondArr).Any() && !secondArr.Except(firstArr).Any();
    }

    public static bool IsConsecutive(this IEnumerable<int> numbers)
    {
        var values = numbers.ToArray();
        return values.SequenceEqual(Enumerable.Range(values[0], values.Length));
    }
}
