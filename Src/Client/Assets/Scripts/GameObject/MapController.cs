using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class MapController : MonoBehaviour {

	public Collider boundBox;
	// Use this for initialization
	void Start () {
		MinimapManager.Instance.UpdateMiniMap(boundBox);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
