using UnityEngine;

namespace OperationBlackwell.UI {
	public class CutsceneController : MonoBehaviour {
		[SerializeField] private Cutscene[] cutscenes_;
		private Cutscene currentCutscene_;

		private void Start() {
			currentCutscene_ = null;
		}

		public void Show(int index) {
			currentCutscene_ = cutscenes_[index];
		}

		public void Hide() {
			currentCutscene_ = null;
		}
	}
}
