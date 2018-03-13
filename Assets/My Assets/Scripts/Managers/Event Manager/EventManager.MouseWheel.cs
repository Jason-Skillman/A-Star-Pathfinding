using UnityEngine;
using System.Collections;
using System;

public partial class EventManager {

	private void CheckMouseScrollEvents() {

		if(Input.GetAxis("Mouse ScrollWheel") != 0) {
			if(Input.GetAxis("Mouse ScrollWheel") > 0) {
				MouseWheelEvent(new MouseWheelEventArgs(ScrollEventType.Up, Input.GetAxis("Mouse ScrollWheel")));
			}
			if(Input.GetAxis("Mouse ScrollWheel") < 0) {
				MouseWheelEvent(new MouseWheelEventArgs(ScrollEventType.Down, Input.GetAxis("Mouse ScrollWheel")));
			}
		}

	}
	
}