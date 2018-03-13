using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AStar;

public class GameManager : MonoBehaviour {

	//Singleton
	public static GameManager main;

	public Camera myCamera;
	public GameMode gameMode;
	public InteractionMode interactionMode;

	

	//Width of GUI menu
	private float m_GuiWidth;
	
	//Action Variables
	//private HoverOver hoverOver;lse;
	//private GameObject currentObject;
	
	//Mode Variables
	//private Mode m_Mode = Mode.Normal;
	
	
	//Building Placement variables
	private Action m_CallBackFunction;
	//private Item m_ItemBeingPlaced;
	private GameObject m_ObjectBeingPlaced;
	//private bool m_PositionValid = true;
	//private bool m_Placed = false;

	public bool readyToPlaceBuilding = false;
	public bool canPlaceBuilding = false;
	public GameObject buildingReadyToPlace;
	private Coroutine coroutineBuilding;

	bool IsShiftDown {
		set;
		get;
	}

	bool IsControlDown {
		set;
		get;
	}



	public void Awake() {
        if(main == null) main = this;
	}
	
	public void Start () {
		gameMode = GameMode.Normal;
		interactionMode = InteractionMode.Nothing;

		//Add Event Handlers
		EventManager.main.KeyBoardEvent += ShiftPressed;
        EventManager.main.KeyBoardEvent += ControlPressed;
        EventManager.main.KeyBoardEvent += KeyBoardPressedHandler;

		EventManager.main.MouseClickEvent += MouseClickedHandler;

		EventManager.main.MouseWheelEvent += MouseScrollHandler;

		EventManager.main.ScreenEdgeEvent += MouseAtScreenEdgeHandler;
	}

	public void Update() {

		HoverOver hoverOver = HoverManager.main.getHoverOver();
		if(gameMode != GameMode.PlaceBuilding) {
			if(hoverOver == HoverOver.UI) {
				gameMode = GameMode.Menu;
			} else {
				gameMode = GameMode.Normal;
			}
		}

		switch(gameMode) {
			case GameMode.Normal:
				GameMode_NormalUpdate();
				break;
			case GameMode.Menu:
				GameMode_MenuUpdate();
				break;
			case GameMode.PlaceBuilding:
				GameMode_PlaceBuilding();
				break;
		}
        
	}

	//------------------------------Game Mode Updates------------------------------
	private void GameMode_NormalUpdate() {
        
	}

	private void GameMode_MenuUpdate() {

	}

	private void GameMode_PlaceBuilding() {
		
	}

    

	//------------------------------KeyBoard Handler------------------------------
	private void ShiftPressed(KeyBoardEventArgs e) {
		if(e.keyCode == KeyCode.LeftShift && e.keyEventType == KeyEventType.Down) {
			IsShiftDown = true;
		} else if(e.keyCode == KeyCode.LeftShift && e.keyEventType == KeyEventType.Up) {
			IsShiftDown = false;
		}
	}

    private void ControlPressed(KeyBoardEventArgs e) {
		if(e.keyCode == KeyCode.LeftControl && e.keyEventType == KeyEventType.Down) {
			IsControlDown = true;
		} else if(e.keyCode == KeyCode.LeftControl && e.keyEventType == KeyEventType.Up) {
			IsControlDown = false;
		}
	}

	private void KeyBoardPressedHandler(KeyBoardEventArgs e) {
		if(e.keyCode == KeyCode.R && e.keyEventType == KeyEventType.Down) {
			if(gameMode == GameMode.PlaceBuilding) {
				//Quaternion newRotation = new Quaternion(buildingReadyToPlace.transform.rotation.x, buildingReadyToPlace.transform.rotation.y + 0.785398f, buildingReadyToPlace.transform.rotation.z, buildingReadyToPlace.transform.rotation.w);
                //buildingReadyToPlace.transform.rotation = newRotation;

                buildingReadyToPlace.transform.Rotate(0, 45, 0);
			}
		}
	}
	


	//------------------------------Mouse Button Handler------------------------------
	private void MouseClickedHandler(MouseClickEventArgs e) {
        DragManager dragManager = DragManager.main;
        HoverManager hoverManager = HoverManager.main;
  
	}
    
	//------------------------------Scroll Wheel Handler-----------------------------
	private void MouseScrollHandler(MouseWheelEventArgs e) {
        
    }

    //------------------------------Mouse At Screen Edge Handler------------------------------
    Vector3 pos = new Vector3();
    private void MouseAtScreenEdgeHandler(ScreenEdgeEventArgs e) {
		
		//Camera Screen Move
		float newX = myCamera.transform.position.x;
		float newY = myCamera.transform.position.y;
		float newZ = myCamera.transform.position.z;
		float amount = 0.3f;

		if(e.screenEdgeEventType == ScreenEdgeEventType.Up) {
			newZ += amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		} else if(e.screenEdgeEventType == ScreenEdgeEventType.Down) {
			newZ -= amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		} else if(e.screenEdgeEventType == ScreenEdgeEventType.Right) {
			newX += amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		} else if(e.screenEdgeEventType == ScreenEdgeEventType.Left) {
			newX -= amount;
			pos.Set(newX, newY, newZ);
			myCamera.transform.position = pos;
		}
	}
}

public enum GameMode {
	Normal,
	Menu,
	PlaceBuilding
}

public enum InteractionMode {
	Nothing,
	Menu,
	Select,
	Move,
	Attack,
	Interact,
	Build
}
