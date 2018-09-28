using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquare : MonoBehaviour {

	public bool isPath;
	public bool isMarkedPath;
	private bool wasPath;
	private Renderer renderRef;
	public Node parentNode;
	
	private void Start()
	{
		renderRef = GetComponent<Renderer>();
	}

	private void OnMouseDown() 
	{
		if(parentNode.walkable && SelectionController.instance.isSelectingPath)
		{
			parentNode.walkable = false;
		}else if (SelectionController.instance.isSelectingPath)
		{
			parentNode.walkable = true;
		}
		
		if(SelectionController.instance.selectedUnit != null && !SelectionController.instance.isSelectingPath && parentNode.totalCost <= 5)
		{
			SelectionController.instance.FinalizePath();
			
		}
	}

	void LateUpdate () 
	{
		if(isPath)
		{
			renderRef.material.color = Color.black;
			wasPath = true;
			isPath = false;
		}
		else
		{
			if(wasPath && SelectionController.instance.isInMoveCommand == false)
			{
				renderRef.material.color = Color.blue;
			}
			else
			{
				renderRef.material.color = Color.gray;
				wasPath = false;
			}
		}
			
		if(SelectionController.instance.isPathSelected == true && parentNode.isInPath){
			renderRef.material.color = Color.yellow;
		} else if(parentNode.isInPath == true)
		{
			renderRef.material.color = Color.green;
		} else{
			if(parentNode.totalCost <= 5)
			{
				renderRef.material.color = Color.magenta;
			} else
			{
				renderRef.material.color = Color.gray;
			}
		}
		if(!parentNode.walkable)
		{
			renderRef.material.color = Color.red;
		}
	
	}
}
