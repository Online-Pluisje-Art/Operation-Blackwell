using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	public class TriggerNode : MonoBehaviour {
		public enum Trigger {
			None,
			Combat,
			Cutscene,
		}
		[SerializeField] private Trigger trigger_;
		[SerializeField] private int index_;

		private void Start() {
			GameController.instance.GetGrid().GetGridObject(transform.position).SetTrigger(this);
		}

		public int GetIndex() {
			return index_;
		}

		public Trigger GetTrigger() {
			return trigger_;
		}
	}
}
