using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

	AStarGrid grid;
	public Vector3 target;
	List<Node> minPQ = new List<Node>();
	List<Node> visited = new List<Node>();
	public bool unitSelected;

	private void Awake() 
	{
		grid = GetComponent<AStarGrid>();

	}

	private void Start() 
	{
		SelectionController.instance.pathController = this;	
	}

	private void Update()
	{
		if(SelectionController.instance.selectedUnit != null)
		{
			Ray mouseRay;
			RaycastHit hit;
			mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(mouseRay, out hit, 1000);
			target = hit.point;

			RetracePath(grid.GridFromWorldPoint(SelectionController.instance.selectedUnit.transform.position), grid.GridFromWorldPoint(target), false, SelectionController.instance.selectedUnit);			
		}
	}

	public void FinalizePath(Player _player)
	{
		RetracePath(grid.GridFromWorldPoint(_player.transform.position), grid.GridFromWorldPoint(target), true, _player);
	}


/*	private void Update() 
	{
		if(SelectionController.instance.selectedUnit != null){
			Ray mouseRay;
			RaycastHit hit;
			mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(mouseRay, out hit, 1000);
			target = hit.point;
		}

		if(SelectionController.instance.selectedUnit != null)
		{
			FindPath(SelectionController.instance.selectedUnit.transform.position, target );		
		}
	} 
	private void Start() {
		SelectionController.instance.pathController = this;
	}*/

	public void FindPaths(Node start)
	{	
		minPQ.Clear();
		visited.Clear();

		start.totalCost = 0;
		minPQ.Add(start);
		foreach(Node n in grid.grid)
		{
			if(n != start)
			{
				n.totalCost = int.MaxValue;
			}
		}

		while(minPQ.Count > 0)
		{
			Node newSmallest = RemoveSmallest(minPQ);
			visited.Add(newSmallest);

			foreach(Node neighbor in grid.GetNeighbours(newSmallest))
			{
				
				if(!visited.Contains(neighbor) && neighbor.walkable)
				{
					if(!minPQ.Contains(neighbor))
					{
						minPQ.Add(neighbor);
					}
					int altPath = newSmallest.totalCost + neighbor.distance;

					if(altPath < neighbor.totalCost)
					{
						neighbor.totalCost = altPath;
						neighbor.parent = newSmallest;
					}

				}
			}

		}	
	}

	public Node RemoveSmallest(List<Node> list)
	{
		Node smallest = list[0];
		foreach(Node n in list)
		{
			if(n.totalCost < smallest.totalCost)
			{
				smallest = n;
			}
		}
		list.Remove(smallest);
		return smallest;
	}
	void RetracePath (Node startNode, Node endNode, bool isFinal, Player hasPlayer)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		//Collider[] currentCollision;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			//currentCollision = Physics.OverlapSphere(currentNode.worldPosition, 1);
			//foreach(Collider c in currentCollision){
			//	if(c.gameObject.tag == "grid")
			//	{
			//		c.gameObject.GetComponent<GridSquare>().isPath = true;
			//	}
			//}
			currentNode.isInPath = true;
			currentNode = currentNode.parent;
		}

		if(!isFinal)
		{
			path.Reverse();
			grid.path = path;
		}else
		{
			Vector3[] toSend = SimplifyAndSend(path);
			Array.Reverse(toSend);
			hasPlayer.StartPath(toSend);
		}
	}

	Vector3[] SimplifyAndSend(List<Node> finalPath)
	{
		List<Vector3> waypoints = new List<Vector3>();


		for (int i = 1; i < finalPath.Count; i++)
		{
			waypoints.Add(finalPath[i-1].worldPosition);	
		}
		return waypoints.ToArray();
	}

	/* 

	int GetDistance(Node nodeA, Node nodeB) //This edited later to avoid diagonals
	{
		int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if(disX>disY)
		 return 14*disY + 10*(disX-disY);
		return 14*disX + 10*(disY-disX);

	}
	*/

	
}
