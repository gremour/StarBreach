using UnityEngine;

public class HUD : MonoBehaviour
{
    private float heartWidth;
    private float heartHeight;

    private GameObject playerHull;

    private void Awake() {
        playerHull = transform.Find("PlayerHull").gameObject;
        heartWidth = playerHull.GetComponent<RectTransform>().sizeDelta.x;
        heartHeight = playerHull.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void SetPlayerHull(int hp)
    {
        playerHull.GetComponent<RectTransform>().sizeDelta = new Vector2(heartWidth * hp, heartHeight);
    }
}
