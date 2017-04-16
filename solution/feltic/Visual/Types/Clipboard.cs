using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual.Types
{
    public class Clipboard
    {
        public static string GetText()
        {
            return System.Windows.Clipboard.GetText(System.Windows.TextDataFormat.UnicodeText);
        }

        public static void SetText(string text)
        {
            System.Windows.Clipboard.SetText(text);
        }
    }
}
