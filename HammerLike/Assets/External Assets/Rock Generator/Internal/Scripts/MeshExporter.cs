using System.Collections.Generic;
using UnityEngine;

namespace JPBotelho
{
	public class MeshExporter : MonoBehaviour //Not a component, just need DestroyImmediate
	{
        public static void ExportMesh(Mesh m)
        {
            string folderLocation = FilePanel.Open();

			if (!string.IsNullOrEmpty(folderLocation))
			{
				GameObject go = new GameObject();
				go.transform.position = Vector3.zero;

				MeshFilter filter = go.AddComponent<MeshFilter>();

				MeshRenderer renderer = go.AddComponent<MeshRenderer>();
				renderer.sharedMaterial = new Material(Shader.Find("Standard"));

				filter.sharedMesh = m;

				m = NormalInverter.InvertUVs(m); //Invers the normals for exporting

				ObjExporter.trans = go.transform;
				ObjExporter.filter = filter;

				ObjExporter.ExportObj(m, folderLocation); //Exports to .obj file                         

				//Destroy(go);
				DestroyImmediate(go);
				m = NormalInverter.InvertUVs(m); //Invers the normals for exporting
			}
		}
	}
}