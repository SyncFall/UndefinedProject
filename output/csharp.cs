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
	public class Editor : VisualElement
	{
		public ActionBar ActionBar;
		public Workspace Workspace;
		public FileExplorer FileExplorer;

		public Editor()
		{
						Workspace = new Workspace(this);
						ActionBar = new ActionBar(this);
						FileExplorer = new FileExplorer(this);
						this.content = new Visual_1_Editor_Editor(this, ActionBar, Workspace, FileExplorer);
		}
	}

	public class ActionBar : VisualElement
	{
		public Editor Editor;
		public Select solutionSelect = new Select(".solution");
		public Select runSelect = new Select(".run");

		public ActionBar(Editor Editor)
		{
						this.Editor = Editor;
						this.content = new Visual_2_ActionBar_ActionBar(this, solutionSelect, runSelect);
		}
	}

	public class Workspace : VisualElement
	{
		public Editor Editor;

		public Workspace(Editor Editor)
		{
						this.Editor = Editor;
						this.content = new Visual_3_Workspace_Workspace(this);
		}
	}

	public class Select : VisualElement
	{
		public string Text;

		public Select(string Text)
		{
						this.Text = Text;
						this.content = new Visual_4_Select_Select(this, Text);
		}
	}

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
								elm = new Visual_5_FileNode(this, depthFolderWidth, name);
								list = new Visual_6_FileNode(this);
								this.add(elm, list);
			}
			else
			{
								elm = new Visual_7_FileNode(this, depthFileWidth, name);
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
		public Editor Editor;
		public FileNode RootNode;

		public FileExplorer(Editor Editor)
		{
						this.Editor = Editor;
						RootNode = new FileNode(null, "ROOT", true, 0);
						this.buildFolder(".", RootNode, 0);
						this.content = RootNode;
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


	public class Visual_1_Editor_Editor : VisualElement
	{
		public Editor Object;

		public Visual_1_Editor_Editor(Editor Object, ActionBar ActionBar, Workspace Workspace, FileExplorer FileExplorer) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = new Way(1, 700f);
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = new Way(1, 500f);
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = ActionBar));
			parent = stack.Pop();
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = new Way(1, 400f);
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(3)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = new Way(3, 0.65f);
			stack.Push(parent);
			parent = element;
			parent.add((element = Workspace));
			parent = stack.Pop();
			parent.add((element = new VisualElement(3)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = new Way(3, 0.35f);
			stack.Push(parent);
			parent = element;
			parent.add((element = FileExplorer));
			parent = stack.Pop();
			parent = stack.Pop();
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = new Way(1, 125f);
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualImageElement()));
			element.source = "1.png";
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = new Way(1, 100f);
			parent = stack.Pop();

		}
	}


	public class Visual_2_ActionBar_ActionBar : VisualElement
	{
		public ActionBar Object;

		public Visual_2_ActionBar_ActionBar(ActionBar Object, Select solutionSelect, Select runSelect) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = solutionSelect));
			parent.add((element = runSelect));

		}
	}


	public class Visual_3_Workspace_Workspace : VisualElement
	{
		public Workspace Object;

		public Visual_3_Workspace_Workspace(Workspace Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));

		}
	}


	public class Visual_4_Select_Select : VisualElement
	{
		public Select Object;

		public Visual_4_Select_Select(Select Object, string Text) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualTextElement(Text)));

		}
	}


	public class Visual_5_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_5_FileNode(FileNode Object, int depthFolderWidth, string name) : base(VisualType.Block)
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

	public class Visual_6_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_6_FileNode(FileNode Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));

		}
	}


	public class Visual_7_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_7_FileNode(FileNode Object, int depthFileWidth, string name) : base(VisualType.Block)
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
