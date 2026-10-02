using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    private static bool _isFirstLaunch = true;

    public Action OnMoveEnd;

    [Header("SO Variables")]
    [SerializeField] private Vector3Variable _inputMoveVariable;
    [SerializeField] private IntVariable _moveBlockVariable;
    [SerializeField] private Vector3Variable _spawnpointVariable;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 300f;
    [SerializeField] private float _rollAngle = 90f;

    [Header("Void Falling")]
    [SerializeField] private float _fallDistance = 2f;
    [SerializeField] private float _fallSpeed = 2f;
    [SerializeField] private float _voidCheckDistance = 2f;

    [Header("References")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform _initialStartPlatform;

    [Header("Transition Settings")]
    [SerializeField] private CanvasGroup _fadeScreen;
    [SerializeField] private float _fadeSpeed = 2f;

    private void Awake()
    {
        Instance = this;


        if (_isFirstLaunch)
        {
            if(_initialStartPlatform != null) 
            { 
                _spawnpointVariable.value = _initialStartPlatform.position + Vector3.up;
            }

            _isFirstLaunch = false;
        }

        transform.position = _spawnpointVariable.value;

        Physics.SyncTransforms();
    }

    private void Start()
    {
            _fadeScreen.alpha = 1f;

            StartCoroutine(FadeScreenRoutine(0f));
    }


    private void Update()
    {
        if(_moveBlockVariable.value > 0) return;

        Vector3 currentInput = _inputMoveVariable.value;
        
        if (MathF.Abs(currentInput.x) > 0.1f && MathF.Abs(currentInput.z) > 0.1f) return;
        
        if (currentInput != Vector3.zero)
        {
            PrepareRoll(currentInput);

            _inputMoveVariable.value = Vector3.zero;
        }
    }

    
    private void PrepareRoll (Vector3 direction)
    {
        _moveBlockVariable.value++;
        
        Vector3 pivot = transform.position + (Vector3.down * 0.5f) + (direction * 0.5f);

        StartCoroutine(RollRoutine(pivot, direction));

    }

    
    private IEnumerator RollRoutine (Vector3 pivot, Vector3 direction)
    {
        float currentAngle = 0f;

        Vector3 rotAxis = Vector3.Cross(Vector3.up, direction);

        while( currentAngle < _rollAngle)
        {
            float step = _movementSpeed * Time.deltaTime;

            if (currentAngle + step > _rollAngle) 
            {
                step = _rollAngle - currentAngle;            
            }

            transform.RotateAround(pivot, rotAxis, step);
            currentAngle += step;
            
            yield return null;
        }

        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
            );

        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _voidCheckDistance))
        {
            if (hit.collider.CompareTag("Void"))
            {
                StartCoroutine(FallIntoVoidRoutine());

                yield break;
            }
            else if (hit.collider.CompareTag("Checkpoint"))
            {
                NewSpawnPoint(hit.collider.transform);
            }
        }

        _moveBlockVariable.value--;

        OnMoveEnd?.Invoke();
    }

    private IEnumerator FallIntoVoidRoutine()
    {
        _camera.Follow = null;

        _moveBlockVariable.value++;

        Vector3 targetPos = transform.position + Vector3.down * _fallDistance;

            StartCoroutine(FadeScreenRoutine(1f));

        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _fallSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void NewSpawnPoint(Transform checkpoint)
    {
        _spawnpointVariable.value = checkpoint.position + Vector3.up;
    }

    public IEnumerator FadeScreenRoutine(float targetAlpha)
    {
        while (!Mathf.Approximately(_fadeScreen.alpha, targetAlpha))
        {
            _fadeScreen.alpha = Mathf.MoveTowards(_fadeScreen.alpha, targetAlpha, _fadeSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
