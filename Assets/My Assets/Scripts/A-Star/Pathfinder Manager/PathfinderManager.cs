using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

    public partial class PathfinderManager : MonoBehaviour {

        //Singleton
        public static PathfinderManager main = new PathfinderManager();

        private Queue<PathResult> results = new Queue<PathResult>();


        public void Awake() {
			if(main == null) {
				main = this;
			} else {
				Destroy(this);
			}
        }

        public void Update() {
            if(results.Count > 0) {
                int itemsInQueue = results.Count;
                lock(results) {
                    for(int i = 0; i < itemsInQueue; i++) {
                        PathResult result = results.Dequeue();
                        result.callback(result.nodeWaypoints, result.path, result.success);
                    }
                }
            }
		}
		
		///<summary>Main path request method for pathfinding.</summary>
		///<param name="pathStart">Starting coords</param>
		///<param name="pathEnd">Ending coords</param>
		///<param name="callback">Callback delegate</param>
		///<param name="pathType">Type of path</param>
		//<returns>Returns an integer based on the passed value.</returns>
		public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<List<Node>, Vector3[], bool> callback, PathType pathType) {
            ThreadStart threadStart = delegate {
                FindPath(pathStart, pathEnd, callback, pathType);   //Method on other page
            };
            threadStart.Invoke();
        }
		
		///<summary>Method called when the pathfinding process has finished.</summary>
		public void OnFinishedProcessingPath(PathResult result) {
            lock(results) {
                results.Enqueue(result);
            }
        }
        
    }
	
    public struct PathResult {
        public List<Node> nodeWaypoints;
        public Vector3[] path;
        public bool success;
        public Action<List<Node>, Vector3[], bool> callback;

        public PathResult(List<Node> nodeWaypoints, Vector3[] path, bool success, Action<List<Node>, Vector3[], bool> callback) {
            this.nodeWaypoints = nodeWaypoints;
            this.path = path;
            this.success = success;
            this.callback = callback;
        }
    }

    public enum PathType {
        PointToPoint,       //Simplifyed path from start to end
        AllPoints,          //All nodes from start to finish
        EndOfTheLine        //Cuts the unwalkable nodes off at the end of the path
    }

}
