using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace OperationBlackwell.Core {
	public class BossTrigger : MonoBehaviour {
		private void Start() {
			GameController.instance.GetGrid().GetGridObject(transform.position).SetBossTrigger(this);
		}
	}
}
