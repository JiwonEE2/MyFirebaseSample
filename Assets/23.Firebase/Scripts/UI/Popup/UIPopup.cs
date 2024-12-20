using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
	protected Action callback;

	public TextMeshProUGUI title;
	public TextMeshProUGUI message;
	public Button closeButton;

	protected virtual void Awake()
	{
		closeButton.onClick.AddListener(CloseButtonClick);
	}

	public virtual void SetPopup(string title, string message, Action callback = null)
	{
		this.title.text = title;
		this.message.text = message;
		this.callback = callback;
	}

	protected virtual void CloseButtonClick()
	{
		UIManager.Instance.PopupClose();
		callback?.Invoke();
	}
}
