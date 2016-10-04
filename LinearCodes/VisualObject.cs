using System;
using System.CodeDom;
using OpenTK;
using OpenTK.Platform.Windows;

namespace LinearCodes
{


    public abstract class VisualObject
    {
        public virtual Matrix3x2 ModelMatrix { get; protected set; } = new Matrix3x2(1, 0, 0, 1, 0, 0);
        
        private Matrix3x2 _parentModelMatrix = new Matrix3x2(1, 0, 0, 1, 0, 0);
        public Matrix3x2 ParentModelMatrix
        {
            get { return _parentModelMatrix; }
            set
            {
                _parentModelMatrix = value;
                ModelMatrix = Mul(_parentModelMatrix, _individualModelMatrix);
            }
        }

        private Matrix3x2 _individualModelMatrix = new Matrix3x2(1, 0, 0, 1, 0, 0);
        
     
        #region Translate
        private Vector2 _translate = Vector2.Zero;
        public Vector2 Translate
        {
            get { return _translate; }
            set
            {
                if (_translate == value) return;
                _translate = value;
                _individualModelMatrix.M31 = _x;
                _individualModelMatrix.M32 = _y;

                ModelMatrix = Mul(_parentModelMatrix, _individualModelMatrix);
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
                _individualModelMatrix.M11 = _scale.X * cos;
                _individualModelMatrix.M12 = _scale.Y * sin;
                _individualModelMatrix.M21 = -_scale.X * sin;
                _individualModelMatrix.M22 = _scale.Y * cos;


                ModelMatrix = Mul(_parentModelMatrix, _individualModelMatrix);
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
                _individualModelMatrix.M11 = _scale.X * cos;
                _individualModelMatrix.M12 = _scale.Y * sin;
                _individualModelMatrix.M21 = -_scale.X * sin;
                _individualModelMatrix.M22 = _scale.Y * cos;

                ModelMatrix = Mul(_parentModelMatrix, _individualModelMatrix);
            }
        }
        #endregion

        public static Matrix3x2 Mul(Matrix3x2 l, Matrix3x2 r)
        {
            return new Matrix3x2(l.M11 * r.M11 + l.M21 * r.M12,
                                 l.M12 * r.M11 + l.M22 * r.M12,
                                 l.M11 * r.M21 + l.M21 * r.M22,
                                 l.M12 * r.M21 + l.M22 * r.M22,
                                 l.M11 * r.M31 + l.M21 * r.M32 + l.M31,
                                 l.M12 * r.M31 + l.M22 * r.M32 + l.M32);

        }

        public void ClearIndividualMatrix()
        {
            _scale = new Vector2(1,1);
            _rotate = 0;
            _translate = new Vector2(0,0);
            _individualModelMatrix = new Matrix3x2(1, 0, 0, 1, 0, 0);
        }

    }
}