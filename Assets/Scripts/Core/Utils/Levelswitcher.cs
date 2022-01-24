using UnityEngine;

namespace OperationBlackwell.Core { 
	public class Levelswitcher : MonoBehaviour {
		public void ContinueLastLevel() {
			GlobalController.instance.ContinueLastLevel();
		}

		public void ReturnMainMenu() {
			GlobalController.instance.ReturnMainMenu();
		}
	}
}
