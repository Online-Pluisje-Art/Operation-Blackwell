using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationBlackwell.Core {
	public class PaintingController : MonoBehaviour {

		private Tilemap.Node.NodeSprite nodeSprite_;

		private void Update() {
			HandlePainting();
		}

		private void HandlePainting() {
			// Tilemap code, rightclick please!
			if(Input.GetKeyDown(KeyCode.T)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.NONE;
			}
			if(Input.GetKeyDown(KeyCode.Y)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.PIT;
			}
			if(Input.GetKeyDown(KeyCode.U)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.FLOOR;
			}
			if(Input.GetKeyDown(KeyCode.I)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.WALL;
			}
			if(Input.GetKeyDown(KeyCode.O)) {
				nodeSprite_ = Tilemap.Node.NodeSprite.COVER;
			}
			if(Input.GetMouseButtonDown(1)) {
				Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
				Tilemap.Node node = GameController.Instance.grid.GetGridObject(mouseWorldPosition);
				node.SetNodeSprite(nodeSprite_);
				GameController.Instance.grid.TriggerGridObjectChanged(node.gridX, node.gridY);
			}
		}

	}
}
