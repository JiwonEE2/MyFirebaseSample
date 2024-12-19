using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// json으로 변경할 것이기 때문에 직렬화
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

	public string userId;
	public string userName;
	public int level;
	public int gold;
	public UserClass userClass;

	// 기본 생성자
	public UserData() { }

	// 회원가입할 때 사용할 생성자
	public UserData(string userId)
	{
		this.userId = userId;
		userName = "무명의 전사";
		level = 1;
		gold = 0;
		userClass = UserClass.Warrior;
	}

	public UserData(string userId, string userName, int level, int gold, UserClass userClass)
	{
		// 로그인할 때 사용할 생성자
		this.userId = userId;
		this.userName = userName;
		this.level = level;
		this.gold = gold;
		this.userClass = userClass;
	}
}
