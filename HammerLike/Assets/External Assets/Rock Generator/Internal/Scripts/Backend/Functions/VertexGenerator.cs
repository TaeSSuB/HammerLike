using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MIRockGenConvexHull;


namespace JPBotelho
{
    //Contains several helper methods
	public class VertexGenerator
	{
		public static IEnumerable<Vector3> PointsFromSize (int vertices, Vector3 size)
		{
			List<Vector3> list = new List<Vector3>();

            for (int i = 0; i < vertices; i++)
            {
				// Division by 2 is so the pivot ends up in the middle, not in the corner.
				list.Add(new Vector3(
                                    Random.Range(-size.x/2, size.x/2), 
                                    Random.Range(-size.y/2, size.y/2), 
                                    Random.Range(-size.z/2, size.z/2)));
            }

            return list; 
		}

		public static IEnumerable<Vector3> PointsFromRadius(int vertices, float radius)
        {
            List<Vector3> list = new List<Vector3>();

            for (int i = 0; i < vertices; i++)
            {
                list.Add(Random.insideUnitSphere * radius);
            }

            return list;
        }

        //Uses MIRockGenConvexHull to generate a (convex) mesh from a given set of points.
        public static Mesh MeshFromPoints (IEnumerable<Vector3> points)
        {
            Mesh m = new Mesh();            

            List<int> triangles = new List<int>();

            List<Vertex> vertices = points.Select(x => new Vertex(x)).ToList();

            var result = RockGenConvexHull.Create(vertices);

            m.vertices = result.Points.Select(x => x.ToVec()).ToArray();

            List<Vertex> resultPoints = result.Points.ToList();

            foreach (var face in result.Faces)
            {
                triangles.Add(resultPoints.IndexOf(face.Vertices[0]));
                triangles.Add(resultPoints.IndexOf(face.Vertices[1]));
                triangles.Add(resultPoints.IndexOf(face.Vertices[2]));
            }

            m.triangles = triangles.ToArray();
            m.RecalculateNormals();

			//Converts the generated mesh to low poly
			m = LowPolyConverter.Convert(m);

            return m;
        }
	}
}