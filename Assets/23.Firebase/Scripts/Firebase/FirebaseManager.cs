using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance { get; private set; }

	public FirebaseApp App { get; private set; }
	public FirebaseAuth Auth { get; private set; }
	public FirebaseDatabase DB { get; private set; }

	private DatabaseReference usersRef;

	private DatabaseReference msgRef;

	private DatabaseReference roomRef;

	public event Action onLogin;

	public event Action<Room, bool> onGameStart;

	public event Action<Turn> onTurnProceed;

	private Room currentRoom;

	private bool isHost;

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

	// DatabaseReference.ChildAdded 이벤트에 등록할 이벤트 함수
	private void OnMessageReceive(object sender, ChildChangedEventArgs args)
	{
		// 에러 없음
		if (args.DatabaseError == null)
		{
			string rawJson = args.Snapshot.GetRawJsonValue();

			Message message = JsonConvert.DeserializeObject<Message>(rawJson);
			//print(rawJson);

			if (message.isNew)
			{
				if (message.type == MessageType.Message)
				{
					var popup = UIManager.Instance.PopupOpen<UIDialogPopup>();

					popup.SetPopup($"From.{message.sender}", $"{message.message}\n{message.GetSendTime()}");
				}
				else if (message.type == MessageType.Invite)
				{
					var popup = UIManager.Instance.PopupOpen<UITwoButtonPopup>();
					popup.SetPopup("초대장", $"{message.sender}님의 게임에 참가하시겠습니까?", ok => { if (ok) { JoinRoom(message.sender); } });
				}

				args.Snapshot.Reference.Child("isNew").SetValueAsync(false);
			}
		}
		// 에러 발생
		else
		{

		}
	}

	public async Task CreateRoom(Room room)
	{
		currentRoom = room;

		isHost = true;

		roomRef = DB.GetReference($"rooms/{Auth.CurrentUser.UserId}");

		string json = JsonConvert.SerializeObject(room);

		await roomRef.SetRawJsonValueAsync(json);

		roomRef.Child("state").ValueChanged += OnRoomStateChange;
	}

	private void OnRoomStateChange(object sender, ValueChangedEventArgs e)
	{
		object value = e.Snapshot.GetValue(true);

		int state = int.Parse(value.ToString());

		if (state == 1)
		{
			// 게임 스타트
			onGameStart?.Invoke(currentRoom, true);
			roomRef.Child("turn").ChildAdded += TurnAdded;
		}
		else if (state == 2)
		{
			// 게임 끝내기

		}
	}

	private void TurnAdded(object sender, ChildChangedEventArgs e)
	{
		string json = e.Snapshot.GetRawJsonValue();
		Turn turn = JsonConvert.DeserializeObject<Turn>(json);
		onTurnProceed?.Invoke(turn);
	}

	public void SendTurn(int turnCount, Turn turn)
	{
		turn.isHostTurn = isHost;

		string json = JsonConvert.SerializeObject(turn);

		roomRef.Child($"turn/{turnCount}").SetRawJsonValueAsync(json);
	}

	private async void JoinRoom(string host)
	{
		roomRef = DB.GetReference($"rooms/{host}");

		DataSnapshot roomSnapshot = await roomRef.GetValueAsync();

		string roomJson = roomSnapshot.GetRawJsonValue();

		Room room = JsonConvert.DeserializeObject<Room>(roomJson);

		currentRoom = room;

		isHost = false;

		await roomRef.Child("state").SetValueAsync((int)RoomState.Playing);

		onGameStart?.Invoke(room, false);
		roomRef.Child("turn").ChildAdded += TurnAdded;
	}

	public async Task MessageToTarget(string target, Message message)
	{
		DatabaseReference targetRef = DB.GetReference($"msg/{target}");
		string messageJson = JsonConvert.SerializeObject(message);
		await targetRef.Child(message.sender + message.sendTime).SetRawJsonValueAsync(messageJson);
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
	public async void Create(string email, string passwd, Action<FirebaseUser, UserData> callback = null)
	{
		try
		{
			var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);

			// 생성된 회원의 database reference를 설정
			usersRef = DB.GetReference($"users/{result.User.UserId}");

			// 회원의 데이터를 database에 생성
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

	// 로그인
	public async void SignIn(string email, string passwd, Action<FirebaseUser, UserData> callback = null)
	{
		try
		{
			var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

			usersRef = DB.GetReference($"users/{result.User.UserId}");

			DataSnapshot userDataValues = await usersRef.GetValueAsync();
			UserData userData = null;
			// DB에 경로가 존재하는 지 검사
			if (userDataValues.Exists)
			{
				string json = userDataValues.GetRawJsonValue();
				userData = JsonConvert.DeserializeObject<UserData>(json);
			}

			CurrentUserData = userData;

			callback?.Invoke(result.User, userData);
			onLogin?.Invoke();
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

	// database의 유저 데이터 수정
	public async void UpdateUserData(string childName, object value, Action<object> callback = null)
	{
		DatabaseReference targetRef = usersRef.Child(childName);
		await targetRef.SetValueAsync(value);

		callback?.Invoke(value);
	}

	internal void SignOut()
	{
		Auth.SignOut();
		msgRef.ChildAdded -= OnMessageReceive;
	}
}