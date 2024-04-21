using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;

namespace JPBotelho
{
    public class ObjExporterScript
    {
        private static int startIndex = 0;

        public static void Reset()
        {
            startIndex = 0;
        }

        public static string MeshToString(MeshFilter filter, Transform transform)
        {
            Quaternion rot = transform.localRotation;

            int numVertices = 0;

            Mesh m = filter.sharedMesh;

            Material[] mats = filter.GetComponent<Renderer>().sharedMaterials;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < m.vertices.Length; i++)
            {
                Vector3 vert = transform.TransformPoint(m.vertices[i]);

                stringBuilder.Append(string.Format("v {0} {1} {2}\n", vert.x, vert.y, -vert.z));
            }

            stringBuilder.Append("\n");

            for (int i = 0; i < m.normals.Length; i++)
            {
                Vector3 v = rot * m.normals[i];
                stringBuilder.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
            }

            stringBuilder.Append("\n");

            for (int i = 0; i < m.uv.Length; i++)
            {
                Vector3 vec = m.uv[i];

                stringBuilder.Append(string.Format("vt {0} {1}\n", vec.x, vec.y));
            }


            for (int material = 0; material < m.subMeshCount; material++)
            {
                stringBuilder.Append("\n");
                stringBuilder.Append("usemtl ").Append(mats[material].name).Append("\n");
                stringBuilder.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = m.GetTriangles(material);

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    stringBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1 + startIndex, triangles[i + 1] + 1 + startIndex, triangles[i + 2] + 1 + startIndex));
                }
            }

            startIndex += numVertices;

            return stringBuilder.ToString();
        }
    }

    public class ObjExporter : ScriptableObject
    {
        public static MeshFilter filter;
        public static Transform trans;

        /// <summary>
        /// Exports a rock to a .obj (Wavefront) file.
        /// </summary>
        /// <param name="mesh">Mesh to export.</param>
        /// <param name="name">Name of the mesh.</param>
        /// <param name="folderPath">Path of the folder where the file will be exported to.</param>
        public static void ExportObj(Mesh mesh, string path)
        {       
            string fileName = path; //Name / Properties of the .obj file

            //Writes Mesh to .obj file

            ObjExporterScript.Reset();

            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + mesh.name + ".obj"
                                + "\n#" + System.DateTime.Now.ToLongDateString()
                                + "\n#" + System.DateTime.Now.ToLongTimeString()
                                + "\n#-------"
                                + "\n\n");
                        

            Vector3 originalPosition = trans.position;

            trans.position = Vector3.zero;

            meshString.Append(ProcessTransform(trans, true));

            WriteToFile(meshString.ToString(), fileName);

            trans.position = originalPosition;           

            ObjExporterScript.Reset();
        }

        static string ProcessTransform(Transform t, bool makeSubmeshes)
        {
            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + t.name
                            + "\n#-------"
                            + "\n");

            if (makeSubmeshes)
            {
                meshString.Append("g ").Append(t.name).Append("\n");
            }
            

            if (filter)
            {
                meshString.Append(ObjExporterScript.MeshToString(filter, trans));
            }

            for (int i = 0; i < trans.childCount; i++)
            {
                meshString.Append(ProcessTransform(trans.GetChild(i), makeSubmeshes));
            }
                      
            return meshString.ToString();
        }

        static void WriteToFile(string s, string filename)
        {            
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(s);
            }
			UnityEditor.AssetDatabase.Refresh();
		}


    }
}