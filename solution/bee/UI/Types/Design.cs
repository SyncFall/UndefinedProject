using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.UI
{
    public enum DesignType
    {
        TextColor,
        BackgroundColor,
    }

    public class Design
    {
        public Color TextColor;
        public Color BackgroundColor;

        public Design()
        { }
    }
}
