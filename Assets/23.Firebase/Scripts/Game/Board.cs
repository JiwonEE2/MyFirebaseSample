using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	public Cell[] cells;
	public Dictionary<string, Cell> cellDictionary = new Dictionary<string, Cell>();

	public GameObject blueMark;
	public GameObject redMark;
	public bool isHost;
	public int turnCount;

	private void Awake()
	{
		cells = GetComponentsInChildren<Cell>();
		int cellNum = 0;
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 8; x++)
			{
				cells[cellNum].coordinate = $"{(char)(x + 65)}{y + 1}";
				cellNum++;
			}
		}

		foreach (Cell cell in cells)
		{
			cell.board = this;
			cellDictionary.Add(cell.coordinate, cell);
		}
	}

	public void SelectCell(Cell cell)
	{
		Turn turn = new Turn()
		{
			isHostTurn = isHost,
			coordinate = cell.coordinate,
		};

		FirebaseManager.Instance.SendTurn(turnCount, turn);
	}

	public void PlaceMark(bool isBlue, string coordinate)
	{
		GameObject prefab = isBlue ? blueMark : redMark;
		Cell targetCell = cellDictionary[coordinate];
		Instantiate(prefab, targetCell.transform, false);
	}
}
