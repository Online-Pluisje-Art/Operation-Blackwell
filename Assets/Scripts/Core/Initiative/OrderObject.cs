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

        // public int Set
    }
}