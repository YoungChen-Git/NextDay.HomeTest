namespace NextDay.HomeTest;

internal class NextDateHandler
{
    private readonly HashSet<int> _longMonths = new()
    {
        1, 3, 5, 7, 8, 10, 12
    };

    public DateOnly Next(DateOnly dateOnly)
    {
        var dateOnlyYear = dateOnly.Year;
        var dateOnlyMonth = dateOnly.Month;
        var dateOnlyDay = dateOnly.Day;
        var isLarge = _longMonths.Contains(dateOnlyMonth);

        if (dateOnlyMonth == 12 && dateOnlyDay == 31)
        {
            dateOnlyYear += 1;
            dateOnlyMonth = 1;
            dateOnlyDay = 1;
        }
        else if (dateOnlyMonth == 2 && dateOnlyDay == 28)
        {
            dateOnlyMonth += 1;
            dateOnlyDay = 1;
        }
        else if (isLarge && dateOnlyDay + 1 > 31)
        {
            dateOnlyMonth += 1;
            dateOnlyDay = 1;
        }
        else if (!isLarge && dateOnlyDay + 1 > 30)
        {
            dateOnlyMonth += 1;
            dateOnlyDay = 1;
        }
        else
        {
            dateOnlyDay += 1;
        }

        return new DateOnly(dateOnlyYear, dateOnlyMonth, dateOnlyDay);
    }
}