
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    [Tooltip("Main menu object")]
    [SerializeField] public GameObject mainMenu;

    [Tooltip("Player prefab")]
    [SerializeField] public Player playerPrefab;

    [Tooltip("Wave")]
    [SerializeField] public Wave wave;

    [Tooltip("Multiplier decay in seconds")]
    [SerializeField] public float multiplierDecay = 5f;

    [Tooltip("Multiplier decay acceleration, percent per multiplier value")]
    [SerializeField] public float multiplierDecayAccel = 3f;

    [Tooltip("Left bottom bound of game field")]
    [SerializeField] public Vector3 boundsMin = new Vector3(-15, 0, 0);

    [Tooltip("Right top bound of game field")]
    [SerializeField] public Vector3 boundsMax = new Vector3(15, 0, 20);

    [Tooltip("Z coodrinate at which enemies spawn")]
    [SerializeField] public float spawnZ = 30;

    [Tooltip("Z coodrinate at which enemies despawn")]
    [SerializeField] public float despawnZ = -10;

    // Direction vectors
    static public Vector3 dirLeft = new Vector3(-1, 0, 0);
    static public Vector3 dirRight = new Vector3(1, 0, 0);
    static public Vector3 dirForward = new Vector3(0, 0, 1);
    static public Vector3 dirBack = new Vector3(0, 0, -1);

    int score;
    int hiScore;
    int multiplier = 1;
    float multiplierTime;

    float gameTime;
    bool over = true;
    bool paused;
    Player player;
    Button continueButton;
    HUD hud;

    // Start is called before the first frame update
    void Awake() {
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        continueButton = mainMenu.transform.Find("ContinueButton").GetComponent<Button>();
        continueButton.interactable = false;
        mainMenu.SetActive(true);
        hud.gameObject.SetActive(false);
    }

    public void New()
    {
        Cleanup();

        over = false;
        player = Instantiate<Player>(playerPrefab, Vector3.zero, Quaternion.identity);
        hud.gameObject.SetActive(true);
        hud.SetGameOver(false);
        Continue();
    }

    public void End()
    {
        over = true;
        continueButton.interactable = false;
        if (score > hiScore)
        {
            hiScore = score;
        }
        hud.SetHiScore(hiScore);
        hud.SetNewHiScore(score >= hiScore);
        hud.SetGameOver(true);
    }

    public void Continue()
    {
        paused = false;
        continueButton.interactable = true;
        mainMenu.SetActive(false);
    }

    public void Pause()
    {
        paused = true;
        mainMenu.SetActive(true);
        hud.SetGameOver(false);
    }

    public void Quit()
    {
        Application.Quit(0);
    }

    public bool IsPaused()
    {
        return paused;
    }

    public bool IsOver()
    {
        return over;
    }

    public void AddScore(int sc)
    {
        score += sc * multiplier;
        hud.SetScore(score);
        if (multiplierTime > 0 || multiplier == 1)
        {
            multiplier++;
        }
        RefreshMultiplierDecay();
    }

    void RefreshMultiplierDecay()
    {
        multiplierTime = multiplierDecay * (1f - (multiplier - 1) * (multiplierDecayAccel / 100f));
        hud.SetMultiplier(multiplier);
    }

    void Cleanup()
    {
        score = 0;
        multiplier = 1;
        hud.SetScore(score);
        hud.SetMultiplier(multiplier);
        wave.Reset();
        var projectiles = GameObject.FindObjectsOfType<Projectile>();
        foreach (Projectile p in projectiles)
        {
            Destroy(p.gameObject, 0.01f);
        }
        var enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy e in enemies)
        {
            Destroy(e.gameObject, 0.01f);
        }
        if (player != null)
        {
            Destroy(player.gameObject, 0.01f);
            player = null;
        }
    }


    // Return game time (not affected by pause)
    public float GameTime()
    {
        return gameTime;
    }

    // Returns true if pos is in bounds of game field, otherwise false
    public bool InBounds(Vector3 pos)
    {
        return (pos.x >= boundsMin.x &&
                pos.x <= boundsMax.x &&
                pos.z >= boundsMin.z &&
                pos.z <= boundsMax.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused && !over)
            {
                Continue();
            }
            else
            {
                Pause();
            }
            return;
        }

        if (paused)
        {
            return;
        }
        gameTime += Time.deltaTime;

        if (over)
        {
            return;
        }

        multiplierTime -= Time.deltaTime;
        if (multiplierTime < 0 && multiplier > 1)
        {
            multiplier--;
            RefreshMultiplierDecay();
        }
        if (multiplier == 1)
        {
            multiplierTime = 0;
        }
        hud.SetMultiplierBar(multiplierTime/multiplierDecay);

        if (wave.IsOver())
        {
            wave.Next();
            var nw = hud.transform.Find("NewWave");
            nw.GetComponent<TextMeshProUGUI>().text = "Wave " + wave.number;
            var anim = nw.GetComponent<Animator>();
            anim.Play("Message3s");
        }
    }
}

public class TimeControllerGame: TimeController{

    Game game;

    public TimeControllerGame(Game g)
    {
        game = g;
    }

    public override float DeltaTime()
    {
        if (game.IsPaused())
        {
            return 0;
        }
        return Time.deltaTime;
    }
}
