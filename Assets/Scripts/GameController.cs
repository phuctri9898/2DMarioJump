using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instante;
    [SerializeField] GameObject bg1, bg2;
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameOverView, pauseView;
    [SerializeField] Text txtScore, txtBestScore;
    [SerializeField] GameObject bullet;

    private float deltaTime = 0;
    public int score = 0;
    public float bulletSpeed = 5f;
    public bool isPause;

    
    private void Update()
    {
        txtScore.text = "Score: " + score;
        deltaTime += Time.time;
        txtBestScore.text = "Best Score: " + PlayerPrefs.GetInt(Const.PLAYER_SCORE);
        if (!isPause)
        {
            float camX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x;

            var pos1 = bg1.transform.position;
            pos1.x -= 0.05f;
            if (pos1.x < -2 * camX)
            {
                pos1.x = 2 * camX + 1 + Random.value * 0.5f;
                pos1.y = -4.28f + Random.value * 2f;
            }

            bg1.transform.position = pos1;

            var pos2 = bg2.transform.position;
            pos2.x -= 0.05f;
            if (pos2.x < -2 * camX)
            {
                pos2.x = 2 * camX + 1 + Random.value * 0.5f;
                pos2.y = -4.28f + Random.value * 2f;
            }

            bg2.transform.position = pos2;
        }
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
        isPause = true;
        gameOverView.SetActive(true);
        gameOverView.transform.Find("txtScore").GetComponent<Text>().text = "Your Score: " + score;
        StartCoroutine(PostScore());

        Time.timeScale = 0;
    }

    public IEnumerator PostScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("userName", "some-user-name");
        form.AddField("score", PlayerPrefs.GetInt(Const.PLAYER_SCORE));
        using (UnityWebRequest www = UnityWebRequest.Post(Const.LEADERBOARD_API, form))
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
        if (this.bulletSpeed > Const.MAX_SPEED)
        {
            this.bulletSpeed = Const.MAX_SPEED;
        }
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