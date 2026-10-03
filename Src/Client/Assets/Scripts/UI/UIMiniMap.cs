using System.Collections;
using System.Collections.Generic;
using Manager;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour
{

	public Text mapName;
	public Image miniMap;
	public Image arrow;
	private Collider miniMapBoundBox;
	private Transform playerTransform;
	
	void Start()
	{
		MinimapManager.Instance.minimap = this;
		UpdateMinimap();
	}

	void Update()
	{
		this.MinimapToMove();
		
	}
	public void UpdateMinimap()
	{
        mapName.text = User.Instance.CurrentMapData.Name;
        this.miniMap.overrideSprite = MinimapManager.Instance.LoadMinimapRecourese();
		this.miniMap.SetNativeSize();
		this.miniMap.transform.localPosition = Vector3.zero;
		this.miniMapBoundBox = MinimapManager.Instance.MiniMapBoundBox;
		this.playerTransform = null;
	}

	void MinimapToMove()
	{
		if(playerTransform == null)
			playerTransform = MinimapManager.Instance.PlayerTransform;
		if (miniMapBoundBox == null || playerTransform == null)
			return;
		
		#region 坐标变换
		// 世界地图真实宽高
		float realWidth = miniMapBoundBox.bounds.size.x;
		float realHeight = miniMapBoundBox.bounds.size.z;
		
		//角色相对坐标位置
		float relaPlayerX = this.playerTransform.position.x - miniMapBoundBox.bounds.min.x;
		float relaPlayerY = this.playerTransform.position.z - miniMapBoundBox.bounds.min.z;
		
		float u = relaPlayerX / realWidth;
		float v = relaPlayerY / realHeight;

		#endregion
		
		#region 地图移动
		RectTransform mapRect = miniMap.rectTransform;
		//方式一 : 修改地图pivot , 相对父节点位置不变
		// 改变锚点/参考点
		// mapRect.pivot = new Vector2(u, v);
		// mapRect.localPosition = Vector2.zero;
		
		// 方式二 : 平移地图 ，相对父节点位置改变 ，参考点不变
		// 固定图片中心为原点
		mapRect.pivot = new Vector2(0.5f, 0.5f); 
		// 计算角色对应的图片坐标
		Vector2 playerMapPosition = new Vector2(
			(u-0.5f)*mapRect.rect.width,
			(v-0.5f)*mapRect.rect.height);
		// 把坐标移到视口中心
		mapRect.anchoredPosition = -playerMapPosition;
		#endregion

		
		#region 箭头朝向
		// Vector3 forward = cam.transform.forward;
		Vector3 forward = playerTransform.forward;
		
		Vector2 mapDirection = new Vector2(
			forward.x * mapRect.rect.width / realWidth,
			forward.z * mapRect.rect.height / realHeight
		);
		float angle = -Mathf.Atan2(mapDirection.x, mapDirection.y) * Mathf.Rad2Deg;

		arrow.rectTransform.localRotation =
			Quaternion.Euler(0f, 0f, angle);
		#endregion
	}

}
