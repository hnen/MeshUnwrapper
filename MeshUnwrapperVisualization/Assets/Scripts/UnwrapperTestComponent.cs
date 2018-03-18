using System;
using System.Linq;
using UnityEngine;
using MeshUnwrapper.Common;

#pragma warning disable 0649

namespace MeshUnwrapperVisualization
{

    [ExecuteInEditMode]
    public class UnwrapperTestComponent : MonoBehaviour
    {
        [SerializeField] MeshFilter sourceMeshFilter;
        [SerializeField] MeshFilter targetMeshFilter;
        [SerializeField] UnwrapStrategy unwrapStrategy;
        [SerializeField] int unwrapRandomStrategySeed;

        public enum UnwrapStrategy
        {
            BFS, DFS, CLOSEST, RANDOM
        }

        public void Update()
        {
            if (sourceMeshFilter == null)
            {
                return;
            }

            var points = sourceMeshFilter.sharedMesh.vertices.Select(a => new Vec3HighPrecision(a.x, a.y, a.z)).ToArray();
            var indices = Enumerable.Range(0, sourceMeshFilter.sharedMesh.subMeshCount).SelectMany(i => sourceMeshFilter.sharedMesh.GetIndices(i));
            var p0s = indices.Where((_, i) => i % 3 == 0);
            var p1s = indices.Where((_, i) => i % 3 == 1);
            var p2s = indices.Where((_, i) => i % 3 == 2);
            var triangles = p0s.Zip(p1s, (p0,p1) => Tuple.Create(p0,p1)).Zip(p2s, (p01, p2) => new Tri(p01.Item1, p01.Item2, p2)).ToArray();

            UnityEngine.Debug.LogFormat("Vertex count {0}, triangle count {1}", points.Length, triangles.Length);

            MeshUnwrapper.Unwrapper.UnwrappedMesh umesh;
            switch (this.unwrapStrategy)
            {
                case UnwrapStrategy.BFS:
                    umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshBFS(points, triangles);
                    break;
                case UnwrapStrategy.DFS:
                    umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshDFS(points, triangles);
                    break;
                case UnwrapStrategy.CLOSEST:
                    umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshClosest(points, triangles);
                    break;
                case UnwrapStrategy.RANDOM:
                    umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshRandom(points, triangles, this.unwrapRandomStrategySeed);
                    break;
                default: throw new NotImplementedException("Not implemented strategy " + unwrapStrategy);
            }

            var mesh = new Mesh();
            mesh.vertices = umesh.Vertices.Select(a => new Vector3((float)a.X, (float)a.Y, (float)a.Z)).ToArray();
            mesh.SetIndices(umesh.Triangles.SelectMany(a => new int[] { a.p0, a.p1, a.p2 }).ToArray(),
                MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
            mesh.hideFlags = HideFlags.DontSaveInEditor;
            if (targetMeshFilter != null)
            {
                targetMeshFilter.sharedMesh = mesh;
            }

        }

    }
}