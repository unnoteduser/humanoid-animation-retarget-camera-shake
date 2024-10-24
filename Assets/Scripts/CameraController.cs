using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float rotationSpeed = 5f, minDistance = 2f, maxDistance = 10f, minHeight = 1f, maxHeight = 10f, transitionSpeed = 5f, shakeIntensity = 0.1f;

    private float currentRotationAngle, currentHeight, currentDistance, shakeTime = 0f, shakeFrequency = 10f;
    private Vector3 initialOffset;
    private bool shouldShake = false, shakeEnabled = true;

    void Start()
    {
        currentRotationAngle = transform.eulerAngles.y;
        currentHeight = offset.y;
        currentDistance = offset.z;
        initialOffset = offset;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) shakeEnabled = !shakeEnabled;

        if (Input.GetMouseButton(1))
        {
            currentRotationAngle += Input.GetAxis("Mouse X") * rotationSpeed;
            currentHeight = Mathf.Clamp(currentHeight - Input.GetAxis("Mouse Y") * rotationSpeed, minHeight, maxHeight);
        }
        else
        {
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, transform.eulerAngles.y, Time.deltaTime * transitionSpeed);
            currentHeight = Mathf.Lerp(currentHeight, initialOffset.y, Time.deltaTime * transitionSpeed);
            currentDistance = Mathf.Lerp(currentDistance, initialOffset.z, Time.deltaTime * transitionSpeed);
        }

        currentDistance = Mathf.Clamp(currentDistance - Input.GetAxis("Mouse ScrollWheel"), minDistance, maxDistance);

        Vector3 position = target.position - (Quaternion.Euler(0, currentRotationAngle, 0) * Vector3.forward * currentDistance) + (Vector3.up * currentHeight);

        if (shouldShake && shakeEnabled)
        {
            shakeTime += Time.deltaTime * shakeFrequency;
            position += new Vector3(
                (Mathf.PerlinNoise(shakeTime, 0) - 0.5f) * shakeIntensity,
                (Mathf.PerlinNoise(0, shakeTime) - 0.5f) * shakeIntensity,
                (Mathf.PerlinNoise(shakeTime, shakeTime) - 0.5f) * shakeIntensity
            );
        }

        transform.position = position;
        transform.LookAt(target.position + Vector3.up * offset.y);
    }

    public void TriggerShake() => StartCoroutine(ShakeCoroutine());

    private IEnumerator ShakeCoroutine()
    {
        shouldShake = true;
        yield return new WaitForSeconds(0.1f);
        shouldShake = false;
    }
}
















