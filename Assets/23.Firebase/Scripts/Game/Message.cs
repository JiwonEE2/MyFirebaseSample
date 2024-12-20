using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Message
{
	public string sender;
	public string message;
	public long sendTime;

	public DateTime GetSendTime()
	{
		return new DateTime(sendTime);
	}
}
