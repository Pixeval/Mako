using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Mako.Util
{
    [PublicAPI]
    public record Result
    {
        [PublicAPI]
        public record Success<T> : Result
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
        public record Failure : Result
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
        public static Result OfSuccess<T>(T value) => new Success<T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result OfFailure(Exception? cause = null) => new Failure(cause);
    }
}