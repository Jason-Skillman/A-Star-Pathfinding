using System;

public class ScreenEdgeEventArgs : EventArgs {

	public ScreenEdgeEventType screenEdgeEventType {
		get;
		private set;
	}


	public ScreenEdgeEventArgs(ScreenEdgeEventType screenEdgeEventType) {
		this.screenEdgeEventType = screenEdgeEventType;
	}
	
}

public enum ScreenEdgeEventType {
	Up,
	Down,
	Right,
	Left
}
