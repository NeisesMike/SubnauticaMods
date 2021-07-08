using System;
using System.Collections.Generic;
using System.Text;

namespace BZCommon.Helpers
{
    public static class ArrayUtils
    {
        public static void Add<T>(ref T[] array, T newItem)
        {
            int newSize = array.Length + 1;
            Array.Resize(ref array, newSize);
            array[newSize - 1] = newItem;
        }

    }
}
