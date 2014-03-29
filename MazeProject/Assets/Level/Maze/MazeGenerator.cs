using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour {
	/// <summary>
	/// The floor.
	/// </summary>
	public GameObject floor;
	/// <summary>
	/// The wall.
	/// </summary>
	public GameObject wall;
	/// <summary>
	/// The wall materials.
	/// </summary>
	public Material[] wallMaterials;
	/// <summary>
	/// The size of the maze in x.
	/// </summary>
	public int x;
	/// <summary>
	/// The size of the maze in y.
	/// </summary>
	public int y;
	private string[,] maze;

	void Start () {
		maze = new string[x, y];
		for(int i = 0; i < x; i++){
			for (int j = 0; j < y; j++){
				maze[i, j] = "n";
			}
		}
		maze[0, 0] = "p";
		System.Random r = new System.Random();
		Generate(Vector2.zero,new Vector2(0, -1));
		for(int i = 0; i < x; i++){
			for (int j = 0; j < y; j++){
				if(maze[i, j] == "p"){
					GameObject fgo = (GameObject)GameObject.Instantiate(
						floor,
						new Vector3(i * floor.renderer.bounds.size.x,
					            0, 
					            j * floor.renderer.bounds.size.z),
								floor.transform.rotation
						)
					;
					fgo.transform.parent = this.transform;
				}else if(maze[i, j] == "w"){
					GameObject go = (GameObject)Instantiate(
						wall,
                        new Vector3(i * wall.renderer.bounds.size.x, 
					            0 + (wall.renderer.bounds.size.y / 2), 
					            j * wall.renderer.bounds.size.z), 
                        		floor.transform.rotation
                        )
					;
					int index = r.Next(wallMaterials.Length);
					go.renderer.material = wallMaterials[index];
					go.transform.parent = this.transform;
				}
			}
		}
		
	}

	/// <summary>
	/// Check neighbors
	/// </summary>
	/// <returns>The neighbors.</returns>
	/// <param name="i">The position in x of the node to check.</param>
	/// <param name="j">The position in y of the node to check.</param>
	/// <param name="count">Set this variable to the number of available neighbors.</param>
	public List <Vector2> CheckNeighbors(int i, int j, out int count){
		List <Vector2> positions = new List<Vector2>();
		int max = 0;
		count = 0;
		if(i>0 && (maze[i - 1, j] == "n" || maze[i - 1, j] == "w")){
			count++;
			positions.Add(new Vector2(i-1,j));
		}
		if(i < x - 1 && (maze[i + 1, j] == "n" || maze[i + 1, j] == "w")){
			count++;
			positions.Add(new Vector2(i+1, j));
		}
		if(j > 0 && (maze[i, j - 1] == "n" || maze[i , j - 1] == "w")){
			count++;
			positions.Add(new Vector2(i, j-1));
		}
		if(j < y - 1 && (maze[i, j + 1] == "n" || maze[i, j+1] == "w")){
			count++;
			positions.Add(new Vector2(i, j+1));
		}
		return positions;
	}

	/// <summary>
	/// Generate the specified p and prev.
	/// </summary>
	/// <param name="p">The actual node.</param>
	/// <param name="prev">Previous node.</param>
	public void Generate(Vector2 p, Vector2 prev){
		int count = 0;
		List<Vector2> nexts = CheckNeighbors((int)p.x,(int)p.y, out count);
		bool a = (p.x == 0 && p.y == 0)||(p.x == x-1 && p.y == y-1);
		bool b =(p.x == 0 || p.y == 0 || p.x == x-1 || p.y == y-1);
		int pr = 0;
		if(nexts.Count == 1){
			System.Random random = new System.Random();
			pr = random.Next(2);
		}
		if((nexts.Count < 3 && (!b)) || nexts.Count == 1 /*|| nexts.Count==0 */|| pr == 1){
			maze[(int)p.x,(int)p.y] = "w";
		}else{
			maze[(int)p.x,(int)p.y] = "p";
			int times = 0;
			while(nexts.Count != 0 /*&& times <4*/){
				int index = Random.Range(0,nexts.Count);
				if(index>nexts.Count - 1){
					index = index % nexts.Count;
				}
				Vector2 next = nexts[index];
				if(maze[(int)next.x,(int)next.y] == "n"){
					Generate(next, p);
				}
				nexts.Remove(next);
			}
		}
	}
	
}
