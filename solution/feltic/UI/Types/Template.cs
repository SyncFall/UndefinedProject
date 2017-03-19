using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public enum TemplateType
    {
        Surface,
        Layout,
        Design,
        Root,
    }

    public class Template
    {
        public TemplateType Type;
        public Surface Surface;
        public Layout Layout;
        public Design Design;
        public ListCollection<Template> Childrens;

        public Template(TemplateType Type)
        {
            this.Type = Type;
        }
    }
}
