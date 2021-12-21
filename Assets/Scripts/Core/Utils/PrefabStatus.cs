using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
    public class PrefabStatus : MonoBehaviour {
        [SerializeField] private bool walkable_;
        [SerializeField] private bool cover_;
        [SerializeField] private float hitChanceModifier_;

        private void Start() {
            Tilemap.Node node = GameController.Instance.grid.GetGridObject(transform.position);
            node.walkable = walkable_;
            node.cover = cover_;
            node.hitChanceModifier = hitChanceModifier_;
            int x, y;
            GameController.Instance.grid.GetXY(transform.position, out x, out y);
            GameController.Instance.grid.TriggerGridObjectChanged(x, y);
            GridPathfinding.Instance.SetWalkable(x, y, walkable_);
        }
    }
}
