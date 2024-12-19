using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// json���� ������ ���̱� ������ ����ȭ
[Serializable]
public class UserData
{
	public enum UserClass
	{
		Warrior,
		Wizard,
		Rogue,
		Archer
	}

	public string userId { get; set; }
	public string userName;
	public int level;
	public int gold;
	public UserClass userClass;

	// �⺻ ������
	public UserData() { }

	// ȸ�������� �� ����� ������
	public UserData(string userId)
	{
		this.userId = userId;
		userName = "������ ����";
		level = 1;
		gold = 0;
		userClass = UserClass.Warrior;
	}

	public UserData(string userId, string userName, int level, int gold, UserClass userClass)
	{
		// �α����� �� ����� ������
		this.userId = userId;
		this.userName = userName;
		this.level = level;
		this.gold = gold;
		this.userClass = userClass;
	}
}

// json ��ȯ �� �������� string���� ����Ǳ� ������ �뷮������. �̷��� ���� ���� �ִ�. �ٵ� ����ŷ?������ ���� �� �ʿ��ϴ� ����!
//public class DatabasePacket
//{
//	// username
//	public string aa;
//	// level
//	public int ab;
//}