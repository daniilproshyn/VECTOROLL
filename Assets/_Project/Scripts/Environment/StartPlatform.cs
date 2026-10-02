using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class StartPlatform : MonoBehaviour
{
    [Header("SO Variables")]
    [SerializeField] private IntVariable _playerMoveBlockVariable;
    [SerializeField] private Vector3Variable _playerSpawnpointVariable;

    [Header("StartPlatform Settings")]
    [SerializeField] float _duration = 2f;
    [SerializeField] private float _moveDelay = 0.5f;
    [SerializeField] private float _checkDistance = 1f;
    [SerializeField] private AnimationCurve _animationCurve;


    private void Start() 
    {
       
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitInfo, _checkDistance))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                _playerMoveBlockVariable.value++;

                StartCoroutine(StartPlatformRoutine(hitInfo.transform));
            }
        }
        else
        {
            _playerMoveBlockVariable.value = 0;
        }
    }


    IEnumerator StartPlatformRoutine (Transform playerTransform)
    {
        playerTransform.SetParent(transform);

        yield return new WaitForSeconds(_moveDelay);

        Vector3 startPos = transform.position;

        Vector3 targetPos = new (transform.position.x, -0.5f, transform.position.z);

        float timeElapsed = 0f;

        while (timeElapsed < _duration)
        {
            float progress = timeElapsed / _duration;

            float curveValue = _animationCurve.Evaluate(progress);
            
            transform.position = Vector3.Lerp(startPos,targetPos,curveValue);

            timeElapsed += Time.deltaTime;

            yield return null;

        }

        playerTransform.position = new Vector3(Mathf.Round(transform.position.x), 0.5f, (Mathf.Round(transform.position.z)));

        playerTransform.SetParent(null);

        _playerMoveBlockVariable.value = 0;

    }
}
