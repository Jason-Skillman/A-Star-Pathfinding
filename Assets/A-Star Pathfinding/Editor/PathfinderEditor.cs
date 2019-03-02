using UnityEditor;
using UnityEngine;
using System.Collections;
using AStar;

[CustomEditor(typeof(Pathfinder), true)]
public class PathfinderEditor : Editor {

	public override void OnInspectorGUI() {
		Pathfinder myTarget = (Pathfinder)target;

		//base.OnInspectorGUI();

		GUILayout.Space(5);
		EditorGUILayout.LabelField("Pathfinder", EditorStyles.boldLabel);
		GUILayout.Space(5);


		myTarget.targetGameObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", myTarget.targetGameObject, typeof(GameObject), true);
		myTarget.targetPosition = EditorGUILayout.Vector3Field(new GUIContent("Target Position", "The position to travel to as a Vector3"), myTarget.targetPosition);
		GUILayout.Space(10);

		myTarget.speed = EditorGUILayout.FloatField(new GUIContent("Movement Speed", "Movement speed"), myTarget.speed);
		myTarget.nextWaypointDistance = EditorGUILayout.FloatField(new GUIContent("Next Waypoint Distance", "Minimum distance you need to be to the current waypoint to go onto the next waypoint"), myTarget.nextWaypointDistance);
		GUILayout.Space(10);

		myTarget.turnSpeed = EditorGUILayout.FloatField(new GUIContent("Turning Speed", "Turning speed"), myTarget.turnSpeed);
		GUI.enabled = false;
		myTarget.nextWaypointTurn = EditorGUILayout.FloatField(new GUIContent("Next Turn Distance", "[Depresiated] Minimum distance to start turning towards next waypoint"), myTarget.nextWaypointTurn);
		GUI.enabled = true;
		GUILayout.Space(10);

		myTarget.updatePath = EditorGUILayout.Toggle(new GUIContent("Update Path", "Do you want the path to be updated?"), myTarget.updatePath);
		if(!myTarget.updatePath) {
			GUI.enabled = false;
		}
		myTarget.pathUpdateInterval = EditorGUILayout.Slider(new GUIContent("Path Update Interval", "Time to wait until pathfinder will recalculate it's position"), myTarget.pathUpdateInterval, 0.1f, 60.0f);

		GUI.enabled = true;
		GUILayout.Space(10);



		EditorGUILayout.LabelField(new GUIContent("Current Waypoint: " + myTarget.currentWaypoint, "The waypoint we are currently moving towards. Resets every time path is updated."));
		EditorGUILayout.LabelField(new GUIContent("Is Moving: " + myTarget.isMoving, "Is the object moving?"));
		
		GUILayout.Space(20);

		if(GUI.changed) {
			EditorUtility.SetDirty(myTarget);
		}
	}

}
