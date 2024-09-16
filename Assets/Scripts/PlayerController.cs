using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float rotationSpeed = 100f; // Added rotation speed variable

    private Animator animator;
    public bool IsRunning { get; private set; } // Public property to expose isRunning

    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private GameObject _mainCamera;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        IsRunning = Input.GetKey(KeyCode.W);
        animator.SetBool("isRunning", IsRunning);

        if (IsRunning)
        {
            Move();
        }

        Turn();
    }

    private void Move()
    {
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : MoveSpeed;

        if (!IsRunning) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(transform.forward.x, 0.0f, transform.forward.z).magnitude;
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        Vector3 movement = transform.forward * _speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    private void Turn()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _targetRotation -= rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _targetRotation += rotationSpeed * Time.deltaTime;
        }

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }
}










