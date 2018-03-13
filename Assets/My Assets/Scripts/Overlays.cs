using UnityEngine;
using System.Collections;

public static class Overlays {

	private static int Height = 100, Width = 100;
	private static int HealthHeight = 20;

	private static Color BorderColour = Color.white;
	private static Color HealthColour = Color.green;

	public static Texture2D CreateTexture() {
		Texture2D texToReturn = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

		for(int i = 0; i<Width; i++) {
			for(int j = 0; j<Height; j++) {
				if(i == 0 || i == 1 || j == 0 || j == 1 || i == Width-1 || i == Width-2 || j == Height-1 || j == Height-2 || j == Height-HealthHeight) {
					texToReturn.SetPixel(i, j, BorderColour);
				} else if(j > Height-HealthHeight) {
					texToReturn.SetPixel(i, j, HealthColour);
				} else {
					texToReturn.SetPixel(i, j, Color.clear);
				}
			}
		}
		texToReturn.Apply();
		return texToReturn;
	}

	public static void UpdateTexture(Texture2D overlay, float healthRatio) {
		for(int i = 0; i<Width; i++) {
			for(int j = Height-HealthHeight; j<Height; j++) {
				if((float)i/(float)Width < healthRatio) {
					overlay.SetPixel(i, j, HealthColour);
				} else {
					overlay.SetPixel(i, j, Color.clear);
				}
			}
		}
	}
}
