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
	public class Node : VisualElement
	{
		public VisualElement list;

		public Node(VisualElement parent, string dir, int depth)
		{
						this.buildFolder(parent, dir, depth);
		}
		public void buildFolder(VisualElement parent, string dir, int depth)
		{
			int margin1 = depth * 20;
			int margin2 = margin1 + 20;
			string []folderArr = Directory.GetDirectories(dir);
			string folder;
			for(int i = 0;i < folderArr.Length;i++)
			{
								folder = folderArr[i].Substring(dir.Length + 1);
								parent.add(new Visual_1_Node(this, margin2, folder));
								new Node(parent, dir + "/" + folder, depth + 1);
			}
			string []fileArr = Directory.GetFiles(dir);
			string file;
			for(int i = 0;i < fileArr.Length;i++)
			{
								file = fileArr[i].Substring(dir.Length + 1);
								parent.add(new Visual_2_Node(this, margin2, file));
			}
		}
		public void Toggle(InputEvent inevt)
		{
			bool toogleList = (inevt.Button.LeftClick);
			if(list != null && toogleList)
			{
								list.display = !list.display;
			}
		}
	}

	public class Test : VisualElement
	{

		public Test()
		{
			VisualElement root = new Visual_3_Test_Test(this);
						new Node(root, ".", 0);
			VisualElement cnt = new Visual_4_Test_Test(this);
						cnt.Nodes[0].add(root);
						this.content = cnt;
		}
	}


	public class Visual_1_Node : VisualElement
	{
		public Node Object;

		public Visual_1_Node(Node Object, int margin2, string folder) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			element.Margin = Spacing.Combine(element.Margin, new Spacing(Way.Try(margin2), null, null, null));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualImage()));
			element.source = "folder.png";
			element.Margin = Spacing.Combine(element.Margin, new Spacing(null, null, Way.Try(10), null));
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(20);
			parent.add((element = new VisualText(folder)));
			parent.add((element = new VisualElement(8)));

		}
	}


	public class Visual_2_Node : VisualElement
	{
		public Node Object;

		public Visual_2_Node(Node Object, int margin2, string file) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			element.Margin = Spacing.Combine(element.Margin, new Spacing(Way.Try(margin2), null, null, null));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualImage()));
			element.source = "file.png";
			element.Margin = Spacing.Combine(element.Margin, new Spacing(null, null, Way.Try(10), null));
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(20);
			parent.add((element = new VisualText(file)));
			parent.add((element = new VisualElement(8)));

		}
	}


	public class Visual_3_Test_Test : VisualElement
	{
		public Test Object;

		public Visual_3_Test_Test(Test Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));

		}
	}


	public class Visual_4_Test_Test : VisualElement
	{
		public Test Object;

		public Visual_4_Test_Test(Test Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualScroll()));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try(150);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(300);

		}
	}


}
