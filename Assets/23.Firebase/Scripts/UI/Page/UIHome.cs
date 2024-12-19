using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : UIPage
{
	public Image profileImage;
	public TextMeshProUGUI displayName;
	public Button profileChangeButton;
	public TextMeshProUGUI gold;
	public Button addGoldButton;
	public Button signOutButton;

	private void Awake()
	{
		profileChangeButton.onClick.AddListener(ProfileChangeButtonClick);
		addGoldButton.onClick.AddListener(AddGoldButtonClick);
		signOutButton.onClick.AddListener(SignOutButtonClick);
	}

	private void SignOutButtonClick()
	{
		FirebaseManager.Instance.SignOut();
		UIManager.Instance.PageOpen<UIMain>();
	}

	private void AddGoldButtonClick()
	{
		UserData data = FirebaseManager.Instance.CurrentUserData;
		data.gold += 10;
		FirebaseManager.Instance.UpdateUserData("gold", data.gold, (x) => { SetUserData(data); });
	}

	public void ProfileChangeButtonClick()
	{
		UIManager.Instance.PopupOpen<UIInputFieldPopup>().SetPopup("닉네임 변경", "변경할 닉네임 입력", ProfileChangeCallback);
	}

	private void ProfileChangeCallback(string newName)
	{
		FirebaseManager.Instance.UpdateUserProfile(newName, SetInfo);
	}

	public void SetInfo(FirebaseUser user)
	{
		displayName.text = user.DisplayName;
		if (user.PhotoUrl != null)
		{
			SetPhoto(user.PhotoUrl.AbsoluteUri);
		}
		else
		{
			SetPhoto("");
		}
	}

	public async void SetPhoto(string url)
	{
		if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
		{
			using (HttpClient client = new HttpClient())
			{
				byte[] response = await client.GetByteArrayAsync(url);
				Texture2D texture = new Texture2D(1, 1);
				texture.LoadImage(response);
				Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				profileImage.sprite = sprite;
			}
		}
		else
		{
			profileImage.sprite = null;
		}
	}

	public void SetUserData(UserData userData)
	{
		gold.text = userData.gold.ToString();
	}
}
