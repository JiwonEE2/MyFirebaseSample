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
		// Check firebase initialization. 비동기(Async) 함수이므로 완료될 때까지 대기
		DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
		// 초기화 성공
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
		// 초기화 실패
		else
		{
			Debug.LogWarning($"파이어베이스 초기화 실패: {status}");
		}
	}

	// 회원 가입 함수
	public async void Create(string email, string passwd, Action<FirebaseUser> callback = null)
	{
		try
		{
			var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);

			// 회원의 데이터를 database에 생성

			callback?.Invoke(result.User);
		}
		catch (FirebaseException e)
		{
			Debug.LogError(e.Message);
		}
	}

	// 로그인
	public async void SignIn(string email, string passwd, Action<FirebaseUser> callback = null)
	{
		try
		{
			var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

			callback?.Invoke(result.User);
		}
		catch (FirebaseException e)
		{
			UIManager.Instance.PopupOpen<UIDialogPopup>().SetPopup("로그인 실패", "이메일 또는 비밀번호가 틀렸습니다");
			Debug.LogError(e.Message);
		}
	}

	// 유저 정보 수정
	public async void UpdateUserProfile(string displayName, Action<FirebaseUser> callback = null)
	{
		// UserProfile 생성
		UserProfile profile = new UserProfile()
		{
			DisplayName = displayName,
			PhotoUrl = new Uri("https://picsum.photos/120"),
		};
		await Auth.CurrentUser.UpdateUserProfileAsync(profile);
		callback?.Invoke(Auth.CurrentUser);
	}
}