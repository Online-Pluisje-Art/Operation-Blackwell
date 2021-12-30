using UnityEngine;

namespace OperationBlackwell.Core {
	public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
		protected override void Awake() {
			if(instance != null) {
				Destroy(gameObject);
				return;
			}
			base.Awake();
		}
	}
}
