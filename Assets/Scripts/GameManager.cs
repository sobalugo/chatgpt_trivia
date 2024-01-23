using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UIController uiController;
    public APIHandler apiHandler;
    public Timer timer;
    public CategoryManager categoryManager;

    private int score;
    private int incorrectAnswers;
    private string currentCategory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        uiController.ShowCategorySelection();
        uiController.onCategorySelected.AddListener(StartGame);
        timer.OnTimerEnd += HandleTimerEnd;
    }

    public void StartGame(string category)
    {
        apiHandler.ClearAskedQuestions();
        currentCategory = category;
        incorrectAnswers = 0;
        uiController.UpdateIncorrectAnswersDisplay(incorrectAnswers);
        score = 0;
        uiController.UpdateScoreDisplay(score);
        NextQuestion(category);
    }

    public void NextQuestion(string category)
    {
        apiHandler.GenerateQuestion(category, question =>
        {
            uiController.DisplayQuestion(question);
            timer.StartTimer(60); // Set the duration to 60 seconds
        });
        Debug.Log("Next Question is called.");
    }

    public void SubmitAnswer(string answer)
    {
        string question = uiController.questionText.text;
        apiHandler.EvaluateAnswer(question, answer, correct =>
        {
            UpdateScore(correct);
            uiController.DisplayResult(correct);
            timer.StopTimer();
            StartCoroutine(WaitForNextQuestion());
        });
    }

    public void UpdateScore(bool correct)
    {
        if (correct)
        {
            score++;
            uiController.UpdateScoreDisplay(score);
        }
        else
        {
            incorrectAnswers++;
            uiController.UpdateIncorrectAnswersDisplay(incorrectAnswers);
        }

        if (incorrectAnswers >= 3) 
        {
            uiController.DisplayQuestion("Game Over");
            StartCoroutine(WaitForGameOver());
        }
    }

    public void HandleTimerEnd()
    {
        uiController.DisplayQuestion("Game Over");
        StartCoroutine(WaitForGameOver());
    }

    public void EndGame()
    {
        uiController.ShowCategorySelection();
    }

    IEnumerator WaitForNextQuestion()
    {
        yield return new WaitForSeconds(3);
        uiController.ClearAnswerInput();
        NextQuestion(currentCategory);
    }

    IEnumerator WaitForGameOver()
    {
        yield return new WaitForSeconds(3);
        EndGame();
    }
}