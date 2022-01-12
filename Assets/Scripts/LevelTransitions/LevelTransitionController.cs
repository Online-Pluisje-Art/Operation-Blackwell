using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using OperationBlackwell.Core;

namespace OperationBlackwell.LevelTransitions {
	public class LevelTransitionController : Singleton<LevelTransitionController> {
		[SerializeField][Range(0, 1)] private float fadeTime_;

		public System.EventHandler<int> LeveltransitionCutsceneTriggered;
		public System.EventHandler<System.EventArgs> TransitionDone;
		private bool triggered_;
		private string levelToLoadAfter_;

		private List<Transform> children_;

		private void Start() {
			triggered_ = false;
			levelToLoadAfter_ = "";
			children_ = new List<Transform>();
			GameController.instance.LevelTransitionStarted += OnLevelTransitionStarted;
			TransitionDone += OnTransitionDone;
			foreach(Transform child in transform) {
				children_.Add(child);
				child.gameObject.SetActive(false);
			}
		}

		private void OnLevelTransitionStarted(object sender, GameController.LevelTransitionArgs e) {
			foreach(Transform child in children_) {
				child.gameObject.SetActive(true);
			}
			triggered_ = true;
			levelToLoadAfter_ = e.nextLevel;
			LeveltransitionCutsceneTriggered?.Invoke(this, e.cutsceneIndex);
		}

		private void OnTransitionDone(object sender, System.EventArgs e) {
			if(triggered_) {
				triggered_ = false;
				StartCoroutine(EndTransitionCoroutine());
			}
		}

		private IEnumerator EndTransitionCoroutine() {
			yield return new WaitForSeconds(fadeTime_);
			GlobalController.instance.LevelCompleted?.Invoke(this, new GlobalController.LevelDataArgs {
				levelData = new GlobalController.LevelData {
					id = SceneManager.GetActiveScene().name,
					completed = true
				},
				newLevelId = levelToLoadAfter_
			});
		}

		public bool IsTransitioning() {
			return triggered_;
		}
	}
}
