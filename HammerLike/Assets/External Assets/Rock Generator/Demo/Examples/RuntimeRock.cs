using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JPBotelho
{
	//Example class that generates a rock at start
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class RuntimeRock : MonoBehaviour
	{
		void Start()
		{
			List<Vector3> vertices = (List<Vector3>)VertexGenerator.PointsFromRadius(250, 3);
			GetComponent<MeshFilter>().sharedMesh = VertexGenerator.MeshFromPoints(vertices);
		}
	}
}