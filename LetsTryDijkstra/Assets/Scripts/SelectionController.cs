using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour {

	public static SelectionController instance;

	public Player selectedUnit;

	public AStarGrid grid;

	public Pathfinding pathController;

	public bool isInMoveCommand= false;

	public bool isPathSelected;
	public bool isSelectingPath;

	private void Awake() {
		if(instance != null)
		{
			Destroy(this);
		}

		instance = this;
	}

	public void FinalizePath()
	{
		Debug.Log("Path selected");
		pathController.FinalizePath(selectedUnit);
		selectedUnit.isSelected = false;
		selectedUnit = null;	
		isInMoveCommand = false;
		isPathSelected = true;

	}

	public void ShowPathsFrom(Player p)
	{
		ResetAndDeselect();
		isPathSelected = false;
		grid.ResetGrid();
		selectedUnit = p;
		pathController.FindPaths(grid.GridFromWorldPoint(p.transform.position));

	}



	private void Update() //Also need to make this not be terrible later
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			ResetAndDeselect();
			grid.ResetGrid();
		}

		if(Input.GetKeyDown(KeyCode.E))
		{
			isSelectingPath = !isSelectingPath;
		}
		
	}

	public void ResetAndDeselect()
	{
		if(selectedUnit != null)
		{
			selectedUnit.isSelected = false;
			selectedUnit = null;
			isPathSelected = false;
		}
			
	}

}
