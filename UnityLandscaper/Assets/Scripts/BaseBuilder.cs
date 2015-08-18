using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base builder. Base class for Mesh builder classes.
/// </summary>
[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class BaseBuilder : MonoBehaviour {


	[SerializeField]
	/// <summary>
	/// The texutre to use for the generated mesh.
	/// </summary>
	protected Texture2D mTexture;
	[SerializeField]
	/// <summary>
	/// The generated mesh object.
	/// </summary>
	protected Mesh mMesh;

	//Lists
	protected List<Vector3> mPointList = new List<Vector3>();
	protected List<int> mTriangleList = new List<int>();


	protected virtual void Start(){
		if(mTexture == null){
			Debug.LogError("You must set the texture.");
		}
		mMesh = GetComponent<MeshFilter>().mesh;
	}

}
