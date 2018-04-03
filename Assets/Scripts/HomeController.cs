using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    [SerializeField] private Button btnStart, btnQuit;

    void Start()
    {
        btnStart.onClick.AddListener(() => { SceneManager.LoadScene("GameScene"); });

        btnQuit.onClick.AddListener(() => { Application.Quit(); });
    }

}