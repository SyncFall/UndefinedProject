using System;
using System.IO;
using System.Collections.Generic;
using feltic.Language;
using feltic.Library;
using feltic.UI;
using feltic.UI.Types;
using feltic.Integrator;

namespace Scope
{
	public class Editor
	{
		public VisualObject Root;
		public Select Select;

		public Editor()
		{
						Select = new Select();
						Root = new Visual_1_Editor_Editor(this)
;
		}
	}

	public class Select
	{
		public VisualObject Root;

		public Select()
		{
						Root = new Visual_2_Select_Select(this)
;
		}
	}


	public class Visual_1_Editor_Editor : VisualObject
	{
		public Editor Object;

		public Visual_1_Editor_Editor(Editor Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(2, parent);
			element.Room.Width = new Way(1, 250f);
			element.Room.Height = new Way(1, 250f);
			stack.Push(parent);
			parent = element;
			element.AddChild(Object.Select.Root.Visual);
		}
	}


	public class Visual_2_Select_Select : VisualObject
	{
		public Select Object;

		public Visual_2_Select_Select(Select Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(3, parent);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement("test", parent);
		}
	}


}
