using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class VisualUniforms
    {
        public Matrix4 ModelMatrix { get; private set; }
        public Color4 Color;

        #region Z
        private float _z;
        public float Z
        {
            get { return _z; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_z == value) return;
                _z = value;
                _translateMatrix = Matrix4.CreateTranslation(_translate.X, _translate.Y, _z);
                UpdateMatrix();
            }
        }
        #endregion

        #region Translate
        private Vector2 _translate;
        public Vector2 Translate
        {
            get { return _translate; }
            set
            {
                if (_translate == value) return;
                _translate = value;
                _translateMatrix = Matrix4.CreateTranslation(_translate.X, _translate.Y, _z);
                UpdateMatrix();
            }
        }
        private Matrix4 _translateMatrix = Matrix4.Identity;
        #endregion

        #region Rotate
        private float _rotate;
        public float Rotate
        {
            get { return _rotate; }
            set
            {
                if (_rotate == value) return;
                _rotate = value;
                _rotateMatrix = Matrix4.CreateRotationZ(_rotate);
                UpdateMatrix();
            }
        }
        private Matrix4 _rotateMatrix = Matrix4.Identity;
        #endregion

        #region Scale
        private Vector2 _scale;
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return;
                _scale = value;
                _scaleMatrix = Matrix4.CreateScale(value.X, value.Y, 0);
                UpdateMatrix();
            }
        }
        private Matrix4 _scaleMatrix = Matrix4.Identity;
        #endregion

        private void UpdateMatrix()
        {
            ModelMatrix = _translateMatrix * _scaleMatrix * _rotateMatrix;
        }

        public VisualUniforms()
        {
            ModelMatrix = Matrix4.Identity;
            Color = Color4.Black;
        }

        public VisualUniforms(Color4 color)
        {
            ModelMatrix = Matrix4.Identity;
            Color = color;
        }

    }
}