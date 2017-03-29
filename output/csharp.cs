using System;
using System.IO;
using System.Collections;
using feltic.Language;
using feltic.UI;
using feltic.UI.Types;
using feltic.Integrator;

namespace Scope
{
	public class Editor
	{
		public ActionBar actionBar = new ActionBar();
		public Workspace workspace = new Workspace();
		public VisualObject Root;

		public Editor()
		{
						Root = new Visual0(this)
;
		}
	}

	public class ActionBar
	{
		public Select solutionSelect = new Select();
		public Select runSelect = new Select();
		public VisualObject Root;

		public ActionBar()
		{
						Root = new Visual1(this)
;
		}
	}

	public class Workspace
	{
		public VisualObject Root;

		public Workspace()
		{
						Root = new Visual2(this)
;
		}
	}

	public class Select
	{
		public string Text;
		public VisualObject Root;

		public Select(string Param)
		{
						Root = new Visual3(this)
;
		}
	}


	public class Visual0 : VisualObject
	{
		public Editor Object;

		public Visual0(Editor Object)
		{
			this.Object = Object;
			Stack stack = new Stack();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(2, parent);
			element.Room.Width = new Way(WayType.Pixel, 700f);
			element.Room.Height = new Way(WayType.Pixel, 500f);
			stack.Push(parent);
			parent = element;
			element = new VisualElement(2, parent);
			stack.Push(parent);
			parent = element;
element.AddChild(this.Object.actionBar.Root.Visual);
			parent = stack.Pop() as VisualElement;
			element = new VisualElement(2, parent);
			element.Room.Height = new Way(WayType.Pixel, 400f);
			stack.Push(parent);
			parent = element;
			element = new VisualElement(4, parent);
			element.Room.Width = new Way(WayType.Percent, 0.65f);
			stack.Push(parent);
			parent = element;
element.AddChild(this.Object.workspace.Root.Visual);
			parent = stack.Pop() as VisualElement;
			element = new VisualElement(4, parent);
			element.Room.Width = new Way(WayType.Percent, 0.35f);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement("navigation", parent);
			parent = stack.Pop() as VisualElement;
			parent = stack.Pop() as VisualElement;
			element = new VisualElement(2, parent);
			element.Room.Height = new Way(WayType.Pixel, 125f);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement("status", parent);
			parent = stack.Pop() as VisualElement;
		}
	}

	public class Visual1 : VisualObject
	{
		public ActionBar Object;

		public Visual1(ActionBar Object)
		{
			this.Object = Object;
			Stack stack = new Stack();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(2, parent);
			stack.Push(parent);
			parent = element;
			element = new VisualElement(3, parent);
			stack.Push(parent);
			parent = element;
element.AddChild(this.Object.solutionSelect.Root.Visual);
			parent = stack.Pop() as VisualElement;
			element = new VisualElement(3, parent);
			stack.Push(parent);
			parent = element;
element.AddChild(this.Object.runSelect.Root.Visual);
			parent = stack.Pop() as VisualElement;
		}
	}

	public class Visual2 : VisualObject
	{
		public Workspace Object;

		public Visual2(Workspace Object)
		{
			this.Object = Object;
			Stack stack = new Stack();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualScrollElement(parent);
			stack.Push(parent);
			parent = element;
			element = new VisualElement(2, parent);
			element.Room.Width = new Way(WayType.Pixel, 800f);
			element.Room.Height = new Way(WayType.Pixel, 1500f);
		}
	}

	public class Visual3 : VisualObject
	{
		public Select Object;

		public Visual3(Select Object)
		{
			this.Object = Object;
			Stack stack = new Stack();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(3, parent);
			stack.Push(parent);
			parent = element;
element.AddChild(this.Object.Param.Visual);
		}
	}

}
