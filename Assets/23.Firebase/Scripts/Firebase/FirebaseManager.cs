using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance { get; private set; }

	public FirebaseApp App { get; private set; }
	public FirebaseAuth Auth { get; private set; }
	public FirebaseDatabase DB { get; private set; }

	private DatabaseReference usersRef;

	private DatabaseReference msgRef;

	public event Action onLogin;

	public UserData CurrentUserData { get; private set; }

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
		onLogin += OnLogin;
	}

	private void OnLogin()
	{
		msgRef = DB.GetReference($"msg/{Auth.CurrentUser.UserId}");
		msgRef.ChildAdded += OnMessageReceive;
	}

	// DatabaseReference.ChildAdded �̺�Ʈ�� ����� �̺�Ʈ �Լ�
	private void OnMessageReceive(object sender, ChildChangedEventArgs args)
	{
		// ���� ����
		if (args.DatabaseError == null)
		{
			string rawJson = args.Snapshot.GetRawJsonValue();

			Message message = JsonConvert.DeserializeObject<Message>(rawJson);
			//print(rawJson);

			var popup = UIManager.Instance.PopupOpen<UIDialogPopup>();

			popup.SetPopup($"From.{message.sender}", $"{message.message}\n{message.GetSendTime()}");
		}
		// ���� �߻�
		else
		{

		}
	}

	public void MessageToTarget(string target, Message message)
	{
		DatabaseReference targetRef = DB.GetReference($"msg/{target}");
		string messageJson = JsonConvert.SerializeObject(message);
		targetRef.Child(message.sender + message.sendTime).SetRawJsonValueAsync(messageJson);

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
	public async void Create(string email, string passwd, Action<FirebaseUser, UserData> callback = null)
	{
		try
		{
			var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);

			// ������ ȸ���� database reference�� ����
			usersRef = DB.GetReference($"users/{result.User.UserId}");

			// ȸ���� �����͸� database�� ����
			UserData userData = new UserData(result.User.UserId);
			string userDataJson = JsonConvert.SerializeObject(userData);

			await usersRef.SetRawJsonValueAsync(userDataJson);

			callback?.Invoke(result.User, userData);

			onLogin?.Invoke();
		}
		catch (FirebaseException e)
		{
			Debug.LogError(e.Message);
		}
	}

	// �α���
	public async void SignIn(string email, string passwd, Action<FirebaseUser, UserData> callback = null)
	{
		try
		{
			var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

			usersRef = DB.GetReference($"users/{result.User.UserId}");

			DataSnapshot userDataValues = await usersRef.GetValueAsync();
			UserData userData = null;
			// DB�� ��ΰ� �����ϴ� �� �˻�
			if (userDataValues.Exists)
			{
				string json = userDataValues.GetRawJsonValue();
				userData = JsonConvert.DeserializeObject<UserData>(json);
			}

			CurrentUserData = userData;

			callback?.Invoke(result.User, userData);
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

	// database�� ���� ������ ����
	public async void UpdateUserData(string childName, object value, Action<object> callback = null)
	{
		DatabaseReference targetRef = usersRef.Child(childName);
		await targetRef.SetValueAsync(value);

		callback?.Invoke(value);
	}

	internal void SignOut()
	{
		Auth.SignOut();
	}
}