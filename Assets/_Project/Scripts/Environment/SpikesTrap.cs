using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikesTrap : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _intervalSeconds = 2f;

    [Header("References")]
    [SerializeField] private Transform _spikes;

    private bool _isKilling = false;
    private float _timer;
    private bool _spikeOut = true;

    
    private void Start()
    {
        _timer = _intervalSeconds;

        _spikeOut = false;

        StartCoroutine(SpikesInRoutine());
    }

    private void Update()
    {
        
        if(_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            if(_spikeOut)
            {
                _spikeOut = false;

                _timer = _intervalSeconds;

                StartCoroutine(SpikesInRoutine());
            }
            else
            {
                _spikeOut = true;

                _timer = _intervalSeconds;

                StartCoroutine(SpikesOutRoutine());
            }
        }
    }


    private void OnTriggerStay(Collider hitInfo)
    {
        if (!_spikeOut) return;

        if (_isKilling) return;

        if (hitInfo.CompareTag("Player"))
        {
            _isKilling = true;

            StartCoroutine(PlayerKillRoutine(hitInfo.transform));
        }
    }


    private IEnumerator SpikesInRoutine()
    {   
        Vector3 targetPos = _spikes.position + Vector3.down;

        while (Vector3.Distance(_spikes.position, targetPos) > 0.01f)
        {
            _spikes.position = Vector3.MoveTowards(_spikes.position, targetPos, _speed * Time.deltaTime);

            yield return null;
        }
    }

    private IEnumerator SpikesOutRoutine()
    {
        Vector3 targetPos = _spikes.position + Vector3.up;

        while (Vector3.Distance(_spikes.position, targetPos) > 0.01f)
        {
            _spikes.position = Vector3.MoveTowards(_spikes.position, targetPos, _speed * Time.deltaTime);

            yield return null;
        }

    }

    IEnumerator PlayerKillRoutine(Transform playerTransform)
    {
        StartCoroutine(PlayerMovement.Instance.FadeScreenRoutine(1f));

        playerTransform.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
