using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthManager :
    MonoSingleton<HealthManager>
{
    [Header("체력 설정")]
    [SerializeField]
    private int maxHealth = 5;


    private int currentHealth;


    [Header("별 UI")]
    [SerializeField]
    private Image[] stars;


    [Header("별 스프라이트")]
    [SerializeField]
    private Sprite filledStarSprite;

    [SerializeField]
    private Sprite emptyStarSprite;


    [Header("배달 실패 이벤트")]
    [SerializeField]
    private UnityEvent onGameOver;


    private bool isGameOver = false;


    private void Start()
    {
        ResetHealth();
    }


    // =====================================
    // 피해
    // =====================================

    public void TakeDamage(
        int damage = 1)
    {
        if (isGameOver)
            return;


        if (damage <= 0)
            return;


        currentHealth -= damage;


        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth
            );


        UpdateHealthUI();


        Debug.Log(
            $"피해 받음 / 현재 별 : " +
            $"{currentHealth}"
        );


        if (currentHealth <= 0)
        {
            GameOver();
        }
    }


    // =====================================
    // 회복
    // =====================================

    public void Heal(
        int amount = 1)
    {
        if (isGameOver)
            return;


        if (amount <= 0)
            return;


        currentHealth += amount;


        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth
            );


        UpdateHealthUI();
    }


    // =====================================
    // 새로운 미션 시작 시 초기화
    // =====================================

    public void ResetHealth()
    {
        currentHealth =
            maxHealth;


        isGameOver =
            false;


        UpdateHealthUI();
    }


    // =====================================
    // UI
    // =====================================

    private void UpdateHealthUI()
    {
        for (int i = 0;
             i < stars.Length;
             i++)
        {
            if (stars[i] == null)
                continue;


            if (i < currentHealth)
            {
                stars[i].sprite =
                    filledStarSprite;
            }
            else
            {
                stars[i].sprite =
                    emptyStarSprite;
            }
        }
    }


    // =====================================
    // 별 0개
    // =====================================

    private void GameOver()
    {
        if (isGameOver)
            return;


        isGameOver =
            true;


        Debug.Log(
            "별점 0개 - 배달 실패"
        );


        onGameOver?.Invoke();
    }


    // =====================================
    // 외부에서 확인
    // =====================================

    public int GetCurrentHealth()
    {
        return currentHealth;
    }


    public int GetMaxHealth()
    {
        return maxHealth;
    }


    public bool IsGameOver()
    {
        return isGameOver;
    }
}