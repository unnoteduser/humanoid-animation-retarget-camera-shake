using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2.0f, sprintSpeed = 5.335f, rotationSmoothTime = 0.12f, speedChangeRate = 10.0f, rotationSpeed = 100f;
    public GameObject footstepDustPrefab;
    public AudioClip[] footstepSounds;

    private Animator animator;
    private float speed, targetRotation = 0.0f, rotationVelocity;
    private GameObject mainCamera;
    private CameraController cameraController;
    private Transform leftFoot, rightFoot;
    private AudioSource audioSource;
    public bool IsRunning { get; private set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraController = mainCamera.GetComponent<CameraController>();
        leftFoot = GameObject.FindGameObjectWithTag("LeftFoot")?.transform;
        rightFoot = GameObject.FindGameObjectWithTag("RightFoot")?.transform;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        IsRunning = Input.GetKey(KeyCode.W);
        animator.SetBool("isRunning", IsRunning);
        if (IsRunning) Move();
        Turn();
    }

    private void Move()
    {
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        if (!IsRunning) targetSpeed = 0.0f;
        float currentHorizontalSpeed = new Vector3(transform.forward.x, 0.0f, transform.forward.z).magnitude;
        float speedOffset = 0.1f;
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else speed = targetSpeed;
        Vector3 movement = transform.forward * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    private void Turn()
    {
        if (Input.GetKey(KeyCode.A)) targetRotation -= rotationSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D)) targetRotation += rotationSpeed * Time.deltaTime;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    public void PlayFootstepEffects()
    {
        CreateFootstepDust();
        PlayFootstepSound();
    }

    private void CreateFootstepDust()
    {
        if (footstepDustPrefab != null)
        {
            StartCoroutine(DestroyAfterPlay(Instantiate(footstepDustPrefab, leftFoot.position, Quaternion.identity)));
            StartCoroutine(DestroyAfterPlay(Instantiate(footstepDustPrefab, rightFoot.position, Quaternion.identity)));
        }
        else Debug.LogError("Footstep dust prefab is not assigned.");
    }

    private IEnumerator DestroyAfterPlay(GameObject particleSystemObject)
    {
        ParticleSystem ps = particleSystemObject.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration);
            Destroy(particleSystemObject);
        }
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }

    public void TriggerFootstepShake() => cameraController?.TriggerShake();
}
















