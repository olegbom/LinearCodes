using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public class Shader
    {
        public Dictionary<string,int> Attributes { get; }= new Dictionary<string, int>();
        public Dictionary<string,int> Uniforms { get; }= new Dictionary<string, int>();
        public int VertexId { get; }
        public int FragmentId { get; }
        public int ProgramId { get; }

        public Shader(string vertexFileName, string fragmentFileName)
        {
            VertexId = GL.CreateShader(ShaderType.VertexShader);
            FragmentId = GL.CreateShader(ShaderType.FragmentShader);

            var vertexShaderCode = File.ReadAllText(vertexFileName);
            var fragmentShaderCode = File.ReadAllText(fragmentFileName);


            Console.WriteLine("Compiling vertex shader: " + vertexFileName);
            GL.ShaderSource(VertexId, vertexShaderCode);
            GL.CompileShader(VertexId);

            int infoLogLength;

            GL.GetShader(VertexId, ShaderParameter.InfoLogLength, out infoLogLength);
            if (infoLogLength > 0)
                Console.WriteLine(GL.GetShaderInfoLog(VertexId));

            Console.WriteLine("Compiling fragment shader: " + fragmentFileName);
            GL.ShaderSource(FragmentId, fragmentShaderCode);
            GL.CompileShader(FragmentId);

            GL.GetShader(FragmentId, ShaderParameter.InfoLogLength, out infoLogLength);
            if (infoLogLength > 0)
                Console.WriteLine(GL.GetShaderInfoLog(FragmentId));

            Console.WriteLine("Linking program...");
            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, VertexId);
            GL.AttachShader(ProgramId, FragmentId);
            GL.BindFragDataLocation(ProgramId,0,"outputF");
            GL.LinkProgram(ProgramId);

            GL.GetShader(ProgramId, ShaderParameter.InfoLogLength, out infoLogLength);
            if (infoLogLength > 0)
                Console.WriteLine(GL.GetProgramInfoLog(ProgramId));
        }
        


        public int GetAttribLocation(string name)
        {
            int attrib;
            if (Attributes.TryGetValue(name, out attrib))
                return attrib;
            attrib = GL.GetAttribLocation(ProgramId, name);
            Attributes[name] = attrib;
            return attrib;
            
        }

        public int GetUniformLocation(string name)
        {
            int uniform;
            if (Uniforms.TryGetValue(name,out uniform))
                return uniform;
            uniform = GL.GetUniformLocation(ProgramId, name);
            Uniforms[name] = uniform;
            return uniform;
        }

        public Uniform<Matrix4> GetUniformMatrix4(string name)
        {
            return new Uniform<Matrix4>(name, this, 
                (id, v) => GL.UniformMatrix4(id, false, ref v));
        }

        public Uniform<Vector4> GetUniformVector4(string name)
        {
            return new Uniform<Vector4>(name, this,
                (id, v) => GL.Uniform4(id, ref v));
        }

        public Uniform<Color4> GetUniformColor4(string name)
        {
            return new Uniform<Color4>(name, this, 
                (id, v) => GL.Uniform4(id, v));
        }

        
    }
}