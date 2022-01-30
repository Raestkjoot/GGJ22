using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformLeft : MonoBehaviour
{
    void Start()
    {
        _initialPosition = transform.position;
        _finalPosition = new Vector3(transform.position.x - 4.0f, transform.position.y, transform.position.z);
    }

    void Update()
    {
        _moveTimer = Mathf.Min(_moveTimer + Time.deltaTime, _moveMaxTime);
        float progression = _moveTimer / _moveMaxTime;

        if (_goingLeft)
        {
            transform.position = Vector3.Lerp(_initialPosition, _finalPosition, progression);
        }
        else
        {
            transform.position = Vector3.Lerp(_finalPosition, _initialPosition, progression);
        }

        if (_moveTimer == _moveMaxTime)
        {
            _moveTimer = 0.0f;
            _goingLeft = !_goingLeft;
        }
    }

    private Vector3 _initialPosition = Vector3.zero;
    private Vector3 _finalPosition = Vector3.zero;
    private float _moveMaxTime = 1.0f;
    private float _moveTimer = 0.0f;
    [SerializeField] private bool _goingLeft = true;
}
