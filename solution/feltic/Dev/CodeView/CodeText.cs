using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using feltic.Visual.Types;
using feltic.Language;
using OpenTK.Graphics.OpenGL;
using feltic.Visual;
using feltic.Language;

namespace feltic.Integrator
{
    public class CodeText
    {
        public Font SourceFont;
        public SourceText SourceText;
        public Registry Registry;
        public CodeColor CodeColor;
        public CodeContainer CodeContainer;
        public CodeCursor CodeCursor;
        public CodeInput CodeInput;
        public CodeSelection CodeSelection;
        public CodeHistory CodeHistory;
        public TokenContainer TokenContainer;
        public VisualElement VisualCode = new VisualElement();

        public CodeText()
        {
            this.SourceText = SourceText.FromFile("Compose/eight.src");
            SourceList list = new SourceList();
            list.Add(this.SourceText);
            this.Registry = new Registry();
            this.Registry.AddSourceList(list);
            this.TokenContainer = Registry.EntryList.GetExist(SourceText).TokenContainer;
            this.CodeColor = new CodeColor();
            this.CodeContainer = new CodeContainer(this);
            this.CodeContainer.SetContainer(Registry.EntryList.GetExist(SourceText).TokenContainer);
            this.CodeCursor = new CodeCursor(this);
            this.CodeSelection = new CodeSelection(this);
            this.CodeHistory = new CodeHistory(this);
        }

        public void SetSourceText(SourceText Source)
        {
            this.SourceText = Source;
            this.Registry.UpdateSource(SourceText);
            this.CodeContainer.SetContainer(Registry.EntryList.GetExist(SourceText).TokenContainer);
            
        }
       
        public void Draw()
        {
            CodeContainer.Build();
            CodeContainer.VisualCode.Metrics(new Position(20, 20));
            CodeContainer.VisualCode.Draw();
            CodeSelection.Draw();
            CodeCursor.Draw();

        }
    }
}
