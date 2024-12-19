using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance { get; private set; }

	public FirebaseApp App { get; private set; }
	public FirebaseAuth Auth { get; private set; }
	public FirebaseDatabase DB { get; private set; }

	private DatabaseReference usersRef;

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private async void Start()
	{
		// Check firebase initialization. �񵿱�(Async) �Լ��̹Ƿ� �Ϸ�� ������ ���
		DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
		// �ʱ�ȭ ����
		if (status == DependencyStatus.Available)
		{
			App = FirebaseApp.DefaultInstance;
			Auth = FirebaseAuth.DefaultInstance;
			DB = FirebaseDatabase.DefaultInstance;

			DataSnapshot dummyData = await DB.GetReference("users").Child("dummy").GetValueAsync();

			if (dummyData.Exists)
			{
				print(dummyData.GetRawJsonValue());
			}
		}
		// �ʱ�ȭ ����
		else
		{
			Debug.LogWarning($"���̾�̽� �ʱ�ȭ ����: {status}");
		}
	}

	// ȸ�� ���� �Լ�
	public async void Create(string email, string passwd, Action<FirebaseUser> callback = null)
	{
		try
		{
			var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);

			// ȸ���� �����͸� database�� ����

			callback?.Invoke(result.User);
		}
		catch (FirebaseException e)
		{
			Debug.LogError(e.Message);
		}
	}

	// �α���
	public async void SignIn(string email, string passwd, Action<FirebaseUser> callback = null)
	{
		try
		{
			var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

			callback?.Invoke(result.User);
		}
		catch (FirebaseException e)
		{
			UIManager.Instance.PopupOpen<UIDialogPopup>().SetPopup("�α��� ����", "�̸��� �Ǵ� ��й�ȣ�� Ʋ�Ƚ��ϴ�");
			Debug.LogError(e.Message);
		}
	}

	// ���� ���� ����
	public async void UpdateUserProfile(string displayName, Action<FirebaseUser> callback = null)
	{
		// UserProfile ����
		UserProfile profile = new UserProfile()
		{
			DisplayName = displayName,
			PhotoUrl = new Uri("https://picsum.photos/120"),
		};
		await Auth.CurrentUser.UpdateUserProfileAsync(profile);
		callback?.Invoke(Auth.CurrentUser);
	}
}