using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{
    public class ToolBox
    {
        public WindowType WindowType;
        public ScreenType ScreenType;
        public ToolBoxItem[] ToolBoxItems;
        public ToolBoxItem ActiveToolboxItem;

        public ToolBox(ScreenType ScreenType)
        {
            this.ScreenType = ScreenType;
            this.WindowType = ScreenType.WindowType;
            this.ToolBoxItems = new ToolBoxItem[]
            {
                new ToolBoxItem(this, ToolBoxItemEnum.SELECT, "select.png"),
                new ToolBoxItem(this, ToolBoxItemEnum.ADD, "plus.png"),
            };
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

            for(int i=0; i<ToolBoxItems.Length; i++)
            {
                if(ActiveToolboxItem != null && ActiveToolboxItem.Type != ToolBoxItems[i].Type)
                {
                    ToolBoxItems[i].IsActive = false;
                }
                ToolBoxItems[i].Draw(i);
            }

            GL.Color3(System.Drawing.Color.Black);
            GL.LineWidth(1.5f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(40, 0);
            GL.Vertex2(40, WindowType.Height);
            GL.End();
        }

        public void MouseEvent(MouseState Result)
        {
            for(int i=0; i<ToolBoxItems.Length; i++)
            {
                ToolBoxItems[i].MouseEvent(Result);
            }
        }
    }

    public enum ToolBoxItemEnum
    {
        SELECT,
        ADD,
    }

    public class ToolBoxItem
    {
        public ToolBox Toolbox;
        public ToolBoxItemEnum Type;
        public string ImagePath;
        public TextureType TextureType;
        public static readonly int DEFAULT_HEIGHT = 20;
        public static readonly int ITEM_MARGIN = 10;
        public int BORDER_MARGIN = 4;
        public int HeightOffset;
        public bool IsMouseOver;
        public bool IsActive;

        public ToolBoxItem(ToolBox Toolbox, ToolBoxItemEnum Type, string ImagePath)
        {
            this.Toolbox = Toolbox;
            this.Type = Type;
            this.ImagePath = ImagePath;
            this.CreateImage();
        }

        public void CreateImage()
        {
            TextureType = new TextureType(ImagePath);
            TextureType.Create();
        }

        public void Draw(int Index)
        {
            HeightOffset = ((Index * (DEFAULT_HEIGHT + ITEM_MARGIN)) + ITEM_MARGIN);

            GL.PushMatrix();
            GL.Translate(ITEM_MARGIN, HeightOffset, 0);

            if (IsMouseOver || IsActive)
            {
                GL.Color3(System.Drawing.Color.Black);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(0 - BORDER_MARGIN, 0 - BORDER_MARGIN);
                GL.Vertex2(TextureType.Width + BORDER_MARGIN, 0 - BORDER_MARGIN);
                GL.Vertex2(TextureType.Width + BORDER_MARGIN, DEFAULT_HEIGHT + BORDER_MARGIN);
                GL.Vertex2(0 - BORDER_MARGIN, DEFAULT_HEIGHT + BORDER_MARGIN);
                GL.End();
            }

            GL.BindTexture(TextureTarget.Texture2D, TextureType.Id);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);
            GL.TexCoord2(1, 0);
            GL.Vertex2(TextureType.Width, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex2(TextureType.Width, DEFAULT_HEIGHT);
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, DEFAULT_HEIGHT);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.PopMatrix();
        }

        public void MouseEvent(MouseState Result)
        {
            /*
            IsMouseOver = GeometryUtils.IntersectPositionWithBound(ITEM_MARGIN, TextureType.Width, HeightOffset, DEFAULT_HEIGHT, Result.Cursor.X, Result.Cursor.Y);
            if(IsMouseOver && Result.InputType == MouseType.BUTTON_EVENT && Result.Button.Event == ButtonEvent.DOWN)
            {
                Toolbox.ActiveToolboxItem = this;
                IsActive = true;
            }
            */
        }
    }
}
