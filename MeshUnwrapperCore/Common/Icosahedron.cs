using System;

using Vec3 = MeshUnwrapper.Common.Vec3HighPrecision;

namespace MeshUnwrapper.Common {

    public struct Tri {
        public int p0, p1, p2;

        public Tri(int p0, int p1, int p2) {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
        }

        public override string ToString() {
            return string.Format("({0}-{1}-{2})", p0, p1, p2);
        }
    }

    public class Icosahedron
    {

        static double r = 2.0 * Math.Sin(2.0 * Math.PI / 5.0);
        static double phi = (1.0 + Math.Sqrt(5)) / 2.0;

        static double X = 1.0 / r;
        static double Z = phi / r;

        public static double EdgeLength = 2.0 / r;

        public static Vec3[] Points = new[] {
            new Vec3(-X, 0.0, Z), new Vec3(X, 0.0, Z), new Vec3(-X, 0.0, -Z), new Vec3(X, 0.0, -Z),
            new Vec3(0.0, Z, X), new Vec3(0.0, Z, -X), new Vec3(0.0, -Z, X), new Vec3(0.0, -Z, -X),
            new Vec3(Z, X, 0.0), new Vec3(-Z, X, 0.0), new Vec3(Z, -X, 0.0), new Vec3(-Z, -X, 0.0)
        };

        public static Tri[] Triangles = new[] {
            new Tri(0,1,4),  new Tri(0,4,9),  new Tri(9,4,5),  new Tri(4,8,5),  new Tri(4,1,8),
            new Tri(8,1,10), new Tri(8,10,3), new Tri(5,8,3),  new Tri(5,3,2),  new Tri(2,3,7),
            new Tri(7,3,10), new Tri(7,10,6), new Tri(7,6,11), new Tri(11,6,0), new Tri(0,6,1),
            new Tri(6,10,1), new Tri(9,11,0), new Tri(9,2,11), new Tri(9,5,2),  new Tri(7,11,2)
        };

        public static double GetEdgeLengthForSubdivLevel(int subdivLevel) {
            return EdgeLength / Math.Exp(subdivLevel * Math.Log(2.0));
        }

        #region Test

        #endregion

    }
}
