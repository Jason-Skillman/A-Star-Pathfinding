using UnityEngine;
using System;
using System.Collections;

namespace AStar {

	[Serializable]
	public class Node : IHeapable<Node> {

		public Vector3 worldPosition;               //Position in world space
		public Vector2 gridPosition;
		public bool isWalkable;                     //Is the node walkable/active
		public int movementPenalty;                 //The added penalty to the fCost
		public Color color = Color.green;           //Color of the node
		public GameObject gameObject;
		public NodeType nodeType;

		public int gridX;
		public int gridY;
		public Node parent;

		public int gCost;                           //Direction penalty
		public int hCost;                           //Distance penalty
		public int fCost {  //g + h = f             //Total penalty
			get {
				return gCost + hCost;
			}
		}

		//Implemented from IComparable in IHeapItem
		public int heapIndex {                      //Index in the heap array
			get;
			set;
		}


		/*
		public Node(Vector3 worldPosition, bool isWalkable, int gridX, int gridY, int movementPenalty, Color color, GameObject gameObject) {
			this.isWalkable = isWalkable;
			this.worldPosition = worldPosition;
			this.gridX = gridX;
			this.gridY = gridY;
			this.movementPenalty = movementPenalty;
			this.color = color;
			this.gameObject = gameObject;
			this.nodeType = nodeType;
		}
		*/
		public Node(Vector3 worldPosition, Vector2 gridPosition, bool isWalkable, int movementPenalty, Color color, GameObject gameObject, NodeType nodeType) {
			this.worldPosition = worldPosition;
			this.gridPosition = gridPosition;

			Set(isWalkable, movementPenalty, color, gameObject, nodeType);
		}


		public void Set(Vector3 worldPosition, Vector2 gridPosition, bool isWalkable, int movementPenalty, Color color, GameObject gameObject, NodeType nodeType) {
			this.worldPosition = worldPosition;
			this.gridPosition = gridPosition;
			this.isWalkable = isWalkable;
			this.movementPenalty = movementPenalty;
			this.color = color;
			this.gameObject = gameObject;
			this.nodeType = nodeType;
		}
		public void Set(bool isWalkable, int movementPenalty, Color color, GameObject gameObject, NodeType nodeType) {
			this.isWalkable = isWalkable;
			this.movementPenalty = movementPenalty;
			this.color = color;
			this.gameObject = gameObject;
			this.nodeType = nodeType;

			gridX = (int)gridPosition.x;
			gridY = (int)gridPosition.y;
		}

		public Color GetColor() {
            //return color;

			if(!isWalkable) {
				return Color.red;
			}

			if(nodeType == NodeType.Obsticle) {
				return Color.yellow;
			} else if(nodeType == NodeType.Unit) {
				return Color.blue;
			} else if(nodeType == NodeType.Building) {
				return Color.cyan;
			}

			return Color.green;
		}

		//Compares the f cost of two nodes, /return 1 if nodeToCompare is lower, 0 if they are equal and -1 if nodeToCompare is greater
		//Implemented from IComparable in IHeapable
		public int CompareTo(Node nodeToCompare) {
			int compare = fCost.CompareTo(nodeToCompare.fCost);

			//If the two fCosts are the smae then use the hCost as the tie breaker
			if(compare == 0) {
				compare = hCost.CompareTo(nodeToCompare.hCost);
			}

			//Negative switches the value
			return -compare;
		}

	}

}

public enum NodeType {
	Nothing,
	Obsticle,
	Unit,
	Building
}
