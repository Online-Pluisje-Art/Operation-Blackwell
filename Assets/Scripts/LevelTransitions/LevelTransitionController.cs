using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using OperationBlackwell.Core;

namespace OperationBlackwell.LevelTransitions {
	public class LevelTransitionController : Singleton<LevelTransitionController> {
		[SerializeField] private string[] levels_;
		[SerializeField][Range(0, 1)] private float fadeTime_;

		private int currentLevelIndex_;

		public System.EventHandler<int> LeveltransitionCutsceneTriggered;

		bool test = false;

		private void Update() {
			currentLevelIndex_ = 0;
			if(!test) {
				test = true;
				LeveltransitionCutsceneTriggered?.Invoke(this, 0);
			}
		}

		public void EndTransition() {
			StartCoroutine(EndTransitionCoroutine());
		}

		private IEnumerator EndTransitionCoroutine() {
			yield return new WaitForSeconds(fadeTime_);
			SceneManager.LoadScene(levels_[currentLevelIndex_]);
			currentLevelIndex_++;
		}
	}
}
