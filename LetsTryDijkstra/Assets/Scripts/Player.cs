using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public bool isSelected = false;
	float Speed = 13f;
	public Vector3[] path;
	int targetIndex;

	private void OnMouseDown() 
	{
		SelectionController.instance.ShowPathsFrom(this);
		
	}

	public void StartPath (Vector3[] pathArray)
	{
		path = pathArray;
		StopCoroutine("FollowPath");
		StartCoroutine("FollowPath");
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];

		while (true)
		{
			if(transform.position == currentWaypoint)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					targetIndex = 0;
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
			yield return null;
		}
	}
	
	
	void Update () {
		//Change and optimize later
		if(SelectionController.instance.selectedUnit != this)
		{
			isSelected = false;
		}
	}
}
