using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using OperationBlackwell.Core;

namespace OperationBlackwell.LevelTransitions {
	public class LevelTranstionController : MonoBehaviour {
		public static LevelTranstionController instance { get; private set; }
		
		[SerializeField] private BaseCutsceneController cutsceneController_;
		[SerializeField] private string[] levels_;
		[SerializeField][Range(0, 1)] private float fadeTime_;

		private int currentLevelIndex_;

		bool test = false;

		private void Awake() {
			if(instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}

			currentLevelIndex_ = 0;
		}

		private void Update() {
			if(!test) {
				test = true;
				cutsceneController_.StartCutscene(0);
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
