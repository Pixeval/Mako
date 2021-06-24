using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Mako.Util
{
    [PublicAPI]
    public record Result<T>
    {
        [PublicAPI]
        public record Success : Result<T>
        {
            public T Value { get; }

            public void Deconstruct(out T value)
            {
                value = Value;
            }

            public Success(T value)
            {
                Value = value;
            }
        }

        [PublicAPI]
        public record Failure : Result<T>
        {
            public Exception? Cause { get; }

            public void Deconstruct([CanBeNull] out Exception? cause)
            {
                cause = Cause;
            }

            public Failure(Exception? cause)
            {
                Cause = cause;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> OfSuccess(T value) => new Success(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> OfFailure(Exception? cause = null) => new Failure(cause);
    }
}