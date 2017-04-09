using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using feltic.Language;
using feltic.Library;
using feltic.UI;
using feltic.UI.Types;
using feltic.Integrator;

namespace Scope
{
	public class Option : VisualObject
	{
		public Select select;
		public VisualObject elm;

		public Option(Select select, string val, string txt)
		{
						this.select = select;
						this.build(txt);
		}
		public void build(string txt)
		{
						elm = new Visual_1_Option(this, txt)
;
						select.list.add(elm);
						Visual = elm.Visual;
		}
	}

	public class Select : VisualObject
	{
		public VisualObject elm;
		public VisualObject view;
		public VisualObject list;
		public int select;

		public Select()
		{
						this.build();
		}
		public void build()
		{
						elm = new Visual_2_Select(this)
;
						view = new Visual_3_Select(this)
;
						list = new Visual_4_Select(this)
;
						elm.add(view, list);
						Visual = elm.Visual;
		}
		public void selectOption(int index)
		{
						select = index;
						view.Childrens.Clear();
						new VisualTextElement("option-" + select, view.Visual);
						list.display = false;
						this.selectedOption(select);
		}
		public ReceiverContainer_1_Select_selectedOption RC_1_Select_selectedOption = new ReceiverContainer_1_Select_selectedOption();
		public void selectedOption(int index)
		{
			RC_1_Select_selectedOption.EventState(index);
		}
		public void elmListener(InputEvent inevt)
		{
			bool lostFocus = (inevt.Visual.LostFocus);
			if(lostFocus)
			{
								list.display = false;
			}
		}
		public void viewListener(InputEvent inevt)
		{
			bool showList = (inevt.Button.LeftClick);
			if(showList)
			{
								list.display = true;
			}
		}
		public void listListener(InputEvent inevt)
		{
			bool select = (inevt.Button.LeftClick);
			if(select)
			{
				for(int i = 0;i < list.Size;i++)
				{
					if(GeometryUtils.IntersectVisual(list[i], Input.Cursor))
					{
												this.selectOption(i);
												break;
					}
				}
			}
		}
	}

	public class Editor : VisualObject
	{
		public Select select;

		public Editor()
		{
						select = new Select();
						new TargetState_1_Select_selectedOption(select.RC_1_Select_selectedOption, this);
						new Option(select, "val1", "text1");
						new Option(select, "val2", "text2");
						new Option(select, "val3", "text3");
						Visual = select.Visual;
		}
	}


	public class Visual_1_Option : VisualObject
	{
		public Option Object;

		public Visual_1_Option(Option Object, string txt)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement(txt, parent);

		}
	}


	public class Visual_2_Select : VisualObject
	{
		public Select Object;

		public Visual_2_Select(Select Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(2, parent);
			listeners.Add(element);
			element.Room.Width = new Way(1, 100f);

			new Visual_Listener_1_Select_elmListener(Object, listeners[0]);
		}
	}

	public class Visual_Listener_1_Select_elmListener : VisualListener
	{
		public Select Object;

		public Visual_Listener_1_Select_elmListener(Select Object, VisualElement Element) : base(Element)
		{
			this.Object = Object;
			this.Element.InputListener = this;
		}

		public override void Event(InputEvent Event){
			this.Object.elmListener(Event);
		}
	}

	public class Visual_3_Select : VisualObject
	{
		public Select Object;

		public Visual_3_Select(Select Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);
			listeners.Add(element);
			stack.Push(parent);
			parent = element;
			element = new VisualTextElement("- Select -", parent);

			new Visual_Listener_2_Select_viewListener(Object, listeners[0]);
		}
	}

	public class Visual_Listener_2_Select_viewListener : VisualListener
	{
		public Select Object;

		public Visual_Listener_2_Select_viewListener(Select Object, VisualElement Element) : base(Element)
		{
			this.Object = Object;
			this.Element.InputListener = this;
		}

		public override void Event(InputEvent Event){
			this.Object.viewListener(Event);
		}
	}

	public class Visual_4_Select : VisualObject
	{
		public Select Object;

		public Visual_4_Select(Select Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);
			listeners.Add(element);
			element.Display = false;

			new Visual_Listener_3_Select_listListener(Object, listeners[0]);
		}
	}

	public class Visual_Listener_3_Select_listListener : VisualListener
	{
		public Select Object;

		public Visual_Listener_3_Select_listListener(Select Object, VisualElement Element) : base(Element)
		{
			this.Object = Object;
			this.Element.InputListener = this;
		}

		public override void Event(InputEvent Event){
			this.Object.listListener(Event);
		}
	}

	public class ReceiverContainer_1_Select_selectedOption
	{
		public MapCollection<int, StateReceiver_1_Select_selectedOption> Receivers = new MapCollection<int, StateReceiver_1_Select_selectedOption>();

		public void EventState(int index)
		{
			int[] keys = Receivers.Keys;
			for(int i=0; i<keys.Length; i++){
				Receivers[keys[i]].EventState(index);
			}
		}
	}
	public abstract class StateReceiver_1_Select_selectedOption
	{
		private static int IdCounter = 0;
		public readonly int Id;
		public ReceiverContainer_1_Select_selectedOption Container;

		public StateReceiver_1_Select_selectedOption(ReceiverContainer_1_Select_selectedOption Container){
			this.Container = Container;
			this.Id = (++IdCounter);
			this.Container.Receivers.Put(this.Id, this);
		}
		public abstract void EventState(int index);
	}
	public class TargetState_1_Select_selectedOption : StateReceiver_1_Select_selectedOption
	{
		public Editor Object;

		public TargetState_1_Select_selectedOption(ReceiverContainer_1_Select_selectedOption Container, Editor Object) : base(Container){
			this.Container = Container;
			this.Object = Object;
		}
		public override void EventState(int index){
						Console.WriteLine("/selected");
		}
	}

}
