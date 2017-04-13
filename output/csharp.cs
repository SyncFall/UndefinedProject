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
	public class FileNode : VisualElement
	{
		public VisualElement elm;
		public VisualElement list;
		public FileNode parent;
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
			if(inevt.Button.LeftClick)
			{
				if(list != null)
				{
										list.display = !list.display;
				}
			}
		}
	}

	public class FileExplorer : VisualElement
	{
		public FileNode Root;

		public FileExplorer()
		{
						Root = new FileNode(null, "ROOT", true, 0);
						this.buildFolder(".", Root, 0);
						this.add(Root);
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
			element.Margin = new Spacing(depthFolderWidth, 0, 0, 0);;
			element.Padding = new Spacing(0, 2, 0, 0);;
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualImageElement()));
			element.source = "folder.png";
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = new Way(1, 14f);
			element.Margin = new Spacing(0, 0, 4, 0);;
			parent.add((element = new VisualTextElement(name)));

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
			element.Margin = new Spacing(depthFileWidth, 0, 0, 0);;
			element.Padding = new Spacing(0, 2, 0, 0);;
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualImageElement()));
			element.source = "file.png";
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = new Way(1, 14f);
			element.Margin = new Spacing(0, 0, 2, 0);;
			parent.add((element = new VisualTextElement(name)));

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

}
