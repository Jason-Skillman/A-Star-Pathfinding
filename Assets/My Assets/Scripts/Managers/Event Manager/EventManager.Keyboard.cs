using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class EventManager {

	//Key_1 key_1 = new Key_1(EventType.Down);
	
	private void CheckKeyBoardEvents() {

		/*
		if(Input.GetKeyDown(KeyCode.Keypad0)) {
			if(KeyAction != null) {
				KeyAction(this, new Key_0());
			}
		}
		*/
		

		foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
			//if(kcode != KeyCode.Mouse0 && kcode != KeyCode.Mouse1 && kcode != KeyCode.Mouse2) {
			//	Debug.Log("Not REAL");
			//}

			if(Input.GetKey(kcode)) {
				if(Input.GetKeyDown(kcode)) {
					KeyBoardEvent(new KeyBoardEventArgs(kcode, KeyEventType.Down));
				} else {
					KeyBoardEvent(new KeyBoardEventArgs(kcode, KeyEventType.Holding));
				}
			} else if(Input.GetKeyUp(kcode)) {
				KeyBoardEvent(new KeyBoardEventArgs(kcode, KeyEventType.Up));
			}
		}


		/*
		//Keypad 0
		if(Input.GetKey(KeyCode.Keypad0)) {
			if(Input.GetKeyDown(KeyCode.Keypad0)) {
				KeyBoardEvent(new KeyBoardEventArgs(KeyCode.Keypad0, KeyType.Down));
			} else {
				KeyBoardEvent(this, new KeyBoardEventArgs(KeyCode.Keypad0, KeyType.Holding));
			}
		} else if(Input.GetKeyUp(KeyCode.Keypad0)) {
			KeyBoardEvent(this, new KeyBoardEventArgs(KeyCode.Keypad0, KeyType.Up));
		}


		//Keypad 1
		if(Input.GetKey(KeyCode.Keypad1)) {
			if(Input.GetKeyDown(KeyCode.Keypad1)) {
				KeyBoardEvent(this, new Key_1(KeyCode.Keypad1, KeyType.Down));
			}
			else {
				KeyBoardEvent(this, new Key_1(KeyCode.Keypad1, KeyType.Holding));
			}
		}
		else if(Input.GetKeyUp(KeyCode.Keypad1)) {
			KeyBoardEvent(this, new Key_1(KeyCode.Keypad1, KeyType.Up));
		}


		//A
		if(Input.GetKey(KeyCode.A)) {
			if(Input.GetKeyDown(KeyCode.A)) {
				KeyBoardEvent(this, new Key_A(KeyCode.Keypad1, KeyType.Down));
			} else {
				KeyBoardEvent(this, new Key_A(KeyType.Holding));
			}
		} else if(Input.GetKeyUp(KeyCode.A)) {
			KeyBoardEvent(this, new Key_A(KeyType.Up));
		}


		//Escape
		if(Input.GetKey(KeyCode.Escape)) {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				KeyBoardEvent(this, new Key_Escape(KeyType.Down));
			} else {
				KeyBoardEvent(this, new Key_Escape(KeyType.Holding));
			}
		} else if(Input.GetKeyUp(KeyCode.Escape)) {
			KeyBoardEvent(this, new Key_Escape(KeyType.Up));
		}


		//LeftShift
		if(Input.GetKey(KeyCode.LeftShift)) {
			if(Input.GetKeyDown(KeyCode.LeftShift)) {
				KeyBoardEvent(this, new Key_LeftShift(KeyType.Down));
			} else {
				KeyBoardEvent(this, new Key_LeftShift(KeyType.Holding));
			}
		} else if(Input.GetKeyUp(KeyCode.LeftShift)) {
			KeyBoardEvent(this, new Key_LeftShift(KeyType.Up));
		}


		//Left Control
		if(Input.GetKey(KeyCode.LeftControl)) {
			if(Input.GetKeyDown(KeyCode.LeftControl)) {
				KeyBoardEvent(this, new Key_LeftControl(KeyType.Down));
			} else {
				KeyBoardEvent(this, new Key_LeftControl(KeyType.Holding));
			}
		} else if(Input.GetKeyUp(KeyCode.LeftControl)) {
			KeyBoardEvent(this, new Key_LeftControl(KeyType.Up));
		}
		*/

	}
}
