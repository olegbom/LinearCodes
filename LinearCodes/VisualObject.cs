using System;
using OpenTK;
using OpenTK.Platform.Windows;

namespace LinearCodes
{
    public abstract class VisualObject
    {
        public virtual Matrix4 ModelMatrix { get; protected set; } = Matrix4.Identity;
        
        private Matrix4 _parentModelMatrix4 = Matrix4.Identity;
        public Matrix4 ParentModelMatrix4
        {
            get { return _parentModelMatrix4; }
            set
            {
                _parentModelMatrix4 = value;
                ModelMatrix = _individualModelMatrix*_parentModelMatrix4;
            }
        }

        private Matrix4 _individualModelMatrix = Matrix4.Identity;
        
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
                _individualModelMatrix.Row3 = new Vector4(_x, _y, _z, 1);
                ModelMatrix = _individualModelMatrix * _parentModelMatrix4;
            }
        }
        #endregion

        #region Translate
        private Vector2 _translate = Vector2.Zero;
        public Vector2 Translate
        {
            get { return _translate; }
            set
            {
                if (_translate == value) return;
                _translate = value;
                _individualModelMatrix.Row3 = new Vector4(_x, _y, _z, 1);
                ModelMatrix = _individualModelMatrix * _parentModelMatrix4;
            }
        }

        private float _x => _translate.X;
        private float _y => _translate.Y;
        #endregion

        #region Rotate
        private float _rotate;
        public float Rotate
        {
            get { return _rotate; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_rotate == value) return;
                _rotate = value;
                var cos = (float)Math.Cos(_rotate);
                var sin = (float)Math.Sin(_rotate);
                _individualModelMatrix.Row0 = new Vector4(_scale.X * cos, _scale.Y * sin, 0, 0);
                _individualModelMatrix.Row1 = new Vector4(-_scale.X * sin, _scale.Y * cos, 0, 0);

                ModelMatrix = _individualModelMatrix * _parentModelMatrix4;
            }
        }
       
        #endregion

        #region Scale
        private Vector2 _scale = new Vector2(1, 1);
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return;
                _scale = value;
                var cos = (float)Math.Cos(_rotate);
                var sin = (float)Math.Sin(_rotate);
                _individualModelMatrix.Row0 = new Vector4(_scale.X * cos, _scale.Y * sin, 0, 0);
                _individualModelMatrix.Row1 = new Vector4(-_scale.X * sin, _scale.Y * cos, 0, 0);

                ModelMatrix = _individualModelMatrix * _parentModelMatrix4;
            }
        }
        #endregion

        public void ClearIndividualMatrix()
        {
            _z = 0;
            _scale = new Vector2(1,1);
            _rotate = 0;
            _translate = new Vector2(0,0);
            _individualModelMatrix = Matrix4.Identity;
        }

    }
}