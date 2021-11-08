using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	public class Menu : MonoBehaviour {

		public virtual void ReturnOrExit() {
			throw new System.NotImplementedException();
		}

		private void Update() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				ReturnOrExit();
			}
		}
	}
}
