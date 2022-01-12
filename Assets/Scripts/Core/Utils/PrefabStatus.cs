using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class PrefabStatus : MonoBehaviour {
		[SerializeField] private bool walkable_;
		[SerializeField] private bool cover_;
		[SerializeField] private float hitChanceModifier_;

		private void Start() {
			GameController con = GameController.instance;
			Tilemap.Node node = con.grid.GetGridObject(transform.position);
			node.walkable = walkable_;
			node.cover = cover_;
			node.hitChanceModifier = hitChanceModifier_;
			int x, y;
			con.grid.GetXY(transform.position, out x, out y);
			con.grid.TriggerGridObjectChanged(x, y);
			GridPathfinding.Instance.SetWalkable(x, y, walkable_);
		}

		private void OnDestroy() {
			int x, y;
			GameController con = GameController.instance;
			con.grid.GetXY(transform.position, out x, out y);
			con.grid.TriggerGridObjectChanged(x, y);
			GridPathfinding.Instance.SetWalkable(x, y, true);
		}
	}
}
