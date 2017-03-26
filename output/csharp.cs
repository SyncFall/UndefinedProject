using System;
using System.IO;
using System.Collections;
using feltic.Language;
using feltic.UI;
using feltic.UI.Types;
using feltic.Integrator;

namespace Scope
{
	public class Editor
	{
		public ActionBar ActionBar actionBar = new ActionBar();
		public Workspace Workspace workspace = new Workspace();
		public VisualObject VisualObject Root;

		public Editor Editor Create()
		{
		}
	}

	public class ActionBar
	{
		public Select Select solutionSelect = new Select();
		public Select Select runSelect = new Select();
		public VisualObject VisualObject Root;

		public ActionBar ActionBar Create()
		{
		}
	}

	public class Workspace
	{
		public VisualObject VisualObject Root;

		public Workspace Workspace Create()
		{
		}
	}

	public class Select
	{
		public VisualObject VisualObject Root;

		public Select Select Create()
		{
		}
	}


}
