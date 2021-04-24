using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenManager.App.Utility
{
    public static class HashHelpers
    {
        /// <summary>
        /// https://stackoverflow.com/a/670204
        /// Get order independent hash code
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetHashCodeOfList<T>(IEnumerable<T> list)
        {
            List<int> codes = list.Select(item => item.GetHashCode()).ToList();
            codes.Sort();
            int hash = 0;
            foreach (int code in codes)
            {
                unchecked
                {
                    hash *= 251;
                    hash += code;
                }
            }
            return hash;
        }
    }
}
