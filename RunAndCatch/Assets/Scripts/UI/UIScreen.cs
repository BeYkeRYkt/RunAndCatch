using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public string mId;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        MoveScreenForward();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    private void MoveScreenForward()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetAsLastSibling();
    }
}
