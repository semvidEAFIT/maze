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
	/// The exit prefab (of the maze).
	/// </summary>
	public GameObject exitPrefab;
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


	private GameObject[] exitsPlaced;
	public GameObject[] ExitsPlaced {
		get {
			return exitsPlaced;
		}
		set {
			exitsPlaced = value;
		}
	}

	void Start () {
		startingX--; //para no tener que poner la mitad menos uno en el editor
		startingY--;
		exitsPlaced = new GameObject[4];
		maze = new string[width, height];
		mazeIsSoundTrigger = new bool[width, height];
		mazeSoundTriggerVisited = new bool[width,height];
		for(int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				maze[i, j] = "n";
				mazeIsSoundTrigger[i, j] = false;
			}
		}
		maze[startingX, startingY] = "p";
		System.Random r = new System.Random();

		Generate(Vector2.zero, new Vector2(startingX, startingY - 1));
		ClearSurroundings(startingX, startingY, 1);

		int[] exitsPos = PlaceExits(new Vector2(0, 0), new Vector2(width, height));

		for(int i = 0; i < width; i++){
			for (int j = 0; j < height; j++){
				if(maze[i, j] == "w"){
					GameObject go = InstantiateObject(wall, i, j);
					go.transform.parent = this.transform;
					int index = r.Next(wallMaterials.Length);
					go.renderer.material = wallMaterials[index];
				} else if(maze[i,j] == "p"){ 
					if(r.Next(100) <= soundTriggerChance){//place soundTrigger
						mazeIsSoundTrigger[i, j] = true;
					} else {
						mazeIsSoundTrigger[i, j] = false;
					}

				} 
			}
		}
		InstantiateObject(human, startingX, startingY);
		PlaceSoundTriggers();
		CreateBarriers(exitsPos);
		PlaceFloor();
	}

	/// <summary>
	/// Clears the surroundings of the specific tile at (x, y).
	/// </summary>
	/// <param name="startingX">x coordinate.</param>
	/// <param name="startingY">y coordinate.</param>
	/// <param name="range">The range around to clear.</param>
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
	/// <returns>The available neighbors in a vector2 list.</returns>
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
	
	void CreateBarriers (int[] exits)
	{
		System.Random r = new System.Random();
		for(int i = 0; i < width; i++){
			//up
			if(i == exits[0] && exits[0] != -1){
				GameObject b1 = InstantiateObject(wall, i, -2);
				b1.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b1.renderer.material = wallMaterials[index];
			} else {
				GameObject b1 = InstantiateObject(wall, i, -1);
				b1.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b1.renderer.material = wallMaterials[index];
			}
			//down
			if(i == exits[2] && exits[2] != -1){
				GameObject b2 = InstantiateObject(wall, i, height+1);
				b2.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b2.renderer.material = wallMaterials[index];
			} else {
				GameObject b2 = InstantiateObject(wall, i, height);
				b2.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b2.renderer.material = wallMaterials[index];
			}
		}
		for(int i = 0; i < height; i++){
			//rigth
			if(i == exits[1] && exits[1] != -1){
				GameObject b1 = InstantiateObject(wall, width+1, i);
				b1.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b1.renderer.material = wallMaterials[index];
			} else {
				GameObject b1 = InstantiateObject(wall, width, i);
				b1.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b1.renderer.material = wallMaterials[index];
			}
			//left
			if(i == exits[3] && exits[3] != -1){
				GameObject b2 = InstantiateObject(wall, -2, i);
				b2.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b2.renderer.material = wallMaterials[index];
			} else {
				GameObject b2 = InstantiateObject(wall, -1, i);
				b2.transform.parent = this.transform;
				int index = r.Next(wallMaterials.Length);
				b2.renderer.material = wallMaterials[index];
			}
		}

		//corners
		GameObject tl = InstantiateObject(wall, -1,-1);//top-left
		GameObject tr = InstantiateObject(wall, width,-1);//top-rigth
		GameObject bl = InstantiateObject(wall, -1,height);//bottom-left
		GameObject br = InstantiateObject(wall, width,height);//bottom-rigth
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
					GameObject go = InstantiateObject(soundTrigger, i, j);
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

	/// <summary>
	/// Finds the posible exits in the given range (open floor in the maze).
	/// </summary>
	/// <returns>The posible exits.</returns>
	/// <param name="start">Start of the range.</param>
	/// <param name="end">End of the range.</param>
	List<Vector2> FindPosibleExitsInRange(Vector2 start, Vector2 end){
		List<Vector2> ans = new List<Vector2>();
		if(start.x == end.x) {
			int i = (int) start.x;
			for(int j = 0; j < height; j++){
				if(maze[i,j] == "p") {
					ans.Add(new Vector2(i,j));
				}
			}
		} else {
			int j = (int) start.y;
			for(int i = 0; i < width; i++){
				if(maze[i,j] == "p"){
					ans.Add(new Vector2(i,j));
				}
			}
		}

		return ans;
	}


	Vector2[] SwitchCaseExits(int i, Vector2 start, Vector2 end){
		Vector2[] ans = new Vector2[2];
		switch(i){
		case 1:
			//la parte superior
			ans[0] = new Vector2(start.x,start.y);
			ans[1] = new Vector2(end.x-1,start.y);
			break;
		case 2:
			//la parte derecha
			ans[0] = new Vector2(end.x-1, start.y);
			ans[1] = new Vector2(end.x-1, end.y-1);
			break;
		case 3:
			//la parte inferior
			ans[0] = new Vector2(end.x-1, end.y-1);
			ans[1] = new Vector2(start.x, end.y-1);
			break;
		case 4:
			//la parte izquierda
			ans[0] = new Vector2(start.x, end.y-1);
			ans[1] = new Vector2(start.x,start.y);
			break;
		}
		return ans;
	}

	/// <summary>
	/// Places the exits in the maze.
	/// </summary>
	/// <returns>The position the exits where placed.</returns>
	/// <param name="start">Start of i and j in the maze (not the bounds).</param>
	/// <param name="end">End of i and j in the maze (not the bounds).</param>
	int[] PlaceExits(Vector2 start, Vector2 end){
		int[] positions = new int[4];

		for(int i = 0; i<4; i++){
			positions[i] = -1;
		}

		for(int i = 1; i < GameMaster.Instance.NumbrerOfPlayers; i++){
			Vector2[] range = SwitchCaseExits(i, start, end);

			Vector2[] posibleExitsArr = FindPosibleExitsInRange(range[0],range[1]).ToArray();

			int j = 0;
			while(posibleExitsArr.Length == 0){
				Vector2 tmpS = new Vector2(start.x+j, start.y+j);
				Vector2 tmpE = new Vector2(end.x-j, end.y-j);
				range = SwitchCaseExits(i,tmpS,tmpE);
				posibleExitsArr = FindPosibleExitsInRange(range[0],range[1]).ToArray();
			}

			Vector2 exit = posibleExitsArr[(int) (Random.value * posibleExitsArr.Length-1)];

			switch(i){
			case 1:
				//la parte superior
				positions[0] = (int)exit.x;
				exitsPlaced[0] = InstantiateObject(exitPrefab, (int)exit.x, (int)exit.y-1);
				if(exit.y != 0){
					maze[(int) exit.x,(int) exit.y-1] = "s";
				}
				break;
			case 2:
				//la parte derecha
				positions[1] = (int)exit.y;
				exitsPlaced[1] = InstantiateObject(exitPrefab, (int)exit.x+1, (int)exit.y);
				if(exit.x != width-1){
					maze[(int) exit.x+1,(int) exit.y] = "s";
				}
				break;
			case 3:
				//la parte inferior
				positions[2] = (int)exit.x;
				exitsPlaced[2] = InstantiateObject(exitPrefab, (int)exit.x, (int)exit.y+1);
				if(exit.y != height-1){
					maze[(int) exit.x,(int) exit.y+1] = "s";
				}
				break;
			case 4:
				//la parte izquierda
				positions[3] = (int)exit.y;
				exitsPlaced[3] = InstantiateObject(exitPrefab, (int)exit.x-1, (int)exit.y);
				if(exit.x != 0){
					maze[(int) exit.x-1,(int) exit.y] = "s";
				}
				break;
			}
		}
		return positions;
	}

	GameObject InstantiateObject(GameObject obj, int i, int j){
		GameObject go = (GameObject)Instantiate(
			obj,
			new Vector3(i * wall.renderer.bounds.size.x, 
		            wall.renderer.bounds.size.y / 2, 
		            j * wall.renderer.bounds.size.z), 
			floor.transform.rotation
			);
		return go;
	}
}
