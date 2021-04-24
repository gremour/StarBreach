using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    GameObject playerHull;
    GameObject energyBar;
    RectTransform energyTrans;
    GameObject gameOver;
    GameObject score;
    GameObject hiScore;
    GameObject endScore;
    GameObject endHiScore;
    GameObject newHiScore;
    GameObject mult;
    RectTransform multBar;

    Vector2 energyPos;
    Vector2 energyDelta;
    Vector2 heartDelta;
    Vector2 multBarDelta;
    Vector2 multBarPos;

    void Awake() {
        playerHull = transform.Find("PlayerHull").gameObject;
        energyBar = transform.Find("EnergyBar").gameObject;
        gameOver = transform.Find("GameOver").gameObject;
        score = transform.Find("Score").gameObject;
        hiScore = transform.Find("HiScore").gameObject;
        endScore = gameOver.transform.Find("EndScore").gameObject;
        endHiScore = gameOver.transform.Find("EndHiScore").gameObject;
        newHiScore = gameOver.transform.Find("NewHiScore").gameObject;
        mult = transform.Find("Multiplier").gameObject;
        multBar = transform.Find("MultiplierBar").gameObject.GetComponent<RectTransform>();

        heartDelta = playerHull.GetComponent<RectTransform>().sizeDelta;
        energyTrans = energyBar.GetComponent<RectTransform>();
        energyPos = energyTrans.position;
        energyDelta = energyTrans.sizeDelta;
        multBarDelta = multBar.sizeDelta;
        multBarPos = multBar.position;
    }

    public void SetPlayerHull(int hp)
    {
        playerHull.GetComponent<RectTransform>().sizeDelta = new Vector2(heartDelta.x * hp, heartDelta.y);
    }

    public void SetEnergy(float energy)
    {
        energyTrans.position = energyPos;
        energyBar.GetComponent<RectTransform>().sizeDelta = new Vector2(energyDelta.x * energy, energyDelta.y);
    }

    public void SetScore(int sc)
    {
        if (score == null)
        {
            return;
        }
        score.GetComponent<TextMeshProUGUI>().text = sc.ToString();
        endScore.GetComponent<TextMeshProUGUI>().text = sc.ToString();
    }

    public void SetHiScore(int sc)
    {
        hiScore.GetComponent<TextMeshProUGUI>().text = "High score: " + sc.ToString();
        endHiScore.GetComponent<TextMeshProUGUI>().text = sc.ToString();
    }

    public void SetNewHiScore(bool on)
    {
        newHiScore.SetActive(on);
    }

    public void SetMultiplier(int mp)
    {
        if (mult == null)
        {
            return;
        }
        mult.GetComponent<TextMeshProUGUI>().text = "x" + mp.ToString();
    }

    public void SetMultiplierBar(float v)
    {
        if (multBar == null)
        {
            return;
        }
        multBar.position = new Vector2(multBarPos.x + multBarDelta.x * (1f-v), multBarPos.y);
        multBar.sizeDelta = new Vector2(multBarDelta.x * v, multBarDelta.y);
    }

    public void SetGameOver(bool on)
    {
        gameOver.SetActive(on);
    }
}
