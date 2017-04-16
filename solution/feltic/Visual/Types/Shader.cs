using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using feltic.Library;

namespace feltic.Visual.Types
{
    public enum ShaderType
    {
        VERTEX,
        FRAGMENT,
    }

    public class ShaderProgramm
    {
        public int ProgramId;
        public VertextShaderType VertexShader;
        public FragmentShaderType FragmentShader;

        public ShaderProgramm()
        { }

        public ShaderProgramm(VertextShaderType VertexShader, FragmentShaderType FragmentShader)
        {
            this.VertexShader = VertexShader;
            this.FragmentShader = FragmentShader;
        }

        public void Create()
        {
            if(this.ProgramId > 0)
            {
                throw new Exception("Shader already created!");
            }
            this.ProgramId = GL.CreateProgram();
            if(VertexShader != null)
            {
                VertexShader.ProgramId = this.ProgramId;
                VertexShader.Create();
            }
            if(FragmentShader != null)
            {
                FragmentShader.ProgramId = this.ProgramId;
                FragmentShader.Create();
            }
        }

        public void Use()
        {
            if (ProgramId <= 0)
            {
                throw new Exception("shader is no linked");
            }
            GL.UseProgram(ProgramId);
            if (VertexShader != null)
            {
                VertexShader.Use();
            }
            if (FragmentShader != null)
            {
                FragmentShader.Use();
            }
        }

        public void Release()
        {
            if(VertexShader != null)
            {
                VertexShader.Release();
            }
            if(FragmentShader != null)
            {
                FragmentShader.Release();
            }
        }

        public void Destroy()
        {
            if(VertexShader != null)
            {
                VertexShader.Destroy();
            }
            if(FragmentShader != null)
            {
                FragmentShader.Destroy();
            }
            if (ProgramId > 0)
            {
                GL.DeleteProgram(ProgramId);
                ProgramId = 0;
            }
        }
    }

    public class VertextShaderType : BaseShaderType
    {
        public VertextShaderType(string Source, string[] Attributes) : base(Source, ShaderType.VERTEX, Attributes)
        { }
    }

    public class FragmentShaderType : BaseShaderType
    {
        public FragmentShaderType(string Source, string[] Attributes) : base(Source, ShaderType.FRAGMENT, Attributes)
        { }
    }

    public class BaseShaderType
    {
        public string Name;
        public string Source;
        public ShaderType Type;
        public int HandleId;
        public int ProgramId;
        public ShaderPropertyCollection Attributes = new ShaderPropertyCollection();

        public BaseShaderType(string Source, ShaderType Type, string[] AttributeKeys)
        {
            this.Source = Source;
            this.Type = Type;
            if(AttributeKeys != null)
            {
                for(int i=0; i< AttributeKeys.Length; i++)
                {
                    this.Attributes.Put(AttributeKeys[i], new ShaderProperty(AttributeKeys[i]));
                }
            }
        }

        public void Create()
        {
            if (HandleId > 0)
            {
                throw new Exception("shader already created");
            }
            if (string.IsNullOrEmpty(Source) || Source.Trim().Length == 0)
            {
                throw new Exception("no shader-source given");
            }
            if (Type == ShaderType.VERTEX)
            {
                HandleId = GL.CreateShader(OpenTK.Graphics.OpenGL.ShaderType.VertexShader);
            }
            else if (Type == ShaderType.FRAGMENT)
            {
                HandleId = GL.CreateShader(OpenTK.Graphics.OpenGL.ShaderType.FragmentShader);
            }
            else
            {
                throw new Exception("unknown shader-type");
            }
            GL.ShaderSource(HandleId, Source);
            GL.CompileShader(HandleId);
            int status_code = -1;
            string info = "";
            GL.GetShaderInfoLog(HandleId, out info);
            GL.GetShader(HandleId, ShaderParameter.CompileStatus, out status_code);
            if (status_code != 1)
            {
                throw new Exception("failed to compile shader: " + status_code.ToString() + " | info: " + info);
            }
            GL.AttachShader(ProgramId, HandleId);
            GL.DeleteShader(HandleId);
            GL.LinkProgram(ProgramId);
            GL.GetProgramInfoLog(ProgramId, out info);
            GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out status_code);
            if (status_code != 1)
            {
                throw new Exception("failed to link shader: " + status_code.ToString() + " | info: " + info);
            }
            string[] attributeKeys = this.Attributes.Keys;
            for(int i=0; i<attributeKeys.Length; i++)
            {
                this.Attributes.GetValue(attributeKeys[i]).Location = GL.GetAttribLocation(ProgramId, attributeKeys[i]);
            }
        }

        public void Use()
        {
            string[] attributeKeys = this.Attributes.Keys;
            for (int i = 0; i < attributeKeys.Length; i++)
            {
                GL.EnableVertexAttribArray(this.Attributes.GetValue(attributeKeys[i]).Location);
            }
        }

        public void Release()
        {
            string[] attributeKeys = this.Attributes.Keys;
            for (int i = 0; i < attributeKeys.Length; i++)
            {
                GL.DisableVertexAttribArray(this.Attributes.GetValue(attributeKeys[i]).Location);
            } 
        }

        public void Destroy()
        {
            if (HandleId > 0)
            {
                GL.DeleteShader(HandleId);
                HandleId = 0;
            }
        }
    }

    public class ShaderPropertyCollection : MapCollection<string, ShaderProperty>
    { }

    public class ShaderProperty
    {
        public string Name;
        public int Location = -1;

        public ShaderProperty(string Name)
        {
            this.Name = Name;
        }
    }
}
