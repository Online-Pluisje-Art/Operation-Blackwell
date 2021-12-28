using UnityEngine;

namespace OperationBlackwell.Core {
	public abstract class BaseAIController : MonoBehaviour {
		public abstract void SetUnitActionsTurn();
		public abstract void LoadStage(int index);
	}
}
