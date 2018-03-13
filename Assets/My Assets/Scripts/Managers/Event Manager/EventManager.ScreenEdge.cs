using UnityEngine;
using System.Collections;

public partial class EventManager {

	//Screen Edge variables
	//private bool atScreenEdge = false;
	//private float atScreenEdgeCounter = 0;

	private int threshold = 75;


	private void CheckScreenEdgeEvents() {

		//ScreenEdgeEventArgs tempEventArgs = null;
		//atScreenEdge = false;

		if(Input.mousePosition.y <= Screen.height-1 &&
			Input.mousePosition.y >= 0+1 &&
			Input.mousePosition.x <= Screen.width-1 &&
			Input.mousePosition.x >= 0+1) {

			//Up
			if(Input.mousePosition.y >= Screen.height - threshold && Input.mousePosition.y <= Screen.height-1) {
				ScreenEdgeEvent(new ScreenEdgeEventArgs(ScreenEdgeEventType.Up));
			}

			//Down
			if(Input.mousePosition.y <= 0 + threshold/3 && Input.mousePosition.y >= 0+1) {
				ScreenEdgeEvent(new ScreenEdgeEventArgs(ScreenEdgeEventType.Down));
			}

			//Right
			if(Input.mousePosition.x >= Screen.width - threshold && Input.mousePosition.x <= Screen.width-1) {
				ScreenEdgeEvent(new ScreenEdgeEventArgs(ScreenEdgeEventType.Right));
			}

			//Left
			if(Input.mousePosition.x <= 0 + threshold && Input.mousePosition.x >= 0+1) {
				ScreenEdgeEvent(new ScreenEdgeEventArgs(ScreenEdgeEventType.Left));
			}

		}
		
		



		/*
		//Up
		if(Input.mousePosition.y == Screen.height-1) {
			if(tempEventArgs == null) {
				//tempEventArgs = new ScreenEdgeEventArgs(0, 1);
			} else {
				//tempEventArgs.y = 1;
			}

			atScreenEdge = true;
		}
		*/

		/*
		if(atScreenEdge) {
			atScreenEdgeCounter += Time.deltaTime;
			//tempEventArgs.duration = atScreenEdgeCounter;
		} else {
			atScreenEdgeCounter = 0;
		}

		if(tempEventArgs != null && ScreenEdgeMousePosition != null) {
			ScreenEdgeMousePosition(this, tempEventArgs);
		}
		*/

	}

}
