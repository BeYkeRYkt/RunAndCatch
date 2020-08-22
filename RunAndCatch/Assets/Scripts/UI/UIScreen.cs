
using UnityEngine;

public abstract class UIScreen : MonoBehaviour
{
    public string mId;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        MoveScreenForward();
        OnShowScreen();
    }

    public virtual void Hide()
    {
        OnHideScreen();
        gameObject.SetActive(false);
    }

    private void MoveScreenForward()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetAsLastSibling();
    }

    public abstract void Initialize();

    public virtual void OnShowScreen() { }

    public virtual void OnHideScreen() { }
}
