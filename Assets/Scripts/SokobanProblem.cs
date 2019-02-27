using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SokobanState 
{

	public List<Vector2> crates;
	public Vector2 player;


	public SokobanState(List<Vector2> crates, Vector2 player)
	{
		this.crates = crates;
		this.player = player;
	}

	// Copy constructor
	public SokobanState(SokobanState other)
	{
		if (other != null) {
			this.crates = new List<Vector2> (other.crates);
			this.player = other.player;
		}
	}

	// Compare two states. Consider that each crate is in the same index in the array for the two states.
	public override bool Equals(System.Object obj)
	{
		if (obj == null) 
		{
			return false;
		}

		SokobanState s = obj as SokobanState;
		if ((System.Object)s == null)
		{
			return false;
		}

		if (player != s.player) {
			return false;
		}
			
		for (int i = 0; i < crates.Count; i++)
		{
			if (crates[i] != s.crates[i])
			{
				return false;
			}
		}

		return true;
	}

	public bool Equals(SokobanState s)
	{
		if ((System.Object)s == null) 
		{
			return false;
		}

		if (player != s.player) {
			return false;
		}

		for (int i = 0; i < crates.Count; i++)
		{
			if (crates[i] != s.crates[i])
			{
				return false;
			}
		}

		return true;
	}

	public override int GetHashCode()
	{
		int hc = crates.Count;
		for(int i = 0; i < crates.Count; i++)
		{
			hc = unchecked(hc * 17 + crates[i].GetHashCode());
		}

		return hc ^ player.GetHashCode ();
	}

	public static bool operator == (SokobanState s1, SokobanState s2)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(s1, s2))
		{
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)s1 == null) || ((object)s2 == null))
		{
			return false;
		}

		if (s1.player != s2.player) {
			return false;
		}

		for (int i = 0; i < s1.crates.Count; i++)
		{
			if (s1.crates[i] != s2.crates[i])
			{
				return false;
			}
		}

		return true;
	}

	public static bool operator != (SokobanState s1, SokobanState s2)
	{
		return !(s1 == s2);
	}
}


public class SokobanProblem : ISearchProblem {
	private bool[,] walls;
	private List<Vector2> goals;
	private SokobanState start_state;
	private Action[] allActions = Actions.GetAll();

	private int visited = 0;
	private int expanded = 0;

	public SokobanProblem(Map map)
	{
		walls = map.GetWalls ();
		goals = map.GetGoals ();

		List<Vector2> crates_copy = new List<Vector2> (map.GetCrates ());
		start_state = new SokobanState (crates_copy, map.GetPlayerStart());
	}

	public object GetStartState()
	{
		return start_state;
	}

	public bool IsGoal (object state)
	{
		SokobanState s = (SokobanState)state;
		int remainingGoals = goals.Count;

		foreach (Vector2 crate in s.crates) {
			if (goals.Contains (crate)) {
				remainingGoals--;
			}
		}

		if (remainingGoals == 0) {
			return true;
		}

		return false;
	}

	public Successor[] GetSuccessors(object state)
	{
		SokobanState s = (SokobanState)state;

		visited++;

		List<Successor> result = new List<Successor> ();

		foreach (Action a in allActions) {
			Vector2 movement = Actions.GetVector (a);

			if (CheckRules(s, movement))
			{
				expanded++;

				SokobanState new_state = new SokobanState (s);

				new_state.player += movement;

				for (int i = 0; i < new_state.crates.Count; i++) {
					if (new_state.crates[i] == new_state.player) {
						new_state.crates[i] += movement;
						break;
					}
				}
					
				result.Add (new Successor (new_state, 1f, a));
			}
		}

		return result.ToArray ();
	}

	public int GetVisited()
	{
		return visited;
	}

	public int GetExpanded()
	{
		return expanded;
	}

	private bool CheckRules(SokobanState state, Vector2 movement)
	{
		Vector2 new_pos = state.player + movement;

		// Move to wall?
		if (walls [(int)new_pos.y, (int)new_pos.x]) {
			return false;
		}

		// Crate in front and able to move?
		int index = state.crates.IndexOf(new_pos);
		if (index != -1) {
			Vector2 new_crate_pos = state.crates [index] + movement;

			if (walls [(int)new_crate_pos.y, (int)new_crate_pos.x]) {
				return false;
			}

			if (state.crates.Contains(new_crate_pos)) {
				return false;
			}
		}

		return true;
	}

	public int getRemainingGoals (object state)
	{
		SokobanState s = (SokobanState)state;

		int remainingGoals = goals.Count;

		foreach (Vector2 crate in s.crates) {
			if (goals.Contains (crate)) {
				remainingGoals--;
			}
		}
		return remainingGoals;
	}

	public float distCratePlayer (object state)
	{
		SokobanState s = (SokobanState)state;
		float distancia = float.MaxValue;
		float total = 0;

		foreach (Vector2 crate in s.crates) 
		{
			distancia = Mathf.Min (distancia, (Mathf.Abs (crate.x - s.player.x)+ Mathf.Abs(crate.y - s.player.y)));
			total += distancia;
		}
		return total;
	}

	public float distCrateGoal (object state)
	{
		SokobanState s = (SokobanState)state;
		float distancia = float.MaxValue;
		float total = 0;

		foreach (Vector2 crate in s.crates) 
		{
			foreach (Vector2 goal in goals) 
			{
				distancia = Mathf.Min (distancia, (Mathf.Abs (crate.x - goal.x)+ Mathf.Abs(crate.y - goal.y)));
				total += distancia;
			}

		}
		return total;
	}

    public float distEuclGoal (object state)
    {
        SokobanState s = (SokobanState)state;
        float distancia = float.MaxValue;
        float total = 0;

        foreach(Vector2 crate in s.crates)
        {
            foreach(Vector2 goal in goals)
            {
                distancia = Mathf.Min(distancia, (Mathf.Sqrt(((crate.x - goal.x) * (crate.x - goal.x) + (crate.y - goal.y) * (crate.y - goal.y)))));
                total += distancia;
            }
        }
        return total;


    }

    public float distEuclPlayer(object state)
    {
        SokobanState s = (SokobanState)state;
        float distancia = float.MaxValue;
        float total = 0;

        foreach (Vector2 crate in s.crates)
        {
            
            distancia = Mathf.Min(distancia, (Mathf.Sqrt(((crate.x - s.player.x) * (crate.x - s.player.x) + (crate.y - s.player.y) * (crate.y - s.player.y)))));
            total += distancia;
            
        }
        return total;


    }




}
