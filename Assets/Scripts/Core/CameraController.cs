using UnityEngine;

namespace OperationBlackwell.Core {
	class CameraController : MonoBehaviour {
		// The gamecontroller of the game that holds all other controller.
		[SerializeField]
		private GameController gameController_;

		private Camera camera_;

		/*
		* Initializes the camera controller
		* and sets the camera in the movement controller.
		*/
		private void Awake() {
			camera_ = GetComponent<Camera>();
			gameController_.Moved += OnPlayerMove;
		}

		private void OnDestroy() {
			gameController_.Moved -= OnPlayerMove;
		}

		private void OnPlayerMove(Vector3 position) {
			camera_.transform.position = new Vector3(position.x, camera_.transform.position.y, position.z);
		}
	}
}
