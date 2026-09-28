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

		float horizontalAngle =
			-eventData.delta.x * rotateSpeed;
		float verticalAngle =
			eventData.delta.y * rotateSpeed;

		root.Rotate(
			Vector3.up,
			horizontalAngle,
			Space.World
		);
		root.Rotate(
			Vector3.right,
			verticalAngle,
			Space.World
		);
	}

}
