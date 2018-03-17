using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshUnwrapper.Common
{
    struct Vec4 {
        double X, Y, Z, W;

        public Vec4(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vec4(Vec3HighPrecision v, double w)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.W = w;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
        }

        public static double Dot(Vec4 a, Vec4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        public Vec3HighPrecision ToVec3() {
            return new Vec3HighPrecision(this.X, this.Y, this.Z);
        }

    }


    struct Mat4HighPrecision
    {
        double _00, _01, _02, _03;
        double _10, _11, _12, _13;
        double _20, _21, _22, _23;
        double _30, _31, _32, _33;

        public Vec4 Row0
        {
            get
            {
                return new Vec4(_00, _01, _02, _03);
            }
        }

        public Vec4 Row1
        {
            get
            {
                return new Vec4(_10, _11, _12, _13);
            }
        }

        public Vec4 Row2
        {
            get
            {
                return new Vec4(_20, _21, _22, _23);
            }
        }

        public Vec4 Row3
        {
            get
            {
                return new Vec4(_30, _31, _32, _33);
            }
        }

        public Vec4 Col0
        {
            get
            {
                return new Vec4(_00, _10, _20, _30);
            }
        }

        public Vec4 Col1
        {
            get
            {
                return new Vec4(_01, _11, _21, _31);
            }
        }

        public Vec4 Col2
        {
            get
            {
                return new Vec4(_02, _12, _22, _32);
            }
        }

        public Vec4 Col3
        {
            get
            {
                return new Vec4(_03, _13, _23, _33);
            }
        }



        public Mat4HighPrecision(double _00, double _01, double _02, double _03,
                                 double _10, double _11, double _12, double _13,
                                 double _20, double _21, double _22, double _23,
                                 double _30, double _31, double _32, double _33)
        {
            this._00 = _00;
            this._01 = _01;
            this._02 = _02;
            this._03 = _03;

            this._10 = _10;
            this._11 = _11;
            this._12 = _12;
            this._13 = _13;

            this._20 = _20;
            this._21 = _21;
            this._22 = _22;
            this._23 = _23;

            this._30 = _30;
            this._31 = _31;
            this._32 = _32;
            this._33 = _33;
        }

        public static Mat4HighPrecision I()
        {
            return new Mat4HighPrecision(1.0, 0.0, 0.0, 0.0,
                                         0.0, 1.0, 0.0, 0.0,
                                         0.0, 0.0, 1.0, 0.0,
                                         0.0, 0.0, 0.0, 1.0);
        }

        public static Mat4HighPrecision Translate(Vec3HighPrecision T)
        {
            return new Mat4HighPrecision(1.0, 0.0, 0.0, 0.0,
                                         0.0, 1.0, 0.0, 0.0,
                                         0.0, 0.0, 1.0, 0.0,
                                         T.X, T.Y, T.Z, 1.0);

        }
        public static Mat4HighPrecision AngleAxisRot(Vec3HighPrecision axis, double cosAngle, double sinAngle)
        {
            var k = axis;
            var c = cosAngle;
            var s = Math.Sqrt(1.0 - c*c) * Math.Sign(sinAngle);
            var cc = 1.0 - c;
            double kX = k.X, kY = k.Y, kZ = k.Z;
            return new Mat4HighPrecision(
                     c+ kX * kX * cc, -kZ*s+ kX * kY * cc,   kY*s+ kX * kZ * cc, 0.0,
                  kZ*s+ kX * kY * cc,     c+ kY * kY * cc,  -kX*s+ kY * kZ * cc, 0.0,
                 -kY*s+ kX * kZ * cc,  kX*s+ kY * kZ * cc,      c+ kZ * kZ * cc, 0.0,
                              0.0,              0.0,               0.0, 1.0 
            ).Transposed();
        }

        public static Mat4HighPrecision Axes(Vec3HighPrecision X, Vec3HighPrecision Y, Vec3HighPrecision Z)
        {
            return new Mat4HighPrecision(X.X, X.Y, X.Z, 0.0,
                                         Y.X, Y.Y, Y.Z, 0.0,
                                         Z.X, Z.Y, Z.Z, 0.0,
                                         0.0, 0.0, 0.0, 1.0);

        }

        public Mat4HighPrecision Transposed()
        {
            return new Mat4HighPrecision(_00, _10, _20, _30,
                                         _01, _11, _21, _31,
                                         _02, _12, _22, _32,
                                         _03, _13, _23, _33);
        }

        public static Vec4 operator *(Vec4 A, Mat4HighPrecision B)
        {
            return new Vec4(Vec4.Dot(A, B.Col0), Vec4.Dot(A, B.Col1), Vec4.Dot(A, B.Col2), Vec4.Dot(A, B.Col3));
        }
        public static Vec4 operator *(Mat4HighPrecision A, Vec4 B)
        {
            return new Vec4(Vec4.Dot(A.Row0, B), Vec4.Dot(A.Row1, B), Vec4.Dot(A.Row2, B), Vec4.Dot(A.Row3, B));
        }

        public static Mat4HighPrecision operator * (Mat4HighPrecision A, Mat4HighPrecision B)
        {
            return new Mat4HighPrecision(
                Vec4.Dot(A.Row0, B.Col0), Vec4.Dot(A.Row0, B.Col1), Vec4.Dot(A.Row0, B.Col2), Vec4.Dot(A.Row0, B.Col3),
                Vec4.Dot(A.Row1, B.Col0), Vec4.Dot(A.Row1, B.Col1), Vec4.Dot(A.Row1, B.Col2), Vec4.Dot(A.Row1, B.Col3),
                Vec4.Dot(A.Row2, B.Col0), Vec4.Dot(A.Row2, B.Col1), Vec4.Dot(A.Row2, B.Col2), Vec4.Dot(A.Row2, B.Col3),
                Vec4.Dot(A.Row3, B.Col0), Vec4.Dot(A.Row3, B.Col1), Vec4.Dot(A.Row3, B.Col2), Vec4.Dot(A.Row3, B.Col3)
                );
        }

        public override string ToString() {
            return string.Format(
                "{0}, {1}, {2}, {3}, " +
                "{4}, {5}, {6}, {7}, " +
                "{8}, {9}, {10}, {11}, " +
                "{12}, {13}, {14}, {15}",
                _00, _01, _02, _03,
                _10, _11, _12, _13,
                _20, _21, _22, _23,
                _30, _31, _32, _33);            
        }

    }
}
