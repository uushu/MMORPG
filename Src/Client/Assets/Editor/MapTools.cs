using System.Collections;
using System.Collections.Generic;
using Common.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTools : MonoBehaviour {
	[MenuItem("Tools/Map Tools/Export Teleporters")]
	public static void ExportTeleporters()
	{
		DataManager.Instance.Load();
		
		Scene current=EditorSceneManager.GetActiveScene();
		string currentScene=current.name;

		if (current.isDirty)
		{
			EditorUtility.DisplayDialog("提示", "当前场景中有未保存的更改，请先保存当前场景.", "确定");
			return;
		}
		
		List <TeleporterObject> allTeleporters =new List <TeleporterObject>();

		foreach (var map in DataManager.Instance.Maps)
		{
			string scenePath = "Assets/Levels/" + map.Value.Resource + ".unity";
			if (!System.IO.File.Exists(scenePath))
			{
				Debug.LogWarningFormat("Scene {0} not existed", scenePath);
				continue;
			}
			EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
			
			TeleporterObject[] teleporters=GameObject.FindObjectsOfType<TeleporterObject>();

			foreach (var td in teleporters)
			{
				if(!DataManager.Instance.Teleporters.ContainsKey(td.ID))
				{
					EditorUtility.DisplayDialog("错误", string.Format("地图 {0} 中的传送点 ID:{1} 不存在，请检查.", map.Value.Resource, td.ID), "确定");
					return;
				}

				TeleporterDefine def = DataManager.Instance.Teleporters[td.ID];
				if(def.MapID != map.Value.ID)
				{
					EditorUtility.DisplayDialog("错误", string.Format("地图 {0} 中的传送点 ID:{1} 的 MapID:{2} 与当前地图ID:{3} 不匹配，请检查.", map.Value.Resource, td.ID, def.MapID, map.Value.ID), "确定");
					return;
				}
				def.Position = GameObjectTool.WorldToLogicN(td.transform.position);
				def.Direction = GameObjectTool.WorldToLogicN(td.transform.forward);
				
			}
			
		}
		
		DataManager.Instance.SaveTeleporters();
		EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
		EditorUtility.DisplayDialog("提示", "传送点导出完成.", "确定");
		
	}

	
}
