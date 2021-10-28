using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OperationBlackwell.UI {
	public class MainMenu : MonoBehaviour {

		public void PlayGame() {
			SceneManager.LoadScene("TutorialLevel");
		}

		public void LaunchMapEditor() {
			SceneManager.LoadScene("MapEditor");
		}

		public void ExitGame() {
			if(!Application.isEditor) {
				Application.Quit(0);
			} else {
				Debug.Log("Game asked to quit while in editor, call ignored.");
			}
		}

		private void Update() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				ExitGame();
			}
		}
	}
}
