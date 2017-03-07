using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes.Textured
{
    class TextureShader: Shader
    {

        public Uniform<Matrix3x2> UniformProjectionMatrix { get; }
        public Uniform<Matrix3x2> UniformModelMatrix { get; }
        public Uniform<Color4> UniformColor { get; }

        public TextureShader(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {
            UniformProjectionMatrix = GetUniformMatrix3x2("projMatrix");
            UniformModelMatrix = GetUniformMatrix3x2("modelMatrix");
            UniformColor = GetUniformColor4("color");
        }
        
    }
}
