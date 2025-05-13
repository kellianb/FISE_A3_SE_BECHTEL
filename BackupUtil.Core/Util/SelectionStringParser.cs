namespace BackupUtil.Core.Util;

public static class SelectionStringParser
{
    private const string ExpressionSeparator = ",";
    private const string RangeSeparator = "-";

    public static HashSet<int> Parse(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        string[] expressionParts = expression.Split(ExpressionSeparator);

        return expressionParts.Aggregate(new HashSet<int>(), (acc, next) =>
        {
            foreach (int id in ParsePart(next))
            {
                acc.Add(id);
            }

            return acc;
        });
    }

    private static List<int> ParsePart(string expressionPart)
    {
        List<int> result = [];

        if (expressionPart.Contains('-'))
        {
            string[] rangeBoundaries = expressionPart.Split(RangeSeparator);

            if (rangeBoundaries.Length == 2
                && int.TryParse(rangeBoundaries[0], out int rangeStart)
                && int.TryParse(rangeBoundaries[1], out int rangeEnd))
            {
                if (rangeStart > rangeEnd)
                {
                    result.Add(rangeStart);
                }
                else
                {
                    result.InsertRange(0, Enumerable.Range(rangeStart, rangeEnd - rangeStart + 1));
                }
            }
        }
        else if (int.TryParse(expressionPart, out int number))
        {
            result.Add(number);
        }

        return result;
    }
}
