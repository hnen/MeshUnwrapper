using System;
using System.Collections.Generic;
using NUnit.Framework;

using Vec3 = MeshUnwrapper.Common.Vec3HighPrecision;
using MeshUnwrapper.Common;

namespace MeshUnwrapper.Unwrapper
{

    public class UnwrappedMesh {
        public List<Vec3> Vertices;
        public List<Tri> Triangles;
        public List<Vec3> VerticesOriginalPositions;
    }

    public class Unwrapper
    {

        class UnwrapIter {
            public int TriIndex;
            public Mat4HighPrecision Transform;
            public Edge EdgeToUnwrap;
            public int UnwrappedInd0;
            public int UnwrappedInd1;

            public UnwrapIter(int triIndex, Mat4HighPrecision transform, Edge edgeToUnwrap, int unwrappedInd0, int unwrappedInd1) {
                this.TriIndex = triIndex;
                this.Transform = transform;
                this.EdgeToUnwrap = edgeToUnwrap;
                this.UnwrappedInd0 = unwrappedInd0;
                this.UnwrappedInd1 = unwrappedInd1;
            }

        }

        public static UnwrappedMesh MakeUnwrappedMeshBFS(IReadOnlyList<Vec3> vertices, IReadOnlyList<Tri> triangles)
        {
            Queue<UnwrapIter> q = new Queue<UnwrapIter>();
            return MakeUnwrappedMesh(vertices, triangles, ui => q.Enqueue(ui), () => q.Dequeue());
        }

        public static UnwrappedMesh MakeUnwrappedMeshDFS(IReadOnlyList<Vec3> vertices, IReadOnlyList<Tri> triangles)
        {
            Stack<UnwrapIter> q = new Stack<UnwrapIter>();
            return MakeUnwrappedMesh(vertices, triangles, ui => q.Push(ui), () => q.Pop());
        }
        public static UnwrappedMesh MakeUnwrappedMeshRandom(IReadOnlyList<Vec3> vertices, IReadOnlyList<Tri> triangles, int seed)
        {
            List<UnwrapIter> q = new List<UnwrapIter>();
            System.Random rnd = new System.Random(seed);
            return MakeUnwrappedMesh(vertices, triangles,
                ui => {
                    q.Add(ui);
                },
                () => {
                    var i = rnd.Next(q.Count);
                    var ui = q[i];
                    q.RemoveAt(i);
                    return ui;
                });
        }
        public static UnwrappedMesh MakeUnwrappedMeshClosest (IReadOnlyList<Vec3> vertices, IReadOnlyList<Tri> triangles)
        {
            List<UnwrapIter> q = new List<UnwrapIter>();
            Vec3? firstVertex = null;
            return MakeUnwrappedMesh(vertices, triangles,
                ui => {
                    q.Add(ui);
                    if (!firstVertex.HasValue)
                    {
                        var vi0 = triangles[ui.TriIndex].p0;
                        firstVertex = vertices[vi0];
                    }
                }, 
                () => {
                    double closestDist = double.PositiveInfinity;
                    int closestUi = -1;
                    for (int i = 0; i < q.Count; i++)
                    {
                        var ui = q[i];
                        var tri = triangles[ui.TriIndex];
                        var midp = (vertices[tri.p0] + vertices[tri.p1] + vertices[tri.p2]) * (1.0/3.0);
                        var d = midp - firstVertex.Value;
                        if (d.LengthSq < closestDist)
                        {
                            closestUi = i;
                            closestDist = d.LengthSq;
                        }
                    }
                    {
                        var ui = q[closestUi];
                        q.RemoveAt(closestUi);
                        return ui;
                    }
                });
        }

