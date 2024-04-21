using UnityEngine;
using System.Collections;

namespace JPBotelho
{
    public class NormalInverter
    {
        //Flips a mesh's normals.
        public static Mesh InvertUVs(Mesh original)
        {
            Vector3[] normals = original.normals;

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }

            original.normals = normals;

            for (int x = 0; x < original.subMeshCount; x++)
            {
                int[] triangles = original.GetTriangles(x);

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }

                original.SetTriangles(triangles, x);
            }


            return original;
        }
    }
}