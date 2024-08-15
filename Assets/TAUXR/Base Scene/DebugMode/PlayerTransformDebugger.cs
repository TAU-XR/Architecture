using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTransformDebugger : MonoBehaviour
{
    [HideInInspector] public bool DebugPlayerTransform;
    [Range(0.1f, 10)] [SerializeField] private float _movementSpeed;
    [Range(10, 360)] [SerializeField] private float _rotationSpeed;
    private Transform _player;
    private float _horizontalInput = 0;
    private float _forwardInput = 0;
    private float _rotationInput = 0;

    private void Start()
    {
        _player = TXRPlayer.Instance.transform;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _forwardInput = Input.GetAxis("Vertical");
        _rotationInput = Input.GetAxis("Rotation");
    }

    private void FixedUpdate()
    {
        if (!DebugPlayerTransform)
        {
            return;
        }

        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        float horizontalDelta = _horizontalInput * _movementSpeed * Time.deltaTime;
        float forwardDelta = _forwardInput * _movementSpeed * Time.deltaTime;

        _player.position = new Vector3(_player.position.x + horizontalDelta, _player.position.y, _player.position.z + forwardDelta);
    }

    private void RotatePlayer()
    {
        float rotationDelta = _rotationInput * _rotationSpeed * Time.deltaTime;
        _player.rotation = Quaternion.Euler(_player.eulerAngles.x, _player.eulerAngles.y + rotationDelta, _player.eulerAngles.z);
    }
}