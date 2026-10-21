using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour {

	public int ID;
	private Mesh mesh = null;


	void Start()
	{
		this.mesh =GetComponent<MeshFilter>().sharedMesh;
	}
	
	
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{

		Vector3 pos = this.transform.position + Vector3.up * this.transform.localScale.y * 0.5f;
		Gizmos.color = Color.red;
		if(this.mesh != null)
			Gizmos.DrawWireMesh(this.mesh, pos, this.transform.rotation, this.transform.localScale);
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.ArrowHandleCap(0,pos, this.transform.rotation, 1f, EventType.Repaint);
		UnityEditor.Handles.Label(pos,"SpawnPoint:" + this.ID);
	}
#endif
}
