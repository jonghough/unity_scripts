using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Land builder. Builds various landscape terrains. 
/// </summary>
public class LandBuilder : BaseBuilder
{
	public int length = 40; 	//number of quads (length x length surface)
	public float height = 20;	//default height of mountain/s.

	public enum LandType{ VOLCANO, MOUNTAIN_RANGE, MOUNTAIN, CIRCLE_TRACK}
	[SerializeField]
	private LandType _landType;

#if UNITY_EDITOR
	public bool saveIt = false;
#endif


	protected override void Start()
	{
		base.Start();

		/***************************************
		 * Some possible landscape. The parameters
		 * are example params.
		 *
		 ***************************************/
		switch(_landType){
			case LandType.VOLCANO:
				BuildVolcano(0.008f, 0.3f, 100, 1000);
				break;
			case LandType.MOUNTAIN_RANGE:
				BuildMountainRange(0,100);
				break;
			case LandType.CIRCLE_TRACK:
				BuildCurvedTrack(Vector3.zero);
				break;
			case LandType.MOUNTAIN:
				BuildMountain(0.009f, 1000);
				break;
		}

	}

#if UNITY_EDITOR
		void SaveAsset()
		{
			AssetDatabase.CreateAsset(mMesh, "Assets/" + "mountains" + ".asset"); 
		
		}
#endif	

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private PeakPoint GeneratePeak()
	{
		int r1 = Random.Range(0, length);
		int r2 = Random.Range(0, length);
		int r3 = length / 2;
		int r4 = (22921 * r3 + 3921) % 100;
		return new PeakPoint(r1, r2, r3, r4);
	}


