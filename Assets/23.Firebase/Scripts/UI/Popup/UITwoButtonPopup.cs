using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITwoButtonPopup : UIPopup
{
	public Button okButton;
	private bool isOk = false;

	protected override void Awake()
	{
		base.Awake();
		okButton.onClick.AddListener(OkButtonClick);
	}

	public void SetPopup(string title, string message, Action<bool> callback)
	{
		base.SetPopup(title, message, () => callback?.Invoke(isOk));
	}

	private void OkButtonClick()
	{
		isOk = true;
		callback?.Invoke();
		UIManager.Instance.PopupClose();
	}

	protected override void CloseButtonClick()
	{
		isOk = false;
		callback?.Invoke();
		UIManager.Instance.PopupClose();
	}
}
