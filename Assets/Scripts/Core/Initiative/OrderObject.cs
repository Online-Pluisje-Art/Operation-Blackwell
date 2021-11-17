using UnityEngine;

namespace OperationBlackwell.Core {
	public class OrderObject {
		private int initiative_;
		private CoreUnit unit_;
		private int totalCost_;

		public OrderObject(int initiative, CoreUnit unit, int totalCost) {
			initiative_ = initiative;
			unit_ = unit;
			totalCost_ = totalCost;
		}

		public CoreUnit GetUnit() {
			return unit_;
		}

		public int GetInitiative() {
			return initiative_;
		}

		public int GetTotalCost() {
			return totalCost_;
		}

		public void SetInitiative(int initiative) {
			initiative_ = initiative;
		}

		public void SetTotalCost(int totalCost) {
			totalCost_ = totalCost;
		}

		public void ExecuteActions() {
			unit_.ExecuteActions();
		}
	}
}
