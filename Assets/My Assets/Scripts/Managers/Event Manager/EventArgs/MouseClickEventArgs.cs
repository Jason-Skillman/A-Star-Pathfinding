using System;
using UnityEngine;

public class MouseClickEventArgs : EventArgs {
	
	public ButtonType buttonType {
		get;
		private set;
	}
	public ButtonEventType buttonEventType {
		get;
		private set;
	}
	public Vector2 screenPosition {
		get;
		private set;
	}
	public Vector3 worldPosition {
		get;
		private set;
	}

	
	public MouseClickEventArgs(ButtonType buttonType, ButtonEventType buttonEventType, Vector2 screenPosition, Vector3 worldPosition) {
		this.buttonType = buttonType;
		this.buttonEventType = buttonEventType;
		this.screenPosition = screenPosition;
		this.worldPosition = worldPosition;
	}

}

public enum ButtonEventType {
	Down,
	Holding,
	Up
}

public enum ButtonType {
	LeftButton,
	MiddleButton,
	RightButton
}
