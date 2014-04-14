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
	/// The sound trigger.
	/// </summary>
	public GameObject soundTrigger;
	/// <summary>
	/// The sound trigger chance of appearing (in %).
	/// </summary>
	public int soundTriggerChance = 65;
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
	/// The chance of deleting SoundTriggers after the initial pass.
	/// </summary>
	public int chanceOfDel = 95;

	private string[,] maze;
	/// <summary>
	/// The places in the maze where there is a sound trigger.
	/// </summary>
	private bool[,] mazeIsSoundTrigger;
	private bool[,] mazeSoundTriggerVisited;

	void Start () {
		maze = new string[width, height];
		mazeIsSoundTrigger = new bool[width, height];
		mazeSoundTriggerVisited = new bool[width,height];
		for(int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				maze[i, j] = "n";
				mazeIsSoundTrigger[i, j] = false;
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
				} else if(maze[i,j] == "p"){ 
					if(r.Next(100) <= soundTriggerChance){//place soundTrigger
						mazeIsSoundTrigger[i,j] = true;
					} else {
						mazeIsSoundTrigger[i,j] = false;
					}
				} 
			}
		}
		Instantiate(human, new Vector3(startingX * wall.renderer.bounds.size.x, human.transform.position.y, startingY * wall.renderer.bounds.size.z),
		            human.transform.rotation);
		PlaceFloor();
		PlaceSoundTriggers();
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
	/// Checks the available neighbors of a maze tile.
	/// </summary>
	/// <returns>The available neighbors.</returns>
	/// <param name="i">The i position of the maze</param>
	/// <param name="j">The j position of the maze</param>
	public List <Vector2> CheckAvailableNeighbors(int i, int j){
		List <Vector2> positions = new List<Vector2>();
		if(i>0 && (maze[i - 1, j] == "p")){
			positions.Add(new Vector2(i-1,j));
		}
		if(i < width - 1 && (maze[i + 1, j] == "p")){
			positions.Add(new Vector2(i+1, j));
		}
		if(j > 0 && (maze[i, j - 1] == "p")){
			positions.Add(new Vector2(i, j-1));
		}
		if(j < height - 1 && (maze[i, j + 1] == "p")){
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
		Vector3 scale = new Vector3(width * wall.renderer.bounds.size.x+10, 0f, 
		                            height * wall.renderer.bounds.size.z+10);
		Vector3 position = (scale/2f) - wall.renderer.bounds.size;
		position.y = 0;

		GameObject g = (GameObject)Instantiate(floor, position, Quaternion.identity);
		g.transform.localScale = scale;

	}

	void PlaceSoundTriggers(){
		for(int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				if(mazeIsSoundTrigger[i,j]){
					GameObject go = (GameObject)Instantiate(
						soundTrigger,
						new Vector3(i * wall.renderer.bounds.size.x, 
					            wall.renderer.bounds.size.y / 2, 
					            j * wall.renderer.bounds.size.z), 
						floor.transform.rotation
					);
					go.transform.parent = this.transform;

					cleanVisited();
					checkSoundTriggerNeighbors(new Vector2(i,j), width+1);
				}
			}
		}
	}

	void checkSoundTriggerNeighbors(Vector2 pos, int val){
		System.Random r = new System.Random();

		mazeSoundTriggerVisited[(int)pos.x, (int)pos.y] = true;

		if(val == 1) return ;

		if(mazeIsSoundTrigger[(int)pos.x, (int)pos.y]){
			if(val != width+1){
				int ran = r.Next(100);
				if((ran) < (100 - chanceOfDel * ((float) val/ (float)width))){  //determinar si se queda o no, 
					mazeIsSoundTrigger[(int)pos.x,(int)pos.y] = true;           //menor probabilidad de dejarlo si esta cerca

				} else {
					mazeIsSoundTrigger[(int)pos.x,(int)pos.y] = false;
				}
			} 
		}

		List<Vector2> neighbors = CheckAvailableNeighbors((int) pos.x, (int) pos.y);

		foreach(Vector2 next in neighbors){
			if(mazeSoundTriggerVisited[(int)next.x, (int)next.y] == false){
				checkSoundTriggerNeighbors(next, val-1);
			}
		}


	}

	void cleanVisited(){
		for(int i = 0; i < width; i++){
			for(int j = 0; j < height; j++){
				mazeSoundTriggerVisited[i,j] = false;
			}
		}
	}

}
