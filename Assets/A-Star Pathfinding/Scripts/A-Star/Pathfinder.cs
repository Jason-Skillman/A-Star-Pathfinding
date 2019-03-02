using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

	[RequireComponent(typeof(CharacterController))]
	public class Pathfinder : MonoBehaviour {

		public GameObject targetGameObject;						//The position to travel to as a GameObject
		public Vector3 targetPosition;							//The position to travel to as a Vector3
		
		public float speed = 10;								//Movement speed
		public float nextWaypointDistance = 1.0f;				//Minimum distance you need to be to the current waypoint to go onto the next waypoint

		public float turnSpeed = 20;							//Turning speed
		[Obsolete]
		public float nextWaypointTurn = 5;                      //Minimum distance to start turning towards next waypoint

		public bool updatePath = true;							
		public float pathUpdateInterval = 0.5f;				    //Time to wait until pathfinder will recalculate it's position
		

		//Read only
		public int currentWaypoint = 0;						    //The waypoint we are currently moving towards. Resets every time path is updated.
		public bool isMoving = false;							//Is the object moving?

        
        public Path waypoints;
        public NodePath nodePath;
        public List<Node> allNodes;
        public Vector3[] vectorWaypoint;
		
		private Coroutine coroutineUpdatePath;
		private Coroutine coroutineFollowPath;
		public delegate void Callback();
		private Callback CallbackStart, CallbackUpdateTarget, CallbackEnd;



		///<summary>Main method for starting the Pathfinding process. Uses a Vector3 as coords. Does not support moving targets.</summary>
		///<param name="targetPosition">Target coords</param>
		///<param name="CallbackStart">Callback will be called when pathfinding starts its path</param>
		///<param name="CallbackStart">Callback will be called every pathfinding update</param>
		///<param name="CallbackStart">Callback will be called when the pathfinding recalculates targets coords based on the updatePathInterval</param>
		///<param name="CallbackStart">Callback will be called when pathfinding finishes its path</param>
		public void TravelToPath(Vector3 targetPosition, Callback CallbackStart,Callback CallbackUpdateTarget, Callback CallbackEnd) {
			this.CallbackStart = CallbackStart;
			this.CallbackUpdateTarget = CallbackUpdateTarget;
			this.CallbackEnd = CallbackEnd;

			TravelToPath(targetPosition);	//Reuse the already existing method below
		}
		///<summary>Main method for starting the Pathfinding process. Uses a GameObject as coords. Does support moving targets.</summary>
		///<param name="targetPosition">Target coords</param>
		///<param name="CallbackStart">Callback will be called when pathfinding starts its path</param>
		///<param name="CallbackStart">Callback will be called every pathfinding update</param>
		///<param name="CallbackStart">Callback will be called when the pathfinding recalculates targets coords based on the updatePathInterval</param>
		///<param name="CallbackStart">Callback will be called when pathfinding finishes its path</param>
		public void TravelToPath(GameObject targetGameObject, Callback CallbackStart, Callback CallbackUpdateTarget, Callback CallbackEnd) {
			this.CallbackStart = CallbackStart;
			this.CallbackUpdateTarget = CallbackUpdateTarget;
			this.CallbackEnd = CallbackEnd;

			TravelToPath(targetGameObject);   //Reuse the already existing method below
		}
		///<summary>Main method for starting the Pathfinding process. Uses a Vector3 as coords. Does not support moving targets.</summary>
		///<param name="targetPosition">Target coords as Vector3</param>
		public void TravelToPath(Vector3 targetPosition) {
			this.targetPosition = targetPosition;
			isMoving = true;

			//Checks if the location is null
			if(targetPosition != null && targetPosition != Vector3.zero) {
				//If UpdatePath() is already running, stop it
				if(coroutineUpdatePath != null) {
					StopCoroutine(coroutineUpdatePath);
				}
				coroutineUpdatePath = StartCoroutine(UpdatePath());
			} else {
				//Dont start the pathfinding process
				Debug.LogError("Cant travel to null location");
			}
		}
		///<summary>Main method for starting the Pathfinding process. Uses a GameObject as coords. Does support moving targets.</summary>
		///<param name="targetGameObject">Target coords as GameObject</param>
		public void TravelToPath(GameObject targetGameObject) {
			this.targetGameObject = targetGameObject;
			isMoving = true;

			//Checks if the location is null
			if(targetGameObject != null) {
				//If UpdatePath() is already running, stop it
				if(coroutineUpdatePath != null) {
					StopCoroutine(coroutineUpdatePath);
				}
				//Start updating the path
				coroutineUpdatePath = StartCoroutine(UpdatePath(targetGameObject));
			}
			else {
				//Dont start the pathfinding process
				Debug.LogError("Cant travel to null location");
			}
		}


		///<summary>Main method to call to stop the pathfinding proccess.</summary>
		public void StopMoving() {
			isMoving = false;
			if(coroutineUpdatePath != null)
				StopCoroutine(coroutineUpdatePath);
			if(coroutineFollowPath != null)
				StopCoroutine(coroutineFollowPath);
		}


		///<summary>Coroutine for updating the pathfinding path. Does not support moving targets.</summary>
		private IEnumerator UpdatePath() {
			//Fixes/Solves large delta time errors
			if(Time.timeSinceLevelLoad < 0.3f) {
				yield return new WaitForSeconds(0.3f);
			}

			CallbackStart(); //Call the callback delagate
			
			do { //Start of inf. loop
				yield return null;

				currentWaypoint = 1;

				PathfinderManager.main.RequestPath(this.transform.position, targetPosition, OnPathCalculation, PathType.AllPoints);
				
				CallbackUpdateTarget(); //Call the callback delagate

				if(updatePath) {
					yield return new WaitForSeconds(pathUpdateInterval);
				} else {
					yield return new WaitForSeconds(Mathf.Infinity);
				}
			} while(true);
		}
		///<summary>Coroutine for updating the pathfinding path. Does support moving targets.</summary>
		///<param name="targetGameObject">Target coords as GameObject</param>
		private IEnumerator UpdatePath(GameObject targetGameObject) {
			//Fixes/Solves large delta time errors
			if(Time.timeSinceLevelLoad < 0.3f) {
				yield return new WaitForSeconds(0.3f);
			}

			CallbackStart(); //Call the callback delagate
			
			do { //Start of inf. loop
				yield return null;

				currentWaypoint = 1;

				PathfinderManager.main.RequestPath(this.transform.position, targetGameObject.transform.position, OnPathCalculation, PathType.AllPoints);

				CallbackUpdateTarget(); //Call the callback delagate
				
				if(updatePath) {
					yield return new WaitForSeconds(pathUpdateInterval);
				} else {
					yield return new WaitForSeconds(Mathf.Infinity);
				}
			} while(true);
		}


		///<summary>Called when the pathfinding process has finished its calculation.</summary>
		///<param name="nodeWaypoints">All of the nodes as a List in the path</param>
		///<param name="waypoints">All of the coords in the path</param>
		///<param name="pathSuccessful">Was the pathfinding successful?</param>
		private void OnPathCalculation(List<Node> nodeWaypoints, Vector3[] waypoints, bool pathSuccessful) {
			//Was the path a success?
			if(pathSuccessful) {
				//Path was a success, start moving
				
                allNodes = nodeWaypoints;
                vectorWaypoint = waypoints;

				this.waypoints = new Path(waypoints, this.transform.position, nextWaypointTurn);

				//If FollowPath() is already running, stop it
				if(coroutineFollowPath != null) {
					StopCoroutine(coroutineFollowPath);
				}
				//Start following the path
				coroutineFollowPath = StartCoroutine(FollowPath());
			} else {
				//Path failed. No possible path to target
				Debug.Log("Path failed. No possible path to target");
				StopMoving();
			}
		}

		///<summary>Called when this object has reached the end of it's destanation.</summary>
		private void OnReachedEndOfPath() {
			//Debug.Log(this.name + " has reached it's destination.");
			StopMoving();
		}


		///<summary>Coroutine for moving this pathfinder along the path.</summary>
		private IEnumerator FollowPath() {
			Vector3 direction = new Vector3();
			Vector2 position2D;

			while(true) {
				yield return null;

				position2D = new Vector2(this.transform.position.x, this.transform.position.z);
				
				if(currentWaypoint < waypoints.waypoints.Length) {	//If their is a waypoint to move towards
					//Direction and speed to the next waypoint
					direction = (waypoints.waypoints[currentWaypoint] - this.transform.position).normalized;
					direction *= speed;

					//Rotate towards the next path
					RotateTowards(direction);

					//Move the object
					//this.transform.Translate(direction * Time.deltaTime, Space.World);	//Trandsform movement
					this.GetComponent<CharacterController>().SimpleMove(direction);         //CharacterController Movement
					
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

		

		///<summary>Coroutine for moving this pathfinder along the path.</summary>
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

				/*
                //Look ahead (future points)
                Gizmos.color = Color.blue;
                for(int i = 0; i < vectorWaypoint.Length; i++) {
                    if(i > currentWaypoint && i <= currentWaypoint+3) {
                        Gizmos.DrawSphere(vectorWaypoint[i] + Vector3.up * 2, 0.5f);
                    }
                }
				*/

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