using UnityEngine;
using System.Collections.Generic;

namespace JPBotelho
{
    //Generates points from a bunch of areas and then makes a mesh out of them
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RockBuilder : MonoBehaviour
    {
        private AreaOfInterest[] areas;

        [Range(1, 1500)]
        public int vertices = 250;

        [Range(0, 10000)]
        public int seed;

        //Generates a rock and assigns it to the meshfilter of this oobject
        public void PreviewRock(bool export)
        {
			areas = GetComponentsInChildren<AreaOfInterest>();
            
            if(areas.Length == 0)
				return;

			transform.position = Vector3.zero;
			Random.InitState(seed);

            List<Vector3> vertexList = new List<Vector3>();
            for (int i = 0; i < areas.Length; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    float scale = areas[i].scale;

                    float x = Random.Range(-scale / 2, scale / 2);
                    float y = Random.Range(-scale / 2, scale / 2);
                    float z = Random.Range(-scale / 2, scale / 2);

                    Vector3 pos = new Vector3(x, y, z);

                    pos += areas[i].transform.position;

                    vertexList.Add(pos);
                }
            }

            Mesh m = VertexGenerator.MeshFromPoints(vertexList);

            MeshFilter filter = GetComponent<MeshFilter>();
            filter.sharedMesh = m;

            transform.position = Vector3.zero;

            if (export)
            {                
                MeshExporter.ExportMesh(m);
            }
        }

        public void RandomSeed ()
        {
            seed = Random.Range(0, 10000);
        }
    }
}