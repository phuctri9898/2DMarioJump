using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instante;
    [SerializeField] GameObject player;
    [SerializeField] GameObject pnlGameOver, pnlPause;
    [SerializeField] Text txtScore, txtBestScore;
    [SerializeField] GameObject bullet;

    float deltaTime = 0;
    bool isStart = false;
    public int score = 0;
    public float enemySpeed = 5f;
    bool isPause;

    private void Update()
    {
        txtScore.text = "Score: " + score;
        deltaTime += Time.time;
        txtBestScore.text = "Best Score: " + PlayerPrefs.GetInt(Const.PLAYER_SCORE);
    }

    private void Awake()
    {
        Instante = this;
    }

    void Start()
    {
        InvokeRepeating("AddBullet", 1, 2);
    }

    public void Pause()
    {
        isPause = !isPause;
        if (isPause)
        {
            pnlPause.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pnlPause.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("HomeScene");
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        pnlGameOver.SetActive(true);
        StartCoroutine(PostScore());

        Time.timeScale = 0;
    }

    public IEnumerator PostScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("score", PlayerPrefs.GetInt(Const.PLAYER_SCORE));
        form.AddField("userName","some-user-name");
        using (UnityWebRequest www = UnityWebRequest.Post("/leaderboard", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
                Debug.Log(www.responseCode);
            else
                Debug.Log("Post Success");
        }
    }

    private void AddBullet()
    {
        if (deltaTime >= 0.5 && Random.value > 0.5)
        {
            var newBullet = Object.Instantiate(bullet);
            Vector2 pos = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x * 2,
                -2.5f + (Random.value*3));
            newBullet.transform.position = pos;
        }
    }

    public void JumpOverBullet()
    {
        this.score += Const.BONUS_SCORE;

        if (PlayerPrefs.GetInt(Const.PLAYER_SCORE) <= this.score)
            PlayerPrefs.SetInt(Const.PLAYER_SCORE, this.score);

        this.enemySpeed += Const.SPEED_STEP;
    }
}