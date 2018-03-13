using UnityEngine;
using System;
using System.Collections;
using AStar;

[RequireComponent(typeof(Pathfinder))]
public class PathfinderTest : MonoBehaviour{

	public GameObject target;
	public Vector3 coordsToTravelTo;
	
	private Pathfinder pathfinder;
	//private Pathfinder.Callback CallbackStart, CallbackUpdate, CallbackUpdateTarget, CallbackEnd;

	public void Start() {
		pathfinder = this.GetComponent<Pathfinder>();
		coordsToTravelTo = target.transform.position;

		//pathfinder.TravelToPath(coordsToTravelTo, CallbackStart, CallbackUpdate, CallbackUpdateTarget, CallbackEnd);
		pathfinder.TravelToPath(target, CallbackStart, CallbackUpdate, CallbackUpdateTarget, CallbackEnd);
	}
	

	public void CallbackStart() {
		Debug.Log("CallbackStart()");
	}
	public void CallbackUpdate() {
		Debug.Log("CallbackUpdate()");
	}
	public void CallbackUpdateTarget() {
		Debug.Log("CallbackUpdateTarget()");
	}
	public void CallbackEnd() {
		Debug.Log("CallbackEnd()");
	}

}