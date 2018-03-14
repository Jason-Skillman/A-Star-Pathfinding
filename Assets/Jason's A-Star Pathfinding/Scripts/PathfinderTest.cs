using UnityEngine;
using System;
using System.Collections;
using AStar;

[RequireComponent(typeof(Pathfinder))]
public class PathfinderTest : MonoBehaviour{

	public GameObject target;
	public Vector3 targetPosition;
	
	private Pathfinder pathfinder;


	public void Start() {
		pathfinder = this.GetComponent<Pathfinder>();
		targetPosition = target.transform.position;

		//pathfinder.TravelToPath(targetPosition, CallbackStart, CallbackUpdateTarget, CallbackEnd);
		pathfinder.TravelToPath(target, CallbackStart, CallbackUpdateTarget, CallbackEnd);
	}
	

	public void CallbackStart() {
		//Debug.Log("CallbackStart()");
	}
	public void CallbackUpdateTarget() {
		//Debug.Log("CallbackUpdateTarget()");
	}
	public void CallbackEnd() {
		//Debug.Log("CallbackEnd()");
	}

}