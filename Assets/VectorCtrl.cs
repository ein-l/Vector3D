using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VectorCtrl : MonoBehaviour {
	public LineRenderer line = null;

	// Use this for initialization
	void Start () {
		UpdateLine();
	}
	
	private Vector3 Calc(float time, Vector3 st, Vector3 end, Vector3 pow, Vector3 pow2)
	{
		float	A = 2.0f;
		float	B = 3.0f;
		
		float	s = time;
		float	s2 = s*s;
		float	s3 = s2*s;
		float	a1 = A*s3 - B*s2 + 1.0f;
		float	a2 = -A*s3 + B*s2;
		float	b1 = s3 - A*s2 + s;
		float	b2 = s3 - s2;

		//Debug.Log (String.Format("Pos: {0}, {1} [{2}, {3}]", a1, a2, b1, b2));		
		Vector3 pos = a1*st + a2*end + b1*pow + b2*pow2;
		return pos;
	}

	void UpdateLine(){
		List<Vector3> pList = new List<Vector3>();
		
		Vector3 before = new Vector3(0,0,0);
		Vector3 bPow = new Vector3(0,0,0);
		for( int i=0; i<this.transform.childCount; ++i ) {
			Transform tf = this.transform.GetChild(i);
			MeshRenderer renderer = tf.gameObject.GetComponent<MeshRenderer>();
			if( renderer ) {
				renderer.enabled = false;
			}
			
			Vector3 cur = tf.position;
			Vector3 cPow = new Vector3(0,0,0);
			if( tf.childCount > 0 ) {
				Transform normal = tf.transform.GetChild(0);
				MeshRenderer renderer2 = normal.gameObject.GetComponent<MeshRenderer>();
				if( renderer2 ) {
					renderer2.enabled = false;
				}
				cPow = normal.position - cur;
				//cPow.Normalize();
			}
			
			//大きさ1で分割する
			if( i > 0 ) {
				float size = (cur - before).sqrMagnitude;
				if( size > 5.0 ) {
					int s = (int)Mathf.Ceil(size / 5);
					for( int k=0; k<s; ++k){
						pList.Add ( Calc ( ((float)k / (float)(s-1)), before, cur, bPow, cPow ) );
					}
				}else{
					//いまのところまで
					pList.Add ( Calc ( 1.0f, before, cur, bPow, cPow ) );
				}
			}else{
				pList.Add ( tf.position );
			}
			
			before = cur;
			bPow = cPow;
		}
		
		line.SetVertexCount(pList.Count);
		for( int i=0; i<pList.Count; ++i ) {
			line.SetPosition( i, pList[i] );
		}
	}

	// Update is called once per frame
	void Update () {
		UpdateLine();
	}
}
