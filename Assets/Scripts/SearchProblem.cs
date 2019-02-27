using UnityEngine;
using System.Collections;


public struct Successor
{
	public object state;
	public float cost;
	public Action action;


	public Successor(object state, float cost, Action a)
	{
		this.state = state;
		this.cost = cost;
		this.action = a;
	}
}


public interface ISearchProblem
{
	object GetStartState ();
	bool IsGoal (object state);
	int getRemainingGoals(object state);
    float distEuclGoal(object state);
    float distEuclPlayer(object state);
    float distCratePlayer (object state);
	float distCrateGoal (object state);
	Successor[] GetSuccessors (object state);

	int GetVisited ();
	int GetExpanded ();
}
