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
	public class FileNode : VisualElement
	{
		public FileNode parent;
		public VisualElement elm;
		public VisualElement list;
		public string path;
		public string name;
		public bool isFolder;
		public int depth;

		public FileNode(FileNode parent, string path, bool isFolder, int depth)
		{
						this.parent = parent;
						this.path = path;
			if(isFolder)
			{
				if(path.StartsWith(".\u005C"))
				{
										this.name = path.Substring(2);
				}
				else
				{
										this.name = path;
				}
			}
			else
			{
								this.name += Path.GetFileName(path);
			}
						this.isFolder = isFolder;
						this.depth = depth;
						this.build();
		}
		public void build()
		{
			int depthFolderWidth = (depth * 16);
			int depthFileWidth = (depthFolderWidth + 16);
			if(this.isFolder)
			{
								elm = new Visual_1_FileNode(this, depthFolderWidth, name);
								list = new Visual_2_FileNode(this);
								this.add(elm, list);
			}
			else
			{
								elm = new Visual_3_FileNode(this, depthFileWidth, name);
								this.add(elm);
			}
		}
		public void addEntry(FileNode child)
		{
						list.add(child);
						list.display = true;
		}
		public void elmListener(InputEvent inevt)
		{
			bool toogleList = (inevt.Button.LeftClick);
			if(list != null && toogleList)
			{
								list.display = !list.display;
			}
		}
	}

	public class FileExplorer : VisualElement
	{
		public FileNode RootNode;

		public FileExplorer()
		{
						RootNode = new FileNode(null, "ROOT", true, 0);
						this.buildFolder(".", RootNode, 0);
						this.content = new Visual_4_FileExplorer_FileExplorer(this, RootNode);
		}
		public void buildFolder(string dir, FileNode parent, int depth)
		{
			if(dir.Contains("\\."))
			{
								return ;
			}
			FileNode folder = new FileNode(parent, dir, true, depth);
			if(dir != ".")
			{
								parent.addEntry(folder);
								parent = folder;
			}
			string []dirArr = Directory.GetDirectories(dir);
			for(int i = 0;i < dirArr.Length;i++)
			{
								this.buildFolder(dirArr[i], parent, depth + 1);
			}
			string []fileArr = Directory.GetFiles(dir);
			for(int i = 0;i < fileArr.Length;i++)
			{
								parent.addEntry(new FileNode(parent, fileArr[i], false, depth));
			}
		}
	}

	public class Test : VisualElement
	{
		public FileExplorer explorer;

		public Test()
		{
						this.explorer = new FileExplorer();
						this.content = this.explorer;
		}
	}


	public class Visual_1_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_1_FileNode(FileNode Object, int depthFolderWidth, string name) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			listeners.Add(element);
			element.Margin = Spacing.Combine(element.Margin, new Spacing(Way.Try(depthFolderWidth), null, null, null));
			element.Padding = Spacing.Combine(element.Padding, new Spacing(null, Way.Try(2), null, null));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualText(name)));

			new Visual_Listener_1_FileNode_elmListener(Object, listeners[0]);
		}
	}

	public class Visual_Listener_1_FileNode_elmListener : VisualListener
	{
		public FileNode Object;

		public Visual_Listener_1_FileNode_elmListener(FileNode Object, VisualElement Element) : base(Element)
		{
			this.Object = Object;
			this.Element.InputListener = this;
		}

		public override void Event(InputEvent Event){
			this.Object.elmListener(Event);
		}
	}

	public class Visual_2_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_2_FileNode(FileNode Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));

		}
	}


	public class Visual_3_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_3_FileNode(FileNode Object, int depthFileWidth, string name) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			listeners.Add(element);
			element.Margin = Spacing.Combine(element.Margin, new Spacing(Way.Try(depthFileWidth), null, null, null));
			element.Padding = Spacing.Combine(element.Padding, new Spacing(null, Way.Try(2), null, null));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualText(name)));

			new Visual_Listener_2_FileNode_elmListener(Object, listeners[0]);
		}
	}

	public class Visual_Listener_2_FileNode_elmListener : VisualListener
	{
		public FileNode Object;

		public Visual_Listener_2_FileNode_elmListener(FileNode Object, VisualElement Element) : base(Element)
		{
			this.Object = Object;
			this.Element.InputListener = this;
		}

		public override void Event(InputEvent Event){
			this.Object.elmListener(Event);
		}
	}

	public class Visual_4_FileExplorer_FileExplorer : VisualElement
	{
		public FileExplorer Object;

		public Visual_4_FileExplorer_FileExplorer(FileExplorer Object, FileNode RootNode) : base(VisualType.Block)
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
			stack.Push(parent);
			parent = element;
			parent.add((element = RootNode));

		}
	}


}
