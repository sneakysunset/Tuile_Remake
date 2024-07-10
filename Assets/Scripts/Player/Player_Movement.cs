using System.Collections.Generic;
using UnityEngine;
using System;
using TriInspector;
using System.Collections;

[DeclareTabGroup("Movement")]

public class PlayerMovement : MonoBehaviour
{

    [SerializeField, Group("Movement"), Tab("Jump")] public float _JumpStrength = 3.0f;
    private float _GravityMult;
    [SerializeField, Group("Movement"), Tab("Jump")] public float _GravityMultiplier ;
    [SerializeField, Group("Movement"), Tab("Jump")] public float _GravityMultiplierInWater ;
    [SerializeField, Group("Movement"), Tab("Jump")] private bool autoJump = true;
    [SerializeField, Group("Movement"), Tab("Rotate")] private float _SmoothTime = 0.05f;
    [SerializeField, Group("Movement"), Tab("Move")] private float _SpeedValue;
    [SerializeField, Group("Movement"), Tab("Drawning")] private float _DrawningDuration;

    private Vector3 _Direction;
    private float _yVelocity;
    private const float _gravity = -9.81f;
    private CharacterController _Controller;
    private Player _Player;
    public Vector3 Direction { get => _Direction; set => _Direction = value; }
    public bool AutoJump { get => autoJump; set => autoJump = value; }
    public float GravityMult { get => _GravityMult; set => _GravityMult = value; }
    public float YVelocity { get => _yVelocity; set => _yVelocity = value; }

    private void Start()
    {
        _Controller = GetComponent<CharacterController>();
        _Player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!_Player.IsOwner)
            return;
        OnPlayerStateUpdate();
        if(_Controller.isGrounded && _Player.PlayerState == Player.EPlayerState.Jump)
        {
            _Player.PlayerState = Player.EPlayerState.Idle;
        }
    }

    private void OnPlayerStateUpdate()
    {
        List<Action> actions = new List<Action>()
        {
            ApplyGravity,
            ApplyRotation,
            ApplyMovement
        };

        switch (_Player.PlayerState)
        {
            case Player.EPlayerState.Idle:
                if (_Direction.x != 0 || _Direction.z != 0) _Player.PlayerState = Player.EPlayerState.Walk;
                actions.Remove(ApplyRotation);
                //actions.Remove(ApplyMovement);
                break;
            case Player.EPlayerState.Walk:
                if (_Direction.x == 0 && _Direction.z == 0) _Player.PlayerState = Player.EPlayerState.Idle;
                break;
            case Player.EPlayerState.Jump:
                break;
            default: break;
        }
        foreach (var action in actions) action.Invoke();
    }


    public void ApplyGravity()
    {
        if(_Player.PlayerState == Player.EPlayerState.Drawning)
        {
            YVelocity += _gravity * _GravityMultiplierInWater * Time.deltaTime;
        }
        else if (_Controller.isGrounded && YVelocity < 0.0f)
        {
            YVelocity = -1.0f;
        }
        else
        {
            YVelocity += _gravity * _GravityMultiplier * Time.deltaTime;
        }
        

        _Direction.y = YVelocity;
    }
    private float _CurrentVelocity;

    private void ApplyRotation()
    {
        if (Direction.x == 0 && Direction.z == 0)
        {
            return;
        }
        var targetAngle = Mathf.Atan2(_Direction.x, _Direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _CurrentVelocity, _SmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    private void ApplyMovement()
    {
        _Controller.Move(_Direction * _SpeedValue * Time.deltaTime);
    }

    public void ApplyJump()
    {
        YVelocity = _JumpStrength;
    }

    private void SpeedModifier()
    {

    }


    public IEnumerator DrawningEnum()
    {
        yield return new WaitForSeconds(_DrawningDuration);
        transform.position = new Vector3(0, 5, 0);
        _Player.PlayerState = Player.EPlayerState.Idle;
    }
}
