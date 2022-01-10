using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace OperationBlackwell.Core {
	public class LevelTransitionTrigger : MonoBehaviour {
		[SerializeField] private string currentLevel_;
		[SerializeField] private string nextLevel_;
		[SerializeField] private int cutsceneIndex_;

		private void Start() {
			GameController.instance.GetGrid().GetGridObject(transform.position).SetLevelTransitionTrigger(this);
		}

		public string GetCurrentLevel() {
			return currentLevel_;
		}

		public string GetNextLevel() {
			return nextLevel_;
		}

		public int GetCutsceneIndex() {
			return cutsceneIndex_;
		}
	}
}
