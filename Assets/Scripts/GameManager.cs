using System;
using TMPro;
using UnityEngine;

[Serializable]
public enum GameState
{
    Attacking,
    Spawning,
    Waiting
}

public class GameManager : MonoBehaviour
{
    public GameState CurrentGameState;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("Events")]
    [SerializeField] private GameEventInt eventHealth;
    [SerializeField] private GameEventInt eventMoney;
    [SerializeField] private GameEvent eventNewGame;
    [SerializeField] private GameEvent eventLose;

    [Header("Visualize / Debug")]
    [SerializeField] private int health;
    [SerializeField] private int money;

    // event subscriptions
    private void OnEnable()
    {
        eventHealth.RegisterListener(AddToHealth);
        eventMoney.RegisterListener(AddToMoney);
        eventNewGame.RegisterListener(OnNewGame);
    }
    
    private void OnDisable()
    {
        eventHealth.UnregisterListener(AddToHealth);
        eventMoney.UnregisterListener(AddToMoney);
        eventNewGame.UnregisterListener(OnNewGame);
    }

    // reset values on new game start
    void OnNewGame()
    {
        health = 5;
        money = 120;
        UpdateUI();
        CurrentGameState = GameState.Waiting;
    }

    public int GetHealth() => health;
    public int GetMoney() => money;
    
    // update value and UI; check for game over
    public void AddToHealth(int value)
    {
        health += value;
        UpdateUI();
        
        if(health<=0) eventLose.Raise();
    }
    
    // update value and UI
    public void AddToMoney(int value)
    {
        money += value;
        UpdateUI();
    }
    
    void UpdateUI()
    {
        healthText.text = health.ToString();
        moneyText.text = money.ToString();
    }
}