using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GreedySearchdistCrateGoal : SearchAlgorithm {

	private List<SearchNode> openList = new List<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();
	private float h_node;

	void Start () 
	{
		problem = GameObject.Find ("Map").GetComponent<Map> ().GetProblem();
		SearchNode start = new SearchNode (problem.GetStartState (), 0);
		openList.Add (start);
	}

	public List<SearchNode> insertion (List <SearchNode> lista)
	{
		int i, j;

		for (i = 1; i < lista.Count; i++)
		{
			SearchNode temp = lista[i];
			j = i;
			while ((j > 0) && (lista[j-1].h > temp.h))
			{
				lista[j] = lista[j-1];
				j = j-1;
			}
			
			lista[j] = temp;
		}

		return lista;
	}
	
	protected override void Step()
	{
		
		if (openList.Count > 0)
		{
			SearchNode cur_node = openList[0]; 
			openList.RemoveAt(0);
			closedSet.Add (cur_node.state);

			if (problem.IsGoal (cur_node.state)) 
			{
				solution = cur_node;
				finished = true;
				running = false;
			} 

			else {
					
						Successor[] sucessors = problem.GetSuccessors (cur_node.state);
						
						foreach (Successor suc in sucessors) 
						{
							if (!closedSet.Contains (suc.state)) 
							{
								List <SearchNode> novaLista = new List <SearchNode> ();
								h_node = problem.distCrateGoal (suc.state);
								SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, h_node, suc.action, cur_node);
								openList.Add (new_node);
								novaLista = insertion(openList);
							}
						}
			}
		}
		else
		{
			finished = true;
			running = false;
		}
	}
}
