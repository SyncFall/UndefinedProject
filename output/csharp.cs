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
		public ActionBar actionBar = new ActionBar();
		public Workspace workspace = new Workspace();
		public VisualObject Root;
		public VisualObject Container;
		public bool doLoop = true;

		public Editor()
		{
						Container = new Visual_1_Editor_Editor(this)
;
						Root = new Visual_2_Editor_Editor(this)
;
		}
	}

	public class ActionBar
	{
		public Select solutionSelect = new Select(".solution");
		public Select runSelect = new Select(".run");
		public VisualObject Root;

		public ActionBar()
		{
						Root = new Visual_3_ActionBar_ActionBar(this)
;
		}
	}

	public class Workspace
	{
		public VisualObject Root;

		public Workspace()
		{
						Root = new Visual_4_Workspace_Workspace(this)
;
		}
	}

	public class Select
	{
		public string Text;
		public VisualObject Root;

		public Select(string Param)
		{
						Root = new Visual_5_Select_Select(this, Param)
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
			element.Room.Height = new Way(1, 50f);
			stack.Push(parent);
			parent = element;
			for(int i = 0;i < 10;i++)
			{
								new VisualTextElement("pie/mue", parent);
			}
		}
	}


	public class Visual_2_Editor_Editor : VisualObject
	{
		public Editor Object;

		public Visual_2_Editor_Editor(Editor Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(2, parent);
			element.Room.Width = new Way(1, 700f);
			element.Room.Height = new Way(1, 500f);
			stack.Push(parent);
			parent = element;
			element = new VisualElement(2, parent);
			stack.Push(parent);
			parent = element;
			element.AddChild(Object.actionBar.Root.Visual);
			parent = stack.Pop();
			element = new VisualElement(2, parent);
			element.Room.Height = new Way(1, 400f);
			stack.Push(parent);
			parent = element;
			element = new VisualElement(4, parent);
			element.Room.Width = new Way(3, 0.65f);
			stack.Push(parent);
			parent = element;
			element.AddChild(Object.workspace.Root.Visual);
			parent = stack.Pop();
			element = new VisualElement(4, parent);
			element.Room.Width = new Way(3, 0.35f);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement("navigation", parent);
			parent = stack.Pop();
			parent = stack.Pop();
			element = new VisualElement(2, parent);
			element.Room.Height = new Way(1, 125f);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement("status", parent);
			parent = stack.Pop();
			element = new VisualElement(2, parent);
			stack.Push(parent);
			parent = element;
			element.AddChild(Object.Container.Visual);
			parent = stack.Pop();
		}
	}


	public class Visual_3_ActionBar_ActionBar : VisualObject
	{
		public ActionBar Object;

		public Visual_3_ActionBar_ActionBar(ActionBar Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(2, parent);
			stack.Push(parent);
			parent = element;
			element.AddChild(Object.solutionSelect.Root.Visual);
			element.AddChild(Object.runSelect.Root.Visual);
		}
	}


	public class Visual_4_Workspace_Workspace : VisualObject
	{
		public Workspace Object;

		public Visual_4_Workspace_Workspace(Workspace Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualScrollElement(parent);
		}
	}


	public class Visual_5_Select_Select : VisualObject
	{
		public Select Object;

		public Visual_5_Select_Select(Select Object, string Param)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(3, parent);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement(Param, parent);
		}
	}


}
