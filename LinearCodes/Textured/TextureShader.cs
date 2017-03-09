using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes.Textured
{
    public class TextureShader: Shader
    {

        public Uniform<Matrix4> UniformProjectionMatrix { get; }
        public Uniform<Matrix4> UniformModelMatrix { get; }

        public Uniform<int> UniformSampler2D { get; }
        public Uniform<Color4> UniformColor { get; }
        public Uniform<Vector4> UniformTexturePos { get; }

        public TextureShader(string vertexPath, string fragmentPath) : base(vertexPath, fragmentPath)
        {
            UniformProjectionMatrix = GetUniformMatrix4("projection");
            UniformModelMatrix = GetUniformMatrix4("model");
            UniformColor = GetUniformColor4("color");
            UniformSampler2D = GetUniformInt("image");
            UniformTexturePos = GetUniformVector4("texturePos");
        }
        
    }
}
