using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace MeshUnwrapperVisualization.Editor {
    public class IcosahedronGen
    {
        [MenuItem("GameObject/3D Object/Icosahedron")]
        public static void CreateIcosahderon()
        {
            GameObject icoObj = new GameObject("Icosahedron");
            var mf = icoObj.AddComponent<MeshFilter>();
            var mr = icoObj.AddComponent<MeshRenderer>();

            var mesh = new Mesh();
            mesh.name = "Icosahedron Mesh";
            mesh.vertices = MeshUnwrapper.Common.Icosahedron.Points.Select(a => new Vector3((float)a.X, (float)a.Y, (float)a.Z)).ToArray();
            mesh.SetIndices(MeshUnwrapper.Common.Icosahedron.Triangles.SelectMany(a => new int[] { a.p0, a.p1, a.p2 }).ToArray(), MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
            mf.sharedMesh = mesh;
        }
    }
}
