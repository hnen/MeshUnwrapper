using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MeshUnwrapper.Common.Tests
{
    [TestFixture]
    class TestMat4
    {
        [Test]
        public void TestMat()
        {
            var a = new Mat4HighPrecision(1, 2, 3, 4, 
                                          5, 6, 7, 8, 
                                          9, 10, 11, 12, 
                                          13, 14, 15, 16);

            Assert.AreEqual(new Vec4(1, 2, 3, 4), a.Row0);
            Assert.AreEqual(new Vec4(5, 6, 7, 8), a.Row1);
            Assert.AreEqual(new Vec4(9,10,11,12), a.Row2);
            Assert.AreEqual(new Vec4(13,14,15,16), a.Row3);

            Assert.AreEqual(new Vec4(1, 5, 9, 13), a.Col0);
            Assert.AreEqual(new Vec4(2, 6,10, 14), a.Col1);
            Assert.AreEqual(new Vec4(3, 7,11, 15), a.Col2);
            Assert.AreEqual(new Vec4(4, 8,12, 16), a.Col3);

            Assert.AreEqual(Mat4HighPrecision.I(), Mat4HighPrecision.I() * Mat4HighPrecision.I());
            Assert.AreEqual(a, a * Mat4HighPrecision.I());
        }

        [Test]
        public void TestAngleAxis() {
            Assert.AreEqual(
                new Vec3HighPrecision(0, 0, 1),
                (new Vec4(0, 0,-1, 0) * Mat4HighPrecision.AngleAxisRot(new Vec3HighPrecision(0,1,0), -1.0, 0.0)).ToVec3() // PI = -1
            );

            Vec3HighPrecision.AssertAreEqual(
                new Vec3HighPrecision(-1, 0, 0),
                (new Vec4(0, 0,-1, 0) * Mat4HighPrecision.AngleAxisRot(new Vec3HighPrecision(0,1,0), 0.0, 1.0)).ToVec3(), // PI/2 = 0
                "Rotation by 90 degrees along Y axis failed");

            Vec3HighPrecision.AssertAreEqual(
                new Vec3HighPrecision( 1, 0, 0),
                (new Vec4(0, 0,-1, 0) * Mat4HighPrecision.AngleAxisRot(new Vec3HighPrecision(0,1,0), 0.0, -1.0)).ToVec3(), // -PI/2 = 0
                "Rotation by -90 degrees along Y axis failed");

            Vec3HighPrecision.AssertAreEqual(
                new Vec3HighPrecision( 0,-1, 0),
                (new Vec4(0, 0,-1, 0) * Mat4HighPrecision.AngleAxisRot(new Vec3HighPrecision(1,0,0), 0.0, -1.0)).ToVec3(), // -PI/2 = 0
                "Rotation by 90 degrees along X axis failed");

            var rnd = new System.Random();
            for (int i = 0; i < 100; i++) {
                var k = (new Vec3HighPrecision(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()) -
                        new Vec3HighPrecision(0.5, 0.5, 0.5)) * 100.0;
                var v = (new Vec3HighPrecision(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()) -
                        new Vec3HighPrecision(0.5, 0.5, 0.5)) * 100.0;
                var a = rnd.NextDouble() * Math.PI * 2.0 - Math.PI;
                var vr = (new Vec4(v, 1.0) * Mat4HighPrecision.AngleAxisRot(k.Normalized, Math.Cos(a), Math.Sin(a)) ).ToVec3();
                var vproj = k.Normalized * Vec3HighPrecision.Dot(v, k.Normalized);
                var ang = Math.Acos(Vec3HighPrecision.Dot((vproj - v).Normalized, (vproj - vr).Normalized));
                Assert.That(Math.Abs(a), Is.EqualTo(ang).Within(0.00001), "Random rotation around random axis failed.");
            }

        }
    }
}
