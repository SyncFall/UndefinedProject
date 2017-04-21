using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using feltic.Language;
using feltic.Library;
using feltic.Visual;
using feltic.Integrator;

namespace Scope
{
	public class Editor : VisualElement
	{

		public Editor()
		{
			string str = File.ReadAllText("csharp.cs");
						this.content = new Visual_1_Editor_Editor(this, str);
		}
	}


	public class Visual_1_Editor_Editor : VisualElement
	{
		public Editor Object;

		public Visual_1_Editor_Editor(Editor Object, string str) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualScrollElement()));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(500);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(500);
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualText(str)));
			parent = stack.Pop();

		}
	}


}
