using Firebase;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseTest : MonoBehaviour
{
	private async void Start()
	{
		DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
		print(status);
		AuthResult result = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync("abc@abc.abc", "abcabc");

		print(result.User.UserId);
	}
}
