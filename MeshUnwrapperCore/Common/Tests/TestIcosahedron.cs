using System;
using NUnit.Framework;

namespace MeshUnwrapper.Common.Tests
{
    [TestFixture]
    class TestIcosahedron
    {

        [Test]
        public void TestRadius()
        {
            // Make sure that radius is 1
            foreach (var p in Icosahedron.Points)
            {
                Assert.That(p.X * p.X + p.Y * p.Y + p.Z * p.Z, Is.EqualTo(1.0).Within(1).Ulps);
            }
        }

        [Test]
        public void TestEdgeLength()
        {
            int i = 0;
            foreach (var t in Icosahedron.Triangles)
            {
                var p0 = Icosahedron.Points[t.p0];
                var p1 = Icosahedron.Points[t.p1];
                var p2 = Icosahedron.Points[t.p2];
                var e0 = Math.Sqrt(Sq(p0.X - p1.X) + Sq(p0.Y - p1.Y) + Sq(p0.Z - p1.Z));
                var e1 = Math.Sqrt(Sq(p1.X - p2.X) + Sq(p1.Y - p2.Y) + Sq(p1.Z - p2.Z));
                var e2 = Math.Sqrt(Sq(p2.X - p0.X) + Sq(p2.Y - p0.Y) + Sq(p2.Z - p0.Z));
                Assert.That(e0, Is.EqualTo(Icosahedron.EdgeLength).Within(1).Ulps, "e0 on triangle " + i + " is not regular");
                Assert.That(e1, Is.EqualTo(Icosahedron.EdgeLength).Within(1).Ulps, "e1 on triangle " + i + " is not regular");
                Assert.That(e2, Is.EqualTo(Icosahedron.EdgeLength).Within(1).Ulps, "e2 on triangle " + i + " is not regular");
                i++;
            }
        }

        static double Sq(double a)
        {
            return a * a;
        }

    }
}
