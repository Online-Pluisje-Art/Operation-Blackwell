using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	public class Menu : MonoBehaviour {

		protected GameObject canvas_;
		protected GameObject mainMenu_;
		protected GameObject optionsMenu_;

		private void Awake() {
			this.canvas_ = GameObject.Find("Canvas");
			this.mainMenu_ = canvas_.transform.Find("MainMenu").gameObject;
			this.optionsMenu_ = canvas_.transform.Find("OptionsMenu").gameObject;
		}

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
