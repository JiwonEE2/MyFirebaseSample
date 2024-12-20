using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	public Cell[] cells;
	public Dictionary<string, Cell> cellDictionary = new Dictionary<string, Cell>();

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
			cellDictionary.Add(cell.coordinate, cell);
		}
	}
}
