using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
	public TextMeshProUGUI title;
	public TextMeshProUGUI message;
	public Button closeButton;

	private Action callback;

	private void Awake()
	{
		closeButton.onClick.AddListener(CloseButtonClick);
	}

	public void SetDialog(string title, string message, Action callback = null)
	{
		this.title.text = title;
		this.message.text = message;
		this.callback = callback;
	}

	private void CloseButtonClick()
	{
		if (callback is null)
		{
			UIManager.Instance.PageOpen<UIHome>();
		}
		else
		{
			callback();
		}
	}
}
