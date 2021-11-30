using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	public class OptionsMenu : Menu {
		public void AudioClicked() {
			Debug.Log("Audio menu clicked");
		}

		public override void ReturnOrExit() {
			optionsMenu_.SetActive(false);
			mainMenu_.SetActive(true);
		}
	}
}
