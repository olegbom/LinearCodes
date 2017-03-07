using System;
using OpenTK;

namespace LinearCodes
{
    public class EmploymentMatrix
    {
        private byte[,] _matrix;

        public float Delta { get; } = 10;
        public readonly int Height;
        public readonly int Wight;

        private readonly int _xCount;
        private readonly int _yCount;

       
        public int this[float x, float y] => _matrix[GetX(x), GetY(y)];

        public EmploymentMatrix(int wight, int height)
        {
            Wight = wight;
            Height = height;

            _xCount = (int) (Wight/Delta);
            _yCount = (int) (Height/Delta);
            _matrix = new byte[_xCount,_yCount];
           
            for (int i = 0; i < _yCount; i++)
            {
                _matrix[0,i] = 1;
                _matrix[_xCount-1,i] = 1;
            }
            for (int i = 1; i < _xCount-1; i++)
            {
                _matrix[i,0] = 0;
                _matrix[1,_yCount-1] = 0;
            }

        }

        public Vector2 GetPoint(int xIndex, int yIndex)
        {
            if(xIndex >= _xCount ||
               yIndex >= _yCount ||
               yIndex < 0 ||
               xIndex < 0)  throw new ArgumentOutOfRangeException();
            return new Vector2(xIndex*Delta, yIndex*Delta);
        }

        public void MountingRectangle(Vector2 position, Vector2 size)
        {
            var xMin = GetX(position.X);
            var xMax = GetX(position.X + size.X);
            var yMin = GetX(position.Y);
            var yMax = GetX(position.Y + size.Y);

            for (int i = xMin; i <= xMax; i++)
                for (int j = yMin; j <= yMax; j++)
                    _matrix[i,j] = 1;
        }

        public Vector2 GoToWall(Vector2 from, Vector2 delta)
        {
            if(Math.Abs(delta.X) > 1 && Math.Abs(delta.Y) > 1)
                throw new ArgumentOutOfRangeException();
            if (Math.Abs(delta.X) < 1 && Math.Abs(delta.Y) < 1)
                return from;

            int x = GetX(from.X);
            int y = GetY(from.Y);

            if (Math.Abs(delta.X) > 1)
            {   //direction X
                int dir = Math.Sign(delta.X);
                var steps = (int) (delta.X/Delta);
                for (int i = 0; i < steps; i++)
                {
                    if(this[x + dir,y] == 1) return GetPoint(x,y);
                    x += dir;
                }
            }
            else
            {   //direction Y
                int dir = Math.Sign(delta.Y);
                var steps = (int)(delta.Y / Delta);
                for (int i = 0; i < steps; i++)
                {
                    if (this[x, y+dir] == 1) return GetPoint(x, y);
                    y += dir;
                }
            }
            return GetPoint(x, y);

        }


        
        public int GetX(float x)
        {
            var xIndex = (int)(x / Delta);
            if (xIndex < 0) xIndex = 0;
            else if (xIndex >= _xCount) xIndex = _xCount - 1;
            return xIndex;
        }


        public int GetY(float y)
        {
            var yIndex = (int)(y / Delta);
            if (yIndex < 0) yIndex = 0;
            else if (yIndex >= _yCount) yIndex = _yCount - 1;
            return yIndex;
        }
    }
}
