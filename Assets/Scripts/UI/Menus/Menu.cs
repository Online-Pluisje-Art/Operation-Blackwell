using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	public class Menu : MonoBehaviour {

		[SerializeField] protected GameObject canvas_;
		[SerializeField] protected GameObject mainMenu_;
		[SerializeField] protected GameObject optionsMenu_;
		[SerializeField] protected GameObject graphicsOptionsMenu_;
		[SerializeField] protected GameObject audioOptionsMenu_;

		public virtual void ReturnOrExit() {
			Debug.Log("Please implement ReturnOrExit for this menu!");
		}

		private void Update() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				ReturnOrExit();
			}
		}
	}
}
