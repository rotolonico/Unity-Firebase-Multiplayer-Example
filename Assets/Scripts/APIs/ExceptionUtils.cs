using System;

namespace APIs
{
    public static class ExceptionUtils
    {
        private static Exception GetInnerException(this Exception exception)
        {
            while (true)
            {
                if (exception.InnerException == null) return exception;
                exception = exception.InnerException;
            }
        }
    }
}