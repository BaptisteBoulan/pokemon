using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destinationPortal;

    PlayerController player;

    void IPlayerTriggerable.OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(SwitchScene());
    }

    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var desPortal = FindObjectsOfType<Portal>().First(x => x!=this && x.destinationPortal == this.destinationPortal);

        player.Character.SetPositionAndSnapToTile(desPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;

    public bool TriggerRepeatly => false;
}