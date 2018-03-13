using UnityEngine;
using System.Collections;
using System;

public partial class EventManager : MonoBehaviour {

	//Singleton
	public static EventManager main;

	//Delegates
	public delegate void KeyBoardActions(KeyBoardEventArgs e);
	public delegate void MouseClickActions(MouseClickEventArgs e);
	public delegate void MouseWheelActions(MouseWheelEventArgs e);
	public delegate void ScreenEdgeActions(ScreenEdgeEventArgs e);

	//Events
	public event KeyBoardActions KeyBoardEvent;
	public event MouseClickActions MouseClickEvent;
	public event MouseWheelActions MouseWheelEvent;
	public event ScreenEdgeActions ScreenEdgeEvent;
	

	public void Awake() {
        if(main == null) main = this;
	}

	public void Start() {
		//MouseClick += DoubleClickCheck;
	}

	public void Update() {
		CheckMouseClickEvents();
		CheckKeyBoardEvents();
		CheckMouseScrollEvents();
		CheckScreenEdgeEvents();
	}
}
