using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{

    public  class Uniform<T>
    {
        public string Name { get; }
        public Shader Shader { get; }
        public int Id { get; }

        private Action<int, T> _valueSetAction;

        private T _value;

        public T Value
        {
            set
            {
                _value = value;
                GL.UseProgram(Shader.ProgramId);
                _valueSetAction(Id, value);
            }
            get { return _value; }
        }

        public Uniform(string name, Shader shader, Action<int, T> valueSet)
        {
            _valueSetAction = valueSet;
            Name = name;
            Shader = shader;

            Id = GL.GetUniformLocation(shader.ProgramId, name);
        }
    }
}
