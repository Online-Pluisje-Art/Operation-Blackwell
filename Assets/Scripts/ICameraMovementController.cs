using UnityEngine;

/*
 * If this interface is implemented by a class, it will be used to control the camera movement.
 * The HandleInput method wil be called every frame.
 * SetCamera will be used to set the camera object to be controlled.
 */
interface ICameraMovementController {
	void HandleInput();
	void SetCamera(Camera camera);
}
