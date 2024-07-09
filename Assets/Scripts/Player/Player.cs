using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    public enum EPlayerState { Idle, Walk, Jump};
    private EPlayerState _PlayerState;
    public EPlayerState PlayerState { get => _PlayerState; set => ChangePlayerState(value); }
    public CharacterController CharacterController { get => CharacterController;}
    public Tile TileUnder { get => _TileUnder; set => _TileUnder = value; }
    public Vector2 Input { get => _Input; set => _Input = value; }
    public string GroundType { get => _GroundType; set => _GroundType = value; }

    [SerializeField] private Transform _CameraTarget;
    private CharacterController _CharacterController;
    private Tile _TileUnder;
    private PlayerMovement _PM;
    private Vector2 _Input;
    private Animator _Animator;
    private string _GroundType;

    private void Start()
    {
        _PM = GetComponent<PlayerMovement>();
        _Animator = GetComponentInChildren<Animator>();
        _CharacterController = GetComponent<CharacterController>();
        _CameraTarget.parent = null;
        GroundType = ETileTypes.Rock.ToString();
    }

    private void ChangePlayerState(EPlayerState newPlayerState)
    {
        OnPlayerStateExit(newPlayerState);
        EPlayerState oldPlayerState = _PlayerState;
        _PlayerState = newPlayerState;
        OnPlayerStateEnter(oldPlayerState);
    }
    private void Update()
    {
        _CameraTarget.position = transform.position;
    }

    private void OnPlayerStateEnter(EPlayerState oldPlayerState)
    {
        _Animator.Play(_PlayerState.ToString());
        switch (_PlayerState)
        {
            case EPlayerState.Jump:
                _Animator.SetFloat("JumpRand", Random.Range(0f, 1f));
                _PM.ApplyJump();
                break;
        }
    }

    private void OnPlayerStateExit(EPlayerState newPlayerState)
    {

    }

    public void OnJump()
    {
        PlayerState = EPlayerState.Jump;
    }

    public void OnMove(InputValue inputValue)
    {
        Input = inputValue.Get<Vector2>();
        float cameraAngle = -Camera.main.transform.rotation.eulerAngles.y;
        Input = Rotate(Input, cameraAngle);
        _PM.Direction = new Vector3(Input.x, 0.0f, Input.y);
        _Animator.SetFloat("walkingSpeed", Input.magnitude);
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public void FootSound()
    {

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.transform.GetComponentInParent<Tile>()) { return; }
        Tile hitTile = hit.transform.GetComponentInParent<Tile>();
        if (_PM.AutoJump && _CharacterController.isGrounded && hit.normal.y > -0.2f && hit.normal.y < 0.2f  && hitTile.transform.position.y - TileUnder.transform.position.y <= 1 && hitTile.transform.position.y - TileUnder.transform.position.y > .3f && _PlayerState != EPlayerState.Jump)
        {
            PlayerState = EPlayerState.Jump;
        }
    }
}
