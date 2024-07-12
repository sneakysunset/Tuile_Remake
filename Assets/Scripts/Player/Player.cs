using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : NetworkBehaviour
{
    public enum EPlayerState { Idle, Walk, Jump, Drawning, Mine};
    private EPlayerState _PlayerState;
    public EPlayerState PlayerState { get => _PlayerState; set => ChangePlayerState(value); }
    public CharacterController Controller { get => _Controller;}
    public Tile TileUnder { get => _TileUnder; set => _TileUnder = value; }
    public Vector2 Input { get => _Input; set => _Input = value; }
    public string GroundType { get => _GroundType; set => _GroundType = value; }

    [SerializeField] private Transform _CameraTarget;
    private CharacterController _Controller;
    private Tile _TileUnder;
    private Player_Movement _PMovement;
    private Player_Mining _PMining;
    private Player_ItemSystem _PItemSystem;
    private Vector2 _Input;
    private Animator _Animator;
    private string _GroundType;

    private void Start()
    {
        if (!IsOwner) return;
        _PMovement = GetComponent<Player_Movement>();
        _Animator = GetComponentInChildren<Animator>();
        _Controller = GetComponent<CharacterController>();
        _PMining = GetComponent<Player_Mining>();
        _PItemSystem = GetComponent<Player_ItemSystem>();
        _CameraTarget.parent = null;
        GroundType = ETileTypes.Rock.ToString();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(!IsOwner)
        {
            GetComponent<Player_Movement>().enabled = false; 
            GetComponent<Player_TileDetection>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<Player_Mining>().enabled = false;
            GetComponent<Player_ItemSystem>().enabled = false;
            enabled = false;
            return;
        }
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
        if(IsOwner)
            _CameraTarget.position = transform.position;
    }

    private void OnPlayerStateEnter(EPlayerState oldPlayerState)
    {
        _Animator.Play(_PlayerState.ToString());
        _Animator.SetFloat("RdModifier", Random.Range(0f, 1f));
        switch (_PlayerState)
        {
            case EPlayerState.Jump:
                _PMovement.ApplyJump();
                break;
            case EPlayerState.Drawning:
                _PMovement.YVelocity = -.5f;
                StartCoroutine(_PMovement.DrawningEnum());
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
        _PMovement.Direction = new Vector3(Input.x, 0.0f, Input.y);
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
        if (TileUnder == null) return;
        Tile hitTile = hit.transform.GetComponentInParent<Tile>();

        if (_PMovement.AutoJump && _Controller.isGrounded && hit.normal.y > -0.2f && hit.normal.y < 0.2f  && hitTile.transform.position.y - TileUnder.transform.position.y <= 1 && hitTile.transform.position.y - TileUnder.transform.position.y > .3f && _PlayerState != EPlayerState.Jump)
        {
            PlayerState = EPlayerState.Jump;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            PlayerState = EPlayerState.Drawning;
        }
    }
}
