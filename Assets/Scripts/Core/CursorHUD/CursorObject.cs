using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class CursorObject : MonoBehaviour {

		[SerializeField] private CursorController.CursorType cursorType_;

		private void OnMouseEnter() {
			CursorController.instance.SetActiveCursorType(cursorType_);
		}

		private void OnMouseExit() {
			CursorController.instance.SetActiveCursorType(CursorController.CursorType.Arrow);
		}
	}
}
