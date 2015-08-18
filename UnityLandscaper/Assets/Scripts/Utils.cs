using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Utility functions.
/// </summary>
public static class Utils {

	/// <summary>
	/// Divides a random triangle in the given tirangle list into three sub-triangles.
	/// </summary>
	/// <param name="spikeValue">Spike value.</param>
	/// <param name="triangleList">Triangle list.</param>
	/// <param name="pointList">Point list.</param>
	public static void Divide(float spikeValue, List<int> triangleList, List<Vector3> pointList)
	{
		int rand = Random.Range(0, triangleList.Count - 3);
		int vert1 = triangleList[rand];
		int vert2;
		int vert3;
		// Check whether current point is the 
		// first,seocnd or third point of a triangle.
		int pos = rand % 3;
		if (pos == 0)
		{
			vert1 = triangleList[rand];
			vert2 = triangleList[rand + 1];
			vert3 = triangleList[rand + 2];
			
		}
		else if (pos == 1)
		{
			vert1 = triangleList[rand - 1];
			vert2 = triangleList[rand];
			vert3 = triangleList[rand + 1];
		}
		else
		{
			vert1 = triangleList[rand - 2];
			vert2 = triangleList[rand - 1];
			vert3 = triangleList[rand];
		}
		
		//int p = Random.Range(-3, 11);
		float pp = rand * 14 / (triangleList.Count - 3) - 3;
		Vector3 vec1 = pointList[vert1];
		Vector3 vec2 = pointList[vert2];
		Vector3 vec3 = pointList[vert3];
		float r = rand / (triangleList.Count - 3);
		//Get (weighted) averages of the x,y,z coordinates of the selected 3 points.
		//Weight the average to make the resulting triangles appear more random.
		float xAvg = 0.1667f * (3 * vec1.x + 2 * vec2.x + vec3.x + r);
		float yAvg = 0.1667f * (3 * vec1.y + 2 * vec2.y + vec3.y + r);
		float zAvg = 0.1667f * (3 * vec1.z + 2 * vec2.z + vec3.z + r);
		Vector3 newPoint = new Vector3(xAvg, yAvg, zAvg);
		Vector3 normal = GetNormal(vec1, vec2, vec3);
		float area = AreaOfTriangle(vec1, vec2, vec3);
		
		//if area is too small, do not create new triangle. (25 is arbitrary)
		if (area < 0.5f) return;
		// The new point is moved in the direction of the normal to the 
		// plane created by the selected three points.
		// This will make it point out or point in,
		// with the weight towards pointing out.
		newPoint += normal * pp * Mathf.Sqrt(area) * spikeValue;
		
		//Remove the current triangle
		//(Remove points in reverse order)
		if (pos == 0)
		{
			triangleList.RemoveAt(rand + 2);
			triangleList.RemoveAt(rand + 1);
			triangleList.RemoveAt(rand);
			
		}
		else if (pos == 1)
		{
			triangleList.RemoveAt(rand + 1);
			triangleList.RemoveAt(rand);
			triangleList.RemoveAt(rand - 1);
		}
		else
		{
			triangleList.RemoveAt(rand);
			triangleList.RemoveAt(rand - 1);
			triangleList.RemoveAt(rand - 2);
		}
		
		//Add the three new triangles created with the new point.
		//add triangle points in clockwise direction
		pointList.Add(newPoint);
		int index = pointList.Count - 1; //index of newest point.
		
		//first triangle
		triangleList.Add(vert1);
		triangleList.Add(vert2);
		triangleList.Add(index);
		//second
		triangleList.Add(index);
		triangleList.Add(vert2);
		triangleList.Add(vert3);
		//third
		triangleList.Add(index);
		triangleList.Add(vert3);
		triangleList.Add(vert1);
	}

	/// <summary>
	/// Creates a mesh from the given vertice sand triangles and attaches the given texture, if not null to the mesh.
	/// </summary>
	/// <param name="mesh"></param>
	/// <param name="vertices"></param>
	/// <param name="triangles"></param>
	/// <param name="go"></param>
	/// <param name="tex"></param>
	public static void CreateMesh(Mesh mesh, List<Vector3> vertices, List<int> triangles, GameObject go, Texture tex = null)
	{
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		//mesh.uv = newUV.ToArray(); // add this line to the code here
		mesh.Optimize();
		mesh.RecalculateNormals();
		if (tex != null && go != null)
		{
			go.transform.GetComponent<Renderer>().material.mainTexture = tex; 
		}
	}
	
	
	
	
	/// <summary>
	/// Gets the normal vector (normalized) to the plane created by the given three points.
	/// </summary>
	/// <param name="v1">first vector point</param>
	/// <param name="v2">second vector point</param>
	/// <param name="v3">third vector point</param>
	/// <returns></returns>
	private static Vector3 GetNormal(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		return Vector3.Cross(v2 - v1, v3 - v2).normalized;
	}
	
	/// <summary>
	/// Calculates the area of the triangle enclosed by the given three points.
	/// </summary>
	/// <param name="v1">first vector point</param>
	/// <param name="v2">second vector point</param>
	/// <param name="v3">third vector point</param>
	/// <returns></returns>
	/// <returns></returns>
	private static float AreaOfTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		return 0.5f * Vector3.Cross(v2 - v1, v3 - v2).magnitude;
	}
}
