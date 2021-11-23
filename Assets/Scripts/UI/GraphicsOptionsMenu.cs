using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace OperationBlackwell.UI {
	public class GraphicsOptionsMenu : Menu {

		[SerializeField] private TextMeshProUGUI fullscreenText_;

		private void Start() {
			UpdateFullscreenText(Screen.fullScreen);
		}

		public void ToggleFullscreen() {
			Screen.fullScreen = !Screen.fullScreen;
			// Looks weird, but fullscreen doesn't update until frame end. This seems to work fine.
			UpdateFullscreenText(!Screen.fullScreen);
		}

		public void Option2Clicked() {
			Debug.Log("Option 2 clicked");
		}

		public override void ReturnOrExit() {
			graphicsOptionsMenu_.SetActive(false);
			optionsMenu_.SetActive(true);
		}

		private void UpdateFullscreenText(bool fullscreen) {
			if(fullscreen) {
				fullscreenText_.text = "Fullscreen: On";
			} else {
				fullscreenText_.text = "Fullscreen: Off";
			}
		}
	}
}
