using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInputFieldPopup : UIPopup
{
	public TMP_InputField inputField;

	public void SetPopup(string title, string message, Action<string> callback)
	{
		base.SetPopup(title, message, () => { callback?.Invoke(inputField.text); });
	}
}
