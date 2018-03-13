using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace AStar {

	public class Grid : MonoBehaviour {

		//Singleton
		public static Grid main;

		public Vector2 gridWorldSize;                   //The size of the grid
		public float nodeRadius = 0.5f;                 //The radius of each node object created
		public float maxHeight = 3.0f;                  //Max height of a walkable node
		public TerrainType[] terrainWeights;            //Weights for walkable areas
		public int objectBorderPenalty = 10;            //Penalty for nodes near and around unwalkable objects
        public bool updateGrid = true;                  //Should the grid be updated
        public float updateGridInterval = 1.0f;
		public bool showGizmos = true;                  //Shows node gizmos

		//public LayerMask unwalkableMask;                //LayerMask for all unwalkable objects
														
		private int gridSizeX, gridSizeY;               //How many nodes there are in each direction
		private LayerMask walkableMask;
		private Dictionary<int, int> walkableAreaDictionary = new Dictionary<int, int>();
		private int penaltyMin = int.MaxValue;
		private int penaltyMax = int.MinValue;
		private Coroutine coroutineUpdateGridInterval;

		[HideInInspector]
		public int maxSize {
			get {
				return gridSizeX * gridSizeY;
			}
		}

		[HideInInspector]
		public float nodeDiameter {
			get {
				return nodeRadius * 2;
			}
		}


		public void Awake() {
            if(main == null) main = this;

			gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
			gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

			//Adds terrainTypes to the dictinary array
			foreach(TerrainType area in terrainWeights) {
				walkableMask.value |= area.terrainMask.value;   //Bitwise operator
				walkableAreaDictionary.Add((int)Mathf.Log(area.terrainMask.value, 2), area.terrainPenalty);
			}
		}

		public void Start() {
            //Create the grid
            CreateGrid();

            //Start updating the grid
            coroutineUpdateGridInterval = StartCoroutine(UpdateGridInterval());
		}
		
		public void Update() {
            //Manual update grid
			if(Input.GetMouseButtonDown(2)) {
				UpdateGrid();
			}

			/*
			currentTime = Time.deltaTime;
			if(currentTime >= waitTime) {
				waitTime = currentTime + waitTimeInterval;
				//UnityEngine.Debug.Log("Hey");
			}
			*/
		}

		public IEnumerator UpdateGridInterval() {
            while(true) {
                while(updateGrid) { //Can the grid update
                    yield return new WaitForSeconds(updateGridInterval);
                    UpdateGrid();
                    //UnityEngine.Debug.Log("Grid Updated");
                }
                yield return null;
            }
        }



		public Node[,] grid;
		Vector3 worldBottomLeft;

		Vector3 worldPosition;
		Vector2 gridPosition;
		bool isWalkable;
		int movementPenalty;
		Color color;
		new GameObject gameObject;
		NodeType nodeType;

        
		[Obsolete("Use CreateGrid() instead.")]
		public void CreateGridOld() {
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			grid = new Node[gridSizeX, gridSizeY];

			worldBottomLeft = this.transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

			for(int x = 0; x < gridSizeX; x++) {
				for(int y = 0; y < gridSizeY; y++) {
					//Node info
					Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
					bool isWalkable = true;
					int movementPenalty = 0;
					Color color = Color.green;
					GameObject gameObject = null;

					
					//Checks if the node has collied with anything, using Physics.Raycast
					{
						//int rayStartHeight = 50;
						//int rayLength = 100;

						//Ray ray = new Ray(worldPoint + Vector3.up * rayStartHeight, Vector3.down);
						//RaycastHit hit;
					
						//if(!Physics.Raycast(ray, out hit, rayLength, unwalkableMask)) {
							//walkable = true;
						//} else {
							//walkable = false;
						//}
					}

					//Adds y axis to worldpoint
					{
						int rayStartHeight = 50;
						int rayLength = 100;

						RaycastHit[] hits = Physics.RaycastAll(worldPoint + Vector3.up * rayStartHeight, Vector3.down, rayLength);
						for(int i = 0; i < hits.Length; i++) {
							RaycastHit hit = hits[i];

							//If you hit the terrain
							if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
								worldPoint.y = hit.point.y;

								//If the point's y is above the max height
								if(worldPoint.y > maxHeight) {
									isWalkable = false;
								}
							}

							//Set the gameobject
							if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Terrain")) {
								gameObject = hit.collider.gameObject;
							}

							//If you hit a unwalkable object
							if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Unwalkable")) {
								isWalkable = false;
							}
						}
					}

					//Check terrain weights
					{
						Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
						RaycastHit hit;
						int rayDistance = 100;
						if(Physics.Raycast(ray, out hit, rayDistance, walkableMask)) {
							walkableAreaDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
						}
					}

					//Unwalkable object border penalty
					if(!isWalkable) {
						movementPenalty += objectBorderPenalty;
					}

					//Sets color
					if(!isWalkable) {
						color = Color.red;
					}

					//Create a new node based on the info above and add it to the arrayList
					//grid[x, y] = new Node(worldPoint, isWalkable, x, y, movementPenalty, color, gameObject);
					grid[x, y] = new Node(worldPoint, new Vector2(x, y), isWalkable, movementPenalty, color, gameObject, NodeType.Nothing);
				}
			}
			BlurPenaltyMap(3);

			stopwatch.Stop();
			print("Timer: " + stopwatch.ElapsedMilliseconds + " ms");

		}

		public void CreateGrid() {
			grid = new Node[gridSizeX, gridSizeY];
			gridPosition = new Vector2();
			worldBottomLeft = this.transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

			UpdateGrid();
		}

		public void UpdateGrid() {
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			for(int x = 0; x < gridSizeX; x++) {
				for(int y = 0; y < gridSizeY; y++) {
					//Reset info
					worldPosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
					gridPosition.Set(x, y);
					isWalkable = true;
					movementPenalty = 0;
					color = Color.green;
					gameObject = null;
					nodeType = NodeType.Nothing;
					
					//Checks if the node has collied with anything, using Physics.RaycastAll
					{
						int rayStartHeight = 25;
						int rayLength = 30;

						RaycastHit[] hits = Physics.RaycastAll(worldPosition + Vector3.up * rayStartHeight, Vector3.down, rayLength);
						for(int i = 0; i < hits.Length; i++) {
							RaycastHit hit = hits[i];

							//If you hit the terrain
							if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
								worldPosition.y = hit.point.y;

								//If the point's y is above the max height
								if(worldPosition.y > maxHeight) {
									isWalkable = false;
								}
							}

							//Set the gameobject
							if(hit.collider.gameObject.layer != LayerMask.NameToLayer("Terrain")) {
								gameObject = hit.collider.gameObject;
							}

							//If you hit a unwalkable object
							if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Unwalkable")) {
								isWalkable = false;
								nodeType = NodeType.Obsticle;
							}
						}
					}

					//Check terrain weights
					{
						Ray ray = new Ray(worldPosition + Vector3.up * 50, Vector3.down);
						RaycastHit hit;
						int rayDistance = 100;
						if(Physics.Raycast(ray, out hit, rayDistance, walkableMask)) {
							walkableAreaDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
						}
					}

					//Unwalkable object border penalty
					if(!isWalkable) {
						movementPenalty += objectBorderPenalty;
					}
					

					//Create a new node based on the info above and add it to the arrayList
					if(grid[x, y] != null) {
						grid[x, y].Set(isWalkable, movementPenalty, color, gameObject, nodeType);
					} else {
						grid[x, y] = new Node(worldPosition, new Vector2(x, y), isWalkable, movementPenalty, color, gameObject, nodeType);
					}
				}
			}
			BlurPenaltyMap(3);

			stopwatch.Stop();
			//print("Timer: " + stopwatch.ElapsedMilliseconds + " ms");

		}

        //Blurs the penalty map for all of the weight terran
        public void BlurPenaltyMap(int blurSize) {
            int kernelSize = blurSize * 2 + 1;
            int kernalExtends = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

            //Horizontal Pass
            for(int y = 0; y < gridSizeY; y++) {
                for(int x = -kernalExtends; x <= kernalExtends; x++) {
                    int sampleX = Mathf.Clamp(x, 0, kernalExtends);

                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                }
                for(int x = 1; x < gridSizeX; x++) {
                    int removeIndex = Mathf.Clamp(x - kernalExtends - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernalExtends, 0, gridSizeX-1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x-1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                }
            }

            //Vertical Pass
            for(int x = 0; x < gridSizeX; x++) {
                for(int y = -kernalExtends; y <= kernalExtends; y++) {
                    int sampleY = Mathf.Clamp(y, 0, kernalExtends);

                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                grid[x, 0].movementPenalty = blurredPenalty;

                for(int y = 1; y < gridSizeY; y++) {
                    int removeIndex = Mathf.Clamp(y - kernalExtends - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernalExtends, 0, gridSizeY-1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y-1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    grid[x, y].movementPenalty = blurredPenalty;

                    if(blurredPenalty > penaltyMax) {
                        penaltyMax = blurredPenalty;
                    }
                    if(blurredPenalty < penaltyMin) {
                        penaltyMin = blurredPenalty;
                    }
                }
            }
        }



        //Returns the neighbouring nodes around the passed in node as a list
        public List<Node> GetNeighbours(Node node) {
			List<Node> neighbours = new List<Node>();

			for(int x = -1; x <= 1; x++) {
				for(int y = -1; y <= 1; y++) {
					if(x == 0 && y == 0) {
						continue;
					}

					int checkX = node.gridX + x;
					int checkY = node.gridY + y;

					//Checks if the current node is not outside of the grid (The edge/corner of the grid)
					if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
						neighbours.Add(grid[checkX, checkY]);
					}
				}
			}
			//-1, 0, 1
			//-1, 0, 1
			//-1, 0, 1

			return neighbours;
		}

		//Returns all nodes that the object is currently over
		public List<Node> GetAllNodesWithin(GameObject gameObject) {
			List<Node> list = new List<Node>();

			for(int x = 0; x < gridSizeX; x++) {
				for(int y = 0; y < gridSizeY; y++) {
					if(grid[x, y].gameObject == gameObject) {
						list.Add(grid[x, y]);
					}
				}
			}

			return list;
		}

		//Returns all neighbouring nodes around the object
		public List<Node> GetNeighbourNodesAroundObject(GameObject gameObject) {
			List<Node> allNeighbourNodes = new List<Node>();    //Stores all of the neighbor nodes around the gameObject

			List<Node> allNodeWithin = Grid.main.GetAllNodesWithin(gameObject);    //Stores all of the nodes that the gameObject is currently on (On top of)
			foreach(Node node in allNodeWithin) {
				List<Node> neighbours = Grid.main.GetNeighbours(node);
				foreach(Node node2 in neighbours) {
					if(!allNeighbourNodes.Contains(node2) && node2.gameObject != gameObject) {
						allNeighbourNodes.Add(node2);
					}
				}
			}

			return allNeighbourNodes;
		}
        
		//Converts vector3 world position into node on the grid
		public Node NodeFromWorldPoint(Vector3 worldPosition) {
			float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
			float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
			int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

			return grid[x, y];
		}
        


        public void OnDrawGizmos() {
            //Show grid borders
            Gizmos.DrawWireCube(this.transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if(showGizmos) {
                if(grid != null) {
                    foreach(Node node in grid) {

                        //Overrides the color
                        Gizmos.color = node.GetColor();

                        //Color penalty
                        //Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, node.movementPenalty));

                        /*
						//Set color based on if the node is walkable
						if(node.isWalkable) {
							//Gizmos.color = Color.green;
						} else {
							Gizmos.color = Color.red;
						}
						*/

                        //Draw the node
                        //Gizmos.DrawCube(node.worldPosition, Vector3.one * nodeDiameter);
                        //Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                        Gizmos.DrawSphere(node.worldPosition, (nodeRadius - 0.1f) / 3);

                        //UnityEditor.Handles.Label(node.worldPosition + Vector3.up, "" + node.fCost);
                    }
                }
            }
        }



        //Nested TerrainType class used in terrain weights
        [System.Serializable]
		public class TerrainType {
			public LayerMask terrainMask;
			public int terrainPenalty;
		}

	}

}

/*
public float maxSlope = 90;

//Use height raycasting normal for max slope calculation. True if #maxSlope is less than 90 degrees. 
public bool useRaycastNormal { get { return Math.Abs(90-maxSlope) > float.Epsilon; } }

// Check if the node is on a slope steeper than permitted
if (walkable && useRaycastNormal && collision.heightCheck) {
	if (hit.normal != Vector3.zero) {
		// Take the dot product to find out the cosinus of the angle it has (faster than Vector3.Angle)
		float angle = Vector3.Dot(hit.normal.normalized, collision.up);

		// Add penalty based on normal
		if (penaltyAngle && resetPenalty) {
			node.Penalty += (uint)Mathf.RoundToInt((1F-Mathf.Pow(angle, penaltyAnglePower))*penaltyAngleFactor);
		}

		// Cosinus of the max slope
		float cosAngle = Mathf.Cos(maxSlope*Mathf.Deg2Rad);

		// Check if the ground is flat enough to stand on
		if (angle < cosAngle) {
			walkable = false;
		}
	}
}
*/
