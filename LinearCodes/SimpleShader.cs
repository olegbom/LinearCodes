using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class SimpleShader: Shader
    {

        public Uniform<Matrix4> UniformProjectionMatrix { get; }
        public Uniform<Matrix4> UniformModelMatrix { get; }
        public Uniform<Color4> UniformColor { get; }
       

        public SimpleShader(string vertexPath, string fragmentPath): base(vertexPath, fragmentPath)
        {
            UniformProjectionMatrix = GetUniformMatrix4("projMatrix");
            UniformModelMatrix = GetUniformMatrix4("modelMatrix");
            UniformColor = GetUniformColor4("color");
        }


    }
}
