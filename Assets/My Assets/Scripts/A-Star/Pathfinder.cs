using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

	[RequireComponent(typeof(CharacterController))]
	public class Pathfinder : MonoBehaviour {

		private GameObject targetGameObject;
		public Vector3 targetPosition;									//The position to travel to as a Vector3
		public int currentWaypoint = 0;									//The waypoint we are currently moving towards. Resets every time path is updated.

		public float speed = 10;										//Movement speed
		public float nextWaypointDistance = 1.2f;						//Minimum distance you need to be to the current waypoint to go onto the next waypoint

		public float turnSpeed = 20;									//Turning speed
		[Obsolete()]
		public float nextWaypointTurn = 5;								//Minimum distance to start turning towards next waypoint

		public float updatePathInterval = 0.5f;							//Time to wait until pathfinder will recalculate it's position

		public bool isMoving = false;									//Is the object moving?

        
        public Path waypoints;
        public NodePath nodePath;

        public List<Node> allNodes;
        public Vector3[] vectorWaypoint;
		

		
		private Coroutine coroutineUpdatePath;
		private Coroutine coroutineFollowPath;
		public delegate void Callback();
		private Callback CallbackStart, CallbackUpdate, CallbackUpdateTarget, CallbackEnd;



		[Obsolete]
		///<summary>Main method for starting the Pathfinding process.</summary>
		///<param name="targetPosition">Target coords</param>
		///<param name="CallbackStart">Callback will be called when pathfinding starts its path</param>
		///<param name="CallbackStart">Callback will be called every pathfinding update</param>
		///<param name="CallbackStart">Callback will be called when the pathfinding recalculates targets coords based on the updatePathInterval</param>
		///<param name="CallbackStart">Callback will be called when pathfinding finishes its path</param>
		public void TravelToPath(Vector3 targetPosition, Callback CallbackStart, Callback CallbackUpdate, Callback CallbackUpdateTarget, Callback CallbackEnd) {
			this.CallbackStart = CallbackStart;
			this.CallbackUpdate = CallbackUpdate;
			this.CallbackUpdateTarget = CallbackUpdateTarget;
			this.CallbackEnd = CallbackEnd;

			TravelToPath(targetPosition);	//Reuse the already existing method below
		}
		public void TravelToPath(GameObject targetGameObject, Callback CallbackStart, Callback CallbackUpdate, Callback CallbackUpdateTarget, Callback CallbackEnd) {
			this.CallbackStart = CallbackStart;
			this.CallbackUpdate = CallbackUpdate;
			this.CallbackUpdateTarget = CallbackUpdateTarget;
			this.CallbackEnd = CallbackEnd;

			TravelToPath(targetGameObject);   //Reuse the already existing method below
		}
		[Obsolete]
		///<summary>Main method for starting the Pathfinding process.</summary>
		///<param name="targetPosition">Target coords</param>
		public void TravelToPath(Vector3 targetPosition) {
			this.targetPosition = targetPosition;
			isMoving = true;

			//Checks if the location is null
			if(targetPosition != Vector3.zero && targetPosition != null) {
				//If UpdatePath() is already running, stop it
				if(coroutineUpdatePath != null) {
					StopCoroutine(coroutineUpdatePath);
				}
				coroutineUpdatePath = StartCoroutine("UpdatePath");
			} else {
				//Dont start the pathfinding process
				Debug.LogError("Cant travel to null location");
			}
		}
		public void TravelToPath(GameObject targetGameObject) {
			this.targetGameObject = targetGameObject;
			isMoving = true;

			//Checks if the location is null
			if(targetGameObject != null) {
				//If UpdatePath() is already running, stop it
				if(coroutineUpdatePath != null) {
					StopCoroutine(coroutineUpdatePath);
				}
				coroutineUpdatePath = StartCoroutine(UpdatePath(targetGameObject));
			}
			else {
				//Dont start the pathfinding process
				Debug.LogError("Cant travel to null location");
			}
		}

		///<summary>Coroutine for updating the pathfinding path.</summary>
		[Obsolete]
		private IEnumerator UpdatePath() {
			//Fixes/Solves large delta time errors
			if(Time.timeSinceLevelLoad < 0.3f) {
				yield return new WaitForSeconds(0.3f);
			}

			CallbackStart(); //Call the callback delagate
			
			Vector3 targetPositionOld = targetPosition;

			PathfinderManager.main.RequestPath(this.transform.position, targetPosition, OnPathCalculation, PathType.AllPoints);

			do { //Start of inf. loop
				yield return null;
				
				PathfinderManager.main.RequestPath(this.transform.position, targetPosition, OnPathCalculation, PathType.AllPoints);
				
				CallbackUpdateTarget(); //Call the callback delagate

				if(updatePathInterval > 0) {
					yield return new WaitForSeconds(updatePathInterval);
				} else {
					yield return new WaitForSeconds(Mathf.Infinity);
				}
			} while(true);
		}
		private IEnumerator UpdatePath(GameObject targetGameObject) {
			//Fixes/Solves large delta time errors
			if(Time.timeSinceLevelLoad < 0.3f) {
				yield return new WaitForSeconds(0.3f);
			}

			CallbackStart(); //Call the callback delagate
			
			PathfinderManager.main.RequestPath(this.transform.position, targetGameObject.transform.position, OnPathCalculation, PathType.AllPoints);

			do { //Start of inf. loop
				yield return null;

				Debug.Log(targetGameObject.transform.position);
				PathfinderManager.main.RequestPath(this.transform.position, targetGameObject.transform.position, OnPathCalculation, PathType.AllPoints);

				CallbackUpdateTarget(); //Call the callback delagate

				if(updatePathInterval > 0) {
					yield return new WaitForSeconds(updatePathInterval);
				}
				else {
					yield return new WaitForSeconds(Mathf.Infinity);
				}
			} while(true);
		}

		//Called when the path has been caculated
		private void OnPathCalculation(List<Node> nodeWaypoints, Vector3[] waypoints, bool pathSuccessful) {
			if(pathSuccessful) {

                foreach(Node node in nodeWaypoints) {
                    node.color = Color.blue;
                }


                allNodes = nodeWaypoints;
                this.waypoints = new Path(waypoints, this.transform.position, nextWaypointTurn);
                vectorWaypoint = waypoints;



                if(coroutineFollowPath != null)
					StopCoroutine(coroutineFollowPath);
				coroutineFollowPath = StartCoroutine("FollowPath");
			} else {
				Debug.LogError("Path failed");
				StopMoving();
			}
		}

        private void OnPathCalculation(List<Node> allNodes, bool pathSuccessful) {
            if(pathSuccessful) {
                nodePath = new NodePath(allNodes);
            }
        }
        
		//Moves the object along the path
		private IEnumerator FollowPath() {
			Vector3 direction = new Vector3();
			Vector2 position2D;

			while(true) {
				yield return null;

				position2D = new Vector2(this.transform.position.x, this.transform.position.z);

				CallbackUpdate();   //Call the callback delagate
				
				if(currentWaypoint < waypoints.waypoints.Length) {	//If their is a waypoint to move towards
					//Direction and speed to the next waypoint
					direction = (waypoints.waypoints[currentWaypoint] - this.transform.position).normalized;
					direction *= speed;

					//Rotate towards the next path
					RotateTowards(direction);

					//Move the object
					//this.transform.Translate(direction * Time.deltaTime, Space.World);
					this.GetComponent<CharacterController>().SimpleMove(direction);
					//this.GetComponent<Rigidbody>().MovePosition(direction);

					//If we are near the next waypoint. Based on "nextWaypointDistance" variable
					if((transform.position - waypoints.waypoints[currentWaypoint]).sqrMagnitude < nextWaypointDistance * nextWaypointDistance) {
						currentWaypoint++;
					}
				} else {    //If we are at the end of the waypoint list
					//End of the path
					currentWaypoint = 1;

					OnReachedEndOfPath();
					CallbackEnd();   //Call the callback delagate
				}
			}
		}

		private void OnReachedEndOfPath() {
			StopMoving();
		}


		//##### MAIN METHOD TO CALL TO STOP PATHFINDING #####
		public void StopMoving() {
			isMoving = false;
			if(coroutineUpdatePath != null)
				StopCoroutine(coroutineUpdatePath);
			if(coroutineFollowPath != null)
				StopCoroutine(coroutineFollowPath);
		}
		
		public void RotateTowards(Vector3 dir) {
			if(dir != Vector3.zero) {
				Quaternion rot = transform.rotation;
				Quaternion toTarget = Quaternion.LookRotation(dir);

				rot = Quaternion.Slerp(rot, toTarget, turnSpeed*Time.deltaTime);
				Vector3 euler = rot.eulerAngles;
				euler.z = 0;
				euler.x = 0;
				rot = Quaternion.Euler(euler);

				transform.rotation = rot;
			}
		}
		

		public void OnDrawGizmos() {
			//Draws the path's gizmos
			if(isMoving) {
				//waypoints.DrawGizmos();

                //Draw Sphere
                Gizmos.color = Color.green;
                foreach(Vector3 point in vectorWaypoint) {
                    Gizmos.DrawSphere(point + Vector3.up, 0.5f);
                }

                //Look ahead
                Gizmos.color = Color.blue;
                for(int i = 0; i < vectorWaypoint.Length; i++) {
                    if(i > currentWaypoint && i <= currentWaypoint+3) {
                        Gizmos.DrawSphere(vectorWaypoint[i] + Vector3.up*2, 0.5f);
                    }
                }

                //Draw Line
                Gizmos.color = Color.green;
                for(int i = 0; i < vectorWaypoint.Length - 1; i++) {
                    Vector3 pointStart = vectorWaypoint[i];
                    Vector3 pointEnd = vectorWaypoint[i+1];
                    Gizmos.DrawLine(pointStart + Vector3.up, pointEnd + Vector3.up);
                }
            }
		}
		
	}

}