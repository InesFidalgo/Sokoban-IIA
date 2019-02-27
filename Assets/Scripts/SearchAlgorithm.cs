using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchNode
{	
	public object state;
	public float g;
	public float f;
	public float h;
	public Action action;
	public SearchNode parent;
	public int depth;

	public SearchNode(object state, float g, Action action=Action.None, SearchNode parent=null)
	{
		this.state = state;
		this.g = g;
		this.f = g;
		this.action = action;
		this.parent = parent;
		if (parent != null) {
			this.depth = parent.depth + 1;
		} else {
			this.depth = 0;
		}
	}

	public SearchNode(object state, float g, float h, Action action=Action.None, SearchNode parent=null)
	{
		this.state = state;
		this.g = g;
		this.f = g + h;
		this.h = h;
		this.action = action;
		this.parent = parent;
		if (parent != null) {
			this.depth = parent.depth + 1;
		} else {
			this.depth = 0;
		}
	}
}


public abstract class SearchAlgorithm : MonoBehaviour {

	public int stepsPerFrame = 100;
	[HideInInspector]public ISearchProblem problem;

	protected bool running = false;
	protected bool finished = false;
	protected SearchNode solution = null;

	void Update () {
		if (running && !finished) {
			for (int i = 0; i < stepsPerFrame; i++) {
				if (!finished) {
					Step ();
				}
			}
		}
	}

	public bool Finished()
	{
		return finished;
	}

	public List<Action> GetActionPath()
	{
		if (finished && solution != null) {
			return BuildActionPath ();
		} else {
			Debug.LogWarning ("Solution path can not be determined! Either the algorithm has not finished, or a solution could not be found.");
			return null;
		}
	}

	// This method should be overriden on each specific search algorithm.
	protected abstract void Step ();

	public void StartRunning()
	{
		running = true;
	}

	private List<Action> BuildActionPath()
	{
		List<Action> path = new List<Action> ();
		SearchNode node = solution;

		while (node.parent != null) {
			path.Insert (0, node.action);
			node = node.parent;
		}

		return path;
	}
}
