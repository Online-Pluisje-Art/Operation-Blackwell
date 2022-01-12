using UnityEngine;

namespace OperationBlackwell.Core {
	public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour {
		public static T instance { get; private set; }
		protected virtual void Awake() => instance = this as T;

		protected virtual void OnApplicationQuit() {
			instance = null;
			Destroy(gameObject);
		}
	}
}
