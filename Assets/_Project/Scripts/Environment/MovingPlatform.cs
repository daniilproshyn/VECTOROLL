using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("SO Variables")]
    [SerializeField] private IntVariable _playerMoveBlockVariable;

    [Header("Platform Settings")]
    [SerializeField] float _duration = 2f;
    [SerializeField] private Vector3 _targetCoordinates;
    [SerializeField] private float _routineDelay = 0.5f;
    [SerializeField] private float _checkDistance = 0.5f;
    [SerializeField] private AnimationCurve _animationCurve;

    private bool _isMoving;

    private bool _used;
  
    private void Update()
    {
        if (_isMoving || _used) return;
        
        if (Physics.Raycast(transform.position + (Vector3.up / 2f), Vector3.up, out RaycastHit hit, _checkDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                _playerMoveBlockVariable.value++;

                _isMoving = true;

                StartCoroutine(PlatformRoutine(hit.transform));
            }
        }
    }    
    
    private IEnumerator PlatformRoutine(Transform playerTransform)
    {
        

        yield return new WaitForSeconds(_routineDelay);

        playerTransform.SetParent(transform);

        Vector3 startPos = transform.position;

        Vector3 targetPos = _targetCoordinates;

        float timeElapsed = 0f;

        while(timeElapsed < _duration)
        {
            float progress = timeElapsed / _duration;

            float curveValue = _animationCurve.Evaluate(progress);

            transform.position = Vector3.Lerp(startPos, targetPos, curveValue);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
            );

        _playerMoveBlockVariable.value--;

        _used = true;
    }
}
