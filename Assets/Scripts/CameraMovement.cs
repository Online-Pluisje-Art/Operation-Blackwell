using UnityEngine;

class CameraMovement : ICameraMovementController {
    // This is a reference to the transform of the camera.
    private Camera camera_;

    /*
     * This sets the camera to the given transform.
     */
    public void SetCamera(Camera camera) {
        camera_ = camera;
    }


    /*
     * This handles the input for the camera and moves the camera with the appropriate vector.
     * This is called every frame.
     */
    public void HandleInput() {
        if (Input.GetKey(KeyCode.W)) {
            this.Move(Vector3.forward);
        } 
        if (Input.GetKey(KeyCode.S)) {
            this.Move(Vector3.back);
        } 
        if (Input.GetKey(KeyCode.A)) {
            this.Move(Vector3.left);
        } 
        if (Input.GetKey(KeyCode.D)) {
            this.Move(Vector3.right);
        }
    }

    // Translate the camera with the given vector.
    private void Move(Vector3 direction) {
        camera_.transform.Translate(direction * Time.deltaTime);
    }
}