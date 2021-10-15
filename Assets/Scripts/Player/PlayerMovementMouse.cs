using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {    
	public class PlayerMovementMouse : MonoBehaviour {
		private void Update() {
			if (Input.GetMouseButtonDown(1)) {
				GetComponent<IMovePosition>().SetMovePosition(Utils.GetMouseWorldPosition());
			}
		}
	}
}
