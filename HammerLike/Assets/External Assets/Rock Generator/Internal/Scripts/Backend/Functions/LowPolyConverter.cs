using UnityEngine;

namespace JPBotelho
{
    public class LowPolyConverter
    {

        //Makes it so no triangles in the mesh share vertices
        public static Mesh Convert(Mesh mesh)
        {
            //Process the triangles
            int[] triangles = mesh.triangles;

            Vector3[] oldVerts = mesh.vertices;
            Vector3[] vertices = new Vector3[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                vertices[i] = oldVerts[triangles[i]];
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}