	/// <summary>
	/// Builds the volcano terrain. The slope gives a measure of the outside gradient and interior slope gives a
	/// measure of the crater gradient. CraterRadius gives the radius of the crater. Divisions is the number of 
	/// triangle divisions to perofrm on the resulting mesh.
	/// </summary>
	/// <param name="slope">Slope.</param>
	/// <param name="interiorSlope">Interior slope.</param>
	/// <param name="craterRadius">Crater radius.</param>
	/// <param name="divisions">Divisions.</param>
	private void BuildVolcano(float slope, float interiorSlope, float craterRadius, int divisions)
	{

		float x = transform.position.x;
		float z = transform.position.z;

		int i = 0;
		int j = 0;
		for (i = 0; i < length; i++)
		{
			for (j = 0; j < length; j++)
			{
				float rnd =Random.Range(0, 5.0f);
				int rad = 0;
				//VOLCANO CAULDRON
				if ((rad = (i - length / 2) * (i - length / 2) + (j - length / 2) * (j - length / 2)) < craterRadius)
				{
					float coeff = (height + rnd) * Mathf.Exp(-craterRadius * (slope + interiorSlope));
					float ht = coeff * Mathf.Exp(interiorSlope * ((i - length / 2) * (i - length / 2) + (j - length / 2) * (j - length / 2)));
					mPointList.Add(new Vector3(x + i, ht, z + j));

				}
				else
				{
					float ht = (height + rnd) * Mathf.Exp(-slope * ((i - length / 2) * (i - length / 2) + (j - length / 2) * (j - length / 2)));
					mPointList.Add(new Vector3(x + i, ht, z + j));
				}

			}
		}

		for (j = 0; j < length - 1; j++)
		{
			for (i = 0; i < length * (length - 1); i++)
			{
				if (i % length != length - 1)
				{
					mTriangleList.Add(i + 1);
					mTriangleList.Add(i + length);
					mTriangleList.Add(i);

					mTriangleList.Add(i + 1);
					mTriangleList.Add(i + length + 1);
					mTriangleList.Add(i + length);
				}
			}
		}


		i = divisions;
		float spike = 0.01f;
		while (i-- >= 0)
		{
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
	/// Builds the mountain. Same as Volcano but without a crater.
	/// </summary>
	/// <param name="slope">Slope.</param>
	/// <param name="divisions">Divisions.</param>
	private void BuildMountain(float slope, int divisions)
	{
		
		float x = transform.position.x;
		float z = transform.position.z;
		
		int i = 0;
		int j = 0;
		for (i = 0; i < length; i++)
		{
			for (j = 0; j < length; j++)
			{
				float rnd =Random.Range(0, 5.0f);
				float ht = (height + rnd) * Mathf.Exp(-slope * ((i - length / 2) * (i - length / 2) + (j - length / 2) * (j - length / 2)));
				mPointList.Add(new Vector3(x + i, ht, z + j));
			}
		}
		
		for (j = 0; j < length - 1; j++)
		{
			for (i = 0; i < length * (length - 1); i++)
			{
				if (i % length != length - 1)
				{
					mTriangleList.Add(i + 1);
					mTriangleList.Add(i + length);
					mTriangleList.Add(i);
					
					mTriangleList.Add(i + 1);
					mTriangleList.Add(i + length + 1);
					mTriangleList.Add(i + length);
				}
			}
		}
		
		
		i = divisions;
		float spike = 0.01f;
		while (i-- >= 0)
		{
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
	/// Builds a randomized mountain range with given peak data and peakScale.
	/// peakScale less than 0.005 will generate a cliff-like or plateau effect.
	/// peakScale greater than 0.05 generates very pointy peaks.
	/// </summary>
	/// <param name="peaks"></param>
	/// <param name="peakScale"></param>
	private void BuildMountainRange(float peakScale, int peakNum)
	{
	
		
		List<PeakPoint> peaks = new List<PeakPoint>();
		int k = peakNum;
		while (k-- > 0)
		{

			peaks.Add(GeneratePeak());
		}

		float x = transform.position.x;
		float z = transform.position.z;

		int i = 0;
		int j = 0;
		for (i = 0; i < length; i++)
		{
			for (j = 0; j < length; j++)
			{
				float rnd = Random.Range(0, 0.02f);
				float radsq = 0;
				bool raised = false;
				int counter = 0;
				Vector3 newPoint = new Vector3();
				foreach(PeakPoint peak in peaks){
					if((radsq = (i - peak.x) * (i - peak.x) + (j - peak.y) * (j - peak.y)) < 100000){
						float height;
						
						height = (peak.height + rnd) * Mathf.Exp(-0.01f * radsq);

						newPoint += new Vector3((x + i), height, (z + j));
						counter ++;
						if (radsq <length*0.05f)
						{
							newPoint += new Vector3((x + i), height, (z + j));
							counter++;
						}
						if (radsq < length*0.1f)
						{
							newPoint += new Vector3((x + i), height, (z + j));
							counter++;
						}
						if (radsq < length*0.2f)
						{
							newPoint += new Vector3((x + i), height, (z + j));
							counter++;
						}
						raised = true;
					}
					
				}
				if (!raised)
				{
					mPointList.Add(new Vector3(x + i, rnd, z + j));
					
				}
				else{
					mPointList.Add(newPoint / counter);
				}

			}
		}

		for (j = 0; j < length - 1; j++)
		{
			for (i = 0; i < length * (length - 1); i++)
			{
				if (i % length != length - 1)
				{
					mTriangleList.Add(i + 1);
					mTriangleList.Add(i + length);
					mTriangleList.Add(i);


					mTriangleList.Add(i + 1);
					mTriangleList.Add(i + length + 1);
					mTriangleList.Add(i + length);
				}
			}
		}
		
		Utils.CreateMesh(mMesh, mPointList, mTriangleList, this.gameObject, mTexture);
	}


	/// <summary>
	/// Builds a curved track around the given center point.
	/// </summary>
	/// <param name="center">Center.</param>
	private void BuildCurvedTrack( Vector3 center)
	{
		mMesh = GetComponent<MeshFilter>().mesh;
		
		int i = 0;
		int j = 0;
		for (i = 0; i < length; i++)
		{
			for (j = 0; j < length; j++)
			{
				float theta = 2 * Mathf.PI * 1.0f / (length-1);
				float xpos = center.x+(30 + j * 10) * Mathf.Cos(theta * i);
				float zpos = center.z+(30 + j * 10) * Mathf.Sin(theta * i);
				mPointList.Add(new Vector3(xpos,0,zpos));
			}
		}
		
		
		for (j = 0; j < length - 1; j++)
		{
			for (i = 0; i < length * (length - 1); i++)
			{
				if (i % length != length - 1)
				{
					mTriangleList.Add(i );
					mTriangleList.Add(i + length);
					mTriangleList.Add(i+ 1);
					
					
					mTriangleList.Add(i + length);
					mTriangleList.Add(i + length + 1);
					mTriangleList.Add(i + 1);
				}
			}
		}
		
		
		
		Utils.CreateMesh(mMesh, mPointList, mTriangleList, this.gameObject, mTexture);
		
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
#if UNITY_EDITOR
		if (saveIt){
			SaveAsset();
			saveIt = false;
		}

#endif
	}
}

/// <summary>
/// Peak point. Wrapper for the data needed to
/// generate a peak of the generated map.
/// </summary>
public class PeakPoint{
	
	public  float x;
	public	float y;
	public	float radius;
	public float height;

	public PeakPoint(float xv, float yv, float rad, float ht){
		x = xv;
		y = yv;
		radius = 8;// rad;
		height = ht;
	}
}