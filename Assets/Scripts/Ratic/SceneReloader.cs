using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ratic
{
    public class SceneReloader : MonoBehaviour
    {
        [SerializeField] private int _sceneIndex;

        private void Awake() => GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(_sceneIndex));
    }
}