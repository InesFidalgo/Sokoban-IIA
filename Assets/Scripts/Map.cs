using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Map : MonoBehaviour {

	public TextAsset map;
	public int cellSize = 4;

	public GameObject wallPrefab;
	public GameObject playerPrefab;
	public GameObject goalPrefab;
	public GameObject cratePrefab;

	private bool[,] walls;
	private List<Vector2> goals = new List<Vector2> ();
	private List<Vector2> crates = new List<Vector2> ();
	private Vector2 player_start;
	private ISearchProblem problem;

	void Awake () {
		// Read map file
		string[] mapString = map.text.TrimEnd('\n').Split('\n');
		int width = mapString [0].Length;
		int height = mapString.Length;
		Vector2 pos;

		// Create game objects from map file
		for (int y = height - 1; y >= 0; y--) {
			for (int x = 0; x < width; x++) {
				pos = new Vector2 (x * cellSize, (height - y - 1) * cellSize);

				if(mapString[y][x] == '#') {
					Instantiate (wallPrefab, pos, Quaternion.identity);
				}
				else if(mapString[y][x] == '.') {
					Instantiate (goalPrefab, pos, Quaternion.identity);
				}
				else if(mapString[y][x] == '$') {
					Instantiate (cratePrefab, pos, Quaternion.identity);
				}
				else if(mapString[y][x] == '@') {
					Instantiate (playerPrefab, pos, Quaternion.identity);
				}

			}
		}

		// Create map information structures
		walls = new bool[height,width];
		for (int y = height - 1; y >= 0; y--) {
			for (int x = 0; x < width; x++) {
				pos = new Vector2 (x, height - y - 1);
				int new_y = height - y - 1;

				if (mapString[y][x] == '#') {
					walls [new_y, x] = true;
				}
				else if(mapString[y][x] == '$') {
					crates.Add (pos);
					walls [new_y, x] = false;
				}
				else if(mapString[y][x] == '.') {
					goals.Add (pos);
					walls [new_y, x] = false;
				}
				else if(mapString[y][x] == '@') {
					player_start = pos;
					walls [new_y, x] = false;
				}
			}
		}
			
		// Position camera to view the whole map.
		Camera.main.orthographicSize = height * cellSize / 2 + 1;
		Camera.main.transform.position = new Vector3 (width * cellSize / 2 - cellSize / 2, 
			height * cellSize / 2 - cellSize / 2, -10f);

		// Initialize the search problem
		problem = new SokobanProblem(this);
	}

	public ISearchProblem GetProblem()
	{
		return problem;
	}

	public bool[,] GetWalls()
	{
		return walls;
	}

	public List<Vector2> GetCrates()
	{
		return crates;
	}

	public List<Vector2> GetGoals()
	{
		return goals;
	}

	public Vector2 GetPlayerStart()
	{
		return player_start;
	}
}
