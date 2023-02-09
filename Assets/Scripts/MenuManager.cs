using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject HowToPlayPanel;
    [SerializeField] private GameObject CreditsPanel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private GameObject PausedText;
    [SerializeField] private GameObject BackToGameButton;
    
    [Header("Events")]
    [SerializeField] private GameEvent eventNewGame;
    [SerializeField] private GameEvent eventWin;
    [SerializeField] private GameEvent eventLose;
    
    private GameObject openMenu;
    private bool menuOpen;
    private bool paused;
    private bool gameStarted;

    // event subscriptions
    private void OnEnable()
    {
        eventWin.RegisterListener(OnGameWon);
        eventLose.RegisterListener(OnGameLose);
    }

    private void OnDisable()
    {
        eventWin.UnregisterListener(OnGameWon);
        eventLose.UnregisterListener(OnGameLose);
    }

    // show the win screen
    void OnGameWon()
    {
        WinPanel.SetActive(true);
        OnGameOver();
    }

    // show the lose screen
    void OnGameLose()
    {
        LosePanel.SetActive(true);
        OnGameOver();
    }

    // pause the game and reset the gamestarted bool
    void OnGameOver()
    {
        Time.timeScale = 0f;
        gameStarted = false;
    }

    void Start()
    {
        // ensure the main menu is set active to start
        SetMainMenuActive();
    }

    void Update()
    {
        // escape for menu open/closed, P for pause on/off
        if(Input.GetKeyDown(KeyCode.Escape)) ToggleMenu();
        if(Input.GetKeyDown(KeyCode.P)) TogglePause();
    }

    public void ToggleMenu()
    {
        // if a menu is open and its not the main menu
        if (menuOpen && openMenu != MainMenuPanel)
        {
            // close the nested menus first
            if(openMenu==CreditsPanel) ClickCloseCredits();
            if(openMenu==HowToPlayPanel) ClickCloseHowToPlay();
        }
        else
        {
            // escape gets eaten if gamestarted is false and we're on the main menu
            if (!gameStarted) return;
            
            // toggle the main menu and pause status
            menuOpen = !menuOpen;
            MainMenuPanel.gameObject.SetActive(menuOpen);
            openMenu = menuOpen ? MainMenuPanel : null;
            TogglePause();
        }
    }

    void TogglePause()
    {
        // if the menu is open, we should be paused; otherwise, toggle pause on/off
        if (!paused || menuOpen)
        {
            Time.timeScale = 0f;
            paused = true;
            PausedText.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            paused = false;
            PausedText.SetActive(false);
        }
    }

    void SetMainMenuActive()
    {
        menuOpen = true;
        paused = true;
        openMenu = MainMenuPanel;
        MainMenuPanel.SetActive(true);
    }

    public void ClickNewGame()
    {
        // hide the win/lose panels
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        // enable the back to game button
        BackToGameButton.SetActive(true);

        // make sure the main menu is active so we can hide it properly, then start a new game
        SetMainMenuActive();
        gameStarted = true;
        eventNewGame.Raise();
        ToggleMenu();
    }
    
    public void ClickQuit() => Application.Quit();
    
    public void ClickOpenCredits()
    {
        openMenu = CreditsPanel;
        CreditsPanel.gameObject.SetActive(true);
    }

    private void ClickCloseCredits()
    {
        openMenu = MainMenuPanel;
        CreditsPanel.gameObject.SetActive(false);
    }

    public void ClickOpenHowToPlay()
    {
        openMenu = HowToPlayPanel;
        HowToPlayPanel.gameObject.SetActive(true);
    }

    private void ClickCloseHowToPlay()
    {
        openMenu = MainMenuPanel;
        HowToPlayPanel.gameObject.SetActive(false);
    }
}
