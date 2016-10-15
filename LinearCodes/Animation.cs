using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public static class AnimationStatic
    {
               
        private static readonly List<AnimationBase> AnimationList = new List<AnimationBase>();
        
        public static void NextFrame(double ms)
        {
            if (!AnimationList.Any()) return;

            for (int i = 0; i < AnimationList.Count; i++)
            {
                AnimationList[i].OnDraw(ms);
            }
            AnimationList.RemoveAll(x => x.Finished);
        }

        public static void Animation<T>(this object o, string name, T n, uint frameCount, Action act = null)
        {
            
            Type tType = typeof (T);
            Type objType = o.GetType();

            PropertyInfo property = objType.GetProperty(name, tType);

            AnimationBase ani = null;

            if (property != null)
            {

                if (tType == typeof (float))
                {
                    ani = new AnimationBaseProperty<float>(o, property,
                        (float) (object) n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof (double))
                {
                    ani = new AnimationBaseProperty<double>(o, property,
                        (double) (object) n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof (int))
                {
                    ani = new AnimationBaseProperty<int>(o, property,
                        (int) (object) n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof (Vector2d))
                {
                    ani = new AnimationBaseProperty<Vector2d>(o, property,
                        (Vector2d) (object) n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof(Vector3d))
                {
                    ani = new AnimationBaseProperty<Vector3d>(o, property,
                        (Vector3d)(object)n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof (Color))
                {
                    ani = new AnimationBaseProperty<Color>(o, property,
                        (Color) (object) n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof(Vector4))
                {
                    ani = new AnimationBaseProperty<Vector4>(o, property,
                        (Vector4)(object)n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof(Color4))
                {
                    ani = new AnimationBaseProperty<Color4>(o, property,
                        (Color4)(object)n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof(Matrix4))
                {
                    ani = new AnimationBaseProperty<Matrix4>(o, property,
                        (Matrix4)(object)n, frameCount, AnimationFunc, act);
                }
                else if (tType == typeof(Vector2))
                {
                    ani = new AnimationBaseProperty<Vector2>(o, property,
                        (Vector2)(object)n, frameCount, AnimationFunc, act);
                }
            }
            else
            {
                FieldInfo filed = objType.GetField(name);
                if (filed != null)
                {
                    if (tType == typeof(float))
                    {
                        ani = new AnimationBaseField<float>(o, filed,
                            (float)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(double))
                    {
                        ani = new AnimationBaseField<double>(o, filed,
                            (double)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(int))
                    {
                        ani = new AnimationBaseField<int>(o, filed,
                            (int)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(Vector2d))
                    {
                        ani = new AnimationBaseField<Vector2d>(o, filed,
                            (Vector2d)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(Vector3d))
                    {
                        ani = new AnimationBaseField<Vector3d>(o, filed,
                            (Vector3d)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(Color))
                    {
                        ani = new AnimationBaseField<Color>(o, filed,
                            (Color)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(Vector4))
                    {
                        ani = new AnimationBaseField<Vector4>(o, filed,
                            (Vector4)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(Color4))
                    {
                        ani = new AnimationBaseField<Color4>(o, filed,
                            (Color4)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(Matrix4))
                    {
                        ani = new AnimationBaseField<Matrix4>(o, filed,
                            (Matrix4)(object)n, frameCount, AnimationFunc, act);
                    }
                    else if (tType == typeof(Vector2))
                    {
                        ani = new AnimationBaseField<Vector2>(o, filed,
                            (Vector2)(object)n, frameCount, AnimationFunc, act);
                    }
                }
            }

            int animationIndex = AnimationList.FindIndex(x => x.Find(o, name));
            if (ani != null)
            {
                if (animationIndex >= 0)
                {
                    AnimationList[animationIndex].Action?.Invoke();
                    AnimationList[animationIndex] = ani;
                }
                else
                    AnimationList.Add(ani);
            }


        }

        private static float AnimationFunc(float old, float n, double proportion)
        {
            return old + (n - old)*(float)proportion;
        }

        private static double AnimationFunc(double old, double n, double proportion)
        {
            return old + (n - old)*proportion;
        }

        private static int AnimationFunc(int old, int n, double proportion)
        {
            return old + (int)((n - old)*proportion);
        }


        private static Vector2d AnimationFunc(Vector2d old, Vector2d n, double proportion)
        {
            return old + (n - old) * proportion;
        }

        private static Vector2 AnimationFunc(Vector2 old, Vector2 n, double proportion)
        {
            return old + (n - old) *(float) proportion;
        }

        private static Vector3d AnimationFunc(Vector3d old, Vector3d n, double proportion)
        {
            return old + (n - old) * proportion;
        }

        private static Vector4 AnimationFunc(Vector4 old, Vector4 n, double proportion)
        {
            return old + (n - old) * (float)proportion;
        }

        private static Color AnimationFunc(Color old, Color n, double proportion)
        {
            return Color.FromArgb(old.A + (int)((n.A - old.A)* proportion),
                old.R + (int)((n.R - old.R)* proportion),
                old.G + (int)((n.G - old.G)* proportion),
                old.B + (int)((n.B - old.B)* proportion));
        }

        private static Color4 AnimationFunc(Color4 old, Color4 n, double proportion)
        {
            return new Color4(old.R + (float)((n.R - old.R) * proportion),
                  old.G + (float)((n.G - old.G) * proportion),
                  old.B + (float)((n.B - old.B) * proportion),
                  old.A + (float)((n.A - old.A) * proportion));
        }

        private static Matrix4 AnimationFunc(Matrix4 old, Matrix4 n, double proportion)
        {
            return old + (n - old) * (float)proportion;
        }

        private static T AnimationFunc<T>(T old, T n, double proportion)
        {
            throw new NotImplementedException();
        }

        public abstract class AnimationBase
        {
            public Action Action; 

            protected readonly object Obj;
            public bool Finished { get; protected set; } = false;
            public double Duration { get; }
            public double Now { get; protected set; }
            protected abstract string Name { get; }

            protected AnimationBase(object obj, double duration, Action action)
            {
                Action = action;
                Obj = obj;
                Duration = duration;
                Now = 0;
            }

            public abstract void OnDraw(double ms);

            public bool Find(object obj, string name)
            {
                return obj == Obj && name == Name;
            }
        }


        public abstract class AnimationBase<T> : AnimationBase
        {
            protected abstract T Value { get; set; }

            private readonly T _new;
            protected T _old;
            public delegate T1 AniFunc<T1>(T1 old, T1 n, double steps);
            private readonly AniFunc<T> _aniFunc;

            protected AnimationBase(object obj, T n, uint frameCount, AniFunc<T> aniFunc, Action action)
                : base(obj, frameCount,action)
            {
                _new = n;
                _aniFunc = aniFunc;
            }

            public override void OnDraw(double ms)
            {
                Now += ms;
                
                double proportion = Now/Duration;
                if (proportion >= 1)
                {
                    Finished = true;
                    proportion = 1;
                    Value = _aniFunc(_old, _new, proportion);
                    Action?.Invoke();
                    return;
                }
                Value = _aniFunc(_old, _new, proportion);

            }
        }

        public class AnimationBaseProperty<T> : AnimationBase<T>
        {
            private readonly PropertyInfo _property;

            public AnimationBaseProperty(object obj, PropertyInfo property, T n, uint frameCount, AniFunc<T> aniFunc, Action action)
                : base(obj, n, frameCount, aniFunc, action)
            {
                _property = property;
                _old = Value;
            }

            protected override T Value
            {
                get { return (T)_property.GetValue(Obj, null); }
                set { _property.SetValue(Obj, value, null); }
            }

            protected override string Name => _property.Name;
        }


        public class AnimationBaseField<T> : AnimationBase<T>
        {
            private readonly FieldInfo _field;

            public AnimationBaseField(object obj, FieldInfo field, T n, uint frameCount, AniFunc<T> aniFunc, Action action)
                : base(obj, n, frameCount, aniFunc, action)
            {
                _field = field;
                _old = Value;
            }

            protected override T Value
            {
                get { return (T)_field.GetValue(Obj); }
                set { _field.SetValue(Obj, value); }
            }

            protected override string Name => _field.Name;
        }




        public static void PathAnimation<T>(this object o, string name, List<T> path, float speed, Func<T, T, float> lenghtFunc, Action act = null)
        {
            if (path.Count == 1)
            {
                Console.WriteLine($"{o} {name}. Точек пути должно быть больше одного");
                return;
            }

            o.PathAnimationRecuse(name, path, 1, speed, lenghtFunc, act);
        }

        private static void PathAnimationRecuse<T>(this object o, string name, List<T> path, int segment, float speed, Func<T, T, float> lenghtFunc, Action act = null)
        {
            if (segment == path.Count)
            {
                act?.Invoke();
                return;
            }

            var lenght = (uint)(lenghtFunc(path[segment], path[segment - 1]) * speed);
            o.Animation(name, path[segment], lenght, () =>
            {
                PathAnimationRecuse(o, name, path, segment + 1, speed, lenghtFunc, act);
            });
        }
    }


   
}