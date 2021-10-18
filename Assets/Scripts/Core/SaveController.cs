using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class SaveController : MonoBehaviour {
		private void Update() {
			HandleSaveLoad();
		}

		private void HandleSaveLoad() {
			if(Input.GetKeyDown(KeyCode.P)) {
				System.String saveName = getSaveName();
				GameController.Instance.tilemap.Save(saveName);
			}
			if(Input.GetKeyDown(KeyCode.L)) {
				GameController.Instance.tilemap.Load();
			}
		}

		private System.String getSaveName() {
			return "testing";
		}
	}
}
