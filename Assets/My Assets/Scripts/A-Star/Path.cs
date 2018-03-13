using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

	[Serializable]
	public class Path {

		public Vector3[] waypoints;                     //All waypoints as vector3
		public int finishLineIndex;
		public Vector3 startPosition;


		public Path(Vector3[] waypoints, Vector3 startPos, float turnDistance) {
			this.waypoints = waypoints; //Debug.Log(this.waypoints.Length + " " + waypoints.Length);
										//turnBoundaries = new Line[this.waypoints.Length];
										//finishLineIndex = turnBoundaries.Length - 1;
			startPosition = startPos;

			Vector2 previousPoint = Vector3ToVector2(startPos);
			for(int i = 0; i < this.waypoints.Length; i++) {
				Vector2 currentPoint = Vector3ToVector2(this.waypoints[i]);
				Vector2 directionToCurrentPoint = (currentPoint - previousPoint).normalized;

				//Vector2 turnBoundaryPoint = (i == finishedLineIndex) ? currentPoint : currentPoint - directionToCurrentPoint * turnDistance;
				Vector2 turnBoundaryPoint;
				if(i == finishLineIndex) {
					turnBoundaryPoint = currentPoint;
				} else {
					turnBoundaryPoint = currentPoint - directionToCurrentPoint * turnDistance;
				}

				//turnBoundaries[i]  = new Line(turnBoundaryPoint, previousPoint - directionToCurrentPoint * turnDistance);
				previousPoint = turnBoundaryPoint;
			}
		}

		Vector2 Vector3ToVector2(Vector3 point) {
			return new Vector2(point.x, point.z);
		}

		public void DrawGizmos() {
            /*
			//Draw Sphere
			Gizmos.color = Color.green;
			foreach(Vector3 point in waypoints) {
				Gizmos.DrawSphere(point + Vector3.up, 0.5f);
			}
            
            //Draw Line
            for(int i = 0; i < waypoints.Length - 1; i++) {
				Vector3 pointStart = waypoints[i];
				Vector3 pointEnd = waypoints[i+1];
				Gizmos.DrawLine(pointStart + Vector3.up, pointEnd + Vector3.up);
			}

			//Debug.Log(waypoints.Length);
            */
		}

	}

}