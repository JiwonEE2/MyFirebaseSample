using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PED = UnityEngine.EventSystems.PointerEventData;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public string coordinate;

	public Board board;

	public void OnPointerClick(PED eventData)
	{
		print(coordinate);
		board.SelectCell(this);
	}

	public void OnPointerEnter(PED eventData)
	{

	}

	public void OnPointerExit(PED eventData)
	{

	}
}
