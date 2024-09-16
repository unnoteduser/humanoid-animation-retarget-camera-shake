using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The target the camera follows (the player)
    public Vector3 offset; // Offset from the target
    public float rotationSpeed = 5f; // Speed of camera rotation
    public float minDistance = 2f; // Minimum distance from the target
    public float maxDistance = 10f; // Maximum distance from the target
    public float minHeight = 1f; // Minimum height of the camera
    public float maxHeight = 10f; // Maximum height of the camera
    public float transitionSpeed = 5f; // Speed of transitioning back to the initial offset

    private float currentRotationAngle;
    private float currentHeight;
    private float currentDistance;
    private Animator animator;
    private Vector3 initialOffset; // Store the initial offset

    // Custom properties for animation
    public float shakeIntensity = 0.1f; // Reduced intensity for less amplitude
    private float shakeTime = 0f;
    private float shakeFrequency = 10f; // Increased frequency for sharper movements

    void Start()
    {
        // Initialize the current rotation angle, height, and distance based on the initial offset
        currentRotationAngle = transform.eulerAngles.y;
        currentHeight = offset.y;
        currentDistance = offset.z;

        // Store the initial offset
        initialOffset = offset;

        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Toggle the Animator component on key press "P"
        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.enabled = !animator.enabled;
        }

        // Check if the right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            // Get mouse input for rotation
            float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed;
            float verticalInput = Input.GetAxis("Mouse Y") * rotationSpeed;

            // Update the rotation angle based on horizontal input
            currentRotationAngle += horizontalInput;

            // Update the height based on vertical input
            currentHeight -= verticalInput;
            currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight); // Clamp the height to avoid going too low or too high
        }
        else
        {
            // Smoothly transition back to the initial offset
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, transform.eulerAngles.y, Time.deltaTime * transitionSpeed);
            currentHeight = Mathf.Lerp(currentHeight, initialOffset.y, Time.deltaTime * transitionSpeed);
            currentDistance = Mathf.Lerp(currentDistance, initialOffset.z, Time.deltaTime * transitionSpeed);
        }

        // Get mouse scroll input for zooming
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scrollInput;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance); // Clamp the distance

        // Calculate the new position and rotation
        Quaternion rotation = Quaternion.Euler(0, currentRotationAngle, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * currentDistance) + (Vector3.up * currentHeight);

        // Apply smooth shake effect using Perlin noise if the animator is enabled
        if (animator.enabled)
        {
            shakeTime += Time.deltaTime * shakeFrequency; // Increase the frequency of the shake
            float shakeOffsetX = (Mathf.PerlinNoise(shakeTime, 0) - 0.5f) * shakeIntensity;
            float shakeOffsetY = (Mathf.PerlinNoise(0, shakeTime) - 0.5f) * shakeIntensity;
            float shakeOffsetZ = (Mathf.PerlinNoise(shakeTime, shakeTime) - 0.5f) * shakeIntensity;
            position += new Vector3(shakeOffsetX, shakeOffsetY, shakeOffsetZ);
        }

        // Apply the new position and rotation to the camera
        transform.position = position;
        transform.LookAt(target.position + Vector3.up * offset.y); // Ensure the camera looks at the target's height

        // Update the animator based on the player's state if the animator is enabled
        if (animator.enabled)
        {
            PlayerController playerController = target.GetComponent<PlayerController>();
            if (playerController != null)
            {
                animator.SetBool("isRunning", playerController.IsRunning);
            }
        }
    }
}














