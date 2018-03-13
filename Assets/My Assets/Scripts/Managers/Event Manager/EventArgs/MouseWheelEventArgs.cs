using System;

public class MouseWheelEventArgs : EventArgs {

	public ScrollEventType scrollEventType {
		get;
		private set;
	}
	public float scrollAmount {
		get;
		private set;
	}


	public MouseWheelEventArgs(ScrollEventType scrollEventType, float scrollAmount) {
		this.scrollEventType = scrollEventType;
		this.scrollAmount = scrollAmount;
	}
	
}

public enum ScrollEventType {
	Up,
	Down
}
