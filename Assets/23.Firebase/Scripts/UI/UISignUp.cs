using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISignUp : MonoBehaviour
{
	public TMP_InputField email;
	public TMP_InputField passwd;

	public Button signUpButton;
	public Button signInButton;

	private void Awake()
	{
		signInButton.onClick.AddListener(SignInButtonClick);
		signUpButton.onClick.AddListener(SignUpButtonClick);
	}

	private void SignUpButtonClick()
	{
		if (passwd.text.Length < 6)
		{
			UIManager.Instance.PageOpen<UIDialog>().SetDialog("알림", "비밀번호는 6글자 이상", DialogCallback);
		}
		else
		{
			FirebaseManager.Instance.Create(email.text, passwd.text, CreateCallback);
		}
	}

	private void DialogCallback()
	{
		UIManager.Instance.PageOpen(GetType().Name);
	}

	private void CreateCallback(FirebaseUser user)
	{
		UIManager.Instance.PageOpen<UIDialog>().SetDialog("회원가입 완료", "회원가입이 완료\n로그인해주세요", DialogCallback);
	}

	private void SignInButtonClick()
	{
		UIManager.Instance.PageOpen<UISignIn>();
	}
}
