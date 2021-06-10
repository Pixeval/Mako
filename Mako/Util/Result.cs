using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Mako.Util
{
    [PublicAPI]
    public readonly struct Result<T>
    {
        public T Value { get; }
        
        public bool IsSuccess { get; }

        public Result(T value, bool isSuccess)
        {
            Value = value;
            IsSuccess = isSuccess;
        }

        public void Deconstruct(out T value, out bool isSuccess)
        {
            value = Value;
            isSuccess = IsSuccess;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Success(T value) => new(value, true);

        public static readonly Result<T> Failure = new(default!, false);
    }
}