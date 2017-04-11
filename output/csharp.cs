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
		public int depth = 0;
		public int depthWidth = 0;
		public int depthListWidth = 0;
		public int depthTextWidth = 0;

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
						this.build(this.name);
		}
		public void build(string n)
		{
						depthWidth = (depth * 12);
						depthListWidth = (depthWidth + 12);
			if(this.isFolder)
			{
								elm = new Visual_1_FileNode(this, n)
;
			}
			else
			{
								elm = new Visual_2_FileNode(this, n)
;
			}
						list = new Visual_3_FileNode(this)
;
			VisualObject b = new Visual_4_FileNode(this)
;
						b.add(elm, list);
						Visual = b.Visual;
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
								list.display = !list.display;
			}
			if(inevt.Visual.GainFocus)
			{
								elm.Childrens[0].color = "#ff0000";
			}
			if(inevt.Visual.LostFocus)
			{
								elm.Childrens[0].color = "#ffffff";
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

		public Visual_1_FileNode(FileNode Object, string n)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);
			listeners.Add(element);
			element.Margin = new Spacing(Object.depthWidth, 0, 0, 0);
			stack.Push(parent);
			parent = element;
			element = new VisualImageElement(parent);
			element.source = "folder.png";
			element.Room.Height = new Way(1, 12f);
			element = new VisualTextElement(n, parent);

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

	public class Visual_2_FileNode : VisualObject
	{
		public FileNode Object;

		public Visual_2_FileNode(FileNode Object, string n)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			this.Visual = 
			element = new VisualElement(1, parent);
			listeners.Add(element);
			element.Margin = new Spacing(Object.depthListWidth, 0, 0, 0);
			stack.Push(parent);
			parent = element;
			element = new VisualImageElement(parent);
			element.source = "file.png";
			element.Room.Height = new Way(1, 12f);
			element = new VisualTextElement(n, parent);

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

		}
	}


}
