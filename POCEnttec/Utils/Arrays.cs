using System;

namespace POCEnttec.Utils
{
    public static class Arrays
    {
        public static TOut[] Select<TIn, TOut>(this TIn[] array, Func<TIn, TOut> selector)
        {
            int length = array.Length;
            TOut[] result = new TOut[length];

            for (int i = 0; i < length; i++)
                result[i] = selector(array[i]);

            return result;
        }
    }
}