using System;
using System.Collections;
using System.Collections.Generic;
using Common.Data;
using Models;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCController : MonoBehaviour
{

	public int NPCID;

	private Animator anim;

	private NPCDefine npc;
	private bool inInteractive = false;

	private SkinnedMeshRenderer renderer;
	Color originColor;

	private void Start()
	{
		anim = GetComponent<Animator>();
		npc = NPCManager.Instance.GetNPCDefine(NPCID);
		//骨骼动画渲染器
		renderer = GetComponentInChildren<SkinnedMeshRenderer>();
		originColor = renderer.sharedMaterial.color;

		StartCoroutine(ActionRelax());
	}

	private IEnumerator ActionRelax()
	{
		while (true)
		{
			if (!inInteractive)
				yield return new WaitForSeconds(Random.Range(5f, 10f));
			this.Relax();
		}
	}

	private void Relax()
	{
		anim.SetTrigger("Relax");
	}

	void OnMouseDown()
	{
		Interactive();
	}

	private void Interactive()
	{
		// 防止重复点击
		if (!inInteractive)
		{
			inInteractive = true;
			// 用协程做交互处理
			StartCoroutine(DoInteractive());
		}
	}

	private IEnumerator DoInteractive()
	{
		yield return FaceToPlayer();
		if(NPCManager.Instance.Interactive(npc))
		{
			anim.SetTrigger("Talk");
		}

		yield return new WaitForSeconds(3f);
		inInteractive = false;
	}

	
	private IEnumerator FaceToPlayer()
	{
		Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
		while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5f) 
		{
			// this.gameObject.transform.forward =
			// 	Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
			Quaternion targetRot = Quaternion.LookRotation(faceTo);
			transform.rotation =Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime*5f);
			
			yield return null;

		}
	}

	void OnMouseOver()
	{
		Highlight(true);
	}

	void OnMouseEnter()
	{
		Highlight(true);
	}

	void OnMouseExit()
	{
		Highlight(false);
	}
	private void Highlight(bool highlight)
	{
		if (highlight)
		{
			if (renderer.sharedMaterial.color != Color.white)
				renderer.sharedMaterial.color = Color.white;
		}
		else
		{
			if(renderer.sharedMaterial.color != originColor)
				renderer.sharedMaterial.color = originColor;
		}
	}
}
