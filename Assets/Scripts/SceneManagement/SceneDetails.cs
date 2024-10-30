using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; private set; }
    List<SavableEntity> savables;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag is "Player")
        {
            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            foreach (var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            // Unload the others
            var prevScene = GameController.Instance.PrevScene;
            if (prevScene != null)
            {
                var previouslyLoadedScenes = GameController.Instance.PrevScene.connectedScenes;
                foreach (var scene in  previouslyLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
                if (!connectedScenes.Contains(prevScene)) prevScene.UnloadScene();
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                savables = GetSavablesEntities();
                SavingSystem.i.RestoreEntityStates(savables);
            };
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savables);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    public List<SavableEntity> GetSavablesEntities()
    {

        var currentScene = SceneManager.GetSceneByName(gameObject.name);

        var savables = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();

        return savables;
    }
}
