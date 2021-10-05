using UnityEngine;

class CameraController : MonoBehaviour {
	// The movementcontroller of the camera.
	private ICameraMovementController cameraMovementController_;

	/*
	 * Initializes the camera controller
	 * and sets the camera in the movement controller.
	 */
	private void Awake() {
		Camera camera = GetComponent<Camera>();
		cameraMovementController_ = new CameraMovement();
		cameraMovementController_.SetCamera(camera);
	}


	/*
	 * Updates the camera movement
	 * and moves the camera.
	 */
	private void Update() {
		cameraMovementController_.HandleInput();
	}
}
