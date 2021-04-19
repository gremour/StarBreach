using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    void Update()
    {
        var enable = gameObject.GetComponent<Button>().interactable;
        transform.Find("Text").gameObject.SetActive(enable);
        transform.Find("DisabledText").gameObject.SetActive(!enable);
    }
}
