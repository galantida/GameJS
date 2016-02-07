using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameJS
{
    public static class util
    {
        public static void swap(ref int a, ref int b) {
            int q = a;
            a = b;
            b = q;
        }

        public static void sort(ref int a, ref int b)
        {
            if (a > b) util.swap(ref a, ref b);
        }
    }
}