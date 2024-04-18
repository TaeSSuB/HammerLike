using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using MIRockGenConvexHull;

namespace JPBotelho
{
    //Class responsible for editor generation/export.
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RockGenerator : MonoBehaviour
    {
        public RockSettings settings;
         
        // List that will be used to store the Vertices of the rock generated in the editor.        
        public List<Vector3> editorList;

        public int seed;

        public void EditorExport(bool random)
        {
            if (random)
            {
                GenerateRock(random);
            }

            MeshExporter.ExportMesh(GetComponent<MeshFilter>().sharedMesh);
        }
        
        //Creates a set of points to make a mesh out of. Random > Get Random Seed
        public void GenerateRock(bool random)
        {
            editorList = new List<Vector3>(settings.verts);

            if (random)
                RandomSeed();  

            Random.InitState(seed);

            if (settings.randomType == RandomType.Vector3)
            {
                editorList = (List<Vector3>)VertexGenerator.PointsFromSize(settings.verts, settings.size);
            }
            else
            {
                editorList = (List<Vector3>)VertexGenerator.PointsFromRadius(settings.verts, settings.radius);
            }

            CreateAndAssignMeshToFilter(editorList);
        }  

        public void RandomSeed()
        {
            seed = Random.Range(0, 10000);
        }

		//Makes a mesh from a given set of points and assigns it to this object's renderer
		private void CreateAndAssignMeshToFilter(IEnumerable<Vector3> verts)
		{
			Mesh m = VertexGenerator.MeshFromPoints(verts);

			MeshFilter filter = GetComponent<MeshFilter>();
			filter.sharedMesh = m;
		}
		
		private void OnDrawGizmos ()
        {
            if (settings.randomType == RandomType.Vector3)
            {
                Gizmos.DrawWireCube(transform.position, settings.size);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, settings.radius);
            }
        }

    }
}