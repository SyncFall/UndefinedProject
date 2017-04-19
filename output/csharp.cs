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
						this.content = new Visual_1_Editor_Editor(this);
		}
	}


	public class Visual_1_Editor_Editor : VisualElement
	{
		public Editor Object;

		public Visual_1_Editor_Editor(Editor Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(1)));
			if(element.Size == null) element.Size = new Room();
			element.Size.Width = Way.Try(100);
			if(element.Size == null) element.Size = new Room();
			element.Size.Height = Way.Try(100);
			parent.add((element = new VisualElement(1)));
			if(element.Size == null) element.Size = new Room();
			element.Size.Width = Way.Try(100);
			if(element.Size == null) element.Size = new Room();
			element.Size.Height = Way.Try(100);
			parent = stack.Pop();

		}
	}


}
