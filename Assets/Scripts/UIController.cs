using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public Text questionText;
    public InputField answerInput;
    public Button submitButton;
    public Text scoreText;
    public GameObject categorySelectionPanel;
    public Button categoryButtonPrefab;
    public Transform categoryButtonContainer;
    public Text timerText;
    public Text incorrectAnswersText; // Reference to the Text component for displaying the number of incorrect answers

    public CategoryManager categoryManager;

    [System.Serializable]
    public class CategorySelectedEvent : UnityEvent<string> { }

    public CategorySelectedEvent onCategorySelected;

    private void Start()
    {
        CreateCategoryButtons();
        InitializeAnswerInput();
    }

    public void DisplayQuestion(string question)
    {
        questionText.text = question;
    }

    public void DisplayResult(bool correct)
    {
        string resultText = correct ? "Correct!" : "Incorrect!";
        questionText.text = resultText;
    }

    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void ClearAnswerInput()
    {
        answerInput.text = "";
    }

    public void ShowCategorySelection()
    {
        categorySelectionPanel.SetActive(true);
    }

    public void SubmitAnswer()
    {
        string answer = answerInput.text.Trim();
        if (!string.IsNullOrEmpty(answer))
        {
            GameManager.Instance.SubmitAnswer(answer);
            ClearAnswerInput();
        }
        // Set the focus back to the input field
        answerInput.Select();
        answerInput.ActivateInputField();
    }

    private void CreateCategoryButtons()
    {
        List<CategoryManager.Category> categories = categoryManager.GetCategories();

        foreach (CategoryManager.Category category in categories)
        {
            Button categoryButton = Instantiate(categoryButtonPrefab, categoryButtonContainer);
            categoryButton.GetComponentInChildren<Text>().text = category.name;
            categoryButton.onClick.AddListener(() => onCategorySelected.Invoke(category.prompt));
        }
    }

    public void UpdateTimerDisplay(float timeRemaining)
    {
        timerText.text = Mathf.RoundToInt(timeRemaining).ToString();
    }

    private void InitializeAnswerInput()
    {
        answerInput.onEndEdit.AddListener(delegate { SubmitAnswerOnEnter(); });
    }

    private void SubmitAnswerOnEnter()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitAnswer();
        }
    }

    public void UpdateIncorrectAnswersDisplay(int incorrectAnswers)
    {
        incorrectAnswersText.text = $"Incorrect Answers: {incorrectAnswers}";
    }
}