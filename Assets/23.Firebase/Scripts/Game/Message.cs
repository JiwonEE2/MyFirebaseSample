using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
	Message,
	Invite
}

[Serializable]
public class Message
{
	public MessageType type;
	public string sender;
	public string message;
	public long sendTime;
	public bool isNew = true;

	public DateTime GetSendTime()
	{
		return new DateTime(sendTime);
	}
}
