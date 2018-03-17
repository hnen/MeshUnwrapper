using System;
using NUnit.Framework;

namespace MeshUnwrapper.Common
{

    public struct Vec3HighPrecision
    {
        public double X, Y, Z;

        public Vec3HighPrecision(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public static Vec3HighPrecision Add(Vec3HighPrecision a, Vec3HighPrecision b)
        {
            return new Vec3HighPrecision(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3HighPrecision operator + (Vec3HighPrecision a, Vec3HighPrecision b)
        {
            return new Vec3HighPrecision(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3HighPrecision Mul(Vec3HighPrecision a, double x)
        {
            return new Vec3HighPrecision(a.X * x, a.Y * x, a.Z * x);
        }
        public static Vec3HighPrecision operator * (Vec3HighPrecision a, double x)
        {
            return new Vec3HighPrecision(a.X * x, a.Y * x, a.Z * x);
        }
        public static Vec3HighPrecision Sub(Vec3HighPrecision a, Vec3HighPrecision b)
        {
            return new Vec3HighPrecision(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vec3HighPrecision operator - (Vec3HighPrecision a, Vec3HighPrecision b)
        {
            return new Vec3HighPrecision(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static double Dot(Vec3HighPrecision a, Vec3HighPrecision b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static Vec3HighPrecision Cross(Vec3HighPrecision a, Vec3HighPrecision b)
        {
            return new Vec3HighPrecision(
                  a.Y * b.Z - a.Z * b.Y,
                -(a.X * b.Z - a.Z * b.X),
                  a.X * b.Y - a.Y * b.X
            );
        }

        public double LengthSq
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        public double Length {
            get {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public Vec3HighPrecision Normalized
        {
            get
            {
                var m = Math.Sqrt(X * X + Y * Y + Z * Z);
                return new Vec3HighPrecision(this.X / m, this.Y / m, this.Z / m);
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }

        #region Test
        public static void AssertAreEqual(Vec3HighPrecision a, Vec3HighPrecision b, string msg = "", int ulpAccuracy = 2)
        {
            Assert.That(a.X, Is.EqualTo(b.X).Within(ulpAccuracy).Ulps,
                       string.Format("{0} ({1} != {2})", msg, a, b)
            );
            Assert.That(a.Y, Is.EqualTo(b.Y).Within(ulpAccuracy).Ulps,
                        string.Format("{0} ({1} != {2})", msg, a, b)
            );
            Assert.That(a.Z, Is.EqualTo(b.Z).Within(ulpAccuracy).Ulps,
                        string.Format("{0} ({1} != {2})", msg, a, b)
            );
        }
        public static void AssertAreEqual(Vec3HighPrecision a, Vec3HighPrecision b, string msg, double accuracy)
        {
            Assert.That(a.X, Is.EqualTo(b.X).Within(accuracy),
                       string.Format("{0} ({1} != {2})", msg, a, b)
            );
            Assert.That(a.Y, Is.EqualTo(b.Y).Within(accuracy),
                        string.Format("{0} ({1} != {2})", msg, a, b)
            );
            Assert.That(a.Z, Is.EqualTo(b.Z).Within(accuracy),
                        string.Format("{0} ({1} != {2})", msg, a, b)
            );
        }
        #endregion

    }
}
