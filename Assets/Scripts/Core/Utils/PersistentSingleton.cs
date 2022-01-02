using UnityEngine;

namespace OperationBlackwell.Core {
	public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
	}
}
