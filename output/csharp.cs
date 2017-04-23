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
						this.add(solutionSelect, runSelect);
		}
	}

	public class Workspace : VisualElement
	{
		public Editor Editor;

		public Workspace(Editor Editor)
		{
						this.Editor = Editor;
						this.content = new Visual_2_Workspace_Workspace(this);
		}
	}

	public class Select : VisualElement
	{
		public string Text;

		public Select(string Text)
		{
						this.Text = Text;
						this.type = VisualType.Inline;
						this.content = new Visual_3_Select_Select(this, Text);
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
								elm = new Visual_4_FileNode(this, depthFolderWidth, name);
								list = new Visual_5_FileNode(this);
								this.add(elm, list);
			}
			else
			{
								elm = new Visual_6_FileNode(this, depthFileWidth, name);
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
						this.content = new Visual_7_FileExplorer_FileExplorer(this, RootNode);
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
			element.Room.Width = Way.Try("700px");
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try("500px");
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(1)));
			stack.Push(parent);
			parent = element;
			parent.add((element = ActionBar));
			parent = stack.Pop();
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try("400px");
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualElement(3)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try("65pc");
			stack.Push(parent);
			parent = element;
			parent.add((element = Workspace));
			parent = stack.Pop();
			parent.add((element = new VisualElement(3)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Width = Way.Try("35pc");
			stack.Push(parent);
			parent = element;
			parent.add((element = FileExplorer));
			parent = stack.Pop();
			parent = stack.Pop();
			parent.add((element = new VisualElement(1)));
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try("125px");
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualText("status")));
			parent = stack.Pop();

		}
	}


	public class Visual_2_Workspace_Workspace : VisualElement
	{
		public Workspace Object;

		public Visual_2_Workspace_Workspace(Workspace Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));

		}
	}


	public class Visual_3_Select_Select : VisualElement
	{
		public Select Object;

		public Visual_3_Select_Select(Select Object, string Text) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(2)));
			stack.Push(parent);
			parent = element;
			parent.add((element = new VisualText(Text)));

		}
	}


	public class Visual_4_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_4_FileNode(FileNode Object, int depthFolderWidth, string name) : base(VisualType.Block)
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
			parent.add((element = new VisualImage()));
			element.source = "folder.png";
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(14);
			element.Margin = Spacing.Combine(element.Margin, new Spacing(null, null, Way.Try(4), null));
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

	public class Visual_5_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_5_FileNode(FileNode Object) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualElement(1)));

		}
	}


	public class Visual_6_FileNode : VisualElement
	{
		public FileNode Object;

		public Visual_6_FileNode(FileNode Object, int depthFileWidth, string name) : base(VisualType.Block)
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
			parent.add((element = new VisualImage()));
			element.source = "file.png";
			if(element.Room == null) element.Room = new Room();
			element.Room.Height = Way.Try(14);
			element.Margin = Spacing.Combine(element.Margin, new Spacing(null, null, Way.Try(2), null));
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

	public class Visual_7_FileExplorer_FileExplorer : VisualElement
	{
		public FileExplorer Object;

		public Visual_7_FileExplorer_FileExplorer(FileExplorer Object, FileNode RootNode) : base(VisualType.Block)
		{
			this.Object = Object;
			Stack<VisualElement> stack = new Stack<VisualElement>();
			List<VisualElement> listeners = new List<VisualElement>();
			VisualElement element, parent=null;

			parent = this;
			parent.add((element = new VisualScroll()));
			stack.Push(parent);
			parent = element;
			parent.add((element = RootNode));

		}
	}


}
