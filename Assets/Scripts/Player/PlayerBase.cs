using System;
using UnityEngine;

namespace OperationBlackwell.Player {
	/*
	* Character Base Class
	* 
	*/
	public class PlayerBase : MonoBehaviour, IGetPosition {
		public Vector3 GetPosition() {
			return transform.position;
		}
	}
}
