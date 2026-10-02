using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class FinishPlatform : MonoBehaviour
{

    [Header("SO Variables")]
    [SerializeField] private IntVariable _playerMoveBlockVariable;
    [SerializeField] private Vector3Variable _playerSpawnpointVariable;

    [Header("Platform Settings")]
    [SerializeField] float _duration = 2f;
    [SerializeField] private float _distance = 2f;
    [SerializeField] private float _routineDelay = 0.5f;
    [SerializeField] private float _checkDistance = 1f;
    [SerializeField] private AnimationCurve _animationCurve;

    [Header("References")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform _initialStartPlatform;

    private void Start()
    {
        PlayerMovement.Instance.OnMoveEnd += FinishRayCast;
    }

    private void OnDestroy()
    {
        PlayerMovement.Instance.OnMoveEnd -= FinishRayCast;
    }


    private void FinishRayCast()
    {
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitInfo, _checkDistance))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                StartCoroutine(FinishRoutine(hitInfo.transform));
            }
        }
    }

    
    IEnumerator FinishRoutine (Transform playerTransform)
    {
        _playerMoveBlockVariable.value++;
        
        yield return new WaitForSeconds(_routineDelay);

        _camera.Follow = null;

        float timeElapsed = 0f;

        Vector3 startPos = transform.position;

        Vector3 targetPos = transform.position + (Vector3.down * _distance);

        playerTransform.SetParent(transform);

        while (timeElapsed < _duration)
        {
            float progress = timeElapsed / _duration;

            float curveValue = _animationCurve.Evaluate(progress);

            transform.position = Vector3.Lerp(startPos, targetPos, curveValue);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        _playerSpawnpointVariable.value = _initialStartPlatform.position + Vector3.up;

        StartCoroutine(PlayerMovement.Instance.FadeScreenRoutine(1f));

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
