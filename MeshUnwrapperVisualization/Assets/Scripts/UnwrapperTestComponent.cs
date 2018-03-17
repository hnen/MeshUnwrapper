using System;
using System.Linq;
using UnityEngine;

#pragma warning disable 0649

[ExecuteInEditMode]
public class UnwrapperTestComponent : MonoBehaviour
{
    [SerializeField] MeshFilter targetMeshFilter;
    [SerializeField] UnwrapStrategy unwrapStrategy;
    [SerializeField] int unwrapRandomStrategySeed;

    public enum UnwrapStrategy
    {
        BFS, DFS, CLOSEST, RANDOM
    }

    public void Update()
    {
        MeshUnwrapper.Unwrapper.UnwrappedMesh umesh;
        switch (this.unwrapStrategy) {
            case UnwrapStrategy.BFS:
                umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshBFS(MeshUnwrapper.Common.Icosahedron.Points, MeshUnwrapper.Common.Icosahedron.Triangles);
                break;
            case UnwrapStrategy.DFS:
                umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshDFS(MeshUnwrapper.Common.Icosahedron.Points, MeshUnwrapper.Common.Icosahedron.Triangles);
                break;
            case UnwrapStrategy.CLOSEST:
                umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshClosest(MeshUnwrapper.Common.Icosahedron.Points, MeshUnwrapper.Common.Icosahedron.Triangles);
                break;
            case UnwrapStrategy.RANDOM:
                umesh = MeshUnwrapper.Unwrapper.Unwrapper.MakeUnwrappedMeshRandom(MeshUnwrapper.Common.Icosahedron.Points, MeshUnwrapper.Common.Icosahedron.Triangles, this.unwrapRandomStrategySeed);
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
