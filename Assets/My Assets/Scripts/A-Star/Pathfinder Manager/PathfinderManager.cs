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
        //private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        //private PathRequest currentPathRequest;
        //private bool isProcessingPath;


        public void Awake() {
            if(main == null) main = this;
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

        //##### Main path request method #####
        public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<List<Node>, Vector3[], bool> callback, PathType pathType) {
            ThreadStart threadStart = delegate {
                FindPath(pathStart, pathEnd, callback, pathType);   //Method on other page
            };
            threadStart.Invoke();
        }
        [Obsolete("Use RequestPath() instead.")]
        public void RequestPathOld(PathRequest request) {
            ThreadStart threadStart = delegate {
                //FindPathOld(request);   //Method on other page
            };
            threadStart.Invoke();
        }

        //Called on the other page when the process finished
        public void OnFinishedProcessingPath(PathResult result) {
            lock(results) {
                results.Enqueue(result);
            }
        }
        
    }

    [Obsolete("Dont use anymore")]
    public struct PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.callback = callback;
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
