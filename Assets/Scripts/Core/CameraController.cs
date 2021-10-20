using UnityEngine;
using System.Collections;

namespace OperationBlackwell.Core {
	class CameraController : MonoBehaviour {
		private Camera camera_;
		private float cameraSpeed_ = 1.5f;
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
			if(targetPosition_ != camera_.transform.position) {
				camera_.transform.position = Vector3.Lerp(camera_.transform.position, targetPosition_, Time.deltaTime * cameraSpeed_);
			}
		}

		private void OnDestroy() {
			GridCombatSystem.Instance.OnUnitSelect -= OnNewPlayerSelect;
			GridCombatSystem.Instance.OnUnitMove -= OnPlayerMove;
		}

		private void OnNewPlayerSelect(object player, GridCombatSystem.UnitPositionEvent args) {
			if(args.unit != null) {
				targetPosition_ = new Vector3(args.position.x, args.position.y, camera_.transform.position.z);
			}
		}

		private void OnPlayerMove(object player, GridCombatSystem.UnitPositionEvent args) {
			if(args.unit != null) {
				targetPosition_ = new Vector3(args.position.x, args.position.y, camera_.transform.position.z);
			}
		}
	}
}
