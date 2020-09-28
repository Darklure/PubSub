using System;
using System.Collections.Generic;
using System.Text;

namespace PubSub.Utils
{
    public class ArrayUtil
    {

        public static bool IsDuplicate(string[] data, int currentIndex)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == data[currentIndex])
                {
                    return true;   
                }
            }

            return false;
        }
    }
}
