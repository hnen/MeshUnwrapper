using System;
using System.Collections.Generic;
using NUnit.Framework;
using MeshUnwrapper.Common;

namespace MeshUnwrapper.Unwrapper.Tests
{

    [TestFixture]
    public class UnwrapperTests {

        [Test]
        public static void TestUnwrappedMeshInXZPlane() {
            var mesh = Unwrapper.MakeUnwrappedMeshBFS(MeshUnwrapper.Common.Icosahedron.Points, MeshUnwrapper.Common.Icosahedron.Triangles);

            Assert.AreEqual(MeshUnwrapper.Common.Icosahedron.Triangles.Length, mesh.Triangles.Count, "Triangle counts don't match!");

            int N = 0;
            foreach(var tri in mesh.Triangles) {
                var p0 = mesh.Vertices[tri.p0];
                var p1 = mesh.Vertices[tri.p1];
                var p2 = mesh.Vertices[tri.p2];
                var e0 = p1-p0;
                var e1 = p2-p0;
                var n = Vec3HighPrecision.Cross(e0, e1).Normalized;
                Assert.That(n.Y, Is.EqualTo(1.0).Within(0.01), string.Format("Normal {0} of tri #{1} ({2}-{3}-{4}) not facing up.", n, N, p0, p1, p2));
                N++;
            }

        }

        [Test]
        public static void TestUnwrappedMeshAreas() {
            var mesh = Unwrapper.MakeUnwrappedMeshBFS(MeshUnwrapper.Common.Icosahedron.Points, MeshUnwrapper.Common.Icosahedron.Triangles);

            HashSet<int> usedTris = new HashSet<int>();

            int N = 0;
            foreach(var tri in mesh.Triangles) {
                var p0 = mesh.Vertices[tri.p0];
                var p1 = mesh.Vertices[tri.p1];
                var p2 = mesh.Vertices[tri.p2];
                var e0 = p1-p0;
                var e1 = p2-p0;
                var A0 = Vec3HighPrecision.Cross(e0,e1).Length/2.0;
                bool match = false;
                for(int i = 0; i < MeshUnwrapper.Common.Icosahedron.Triangles.Length; i++) {
                    if (usedTris.Contains(i)) {
                        continue;
                    }
                    var tri0 = MeshUnwrapper.Common.Icosahedron.Triangles[i];
                    var q0 = MeshUnwrapper.Common.Icosahedron.Points[tri0.p0];
                    var q1 = MeshUnwrapper.Common.Icosahedron.Points[tri0.p1];
                    var q2 = MeshUnwrapper.Common.Icosahedron.Points[tri0.p2];                    
                    var f0 = q1-q0;
                    var f1 = q2-q0;
                    var A1 = Vec3HighPrecision.Cross(f0,f1).Length/2.0;
                    var err = Math.Abs(A0-A1);
                    if (err < 0.001) {
                        usedTris.Add(i);
                        match = true;
                        break;
                    }
                }
                Assert.True(match, "Area of triangles is distorted, N="+N+ ", A0="+A0);
                N++;
            }
        }


    }

}


