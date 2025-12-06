using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;




public class QuestionManager : MonoBehaviour
{

    [Header("Trivia UI")]
    [SerializeField] private TextMeshProUGUI QuestionUI;
    [SerializeField] private TextMeshProUGUI Answer1UI;
    [SerializeField] private TextMeshProUGUI Answer2UI;
    [SerializeField] private TextMeshProUGUI Answer3UI;
    [SerializeField] private TextMeshProUGUI Answer4UI;
    [SerializeField] private Button Answer1Button;
    [SerializeField] private Button Answer2Button;
    [SerializeField] private Button Answer3Button;
    [SerializeField] private Button Answer4Button;
    [SerializeField] private Canvas TriviaCanvas;

    [Header("Settings")]
    [SerializeField] private InteractableManager Im;
    public TextAsset textFile;
    public int maxQuestions = 10;
    private int currentQuestion = 0;
    private Question[] questions;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime = 60f;


    // Start is called before the first frame update
    void Start()
    {
        //intializes the text boxes for questions and answers
        questions = ExtractQuestins();
        //printQuestions(questions);
        AskQuestion(questions[0]);


        //initializes the button event system
        Answer1Button.onClick.AddListener(delegate { ButtonPressed(1); });
        Answer2Button.onClick.AddListener(delegate { ButtonPressed(2); });
        Answer3Button.onClick.AddListener(delegate { ButtonPressed(3); });
        Answer4Button.onClick.AddListener(delegate { ButtonPressed(4); });
    }

    //update runs a timer 
    private void Update()
    {
        if (remainingTime > 0 && Im.CanPlay())
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0) {
                Im.PlayerLost(); 
                remainingTime = 0;
            }
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"Time Remaining: {minutes:00}:{seconds:00}";
    }

    //this method gets called when a button is pressed. and handles the answer accordingly
    public void ButtonPressed(int n)
    {
        //this is determining whether they gave the right or wrong answer
        if (questions[currentQuestion].getCorrectAnswerIndex() == n)
        {
            //if they did they recieve a resource token and the question is marked correct
            Im.AddResourceTokens(1);
            questions[currentQuestion].correct = 1;
            int maxQuestionCOunter = 0;

            //this finds the next incorrect 
            while (questions[currentQuestion].correct > 0 && maxQuestionCOunter < maxQuestions)
            {
                currentQuestion++;
                if (currentQuestion >= questions.Length) { currentQuestion = 0; }
                maxQuestionCOunter++;
            }

            if (maxQuestionCOunter >= maxQuestions)
            {
                TriviaCanvas.enabled = false;
            }

            AskQuestion(questions[currentQuestion]);
        }
        else
        {
            int maxQuestionCOunter = 0;
            //this forces it to move to the next step in the question pool
            currentQuestion++;
            if (currentQuestion >= questions.Length) { currentQuestion = 0; }

            //this finds the next incorrect question going around hte circular array
            while (questions[currentQuestion].correct > 0 && maxQuestionCOunter < maxQuestions)
            {
                currentQuestion++;
                if (currentQuestion >= questions.Length) { currentQuestion = 0; }
                maxQuestionCOunter++;
            }
            AskQuestion(questions[currentQuestion]);
        }
    }

    public Question[] GetQuestions()
    {
        return questions;
    }

    public Question ExtractQuestionFromLine(String s)
    {
        String[] splitString = s.Split('|');
        if (splitString.Length == 3)
        {
            string[] AnswerChoices = splitString[1].Split(',');
            Question question = new Question(splitString[0], AnswerChoices, int.Parse(splitString[2]));
            return question;
        }
        else { return null; }

    }

    public Question[] ExtractQuestins()
    {
        Question[] tempQuestions = new Question[maxQuestions];
        int counter = 0;

        if (textFile != null)
        {
            string textContent = textFile.text;

            string[] textContentLines = textContent.Split('\n');
            foreach (String i in textContentLines)
            {
                if (i != "")
                {
                    if (counter >= maxQuestions) { break; }
                    tempQuestions[counter] = ExtractQuestionFromLine(i);
                    counter++;
                }
            }
        }
        else
        {
            Debug.Log("Empty FIle");
        }

        Question[] questions = new Question[counter];
        counter = 0;

        foreach (Question q in tempQuestions)
        {
            //print(q.getQuestion()); 
            if (q != null)
            {
                questions[counter] = q;
                counter++;

            }
        }
        maxQuestions = counter;
        return questions;
    }

    public void printQuestions(Question[] questions)
    {
        foreach (Question question in questions)
        {
            string q = question.getQuestion();
            string[] a = question.getAnswerChoices();
            string aCombined = "";
            foreach (String j in a) { aCombined += j; }
            print(q + aCombined);
        }
    }

    public void AskQuestion(Question question)
    {
        QuestionUI.text = question.getQuestion();
        string[] answers = question.getAnswerChoices();
        Answer1UI.text = answers[0];
        Answer2UI.text = answers[1];
        Answer3UI.text = answers[2];
        Answer4UI.text = answers[3];
    }


}


public class Question
{

    private String question = "";
    private String[] AnswerChoices;
    private int CorrectAnswer;
    public int correct = 0; 

    public Question(String question, String[] AnswerChoices, int CorrectAnswer)
    {
        this.question = question;
        this.AnswerChoices = AnswerChoices;
        this.CorrectAnswer = CorrectAnswer;
    }

    public string getQuestion()
    {
        return question;
    }

    public string[] getAnswerChoices()
    {
        return AnswerChoices;
    }

    public int getCorrectAnswerIndex()
    {
        return CorrectAnswer;
    }


}
