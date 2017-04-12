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
	public class FileNode : VisualObject
	{
		public VisualObject elm;
		public VisualObject list;
		public FileNode parent;
		public string path;
		public string name;
		public bool isFolder;
		public int depth;
		public int depthFolderWidth;
		public int depthFileWidth;

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
						this.depthFolderWidth = (depth * 16);
						this.depthFileWidth = (depthFolderWidth + 16);
						this.build();
		}
		public void build()
		{
			if(this.isFolder)
			{
				VisualObject b = new Visual_1_FileNode(this)
;
								elm = new Visual_2_FileNode(this)
;
								list = new Visual_3_FileNode(this)
;
								b.add(elm, list);
								Visual = b.Visual;
			}
			else
			{
								elm = new Visual_4_FileNode(this)
;
								Visual = elm.Visual;
			}
		}
		public void addEntry(FileNode child)
		{
						list.add(child.Visual);
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

	public class FileExplorer : VisualObject
	{
		public FileNode Root;

		public FileExplorer()
		{
						Root = new FileNode(null, "ROOT", true, 0);
						this.buildFolder(".", Root, 0);
						Visual = Root.Visual;
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


	public class Visual_1_FileNode : VisualObject
	{
		public FileNode Object;

		public Visual_1_FileNode(FileNode Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);

		}
	}


	public class Visual_2_FileNode : VisualObject
	{
		public FileNode Object;

		public Visual_2_FileNode(FileNode Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);
			listeners.Add(element);
			element.Margin = new Spacing(Object.depthFolderWidth, 0, 0, 0);;
			element.Padding = new Spacing(0, 2, 0, 0);;
			stack.Push(parent);
			parent = element;
			element = new VisualImageElement(parent);
			element.source = "folder.png";
			element.Room.Height = new Way(1, 14f);
			element.Margin = new Spacing(0, 0, 4, 0);;
			element.AddChild(new VisualTextElement(Object.name, parent));

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

	public class Visual_3_FileNode : VisualObject
	{
		public FileNode Object;

		public Visual_3_FileNode(FileNode Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);

		}
	}


	public class Visual_4_FileNode : VisualObject
	{
		public FileNode Object;

		public Visual_4_FileNode(FileNode Object)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);
			listeners.Add(element);
			element.Margin = new Spacing(Object.depthFileWidth, 0, 0, 0);;
			element.Padding = new Spacing(0, 2, 0, 0);;
			stack.Push(parent);
			parent = element;
			element = new VisualImageElement(parent);
			element.source = "file.png";
			element.Room.Height = new Way(1, 14f);
			element.Margin = new Spacing(0, 0, 2, 0);;
			element.AddChild(new VisualTextElement(Object.name, parent));

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
