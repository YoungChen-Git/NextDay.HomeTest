namespace NextDay.HomeTest;

public class NextDateHandler
{
    /// <summary>
    /// 判斷是否為閏年
    /// 閏年規則：
    /// 1. 能被4整除的年份是閏年
    /// 2. 但能被100整除的年份不是閏年
    /// 3. 但能被400整除的年份是閏年
    /// </summary>
    /// <param name="year">年份</param>
    /// <returns>是否為閏年</returns>
    private bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }

    public DateOnly Next(DateOnly dateOnly)
    {
        var year = dateOnly.Year;
        var month = dateOnly.Month;
        var day = dateOnly.Day;

        // 獲取當月的總天數
        var daysInCurrentMonth = GetDaysInMonth(year, month);

        // 如果不是月底，直接加一天
        if (day < daysInCurrentMonth)
        {
            return new DateOnly(year, month, day + 1);
        }

        // 月底情況：需要進入下個月
        if (month < 12)
        {
            // 不是12月，進入下個月的第一天
            return new DateOnly(year, month + 1, 1);
        }
        else
        {
            // 12月31日，進入下一年的1月1日
            return new DateOnly(year + 1, 1, 1);
        }
    }

    /// <summary>
    /// 計算兩個日期之間的天數
    /// </summary>
    /// <param name="startDate">起始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <returns>兩個日期之間的天數差（結束日期 - 起始日期）</returns>
    public int DaysBetween(DateOnly startDate, DateOnly endDate)
    {
        // 如果起始日期等於結束日期，返回0
        if (startDate == endDate)
        {
            return 0;
        }

        // 確保起始日期小於結束日期，如果不是則交換
        var isNegative = startDate > endDate;
        if (isNegative)
        {
            (startDate, endDate) = (endDate, startDate);
        }

        var daysBetween = 0;
        var currentDate = startDate;

        // 逐日計算直到達到結束日期
        while (currentDate < endDate)
        {
            currentDate = Next(currentDate);
            daysBetween++;
        }

        return daysBetween;
    }

    /// <summary>
    /// 獲取指定月份的天數
    /// </summary>
    /// <param name="year">年份</param>
    /// <param name="month">月份</param>
    /// <returns>該月份的天數</returns>
    private int GetDaysInMonth(int year, int month)
    {
        return month switch
        {
            2 => IsLeapYear(year) ? 29 : 28,
            4 or 6 or 9 or 11 => 30,
            _ => 31
        };
    }

    /// <summary>
    /// 計算兩個日期之間的天數（優化版本）
    /// 使用年、月、日的計算來提高效能，適用於較大的日期範圍
    /// </summary>
    /// <param name="startDate">起始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <returns>兩個日期之間的天數差（結束日期 - 起始日期）</returns>
    public int DaysBetweenOptimized(DateOnly startDate, DateOnly endDate)
    {
        // 如果起始日期等於結束日期，返回0
        if (startDate == endDate)
        {
            return 0;
        }

        // 確保起始日期小於結束日期，如果不是則交換
        var isNegative = startDate > endDate;
        if (isNegative)
        {
            (startDate, endDate) = (endDate, startDate);
        }

        var totalDays = 0;
        var currentYear = startDate.Year;
        var currentMonth = startDate.Month;
        var currentDay = startDate.Day;

        // 如果是同一年
        if (startDate.Year == endDate.Year)
        {
            // 如果是同一個月
            if (startDate.Month == endDate.Month)
            {
                totalDays = endDate.Day - startDate.Day;
            }
            else
            {
                // 計算從起始日期到當月底的天數
                totalDays += GetDaysInMonth(currentYear, currentMonth) - currentDay;
                currentMonth++;

                // 計算完整月份的天數
                while (currentMonth < endDate.Month)
                {
                    totalDays += GetDaysInMonth(currentYear, currentMonth);
                    currentMonth++;
                }

                // 加上結束月份的天數
                totalDays += endDate.Day;
            }
        }
        else
        {
            // 計算從起始日期到當年底的天數
            // 先計算到當月底
            totalDays += GetDaysInMonth(currentYear, currentMonth) - currentDay;
            currentMonth++;

            // 計算當年剩餘月份
            while (currentMonth <= 12)
            {
                totalDays += GetDaysInMonth(currentYear, currentMonth);
                currentMonth++;
            }

            currentYear++;

            // 計算完整年份的天數
            while (currentYear < endDate.Year)
            {
                totalDays += IsLeapYear(currentYear) ? 366 : 365;
                currentYear++;
            }

            // 計算結束年份到結束日期的天數
            currentMonth = 1;
            while (currentMonth < endDate.Month)
            {
                totalDays += GetDaysInMonth(endDate.Year, currentMonth);
                currentMonth++;
            }

            // 加上結束月份的天數
            totalDays += endDate.Day;
        }

        return totalDays;
    }
}