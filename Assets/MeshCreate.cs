using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCreate : MonoBehaviour {
	public MeshFilter filter = null;
	// Use this for initialization

	public void Create( List<Vector3> list ) {
		Mesh m = new Mesh();
/*
		List<Vector3> newlist = new List<Vector3>();
		newlist.Add ( list[1] );
		newlist.Add ( list[8] );
		list = newlist;
*/
		Vector3[] vertices = new Vector3[list.Count + 1];
		Vector2[] uv = new Vector2[list.Count + 1];
		int[] triangles = new int[list.Count*3];
		vertices[0] = new Vector3(0,0,0);
		uv[0] = new Vector2(0,0);

		Vector3 cent = new Vector3(0,0,0);
		float u = 0.0f;
		int triCount = 0;
		for( int i=0; i<list.Count; ++i ) {
			vertices[i+1] = list[i];
			uv[i+1] = new Vector2(u, 1);
			if( u < 0.5f ) {
				u = 1.0f;
			}else{
				u = 0.0f;
			}

			cent += list[i];

			if( i == 0 ) continue; 
			triangles[triCount++] = 0;
			triangles[triCount++] = i;
			triangles[triCount++] = i+1;
		}

		cent /= list.Count;
		vertices[0] = cent;
		/*
		for( int i=1; i<vertices.Length; ++i ){
			Vector3 vect = (vertices[i] - cent);
			vect.Normalize();
			vect.Scale( new Vector3( size, size, size ) );
			vertices[i] -= vect;
		}
		*/

		m.vertices = vertices;
		m.uv = uv;
		m.triangles = triangles;

		m.RecalculateNormals();
		m.RecalculateBounds();
		
		filter.sharedMesh = m;
	}
}
