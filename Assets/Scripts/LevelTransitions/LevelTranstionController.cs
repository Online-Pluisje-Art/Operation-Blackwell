using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.LevelTransitions {
	public class LevelTranstionController : MonoBehaviour {
		public static LevelTranstionController instance { get; private set; }
		
		[SerializeField] private BaseCutsceneController cutsceneController_;
		[SerializeField] private Plane plane_;

		bool test = false;

		private void Awake() {
			if(instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}
		}

		private void Update() {
			if(!test) {
				test = true;
				cutsceneController_.StartCutscene(0);
			}
		}
	}
}
