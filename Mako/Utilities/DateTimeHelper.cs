using System;

namespace Mako.Utilities;

public static class DateTimeHelper
{
    public static DateTimeOffset ToJapanTime(this DateTimeOffset dateTimeOffset)
        => dateTimeOffset.ToOffset(TimeSpan.FromHours(9));

    public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)
        => DateOnly.FromDateTime(dateTimeOffset.DateTime);

    public static DateOnly JapanToday => DateTimeOffset.Now.ToJapanTime().ToDateOnly();
}
