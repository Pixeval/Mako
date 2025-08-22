// Copyright (c) Pixeval.CoreApi.
// Licensed under the GPL v3 License.

namespace Mako.Global.Exception;

public class DateOutOfRangeException : MakoException
{
    public DateOutOfRangeException()
    {
    }

    public DateOutOfRangeException(string? message) : base(message)
    {
    }

    public DateOutOfRangeException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
}
