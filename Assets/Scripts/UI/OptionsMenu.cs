using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	public class OptionsMenu : MonoBehaviour {

		public void ToggleFullscreen() {
			Screen.fullScreen = !Screen.fullScreen;
		}

		public void Option2Clicked() {
			Debug.Log("Option 2 clicked");
		}
	}
}
