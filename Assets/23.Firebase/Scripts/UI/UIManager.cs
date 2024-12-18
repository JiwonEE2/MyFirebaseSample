using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	public List<MonoBehaviour> uiPages;

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		_ = PageOpen("UIHome");
	}

	public T PageOpen<T>() where T : MonoBehaviour
	{
		T @return = null;
		foreach (MonoBehaviour uiPage in uiPages)
		{
			bool isActive = uiPage is T;
			uiPage.gameObject.SetActive(isActive);
			if (isActive) @return = uiPage as T;
		}
		return @return;
	}

	public MonoBehaviour PageOpen(string pageName)
	{
		MonoBehaviour @return = null;
		foreach (MonoBehaviour uiPage in uiPages)
		{
			bool isActive = uiPage.GetType().Name.Equals(pageName);
			uiPage.gameObject.SetActive(isActive);
			if (isActive) @return = uiPage;
		}
		return @return;
	}
}
