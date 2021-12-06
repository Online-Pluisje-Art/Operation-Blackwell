using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.LevelTransitions {
	public class LevelTranstionController : MonoBehaviour {
		public static LevelTranstionController instance { get; private set; }
		
		[SerializeField] private BaseCutsceneController cutsceneController_;
		[SerializeField] private Plane plane_;

		private void Awake() {
			if(instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}
		}
	}
}
