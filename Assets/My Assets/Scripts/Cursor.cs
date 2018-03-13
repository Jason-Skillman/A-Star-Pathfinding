using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cursor : MonoBehaviour {

	public CursorID cursorID;
	public List<Texture2D> cursorTextures;

	public bool centerTexture = true;
	public float frameTime = 0.1f;
	
	private bool isAnimated = false;
	private float frameCurrentTime = 0;
	private int frameIndex = 0;
	
	
	public void Awake() {
		if(cursorTextures.Count > 1) {
			isAnimated = true;
		}
	}
    

	public void Animate() {
		frameCurrentTime += Time.deltaTime;

		if(frameTime <= frameCurrentTime) {
			frameCurrentTime = 0;
			frameIndex++;
			if(frameIndex >= cursorTextures.Count) {
				frameIndex = 0;
			}
		}
	}

	public Texture2D getCursorPicture() {
		return cursorTextures[frameIndex];
	}

	public bool getAnimate() {
		return isAnimated;
	}
}

public enum CursorID {
	Normal,
	Selectable,
	NotSelectable,
	Attackable,
	Moveable,
	NotMoveable,
	Build
}
