using UnityEngine;
using System;
using System.Collections;
using AStar;

[RequireComponent(typeof(Pathfinder))]
public class PathfinderTest : MonoBehaviour{

	Pathfinder pathfinder;
	public GameObject target;
	
	private Pathfinder.Callback CallbackStart, CallbackUpdate, CallbackUpdateTarget, CallbackEnd;

	public void Start() {
		pathfinder = this.GetComponent<Pathfinder>();

		pathfinder.TravelToPath(target.transform.position, Test, Test, Test, Test);
	}

	public void Update() {
		
	}

	public void Test() { }

}