using UnityEngine;

namespace OperationBlackwell.Core {
	public interface IInteractable {
		void Interact();
		bool IsInRange(Vector3 unitPos);
		int GetCost();
	}
}
