using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.UI {
	public class OptionsMenu : Menu {
		public override void ReturnOrExit() {
			optionsMenu_.SetActive(false);
			mainMenu_.SetActive(true);
		}
	}
}
