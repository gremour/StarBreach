using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] public int minimumValue = 0;
    [SerializeField] public int maximumValue = 100;

    public void SetValue(int v)
    {
        var tf = transform.Find("Fill");
        var sc = tf.localScale;
        var pos = tf.localPosition;
        sc.y = (float) v / (float) (maximumValue - minimumValue);
        pos.x = -(1f - sc.y);
        tf.localScale = sc;
        tf.localPosition = pos;
    }

    public void SetVisible(bool v) {
        transform.Find("Fill").gameObject.SetActive(v);
        transform.Find("Background").gameObject.SetActive(v);
    }
}
