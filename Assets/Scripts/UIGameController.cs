using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGameController : MonoBehaviour
{
    public GameObject settingPanel;
    public GameObject statPanel;
    public GameObject gameOverPanel;
    public Text timerText;
    private float gameTime = 60f; // Total game time in seconds
    private float currentTime;
    private Coroutine timerCoroutine; // Coroutine reference for the timer 

    private int wrongGuessCount = 0;
    private int correctGuessCount = 0; // Track the number of correct guesses
    private bool wordCompleted = false; // Flag to indicate if the current word is completed

    public Text wordDisplayText;

    private int totalCorrectGuesses = 0;
    public Text totalGuessedWord; // Reference to the "Total Guessed" Text in the StatPanel
    public Text totalScoreText; // Reference to the "Total Score" Text in the StatPanel
    public Text totalWrongGuessesText; // Reference to the "Total Guessed Wrong" Text in the StatPanel
    public Text finalScoreText; // Reference to the "Final Score" Text in the GameOverPanel
    public Text finalWrongGuessesText; // Reference to the "Total Wrong Guesses" Text in the GameOverPanel

    private int totalScore = 0; // Track the total score
    private int totalWrongGuesses = 0; // Track the total number of incorrect guesses

    public Button[] letterButtons;
    public string[] easyWords;
    public string[] mediumWords;
    private string currentWord;
    public GameObject[] leafs; // Reference to the leaf panels
    public HandleSound Leafsound;
    private AudioManager audioManager; // Reference to the AudioManager

    private void Start()
    {
        // Initialize current time
        currentTime = gameTime;
        // Update the timer text initially
        UpdateTimerText();
        // Start the timer coroutine after a delay
        StartCoroutine(StartTimerWithDelay());

        // Set up the letter buttons with click events
        SetupLetterButtons();
        // Choose a random word to guess
        SetRandomWord();
        // Display hint for the current word
        DisplayHintForWord();

        // Initialize total guessed text
        totalGuessedWord.text = "Total Guessed: 0";
        totalScoreText.text = "Total Score: 0";
        totalWrongGuessesText.text = "Total Guessed Wrong: 0";

        // Initialize total guess text in gameover panel
        finalScoreText.text = "Final Score: 0";
        finalWrongGuessesText.text = "Total Wrong Guesses: 0";

        // Find the AudioManager in the scene
        audioManager = FindObjectOfType<AudioManager>();
    }

    IEnumerator StartTimerWithDelay()
    {
        // Display "1:00" for a brief moment
        UpdateTimerText();
        yield return new WaitForSeconds(2f);

        // Start the timer coroutine
        timerCoroutine = StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (currentTime > 0)
        {
            // Decrement current time
            currentTime -= Time.deltaTime;
            // Update the timer text
            UpdateTimerText();
            // Wait for the next frame
            yield return null;
        }
        ShowGameOverPanel();
    }

    void UpdateTimerText()
    {
        if (currentTime <= 0)
        {
            // Reset the timer to 0:00
            currentTime = 0;
        }
        else
        {
            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // Format the timer text as "mm:ss"
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ShowGameOverPanel()
    {
        // Display the Game Over panel
        gameOverPanel.SetActive(true);
        // Display the final score and total wrong guesses
        finalScoreText.text = "Final Score: " + totalScore;
        finalWrongGuessesText.text = "Total Wrong Guesses: " + totalWrongGuesses;

        // Play the gameover sound and stop background music
        if (audioManager != null)
        {
            audioManager.PlayGameoverSound();
        }
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRestartButtonClicked()
    {
        // Reload the current scene
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void OnSettingButtonClicked()
    {
        settingPanel.SetActive(true);
        PauseTimer();
    }

    public void OnStatButtonClicked()
    {
        statPanel.SetActive(true);
        PauseTimer();
    }

    public void ResetGameStatistics()
    {
        // Clear total score and total wrong guesses
        totalScore = 0;
        totalWrongGuesses = 0;
        // Update the total score and total wrong guesses text in the stat panel
        totalScoreText.text = "Total Score: " + totalScore;
        totalWrongGuessesText.text = "Total Guessed Wrong: " + totalWrongGuesses;
    }
public void OnGameoverRestartButton()
{

    // Stop the game over sound
    if (audioManager != null)
    {
        audioManager.StopGameoverSound();
        audioManager.PlayBackgroundMusic(); // Resume background music
    }

    // Reload the current scene
    gameOverPanel.SetActive(false);
    // Reset game statistics
    ResetGameStatistics();
    // Clear correct guesses count
    totalCorrectGuesses = 0;
    // Restart the game
    Scene scene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(scene.name);
}



    public void ExitStatPanel()
    {
        statPanel.SetActive(false);
        ResumeTimer();
    }

    public void OnBackToGameButtonClicked()
    {
        settingPanel.SetActive(false);
        ResumeTimer();
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void PauseTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    void ResumeTimer()
    {
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(StartTimer());
        }
    }

    void SetupLetterButtons()
    {
        foreach (Button button in letterButtons)
        {
            button.onClick.AddListener(() =>
            {
                string letter = button.GetComponentInChildren<Text>().text;
                OnLetterButtonClicked(letter);
            });
        }
    }

    void OnLetterButtonClicked(string letter)
    {
        if (wordCompleted)
        {
            return;
        }

        // Convert both to the same case to avoid case sensitivity issues
        letter = letter.Trim().ToUpper();
        string upperCurrentWord = currentWord.Trim().ToUpper();

        // Check if the guessed letter is in the current word
        if (upperCurrentWord.Contains(letter))
        {
            // Display the guessed letter in the word display text
            DisplayGuessedLetter(letter);
        }
        else
        {
            // Increment the wrong guess count
            wrongGuessCount++;
            totalWrongGuesses++;

            // Update the total wrong guesses text in the stat panel
            totalWrongGuessesText.text = "Total Guessed Wrong: " + totalWrongGuesses;
            Debug.Log("Wrong guess: " + letter + " | Wrong guess count: " + wrongGuessCount);

            // Hide a leaf panel and play sound effect
            if (wrongGuessCount <= leafs.Length)
            {
                leafs[wrongGuessCount - 1].SetActive(false);
                Leafsound.playSound();
            }

            // Check if all leaves are lost
            if (wrongGuessCount == leafs.Length)
            {
                ShowGameOverPanel();
            }
        }
    }


    void DisplayHintForWord()
    {
        char[] hintWordChars = new char[currentWord.Length * 2 - 1]; // Adjust the length to accommodate spaces
        for (int i = 0; i < currentWord.Length; i++)
        {
            hintWordChars[i * 2] = '_'; // Display underscore for each letter
            if (i < currentWord.Length - 1)
            {
                hintWordChars[i * 2 + 1] = ' '; // Add a space after each underscore (except for the last one)
            }
        }

        // Reveal at least 1 or 2 random letters as a hint
        int hintsToShow = Random.Range(1, 3);
        for (int i = 0; i < hintsToShow; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, currentWord.Length);
            } while (hintWordChars[randomIndex * 2] != '_'); // Ensure the same letter is not chosen multiple times

            hintWordChars[randomIndex * 2] = currentWord[randomIndex];
        }

        wordDisplayText.text = new string(hintWordChars);
        Debug.Log("Initial word display: " + wordDisplayText.text);
    }

     void SetRandomWord()
    {
        string[] wordList;
        if (Random.value < 0.5f)
        {
            wordList = easyWords;
        }
        else
        {
            wordList = mediumWords;
        }
        currentWord = wordList[Random.Range(0, wordList.Length)];

        // Debug the chosen word
        Debug.Log("Word to guess: " + currentWord);
    }

    bool IsTimerPaused()
    {
        return timerCoroutine == null;
    }
void DisplayGuessedLetter(string letter)
{
    char[] displayWordChars = wordDisplayText.text.ToCharArray();
    bool letterGuessed = false; // Flag to track if the letter was guessed

    string upperCurrentWord = currentWord.Trim().ToUpper();
    letter = letter.Trim().ToUpper();

    for (int i = 0; i < upperCurrentWord.Length; i++)
    {
        if (upperCurrentWord[i].ToString() == letter)
        {
            displayWordChars[i * 2] = letter[0];
            letterGuessed = true;
            correctGuessCount++; // Increment correct guess count
        }
    }

    // Update the word display text
    wordDisplayText.text = new string(displayWordChars);
    Debug.Log("Updated word display: " + wordDisplayText.text);

    Debug.Log("Correct Guess Count: " + correctGuessCount);
    Debug.Log("Word Length: " + currentWord.Length);

    // Check if all letters are guessed correctly
    if (AllLettersGuessedCorrectly())
    {
        wordCompleted = true;
        Debug.Log("Word completed: " + currentWord);
        totalCorrectGuesses++;
        // Increment the total score (assuming 1 point per correct word)
        totalScore += 1; // Adjust scoring logic as needed
        // Update the total score text in the stat panel
        totalScoreText.text = "Total Score: " + totalScore;

        // Delay moving to the next word
        StartCoroutine(DelayBeforeNextWord());
    }
}

bool AllLettersGuessedCorrectly()
{
    return wordDisplayText.text.Replace(" ", "") == currentWord;
}


IEnumerator DelayBeforeNextWord()
{
    yield return new WaitForSeconds(2f); // 2 second delay
    // Reset the word completion flag and counts
    wordCompleted = false;
    correctGuessCount = 0; // Reset correct guess count
    wrongGuessCount = 0;

    SetRandomWord();
    // Display hint for the new word
    DisplayHintForWord();
}

}

