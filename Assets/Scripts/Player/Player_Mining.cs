using UnityEngine;

public class Player_Mining : MonoBehaviour
{
    Player _Player;

    private void Start()
    {
        _Player = GetComponent<Player>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_Player == null || !_Player.IsOwner) return;
        if(other.transform.parent.TryGetComponent(out Interactor interactor) && interactor.enabled)
        {
            interactor.StartMining(this);
            _Player.PlayerState = Player.EPlayerState.Mine;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_Player == null || !_Player.IsOwner) return;
        if (other.transform.parent.TryGetComponent(out Interactor interactor))
        {
            interactor.StopMining();
        }
    }

    public void OnStopMining()
    {
        _Player.PlayerState = Player.EPlayerState.Idle;

    }
}