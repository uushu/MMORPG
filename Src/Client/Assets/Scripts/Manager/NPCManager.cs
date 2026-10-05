using System.Collections;
using System.Collections.Generic;
using Common.Data;
using UI;
using UnityEditor.VersionControl;
using UnityEngine;

public class NPCManager : Singleton<NPCManager> {

	public delegate bool NPCActionHandler(NPCDefine npc);
	Dictionary<NPCFunction,NPCActionHandler> npcEvents = new Dictionary<NPCFunction, NPCActionHandler>();

	public void RegisterNPCEvent(NPCFunction function, NPCActionHandler action)
	{
		if (!npcEvents.ContainsKey(function))
		{
			npcEvents[function] = action;
		}
		else
		{
			npcEvents[function] += action;
		}
	}
	
	
	public NPCDefine GetNPCDefine(int npcID)
	{
		NPCDefine npc = null;
		DataManager.Instance.NPCs.TryGetValue(npcID, out npc);
		return npc;
	}


	/// <summary>
	/// 交互判断
	/// </summary>
	/// <param name="npcID"></param>
	/// <returns></returns>
	public bool Interactive(int npcID)
	{
		if(DataManager.Instance.NPCs.ContainsKey(npcID))
		{
			var npc = DataManager.Instance.NPCs[npcID];
			return Interactive(npc);
		}
		return false;
	}
	
	/// <summary>
	/// 执行交互
	/// </summary>
	/// <param name="npc"></param>
	/// <returns></returns>
	public bool Interactive(NPCDefine npc)
	{
		if (npc.Type == NPCType.Functional)
		{
			return DoFunctionalInteractive(npc);
		}
		else if(npc.Type == NPCType.Task)
		{
			return DoTaskInteractive(npc);
		}
		return false;
	}


	private bool DoFunctionalInteractive(NPCDefine npc)
	{
		if(npc.Type != NPCType.Functional)
		{
			return false;
		}
		if(!npcEvents.ContainsKey(npc.Fuction))
		{
			return false;
		}

		return npcEvents[npc.Fuction](npc);
	}
	private bool DoTaskInteractive(NPCDefine npc)
	{
		MessageBox.Show("点击了NPC："+npc.Name);
		return true;
	}
}
