using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour {

	public GameObject human;
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
	public int width;
	/// <summary>
	/// The size of the maze in y.
	/// </summary>
	public int height;
	/// <summary>
	/// x & y where the algorithm starts running.
	/// </summary>
	public int startingX, startingY;
	private string[,] maze;

	void Start () {
		maze = new string[width, height];
		for(int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				maze[i, j] = "n";
			}
		}
		maze[startingX, startingY] = "p"; //TODO: Try starting in the center.
		System.Random r = new System.Random();
		Generate(Vector2.zero, new Vector2(startingX, startingY - 1));
		//		Generate(Vector2.zero, new Vector2(0, -1));
		ClearSurroundings(startingX, startingY, 1);
		for(int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				if(maze[i, j] == "w"){
					GameObject go = (GameObject)Instantiate(
						wall,
                        new Vector3(i * wall.renderer.bounds.size.x, 
					            wall.renderer.bounds.size.y / 2, 
					            j * wall.renderer.bounds.size.z), 
                        		floor.transform.rotation
                        );
					int index = r.Next(wallMaterials.Length);
					go.renderer.material = wallMaterials[index];
					go.transform.parent = this.transform;
				}
			}
		}
		Instantiate(human, new Vector3(startingX * wall.renderer.bounds.size.x, human.transform.position.y, startingY * wall.renderer.bounds.size.y),
		            human.transform.rotation);
		PlaceFloor();
		CreateBarriers();
	}
	/// <summary>
	/// Clears the surroundings of the specific tile at (x, y).
	/// </summary>
	/// <param name="startingX">x coordinate.</param>
	/// <param name="startingY">y coordinate.</param>
	/// <param name="range">The range around to clear.</param>
	/// <param name="width">The bigger matrix's width</param>
	/// <param name="heigth">The bigger matrix's heigth</param>
	void ClearSurroundings (int x, int y, int range)
	{
		for(int i = x - range; i <= x + range; i++){
			for(int j = y - range; j <= y + range; j++){
				maze[i, j] = "p";
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
		if(i > 0 && (maze[i - 1, j] == "n" || maze[i - 1, j] == "w")){
			count++;
			positions.Add(new Vector2(i - 1,j));
		}
		if(i < width - 1 && (maze[i + 1, j] == "n" || maze[i + 1, j] == "w")){
			count++;
			positions.Add(new Vector2(i + 1, j));
		}
		if(j > 0 && (maze[i, j - 1] == "n" || maze[i , j - 1] == "w")){
			count++;
			positions.Add(new Vector2(i, j - 1));
		}
		if(j < height - 1 && (maze[i, j + 1] == "n" || maze[i, j+1] == "w")){
			count++;
			positions.Add(new Vector2(i, j + 1));
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
		//is it in (0, 0) or (width -1, heigth - 1)?
		bool a = (p.x == 0 && p.y == 0)||(p.x == width-1 && p.y == height-1);
		//is it in any border, be it right, left, top or bottom?
		bool b =(p.x == 0 || p.y == 0 || p.x == width-1 || p.y == height-1);
		int pr = 0;
		if(nexts.Count == 1){
			System.Random random = new System.Random();
			pr = random.Next(2);
		}
		if((nexts.Count < 3 && (!b)) || nexts.Count == 1 /*|| nexts.Count==0 */|| pr == 1){
			maze[(int)p.x, (int)p.y] = "w";
		}else{
			maze[(int)p.x, (int)p.y] = "p";
			//TODO: unused variable.
			int times = 0;
			while(nexts.Count != 0 /*&& times <4*/){
				int index = Random.Range(0, nexts.Count);
				//TODO: SERIOUSLY? ACABA DE GENERAR UN RANDOM ENTRE 0 Y COUNT EXCLUSIVO. PARA QUE PUTAS REVISA SI ES MAYOR QUE COUNT?!
				//*has a heartattack*
				if(index > nexts.Count - 1){
					//TODO: potato is never printed. index is always less than count.
					Debug.Log("potato");
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
	
	void CreateBarriers ()
	{
		for(int i = 0; i < width; i++){
			GameObject b1 = (GameObject)Instantiate(wall, new Vector3(i * wall.renderer.bounds.size.x,wall.renderer.bounds.size.y / 2, -wall.renderer.bounds.size.z), wall.transform.rotation);
			b1.transform.parent = this.transform;
			GameObject b2 = (GameObject)Instantiate(wall, new Vector3(i * wall.renderer.bounds.size.x, wall.renderer.bounds.size.y / 2, height * wall.renderer.bounds.size.z), wall.transform.rotation);
			b2.transform.parent = this.transform;
		}
		for(int i = 0; i < height; i++){
			GameObject b1 = (GameObject)Instantiate(wall, new Vector3(-wall.renderer.bounds.size.x, wall.renderer.bounds.size.y / 2, i * wall.renderer.bounds.size.z), wall.transform.rotation);
			b1.transform.parent = this.transform;
			GameObject b2 = (GameObject)Instantiate(wall, new Vector3(width * wall.renderer.bounds.size.x, wall.renderer.bounds.size.y / 2, i * wall.renderer.bounds.size.z), wall.transform.rotation);
			b2.transform.parent = this.transform;
		}
	}

	void PlaceFloor ()
	{
		Vector3 scale = new Vector3(width * wall.renderer.bounds.size.x, 0f, 
		                            height * wall.renderer.bounds.size.z);
		Vector3 position = (scale/2f) - wall.renderer.bounds.size;
		position.y = 0;

		GameObject g = (GameObject)Instantiate(floor, position, Quaternion.identity);
		g.transform.localScale = scale;
	}
}
