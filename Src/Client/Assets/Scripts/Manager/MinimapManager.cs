using Assets;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Manager
{
    class MinimapManager : Singleton<MinimapManager>
    {

        public UIMiniMap minimap;
        private Collider miniMapBoundBox;
        public Collider MiniMapBoundBox
        {
            get
            {
                return miniMapBoundBox;
            }
        }
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                    return null;
                return User.Instance.CurrentCharacterObject.transform;
            }
        }
        public Sprite LoadMinimapRecourese()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }

        public void UpdateMiniMap(Collider boundBox)
        {
            this.miniMapBoundBox = boundBox;
            if(minimap != null)
            {
                minimap.UpdateMinimap();
            }
        }
    }
}
