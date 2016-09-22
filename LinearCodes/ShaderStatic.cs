using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public static class ShaderStatic
    {
        public static int Load(string vertexFileName, string fragmentFileName)
        {
            int vertexShaderId = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);

            var vertexShaderCode = File.ReadAllText(vertexFileName);
            var fragmentShaderCode = File.ReadAllText(fragmentFileName);


            Console.WriteLine("Compiling vertex shader: " + vertexFileName);
            GL.ShaderSource(vertexShaderId, vertexShaderCode);
            GL.CompileShader(vertexShaderId);

            int infoLogLength;

            GL.GetShader(vertexShaderId, ShaderParameter.InfoLogLength, out infoLogLength);
            if (infoLogLength > 0)
                Console.WriteLine(GL.GetShaderInfoLog(vertexShaderId));

            Console.WriteLine("Compiling fragment shader: " + fragmentFileName);
            GL.ShaderSource(fragmentShaderId, fragmentShaderCode);
            GL.CompileShader(fragmentShaderId);

            GL.GetShader(fragmentShaderId, ShaderParameter.InfoLogLength, out infoLogLength);
            if (infoLogLength > 0) 
                Console.WriteLine(GL.GetShaderInfoLog(fragmentShaderId));

            Console.WriteLine("Linking program...");
            int programId = GL.CreateProgram();
            GL.AttachShader(programId, vertexShaderId);
            GL.AttachShader(programId, fragmentShaderId);
            GL.LinkProgram(programId);

            GL.GetShader(fragmentShaderId, ShaderParameter.InfoLogLength, out infoLogLength);
            if (infoLogLength > 0)
                Console.WriteLine(GL.GetShaderInfoLog(programId));

            GL.DetachShader(programId, vertexShaderId);
            GL.DetachShader(programId, fragmentShaderId);

            GL.DeleteShader(vertexShaderId);
            GL.DeleteShader(fragmentShaderId);

            return programId;
        }

    }
}
