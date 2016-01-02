using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VectorCtrl : MonoBehaviour {
	public LineRenderer line = null;
	public bool isDebug = true;
	public bool isFill = true;
	public bool hasTerminate = true;

	private GameObject meshObject = null;

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
		List<Vector3> plList = new List<Vector3>();
		
		Vector3 before = new Vector3(0,0,0);
		Vector3 bPow = new Vector3(0,0,0);
		bool terminate = false;
		bool endOfLoop = false;
		for( int i=0; i<this.transform.childCount; ++i ) {
			Transform tf = this.transform.GetChild(i);
			if( tf.name.IndexOf("Mesh") != -1 ) {
				if( !hasTerminate && i == this.transform.childCount-1 ) {
					i = -1;
					endOfLoop = true;
				}
				continue;
			}

			MeshRenderer renderer = tf.gameObject.GetComponent<MeshRenderer>();
			if( renderer ) {
				if( !isDebug ) renderer.enabled = false;
				else renderer.enabled = true;
			}
			
			Vector3 cur = tf.position;
			Vector3 cPow = new Vector3(0,0,0);
			if( tf.childCount > 0 ) {
				Transform normal = tf.transform.GetChild(0);
				MeshRenderer renderer2 = normal.gameObject.GetComponent<MeshRenderer>();
				if( renderer2 ) {
					if( !isDebug ) renderer2.enabled = false;
					else renderer2.enabled = true;
				}
				cPow = normal.position - cur;
				//cPow.Normalize();
			}
			
			//大きさ1で分割する
			if( i > 0 || endOfLoop ) {
				float size = (cur - before).sqrMagnitude;
				if( size > 5.0 ) {
					int s = (int)Mathf.Ceil(size / 5);
					for( int k=0; k<s; ++k){
						Vector3 vec = Calc ( ((float)k / (float)(s-1)), before, cur, bPow, cPow );
						if( !terminate ) {
							plList.Add ( vec );
						}
						pList.Add (vec);
					}
				}else{
					Vector3 vec = Calc ( 1.0f, before, cur, bPow, cPow );
					if( !terminate ) {
						plList.Add ( vec );
					}
					pList.Add (vec);
				}
			}else{
				if( !terminate ) {
					plList.Add ( tf.position );
				}
				pList.Add ( tf.position );
			}
			
			before = cur;
			bPow = cPow;
			
			//terminater?
			// NOTE: terminater以降のラインは描画されない。パスとじ用
			if( tf.name == "Terminater" ) {
				terminate = true;
			}
			if( endOfLoop ) {
				break;
			}
			if( !hasTerminate && i == this.transform.childCount-1 ) {
				i = -1;
				endOfLoop = true;
			}
		}
		
		line.SetVertexCount(plList.Count);
		for( int i=0; i<plList.Count; ++i ) {
			line.SetPosition( i, plList[i] );
		}

		//fillフラグがたっていたら、メッシュを作る
		if( isFill ) {
			if( meshObject == null ) {
				meshObject = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Mesh"), this.transform.position, this.transform.rotation);
				meshObject.transform.parent = this.transform;
			}

			MeshCreate script = meshObject.GetComponent<MeshCreate>();
			script.Create( pList );
		}
	}

	// Update is called once per frame
	void Update () {
		UpdateLine();
	}
}
