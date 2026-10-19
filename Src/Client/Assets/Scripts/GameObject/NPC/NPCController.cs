using System;
using System.Collections;
using System.Collections.Generic;
using Common.Data;
using Manager;
using Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class NPCController : MonoBehaviour
{

	public int npcId;

	private Animator anim;

	private NPCDefine npc;
	private bool inInteractive = false;

	private SkinnedMeshRenderer renderer;
	Color originColor;
	private NpcQuestStatus questStatus;

	private void Start()
	{
		anim = GetComponent<Animator>();
		npc = NPCManager.Instance.GetNPCDefine(npcId);
		//骨骼动画渲染器
		renderer = GetComponentInChildren<SkinnedMeshRenderer>();
		originColor = renderer.sharedMaterial.color;

		StartCoroutine(ActionRelax());

		RefreshQuestStatus();
		QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;
	}

	void OnQuestStatusChanged(Quest quest)
	{
		this.RefreshQuestStatus();
	}

	private void RefreshQuestStatus()
	{
		questStatus = QuestManager.Instance.GetQuestStatus(this.npcId);
		UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform,questStatus);
	}

	private void OnDestroy()
	{
		QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
		if(UIWorldElementManager .Instance != null)
			UIWorldElementManager.Instance.RemoveNpcStatus(this.transform);
	}

	private IEnumerator ActionRelax()
	{
		while (true)
		{
			if (inInteractive)
				yield return new WaitForSeconds(6f);
			else
				yield return new WaitForSeconds(Random.Range(8f, 15f));
			this.Relax();
		}
	}

	private void Relax()
	{
		anim.SetTrigger("Relax");
	}

	void OnMouseDown()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;
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
