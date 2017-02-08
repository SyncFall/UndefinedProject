using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Lib
{
    public class TimeUtils
    {
        public static long CurrentMilliseconds
        {
            get
            {
                return (DateTime.Now.Ticks / 10000);
            }
        }
    }
}
