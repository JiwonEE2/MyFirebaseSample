using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomState
{
	Waiting,
	Playing,
	Finished
}

// Json으로 직렬화하려고 만듦
[Serializable]
public class Room
{
	public string host;
	public string guest;
	public RoomState state;
	public Dictionary<int, Turn> turn = new Dictionary<int, Turn>();
}

[Serializable]
public class Turn
{
	public bool isHostTurn;
	public string coordinate;
}
