using System;

namespace NUnit.Framework
{
    public delegate TActual ReturnsTestDelegate<TActual>();
    public delegate object ReturnsTestDelegate();

    static class CustomAssert
    {
        public static Exception CatchOrNull(ReturnsTestDelegate code)
        {
            return Assert.Catch(() =>
            {
                var value = code();
                if (value == null)
                    throw new ArgumentNullException();
            });
        }
    }
}
