using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleSceneLoader : MonoBehaviour
{
    [SerializeField, SceneDetails]
    private SerializedScene scene;

    private void Start()
    {
        SceneManager.LoadScene(scene.BuildIndex);
    }
}