using UnityEngine;
using System;

namespace OperationBlackwell.UI {
	[CreateAssetMenu(fileName = "New Cursor Animation", menuName = "Cursor/Animation")]
	public class CursorAnimation : ScriptableObject {
		public CursorController.CursorType cursorType;
		public Texture2D[] textureArray;
		public float frameRate;
		public Vector2 offset;
	}
}
