using UnityEngine;

namespace OperationBlackwell.Core {
	public interface IInteractable {
		void Interact(CoreUnit unit);
		bool IsInRange(Vector3 unitPos);
		int GetCost();
	}
}
