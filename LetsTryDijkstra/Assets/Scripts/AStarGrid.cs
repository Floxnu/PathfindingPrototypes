using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour {

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public Node[,] grid;
	public float nodeRadius;
	public GameObject cubePrefab;
	public bool OnlyDisplayPathGizmos;
	//public Transform player;

	float nodeDiameter;
	int gridSizeX;
	int gridSizeY;


	public List<Node> path;

	public int MaxSize
	{
		get {
			return gridSizeX * gridSizeY;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, 1, gridWorldSize.y));

		if(OnlyDisplayPathGizmos)
		{
			if(path != null)
			{
				foreach (Node n in path)
				{
					Gizmos.color = Color.black;
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f) - Vector3.up * nodeRadius);
				}
			}
		}else{
			if (grid != null) 
			{
	//			Node playeNode = GridFromWorldPoint (player.position);
				foreach (Node n in grid) 
				{
					Gizmos.color = (n.walkable) ? Color.white : Color.red;
					//if (playeNode == n) {
					//	Gizmos.color = Color.magenta;
					//}
					if (path != null)
					{
						if (path.Contains(n))
						{
							if(SelectionController.instance.isInMoveCommand)
							{
								Gizmos.color = Color.black;
							} else 
							{
								Gizmos.color = Color.red;
							}
						}
					}
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f) - Vector3.up * nodeRadius);
				}
			}

		}

	}

	// Use this for initialization
	void Start () 
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		CreateGrid ();

		SelectionController.instance.grid = this;

	}

	public Node GridFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid [x, y];
	}


	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					if((checkX == node.gridX && checkY != node.gridY) || (checkX != node.gridX && checkY == node.gridY))
					{
						neighbours.Add(grid[checkX, checkY]);
					}
					
				}
			}
		}

		return neighbours;
	}

	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY];
		GameObject currentObject = null;
		Vector3 gridBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++) 
		{
			for (int y = 0; y < gridSizeY; y++) 
			{
				Vector3 worldPoint = gridBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere (worldPoint, nodeRadius, unwalkableMask));
				grid [x, y] = new Node (walkable, worldPoint, x, y);

				

				currentObject = Instantiate(cubePrefab, worldPoint, Quaternion.identity);
				print(worldPoint);
				currentObject.name = "GridPoint " + x + y; 
				currentObject.GetComponent<GridSquare>().parentNode = grid[x, y];
					//currentObject.transform.localScale = new Vector3(1.5f, .1f, 1.5f);
					//currentObject.tag = "grid";
					//currentObject.AddComponent<GridSquare>();

				if(walkable && currentObject != null)
				{
					currentObject.GetComponent<Renderer>().material.color = Color.gray;
				} 
				else if (currentObject != null)
				{
					currentObject.GetComponent<Renderer>().material.color = Color.red;
				}
			}
		}
	}


	public void ResetGrid()
	{	
		foreach(Node n in grid)
		{
			n.isInPath = false;
			n.totalCost = int.MaxValue;
			n.parent = null;
		}
		if(path != null)
		{
			foreach (Node n in path)
			{
				n.isInPath = false;
			}

			path.Clear();
		}
	}

	private void Update() {
		if (path != null){
			foreach(Node n in grid)
			{
				if(path.Contains(n))
				{
					if(SelectionController.instance.isInMoveCommand)
					{
						n.isInPath = true;
					} else 
					{
						n.isPathSet = true;
					}
				} else 
				{
					n.isInPath = false;
					n.isPathSet = false;
				}
			}
		} 
	}
}
