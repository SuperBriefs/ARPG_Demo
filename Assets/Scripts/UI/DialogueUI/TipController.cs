using UnityEngine;
using UnityEngine.UI;

public class TipController : MonoBehaviour
{
    [SerializeField] private Image interactImg;

    void Start()
    {
        Hide();
    }

    public void Show()
    {
        interactImg.gameObject.SetActive(true);
    }

    public void Hide()
    {
        interactImg.gameObject.SetActive(false);
    }
}