        static UnwrappedMesh MakeUnwrappedMesh(IReadOnlyList<Vec3> vertices, IReadOnlyList<Tri> triangles, Action<UnwrapIter> pushAction, Func<UnwrapIter> popAction)
        {
            var connections = GenerateEdgeConnections(triangles);
            
            int firstTri = 0;

            // transform first triangle on XZ plane
            Mat4HighPrecision firstRot;
            {
                var tri = triangles[firstTri];
                var p0 = vertices[tri.p0];
                var p1 = vertices[tri.p1];
                var p2 = vertices[tri.p2];
                var e0 = p1 - p0;
                var e1 = p2 - p0;
                var n = Vec3.Cross(e0,e1);
                firstRot = Mat4HighPrecision.Axes(e0.Normalized, n.Normalized, Vec3.Cross(e0,n).Normalized).Transposed();
            }

            var firstTf = firstRot;

            
            var firstEdge = new Edge(triangles[firstTri].p0, triangles[firstTri].p1);

            var iteratedTris = new HashSet<int>();
            List<Vec3> unwrappedVertices = new List<Vec3>();
            List<Vec3> originalVertices = new List<Vec3>();
            List<Tri> unwrappedTriangles = new List<Tri>();

            originalVertices.Add(vertices[firstEdge.p0]);
            originalVertices.Add(vertices[firstEdge.p1]);
            unwrappedVertices.Add((new Vec4(vertices[firstEdge.p0], 1.0) * firstRot).ToVec3());
            unwrappedVertices.Add((new Vec4(vertices[firstEdge.p1], 1.0) * firstRot).ToVec3());

            pushAction(new UnwrapIter(firstTri, firstTf, firstEdge, 1, 0));

            int N = 0;

            while (unwrappedTriangles.Count < triangles.Count)
            {
                var i = popAction();
                if (iteratedTris.Contains(i.TriIndex)) {
                    continue;
                }
                iteratedTris.Add(i.TriIndex);
                var t = triangles[i.TriIndex];
                
                var a = vertices[t.p0];
                var b = vertices[t.p1];
                var c = vertices[t.p2];

                var p0 = (new Vec4(a, 1.0) * i.Transform).ToVec3();
                var p1 = (new Vec4(b, 1.0) * i.Transform).ToVec3();
                var p2 = (new Vec4(c, 1.0) * i.Transform).ToVec3();

                var e01 = i.EdgeToUnwrap.Equals(new Edge(t.p0, t.p1));
                var e12 = i.EdgeToUnwrap.Equals(new Edge(t.p1, t.p2));
                var e20 = i.EdgeToUnwrap.Equals(new Edge(t.p2, t.p0));

                //AssertOnXZPlane(p0,p1,p2, string.Format("Failed after dequeue, N={0}", N));
                
                Vec3HighPrecision e0, e1, mp;
                if (e01)
                {
                    e0 = p1 - p0;
                    e1 = p2 - p0;
                    mp = (p1 + p0) * 0.5;
                }
                else if (e12)
                {
                    e0 = p2 - p1;
                    e1 = p0 - p1;
                    mp = (p2 + p1) * 0.5;
                }
                else if (e20)
                {
                    e0 = p0 - p2;
                    e1 = p1 - p2;
                    mp = (p0 + p2) * 0.5;
                }
                else
                {
                    throw new Exception("Unreachable code executed.");
                }

                var n = Vec3.Cross(e0, e1).Normalized;
                var rotDir = Math.Sign(Vec3.Dot(Vec3.Cross(n, new Vec3(0,1,0)), e0));

                // rotate next triangle to XZ plane rotating around the shared edge with the previous triangle (which is already on XZ plane)
                var rot =   Mat4HighPrecision.Translate(mp * -1.0) *
                            Mat4HighPrecision.AngleAxisRot(e0.Normalized, n.Y, Math.Sqrt(1-n.Y*n.Y)*rotDir) *
                            Mat4HighPrecision.Translate(mp * 1.0);

                var ntf = i.Transform * rot;

                var np0 = (new Vec4(a, 1.0) * ntf).ToVec3();
                var np1 = (new Vec4(b, 1.0) * ntf).ToVec3();
                var np2 = (new Vec4(c, 1.0) * ntf).ToVec3();

                int v0, v1, v2;
                if (e01) {
                    v0 = i.UnwrappedInd1;
                    v1 = i.UnwrappedInd0;
                    v2 = unwrappedVertices.Count;
                    unwrappedVertices.Add(np2);
                    originalVertices.Add(c);
                }
                else if(e12) {
                    v0 = unwrappedVertices.Count;
                    v1 = i.UnwrappedInd1;
                    v2 = i.UnwrappedInd0;
                    unwrappedVertices.Add(np0);
                    originalVertices.Add(a);
                }
                else if (e20) {
                    v0 = i.UnwrappedInd0;
                    v1 = unwrappedVertices.Count;
                    v2 = i.UnwrappedInd1;
                    unwrappedVertices.Add(np1);
                    originalVertices.Add(b);
                }
                else {
                    throw new Exception("Unreachable code executed.");
                }

                unwrappedTriangles.Add(new Tri(v0,v1,v2));

                EnqueueNeighborTriangles(i.TriIndex, t, ntf, v0, v1, v2, pushAction, connections);

                N++;
            }

            return new UnwrappedMesh {
                Vertices = unwrappedVertices,
                Triangles = unwrappedTriangles,
                VerticesOriginalPositions = originalVertices
            };
        }

