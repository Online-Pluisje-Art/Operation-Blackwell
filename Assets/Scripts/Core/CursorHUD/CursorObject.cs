using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class CursorObject : MonoBehaviour {

		[SerializeField] private CursorController.CursorType cursorType_;

		private void OnMouseEnter() {
			CursorController.Instance.SetActiveCursorType(cursorType_);
		}

		private void OnMouseExit() {
			CursorController.Instance.SetActiveCursorType(CursorController.CursorType.Arrow);
		}
	}
}
