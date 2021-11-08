using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	public class OptionsMenu : Menu {

		public void ToggleFullscreen() {
			Screen.fullScreen = !Screen.fullScreen;
		}

		public void Option2Clicked() {
			Debug.Log("Option 2 clicked");
		}

		public override void ReturnOrExit() {
			optionsMenu_.SetActive(false);
			mainMenu_.SetActive(true);
		}
	}
}
