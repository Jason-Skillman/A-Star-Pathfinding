using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragManager : MonoBehaviour {
    
    //Singleton
    public static DragManager main;
    
    public bool isDragging = false;
    public Rect dragArea;

    public Vector2 dragLocationStart = Vector2.zero;
	public Vector2 dragLocationEnd = Vector2.zero;
	
	private Texture2D texture2D;
	private GUIStyle dragStyle;
    
    
    public void Awake() {
        if(main == null) main = this;
	}
    
    public void Start() {
		EventManager.main.MouseClickEvent += MouseButtonPressed;

		//Create Texture2D
		texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture2D.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f, 0.3f));
   		texture2D.Apply();
        
        //Create GUIStyle
        dragStyle = new GUIStyle();
        dragStyle.normal.background = texture2D;
        dragStyle.border.top = 1;
		dragStyle.border.bottom = 1;
		dragStyle.border.right = 1;
		dragStyle.border.left = 1;
	}


    int threshold = 15;
    private void MouseButtonPressed(MouseClickEventArgs e) {
        
        if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Down) {
            dragLocationStart = e.screenPosition;
        } else if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Holding) {
            dragLocationEnd = e.screenPosition;
        } else if(e.buttonType == ButtonType.LeftButton && e.buttonEventType == ButtonEventType.Up) {
            dragLocationStart = Vector2.zero;
            dragLocationEnd = Vector2.zero;
        }


        if(Mathf.Abs(dragLocationStart.x - dragLocationEnd.x) > threshold || Mathf.Abs(dragLocationStart.y - dragLocationEnd.y) > threshold) {
            if(dragLocationEnd != Vector2.zero) {
                isDragging = true;
            }
        } else {
            isDragging = false;
        }
    }
    

    /*
	public void SelectObject(RTSGameObject rtsGameObject) {
		if(!selectedObjects.Contains(rtsGameObject)) { //If the selectedObject does NOT already contain the newly selected object
            
			if(TeamManager.main.player1.IsRTSAlly(rtsGameObject)) {
				if(rtsGameObject.unitType == UnitType.Unit) {
					selectedObjects.Add(rtsGameObject);
				}
			}

		}
	}

	public void DeselectObject(RTSGameObject gameObject) {
		selectedObjects.Remove(gameObject);
	}

	public void DeselectAllObjects() {
		selectedObjects.Clear();
	}
    */


    public void DragBox(Vector2 topLeft, Vector2 bottomRight, GUIStyle style) {
		float minX = Mathf.Max(topLeft.x, bottomRight.x);   //minX
		float maxX = Mathf.Min(topLeft.x, bottomRight.x);   //maxX
		
		float minY = Mathf.Max(Screen.height-topLeft.y, Screen.height-bottomRight.y);   //minY
		float maxY = Mathf.Min(Screen.height-topLeft.y, Screen.height-bottomRight.y);   //maxY
				
		Rect rect = new Rect(minX, minY, maxX-minX, maxY-minY);
		
		dragArea = new Rect(maxX, maxY, minX-maxX, minY-maxY);
		
		GUI.Box(rect, "", style);
	}
    
    public bool IsWithinDragBox(Vector3 worldPos) {
		if(isDragging) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
			Vector3 realScreenPos = new Vector3(screenPos.x, Screen.height-screenPos.y, screenPos.z);

			if(dragArea.Contains(realScreenPos)) {
				return true;
			}
		}
		return false;
	}

    void OnGUI() {
        if(isDragging) {
            dragLocationEnd = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            DragBox(dragLocationStart, dragLocationEnd, dragStyle);
        }
    }

}