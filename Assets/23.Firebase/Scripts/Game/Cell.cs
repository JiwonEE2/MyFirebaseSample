using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PED = UnityEngine.EventSystems.PointerEventData;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public string coordinate;

	public void OnPointerClick(PED eventData)
	{
		print(coordinate);
	}

	public void OnPointerEnter(PED eventData)
	{

	}

	public void OnPointerExit(PED eventData)
	{

	}
}
