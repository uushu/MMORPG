using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestLogin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Network.NetClient.Instance.Init("127.0.0.1", 8000); //服务端IP，初始化一遍IP端口

		Network.NetClient.Instance.Connect(); //客户端链接到服务器

		SkillBridge.Message.NetMessage msg= new SkillBridge.Message.NetMessage();// 创建主消息

		msg.Request = new SkillBridge.Message.NetMessageRequest();
		msg.Request.firtstRequest = new SkillBridge.Message.FirstTestRquest(); //创建我们自己的消息
        msg.Request.firtstRequest.Name = "HJX SAY HI !"; //填充数据

		Network.NetClient.Instance.SendMessage(msg); //发送
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
