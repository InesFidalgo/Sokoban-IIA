using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Action {North, East, South, West, None=-1};

public class Actions
{
	private static readonly Action[] all_actions = { Action.North, Action.East, Action.South, Action.West };
    
	public static Action[] GetAll()
	{
		return all_actions;
	}

	public static Vector2 GetVector(Action a)
	{
		if (a == Action.North) {
			return new Vector2(0, 1);
		} else if (a == Action.East) {
			return new Vector2(1, 0);
		} else if (a == Action.South) {
			return new Vector2(0, -1);
		} else {
			return new Vector2(-1, 0);
		}
	}

	public static Action Reverse(Action dir)
	{
		if (dir == Action.North) {
			return Action.South;
		} else if (dir == Action.East) {
			return Action.West;
		} else if (dir == Action.South) {
			return Action.North;
		} else {
			return Action.East;
		}
	}

	public static string ToString(Action a)
	{
		if (a == Action.North) {
			return "North";
		} else if (a == Action.East) {
			return "East";
		} else if (a == Action.South) {
			return "South";
		} else {
			return "West";
		}
	}
}


public class SearchAgent : MonoBehaviour {
    private int cellSize;
	private SearchAlgorithm search;
	private List<Action> path;
	private GameObject[] crates;

	void Start () {
		// Get the cell size from the map.
		cellSize = GameObject.Find("Map").GetComponent<Map>().cellSize;

		// Get the search algorithm to use from the map
		search = GameObject.Find ("Map").GetComponent<SearchAlgorithm> ();
		search.StartRunning ();

		//Get the crate objects
		crates = GameObject.FindGameObjectsWithTag ("Crate");
       
	}
	
	// Update is called once per frame
	void Update () {
		if (path == null && search.Finished ()) {
			Debug.Log ("Visited: " + search.problem.GetVisited ().ToString ());
			Debug.Log ("Expanded: " + search.problem.GetExpanded ().ToString ());
			path = search.GetActionPath ();
			if (path != null) {
				Debug.Log ("Path Length: " + path.Count.ToString ());
				Debug.Log ("[" + string.Join (",", path.ConvertAll<string> (Actions.ToString).ToArray()) + "]");
                
            }
		}
	}

	void FixedUpdate() {
		if (path != null && path.Count > 0) {
			Time.timeScale = 0.05f;
			Action action = path [0];
			path.RemoveAt (0);

			Vector3 movement = Actions.GetVector (action);

			Move (movement);
		}

	}

	void Move(Vector3 movement)
	{
		Vector3 new_pos = transform.position + movement * cellSize;

		// Check if there is a crate in the new position
		foreach (GameObject crate in crates) {
			if (crate.transform.position == new_pos)
			{
				// Move crate
				crate.transform.position += movement * cellSize;
			}
		}

		// Move player
		transform.position += movement * cellSize;
	}
}
