using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private MeeleFighter fighter;
    [SerializeField] private Transform healthBarPanel;
    [SerializeField] private RectTransform healthBarBack;
    [SerializeField] private RectTransform healthBack;
    [SerializeField] private RectTransform health;
    [SerializeField] private float healthSpeed = 10f;

    private float maxHealth;
    private float nowHealth;
    private float healthProgress = 1;

    void Start()
    {
        maxHealth = fighter.Health;
    }

    void Update()
    {
        nowHealth = fighter.Health;
        healthProgress = Mathf.Clamp01((float)nowHealth / maxHealth);

        // 根据背景宽度设置前景宽度
        float backWidth = healthBarBack.rect.width;
        Vector2 size = health.sizeDelta;
        size.x = backWidth * healthProgress;
        health.sizeDelta = size;

        // 让白血条去追红血条
        if(healthBack.sizeDelta != health.sizeDelta)
        {
            size = healthBack.sizeDelta;
            size.x = Mathf.Lerp(healthBack.sizeDelta.x, health.sizeDelta.x, healthSpeed * Time.deltaTime);
            healthBack.sizeDelta = size;
        }
    }

    public void Show()
    {
        healthBarPanel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        healthBarPanel.gameObject.SetActive(false);
    }
}
