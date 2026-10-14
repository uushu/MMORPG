using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
   

    public class EquipDefine
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public EquipSlot Slot { get; set; } //装备类型
        public string Category { get; set; }
        public float MaxH { get; set; } //生命
        public float MaxM { get; set; } //法力
        public float STR { get; set; } //力量
        public float INT { get; set; } //智力
        public float DEX { get; set; } //敏捷
        public float AD { get; set; } //物理攻击
        public float AP { get; set; } //法术攻击
        public float DEF { get; set; } //物理防御
        public float MDEF { get; set; } //法术防御
        public float SPD { get; set; } //攻击速度
        public float CRI { get; set; } //暴击率






    }

}
