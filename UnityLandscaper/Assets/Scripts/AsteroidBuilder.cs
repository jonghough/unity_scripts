using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidBuilder : BaseBuilder
{


	protected override void Start()
	{
		base.Start();

		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		mPointList.Add(new Vector3(x + 10, y + 10, z + 10));
		mPointList.Add(new Vector3(x + 10, y + 10, z - 10));
		mPointList.Add(new Vector3(x + 10, y - 10, z + 10));
		mPointList.Add(new Vector3(x + 10, y - 10, z - 10));
		mPointList.Add(new Vector3(x - 10, y + 10, z + 10));
		mPointList.Add(new Vector3(x - 10, y + 10, z - 10));
		mPointList.Add(new Vector3(x - 10, y - 10, z + 10));
		mPointList.Add(new Vector3(x - 10, y - 10, z - 10));
		//+x face
		mTriangleList.Add(0);
		mTriangleList.Add(2);
		mTriangleList.Add(1);
		mTriangleList.Add(1);
		mTriangleList.Add(2);
		mTriangleList.Add(3);
		//-x face
		mTriangleList.Add(7);
		mTriangleList.Add(6);
		mTriangleList.Add(5);
		mTriangleList.Add(5);
		mTriangleList.Add(6);
		mTriangleList.Add(4);
		//+y face
		mTriangleList.Add(0);
		mTriangleList.Add(1);
		mTriangleList.Add(4);
		mTriangleList.Add(1);
		mTriangleList.Add(5);
		mTriangleList.Add(4);
		//-y face
		mTriangleList.Add(7);
		mTriangleList.Add(3);
		mTriangleList.Add(6);
		mTriangleList.Add(6);
		mTriangleList.Add(3);
		mTriangleList.Add(2);
		//+z face
		mTriangleList.Add(0);
		mTriangleList.Add(4);
		mTriangleList.Add(2);
		mTriangleList.Add(2);
		mTriangleList.Add(4);
		mTriangleList.Add(6);
		//-z face
		mTriangleList.Add(7);
		mTriangleList.Add(5);
		mTriangleList.Add(3);
		mTriangleList.Add(3);
		mTriangleList.Add(5);
		mTriangleList.Add(1);
		mMesh.Clear();

		//keep going!
		int i = 170;
		float spike = 0.2f;
		while (i-- >= 0)
		{
			//spike *= 0.9995f;
			Utils.Divide(spike, mTriangleList, mPointList);
		}

		mMesh.Clear();
		mMesh.vertices = mPointList.ToArray();
		mMesh.triangles = mTriangleList.ToArray();

		mMesh.Optimize();
		mMesh.RecalculateNormals();
		GetComponent<Renderer>().material.mainTexture = mTexture;
		ResetPosition();
	}


	/// <summary>
	/// Resets the position of the game object.
	/// </summary>
	private void ResetPosition()
	{
		Vector3 center = new Vector3();
		foreach (Vector3 v in mPointList)
		{
			center += v;
		}
		center /= mPointList.Count;

		transform.position = center;
	}

	public void Update()
	{

	}
}