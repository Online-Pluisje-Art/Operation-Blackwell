using UnityEngine;

namespace OperationBlackwell.Core {
    public class Actions {
        public enum ActionType {
            None,
            Move,
            Attack,
        }

        public ActionType type { get; private set; }
        public Vector3 destination { get; private set; }
        public Vector3 origin { get; private set; }
        public CoreUnit target { get; private set; }
        public int cost { get; private set; }

        public Actions(ActionType type, Vector3 destination, Vector3 origin, CoreUnit target, int cost) {
            this.type = type;
            this.destination = destination;
            this.origin = origin;
            this.target = target;
            this.cost = cost;
        }
    }
}