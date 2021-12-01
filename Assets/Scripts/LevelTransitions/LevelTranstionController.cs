using UnityEngine;

namespace OperationBlackwell.LevelTransitions {
	public class LevelTranstionController : MonoBehaviour {
		public static LevelTranstionController instance { get; private set; }

		private void Awake() {
			if(instance == null) {
				instance = this;
			} else {
				Destroy(gameObject);
			}
		}
	}
}
