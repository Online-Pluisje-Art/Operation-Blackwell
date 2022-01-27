using UnityEngine;

namespace OperationBlackwell.Core {
	public class CreditController : Singleton<CreditController> {
		private void Update() {
			HandleMisc();
		}

		private void HandleMisc() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				GlobalController.instance.ReturnMainMenu();
			}
		}
	}
}
