using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Types
{
    public abstract class Interact
    {
        public abstract void Pick();
        public abstract void Move();
        public abstract void Release();
        public abstract void Input();
    }
}
