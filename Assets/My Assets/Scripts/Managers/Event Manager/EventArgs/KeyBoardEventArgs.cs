using UnityEngine;
using System.Collections;
using System;

public class KeyBoardEventArgs : EventArgs {

	public KeyCode keyCode {
		get;
		private set;
	}
	public KeyEventType keyEventType {
		get;
		private set;
	}


	public KeyBoardEventArgs(KeyCode keyCode, KeyEventType keyEventType) {
		this.keyCode = keyCode;
		this.keyEventType = keyEventType;
	}
	
}

public enum KeyEventType {
	Down,
	Holding,
	Up
}
