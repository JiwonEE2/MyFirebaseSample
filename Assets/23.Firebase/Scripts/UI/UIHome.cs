using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{
	public TextMeshProUGUI info;
	public Button signInButton;

	private void Awake()
	{
		signInButton.onClick.AddListener(SignInButtonClick);
	}

	private void SignInButtonClick()
	{
		UIManager.Instance.PageOpen<UISignIn>();
	}

	public void SetInfo(FirebaseUser user)
	{
		info.text = $"이메일 : {user.Email}\n이름 : {user.DisplayName}";
	}
}
