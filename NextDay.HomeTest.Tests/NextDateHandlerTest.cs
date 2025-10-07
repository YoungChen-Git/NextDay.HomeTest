using NextDay.HomeTest;

namespace NextDay.HomeTest.Tests;

public class NextDateHandlerTests
{
    private readonly NextDateHandler _nextDateHandler;

    public NextDateHandlerTests()
    {
        _nextDateHandler = new NextDateHandler();
    }

    [Theory]
    [InlineData(2024, 1, 15, 2024, 1, 16)] // 一般日期
    [InlineData(2024, 1, 31, 2024, 2, 1)]  // 1月最後一天 (長月)
    [InlineData(2024, 4, 30, 2024, 5, 1)]  // 4月最後一天 (短月)
    [InlineData(2024, 12, 31, 2025, 1, 1)] // 年底
    public void Next_NormalDates_ShouldReturnCorrectNextDay(int year, int month, int day, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Arrange
        var inputDate = new DateOnly(year, month, day);
        var expectedDate = new DateOnly(expectedYear, expectedMonth, expectedDay);

        // Act
        var result = _nextDateHandler.Next(inputDate);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Theory]
    [InlineData(2024, 2, 28, 2024, 2, 29)] // 閏年2月28日 -> 2月29日
    [InlineData(2024, 2, 29, 2024, 3, 1)]  // 閏年2月29日 -> 3月1日
    [InlineData(2023, 2, 28, 2023, 3, 1)]  // 平年2月28日 -> 3月1日
    public void Next_February_ShouldHandleLeapYearCorrectly(int year, int month, int day, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Arrange
        var inputDate = new DateOnly(year, month, day);
        var expectedDate = new DateOnly(expectedYear, expectedMonth, expectedDay);

        // Act
        var result = _nextDateHandler.Next(inputDate);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Theory]
    [InlineData(2000, 2, 29, 2000, 3, 1)]  // 2000年是閏年 (能被400整除)
    [InlineData(1900, 2, 28, 1900, 3, 1)]  // 1900年不是閏年 (能被100整除但不能被400整除)
    [InlineData(2004, 2, 29, 2004, 3, 1)]  // 2004年是閏年 (能被4整除但不能被100整除)
    public void Next_LeapYearRules_ShouldBeAppliedCorrectly(int year, int month, int day, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Arrange
        var inputDate = new DateOnly(year, month, day);
        var expectedDate = new DateOnly(expectedYear, expectedMonth, expectedDay);

        // Act
        var result = _nextDateHandler.Next(inputDate);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Theory]
    [InlineData(1, 31, 2, 1)]   // 1月 (長月)
    [InlineData(3, 31, 4, 1)]   // 3月 (長月)
    [InlineData(5, 31, 6, 1)]   // 5月 (長月)
    [InlineData(7, 31, 8, 1)]   // 7月 (長月)
    [InlineData(8, 31, 9, 1)]   // 8月 (長月)
    [InlineData(10, 31, 11, 1)] // 10月 (長月)
    [InlineData(12, 31, 1, 1)]  // 12月 (長月) - 特殊情況，跨年
    public void Next_LongMonths_ShouldTransitionCorrectly(int month, int day, int expectedMonth, int expectedDay)
    {
        // Arrange
        var year = 2024;
        var expectedYear = month == 12 ? year + 1 : year;
        var inputDate = new DateOnly(year, month, day);
        var expectedDate = new DateOnly(expectedYear, expectedMonth, expectedDay);

        // Act
        var result = _nextDateHandler.Next(inputDate);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Theory]
    [InlineData(4, 30, 5, 1)]   // 4月 (短月)
    [InlineData(6, 30, 7, 1)]   // 6月 (短月)
    [InlineData(9, 30, 10, 1)]  // 9月 (短月)
    [InlineData(11, 30, 12, 1)] // 11月 (短月)
    public void Next_ShortMonths_ShouldTransitionCorrectly(int month, int day, int expectedMonth, int expectedDay)
    {
        // Arrange
        var year = 2024;
        var inputDate = new DateOnly(year, month, day);
        var expectedDate = new DateOnly(year, expectedMonth, expectedDay);

        // Act
        var result = _nextDateHandler.Next(inputDate);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void Next_February29InLeapYear_ShouldTransitionToMarch1()
    {
        // Arrange - 測試閏年2月29日
        var inputDate = new DateOnly(2024, 2, 29);
        var expectedDate = new DateOnly(2024, 3, 1);

        // Act
        var result = _nextDateHandler.Next(inputDate);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void Next_YearEnd_ShouldTransitionToNextYear()
    {
        // Arrange - 測試年末最後一天
        var inputDate = new DateOnly(2024, 12, 31);
        var expectedDate = new DateOnly(2025, 1, 1);

        // Act
        var result = _nextDateHandler.Next(inputDate);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    #region DaysBetween Tests

    [Theory]
    [InlineData("2024-01-01", "2024-01-01", 0)]     // 同一天
    [InlineData("2024-01-01", "2024-01-02", 1)]     // 相鄰兩天
    [InlineData("2024-01-01", "2024-01-31", 30)]    // 同一個月
    [InlineData("2024-01-31", "2024-02-01", 1)]     // 跨月
    [InlineData("2024-12-31", "2025-01-01", 1)]     // 跨年
    [InlineData("2024-02-28", "2024-03-01", 2)]     // 閏年2月到3月
    [InlineData("2023-02-28", "2023-03-01", 1)]     // 平年2月到3月
    public void DaysBetween_VariousDates_ShouldReturnCorrectDays(string startDateStr, string endDateStr, int expectedDays)
    {
        // Arrange
        var startDate = DateOnly.Parse(startDateStr);
        var endDate = DateOnly.Parse(endDateStr);

        // Act
        var result = _nextDateHandler.DaysBetween(startDate, endDate);

        // Assert
        Assert.Equal(expectedDays, result);
    }

    [Theory]
    [InlineData("2024-01-02", "2024-01-01", 1)]    // 反向日期
    [InlineData("2024-02-01", "2024-01-31", 1)]    // 反向跨月
    [InlineData("2025-01-01", "2024-12-31", 1)]    // 反向跨年
    public void DaysBetween_ReverseDates_ShouldReturnNegativeDays(string startDateStr, string endDateStr, int expectedDays)
    {
        // Arrange
        var startDate = DateOnly.Parse(startDateStr);
        var endDate = DateOnly.Parse(endDateStr);

        // Act
        var result = _nextDateHandler.DaysBetween(startDate, endDate);

        // Assert
        Assert.Equal(expectedDays, result);
    }

    [Theory]
    [InlineData("2024-01-01", "2024-12-31", 365)]   // 閏年整年 (366天 - 1)
    [InlineData("2023-01-01", "2023-12-31", 364)]   // 平年整年 (365天 - 1)
    [InlineData("2024-02-01", "2024-02-29", 28)]    // 閏年2月整月 (29天 - 1)
    [InlineData("2023-02-01", "2023-02-28", 27)]    // 平年2月整月 (28天 - 1)
    public void DaysBetween_LongPeriods_ShouldHandleLeapYears(string startDateStr, string endDateStr, int expectedDays)
    {
        // Arrange
        var startDate = DateOnly.Parse(startDateStr);
        var endDate = DateOnly.Parse(endDateStr);

        // Act
        var result = _nextDateHandler.DaysBetween(startDate, endDate);

        // Assert
        Assert.Equal(expectedDays, result);
    }

    [Theory]
    [InlineData("2020-01-01", "2024-01-01", 1461)]  // 4年期間（包含一個閏年）
    [InlineData("1900-01-01", "2000-01-01", 36524)] // 100年期間
    public void DaysBetween_VeryLongPeriods_ShouldBeAccurate(string startDateStr, string endDateStr, int expectedDays)
    {
        // Arrange
        var startDate = DateOnly.Parse(startDateStr);
        var endDate = DateOnly.Parse(endDateStr);

        // Act
        var result = _nextDateHandler.DaysBetween(startDate, endDate);

        // Assert
        Assert.Equal(expectedDays, result);
    }

    #endregion

    #region DaysBetweenOptimized Tests

    [Theory]
    [InlineData("2024-01-01", "2024-01-01", 0)]     // 同一天
    [InlineData("2024-01-01", "2024-01-02", 1)]     // 相鄰兩天
    [InlineData("2024-01-01", "2024-01-31", 30)]    // 同一個月
    [InlineData("2024-01-31", "2024-02-01", 1)]     // 跨月
    [InlineData("2024-12-31", "2025-01-01", 1)]     // 跨年
    [InlineData("2024-02-28", "2024-03-01", 2)]     // 閏年2月到3月
    [InlineData("2023-02-28", "2023-03-01", 1)]     // 平年2月到3月
    public void DaysBetweenOptimized_VariousDates_ShouldReturnCorrectDays(string startDateStr, string endDateStr, int expectedDays)
    {
        // Arrange
        var startDate = DateOnly.Parse(startDateStr);
        var endDate = DateOnly.Parse(endDateStr);

        // Act
        var result = _nextDateHandler.DaysBetweenOptimized(startDate, endDate);

        // Assert
        Assert.Equal(expectedDays, result);
    }

    [Theory]
    [InlineData("2024-01-01", "2024-12-31", 365)]   // 閏年整年
    [InlineData("2023-01-01", "2023-12-31", 364)]   // 平年整年
    [InlineData("2020-01-01", "2024-01-01", 1461)]  // 4年期間
    public void DaysBetweenOptimized_LongPeriods_ShouldMatchStandardMethod(string startDateStr, string endDateStr, int expectedDays)
    {
        // Arrange
        var startDate = DateOnly.Parse(startDateStr);
        var endDate = DateOnly.Parse(endDateStr);

        // Act
        var standardResult = _nextDateHandler.DaysBetween(startDate, endDate);
        var optimizedResult = _nextDateHandler.DaysBetweenOptimized(startDate, endDate);

        // Assert
        Assert.Equal(expectedDays, standardResult);
        Assert.Equal(expectedDays, optimizedResult);
        Assert.Equal(standardResult, optimizedResult);
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void Next_PerformanceTest_ShouldBeEfficient()
    {
        // Arrange - 測試大量日期計算的效能
        var startDate = new DateOnly(2024, 1, 1);
        var iterations = 10000;

        // Act & Assert - 確保在合理時間內完成
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var currentDate = startDate;
        for (int i = 0; i < iterations; i++)
        {
            currentDate = _nextDateHandler.Next(currentDate);
        }
        
        stopwatch.Stop();
        
        // 驗證結果正確性
        var expectedDate = startDate.AddDays(iterations);
        Assert.Equal(expectedDate, currentDate);
        
        // 效能應該在合理範圍內（這裡設為1秒，實際會更快）
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
            $"Performance test took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
    }

    #endregion
}
