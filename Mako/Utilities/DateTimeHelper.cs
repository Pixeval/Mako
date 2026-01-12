using System;

namespace Mako.Utilities;

public static class DateTimeHelper
{
    extension(DateTimeOffset dateTimeOffset)
    {
        public DateTimeOffset ToJapanTime()
            => dateTimeOffset.ToOffset(TimeSpan.FromHours(9));

        public DateOnly ToDateOnly()
            => DateOnly.FromDateTime(dateTimeOffset.DateTime);
    }

    public static DateOnly JapanToday => DateTimeOffset.Now.ToJapanTime().ToDateOnly();
}
