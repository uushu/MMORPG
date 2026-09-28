using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour
{
	public Transform charOwner;
	public float height;

	void Start()
	{
		
	}

	void Update()
	{
		if (charOwner != null)
		{
			this.transform.position = charOwner.transform.position + Vector3.up * height;
		}
		
		this.transform.forward = Camera.main.transform.forward;
	}
	

}