        static void EnqueueNeighborTriangles(int currTriIndex, Tri t, Mat4HighPrecision ntf, int v0, int v1, int v2, Action<UnwrapIter> pushAction, Dictionary<Edge, EdgeTris>  connections ) {
            pushAction(new UnwrapIter(GetNeighborTriangle(currTriIndex, connections[new Edge(t.p0, t.p1)]), ntf, new Edge(t.p0, t.p1), v0, v1));
            pushAction(new UnwrapIter(GetNeighborTriangle(currTriIndex, connections[new Edge(t.p1, t.p2)]), ntf, new Edge(t.p1, t.p2), v1, v2));
            pushAction(new UnwrapIter(GetNeighborTriangle(currTriIndex, connections[new Edge(t.p2, t.p0)]), ntf, new Edge(t.p2, t.p0), v2, v0));            
        }

        
        static Dictionary<Edge, EdgeTris> GenerateEdgeConnections(IReadOnlyList<Tri> triangles)
        {
            Dictionary<Edge, EdgeTris> connections = new Dictionary<Edge, EdgeTris>();
            for (int i = 0; i < triangles.Count; i++)
            {
                var tri = triangles[i];
                var e0 = new Edge(tri.p0, tri.p1);
                var e1 = new Edge(tri.p1, tri.p2);
                var e2 = new Edge(tri.p2, tri.p0);

                if (!connections.ContainsKey(e0))
                    connections.Add(e0, new EdgeTris(-1, -1));
                if (!connections.ContainsKey(e1))
                    connections.Add(e1, new EdgeTris(-1, -1));
                if (!connections.ContainsKey(e2))
                    connections.Add(e2, new EdgeTris(-1, -1));

                if (e0.p0 == tri.p0)
                    connections[e0] = new EdgeTris(i, connections[e0].Right);
                else
                    connections[e0] = new EdgeTris(connections[e0].Left, i);
                if (e1.p0 == tri.p1)
                    connections[e1] = new EdgeTris(i, connections[e1].Right);
                else
                    connections[e1] = new EdgeTris(connections[e1].Left, i);
                if (e2.p0 == tri.p2)
                    connections[e2] = new EdgeTris(i, connections[e2].Right);
                else
                    connections[e2] = new EdgeTris(connections[e2].Left, i);
            }

            return connections;
        }

        static int GetNeighborTriangle(int currTriangle, EdgeTris et) {
            if (currTriangle == et.Left) {
                return et.Right;
            } else if (currTriangle == et.Right) {
                return et.Left;
            } else {
                throw new Exception("currTriangle not contained in edge tringles.");
            }
        }

        struct EdgeTris
        {
            public int Left, Right;

            public EdgeTris(int left, int right)
            {
                this.Left = left;
                this.Right = right;
            }
        }

        
        struct Edge : IEquatable<Edge>
        {
            public int p0, p1;

            public Edge(int e0, int e1)
            {
                if (e0 < e1)
                {
                    this.p0 = e0;
                    this.p1 = e1;
                } else
                {
                    this.p1 = e0;
                    this.p0 = e1;
                }
            }

            public bool Equals(Edge e)
            {
                return e.p0 == this.p0 && e.p1 == this.p1;
            }
        }
        
        [Test]
        public void TestGenerateEdgeConnections()
        {
            var conns = GenerateEdgeConnections(MeshUnwrapper.Common.Icosahedron.Triangles);
            Assert.Greater(conns.Count, MeshUnwrapper.Common.Icosahedron.Points.Length);
            foreach(var conn in conns)
            {
                Assert.AreNotEqual(conn.Value.Left, -1);
                Assert.AreNotEqual(conn.Value.Right, -1);
            }
            foreach(var conn in conns)
            {
                var l = MeshUnwrapper.Common.Icosahedron.Triangles[conn.Value.Left];
                var r = MeshUnwrapper.Common.Icosahedron.Triangles[conn.Value.Right];
                Assert.True( conn.Key.Equals(new Edge(l.p0, l.p1)) || conn.Key.Equals(new Edge(l.p1, l.p2)) || conn.Key.Equals(new Edge(l.p2, l.p0))) ;
                Assert.True( conn.Key.Equals(new Edge(r.p0, r.p1)) || conn.Key.Equals(new Edge(r.p1, r.p2)) || conn.Key.Equals(new Edge(r.p2, r.p0))) ;
            }

        }



    }
}
