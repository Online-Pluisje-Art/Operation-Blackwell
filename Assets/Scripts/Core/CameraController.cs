using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	class CameraController : MonoBehaviour {
		private Camera camera_;
		private float cameraSpeed_ = 1f;
		private Vector3 targetPosition_;

		/*
		* Initializes the camera controller
		* and sets the camera in the movement controller.
		*/
		private void Awake() {
			camera_ = Camera.main;
		}

		private void Start() {
			GridCombatSystem.Instance.OnUnitSelect += OnNewPlayerSelect;
			GridCombatSystem.Instance.OnUnitMove += OnPlayerMove;
		}

		private void FixedUpdate() {
			if(targetPosition_ != camera_.transform.position && !HandleCameraMovement()) {
				camera_.transform.position = Vector3.Lerp(camera_.transform.position, targetPosition_, Time.deltaTime * cameraSpeed_);
			}
		}

		private bool HandleCameraMovement(float distance = 10f) {
			Vector3 moveDir = new Vector3(0, 0);
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
				moveDir.y = +1;
			}
			if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
				moveDir.y = -1;
			}
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
				moveDir.x = -1;
			}
			if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
				moveDir.x = +1;
			}
			moveDir.Normalize();

			float moveSpeed = 15f;

			Vector3 activePlayerPosition = new Vector3(0, 0, 0);
			if(GridCombatSystem.Instance.GetActiveUnit() != null) {
				activePlayerPosition = GridCombatSystem.Instance.GetActiveUnit().transform.position;
			}
			//Lock the camera to the active player plus a range of 10
			if(activePlayerPosition.x - distance > camera_.transform.position.x || activePlayerPosition.x + distance < camera_.transform.position.x) {
				moveDir.x = 0;
			}
			if(activePlayerPosition.y - distance > camera_.transform.position.y || activePlayerPosition.y + distance < camera_.transform.position.y) {
				moveDir.y = 0;
			}
			camera_.transform.position += moveDir * moveSpeed * Time.deltaTime;

			if(moveDir.magnitude > 0) {
				return true;
			} else if(!Input.anyKey) {
				return false;
			} else if(camera_.transform.position.x <= activePlayerPosition.x - distance
				|| camera_.transform.position.x >= activePlayerPosition.x + distance
				|| camera_.transform.position.y <= activePlayerPosition.y - distance 
				|| camera_.transform.position.y >= activePlayerPosition.y + distance) {
				return true;
			}
			return false;
		}

		private void OnDestroy() {
			GridCombatSystem.Instance.OnUnitSelect -= OnNewPlayerSelect;
			GridCombatSystem.Instance.OnUnitMove -= OnPlayerMove;
		}

		private void OnNewPlayerSelect(object player, GridCombatSystem.UnitPositionEvent args) {
			if(args.unit != null) {
				targetPosition_ = new Vector3((int)args.position.x, (int)args.position.y, camera_.transform.position.z);
			}
		}

		private void OnPlayerMove(object player, GridCombatSystem.UnitPositionEvent args) {
			if(args.unit != null) {
				targetPosition_ = new Vector3((int)args.position.x, (int)args.position.y, camera_.transform.position.z);
			}
		}
	}
}
