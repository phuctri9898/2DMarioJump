using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instante;
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameOverView, pauseView;
    [SerializeField] Text txtScore, txtBestScore;
    [SerializeField] GameObject bullet;

    private float deltaTime = 0;
    public int score = 0;
    public float bulletSpeed = 5f;
    private bool isPause;

    
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

    private void Init()
    {
        player.transform.position = Vector2.zero;
        player.transform.rotation = Quaternion.identity;
        deltaTime = 0;
        score = 0;
        bulletSpeed = 5f;
        Time.timeScale = 1;
    }

    public void Pause()
    {
        isPause = !isPause;
        if (isPause)
        {
            pauseView.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseView.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Replay()
    {
        DestroyBullet();
        Init();
        gameOverView.SetActive(false);
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
        gameOverView.SetActive(true);
        gameOverView.transform.Find("txtScore").GetComponent<Text>().text = "Your Score: " + score;
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

        this.bulletSpeed += Const.SPEED_STEP;
    }

    private void DestroyBullet()
    {
        GameObject[] listBullet = GameObject.FindGameObjectsWithTag(Const.BULLET_TAG);
        for (int i=0; i < listBullet.Length; i++)
        {
            Destroy(listBullet[i]);
        }

    }
}