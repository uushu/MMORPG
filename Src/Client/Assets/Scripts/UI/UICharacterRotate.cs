using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICharacterRotate : MonoBehaviour , IDragHandler
{

	public Transform root;
	public float rotateSpeed = 0.5f;

	public void OnDrag(PointerEventData eventData)
	{
		if (root == null)
		{
			return;
		}

		float angle =
			-eventData.delta.x * rotateSpeed;

		root.Rotate(
			Vector3.up,
			angle,
			Space.World
		);
	}

}
