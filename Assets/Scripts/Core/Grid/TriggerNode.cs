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
		[Header("Cutscene")]
		// Only set this if you want to use the trigger node for a cutscene. Use -1 for the combat triggers.
		[SerializeField] private int cutsceneIndex_;

		private void Start() {
			GameController.Instance.GetGrid().GetGridObject(transform.position).SetTrigger(this);
		}

		public int GetCutsceneIndex() {
			return cutsceneIndex_;
		}

		public Trigger GetTrigger() {
			return trigger_;
		}
	}
}
