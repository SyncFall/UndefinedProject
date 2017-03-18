using System;
using System.IO;
using System.Collections;
using feltic.Language;
using feltic.UI;
using feltic.UI.Types;
using feltic.Integrator;

namespace Scope
{
	public class Visual1
	{
		public VisualElement Element;

		public Visual1()
		{
			Stack stack = new Stack();
			VisualElement element, parent=null;

			this.Element = 			element = new VisualElement(VisualElementType.Compose, parent);
			element.RoomFromDefinition.Width = new Way(WayType.Pixel, 700f);
			element.RoomFromDefinition.Height = new Way(WayType.Pixel, 500f);
			stack.Push(parent);
			parent = element;
		}
	}

}
