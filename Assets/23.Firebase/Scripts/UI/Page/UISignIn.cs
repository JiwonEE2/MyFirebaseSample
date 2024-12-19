using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISignIn : UIPage
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
		UIManager.Instance.PageOpen<UISignUp>();
	}

	private void SignInButtonClick()
	{
		FirebaseManager.Instance.SignIn(email.text, passwd.text, (user, userData) =>
		{
			UIHome home = UIManager.Instance.PageOpen<UIHome>();
			home.SetInfo(user);
			home.SetUserData(userData);
		});
	}
}
