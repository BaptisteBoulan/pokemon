using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destinationPortal;

    PlayerController player;

    void IPlayerTriggerable.OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport());
    }

    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Teleport()
    {

        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);

        var desPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);

        player.Character.SetPositionAndSnapToTile(desPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatly => false;
}

public enum DestinationIdentifier
{
    A, B, C, D
}
