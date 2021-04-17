using UnityEngine;

public class Game : MonoBehaviour
{
    [Tooltip("Left bottom bound of game field")]
    [SerializeField] public Vector3 boundsMin = new Vector3(-15, 0, 0);

    [Tooltip("Right top bound of game field")]
    [SerializeField] public Vector3 boundsMax = new Vector3(15, 0, 20);

    public static Game instance;

    // Start is called before the first frame update
    void Awake() {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
