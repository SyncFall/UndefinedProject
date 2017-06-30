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
	public class Test : VisualElement
	{

		public Test()
		{
						this.content = new Visual_1_Test_Test(this);
		}
	}


	public class Visual_1_Test_Test : VisualElement
	{
		public Test Object;

		public Visual_1_Test_Test(Test Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualScroll()));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(175);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(150);
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(300);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(75);
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(200);
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(75);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(50);
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(100);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(50);
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(25);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(25);
			parent = stack.Pop();
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(125);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(75);
			parent = stack.Pop();

		}
	}


}
