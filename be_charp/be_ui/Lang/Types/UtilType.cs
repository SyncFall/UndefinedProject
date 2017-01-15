using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class UtilType
    {
        public static bool EqualString(string a, string b)
        {
            if(a == null && b == null)
            {
                return true;
            }
            else if(a == null && b != null)
            {
                return false;
            }
            else if(a != null && b == null)
            {
                return false;
            }
            else if (a.Equals(b))
            {
                return true;
            }
            return false;
        }

        public static bool EqualNullObject(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            else if (a == null && b != null)
            {
                return false;
            }
            else if (a != null && b == null)
            {
                return false;
            }
            return true;
        }
    }
}
