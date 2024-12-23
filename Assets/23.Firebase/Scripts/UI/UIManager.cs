using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	public List<MonoBehaviour> uiPages;
	public List<UIPopup> popups;

	private Stack<UIPopup> openPopups = new Stack<UIPopup>();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		_ = PageOpen("UIMain");
		foreach (UIPopup popup in popups)
		{
			popup.gameObject.SetActive(false);
		}
	}

	public T PopupOpen<T>() where T : UIPopup
	{
		// 팝업 찾기
		T @return = popups.Find((popup) => popup is T) as T;
		// 찾는 팝업이 있으면
		if (@return != null)
		{
			// 팝업 활성화
			@return.gameObject.SetActive(true);
			// 활성 팝업 스택에 추가
			openPopups.Push(@return);
		}
		return @return;
	}

	public void PopupClose()
	{
		// 활성 팝업 스택에 팝업이 있으면
		if (openPopups.Count > 0)
		{
			// 꺼냄
			UIPopup targetPopup = openPopups.Pop();
			// 비활성화
			targetPopup.gameObject.SetActive(false);
		}
	}

	public T PageOpen<T>() where T : UIPage
	{
		T @return = null;
		foreach (UIPage uiPage in uiPages)
		{
			bool isActive = uiPage is T;
			uiPage.gameObject.SetActive(isActive);
			if (isActive) @return = uiPage as T;
		}
		return @return;
	}

	public UIPage PageOpen(string pageName)
	{
		UIPage @return = null;
		foreach (UIPage uiPage in uiPages)
		{
			bool isActive = uiPage.GetType().Name.Equals(pageName);
			uiPage.gameObject.SetActive(isActive);
			if (isActive) @return = uiPage;
		}
		return @return;
	}
}
