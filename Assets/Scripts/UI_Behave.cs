using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class UI_Behave : MonoBehaviour
{

    //This will be scuffed, sorry

    public Button NewGameButton,StartButton,GameModeButton1, GameModeButton2, GameModeButton3, QuitButton;
    public Dropdown Category, Difficulty;
    public InputField QuestionCountField;

    public Button True, False, Submit, Answer1, Answer2, Answer3, Answer4;
    public InputField Question, Answer;

    public Text GM, CAT, Count, Diff, Star, Que, Ans, Score, Timer, QuestionsLeft, FinalScore;
    //public List<Text> TextList = new List<Text>();

    public string TokenDebug;
    public List<category> catDebug;
    private List<string> cats;
    //private WWWForm ParamForm = new WWWForm();
    TokenClass myToken = new TokenClass();
    Categories AllCategories = new Categories();
    questionData questionsDataList = new questionData();
    WWWForm ParamForm;

    

    private int diffNumber = 0;
    private string diffText = "";
    private int roundCount = 10;
    private int categoryPH = 0;
    private string typePH = "multiple";
    private bool timerIsRunning = false;
    private float timeRemaining = 60;
    //private bool typeSwitch = true;
    int SCORE= 0;

    private int CurrentRound = 0;
    private int QueRemain = 0;
    public Text[] butts = new Text[4];

    public QuitGame q;

    //https://opentdb.com/api.php?amount=7&category=9&difficulty=medium&type=multiple
    private void Awake()
    {   //Webfomr to contain game parameters later, amount of games, difficulty, selected category.
        ParamForm = new WWWForm();

        /*
        butts[0] = Answer1.GetComponent<Text>();
        //Answer1.GetComponent<Text>().text = "test";
        butts[1] = Answer2.GetComponent<Text>();
        butts[2] = Answer3.GetComponent<Text>();
        butts[3] = Answer4.GetComponent<Text>();
        */
        butts[0].text = "test";
    }
    void Start()
    {
        Category.ClearOptions(); //Clear defaults from dropdown 
        catDebug = new List<category>();
        cats = new List<string>();
        SetCategory();
        ///StartCoroutine(GetDifficulty(diffBug));
        //StartCoroutine(GetCategory(categoryStorage));

        //Difficulty.ClearOptions();
        NewGameButton.onClick.AddListener(NewGame);
        StartButton.onClick.AddListener(StartGame);
        StartButton.onClick.AddListener(q.QuitTheGame);
        GameModeButton1.onClick.AddListener(delegate { gameType(0); }); 
        GameModeButton2.onClick.AddListener(delegate { gameType(1); });
        GameModeButton3.onClick.AddListener(delegate { gameType(3); });

        QuestionCountField.onValueChanged.AddListener(delegate { setCount(int.Parse(QuestionCountField.text)); });
        //{ setCategoryForm(Category.itemText.text); });
        Category.onValueChanged.AddListener(delegate { setCategoryForm(Category.value); }); //Listener will pass index selected item
        Difficulty.onValueChanged.AddListener(delegate { setDifficultyForm(Difficulty.value); });

        True.onClick.AddListener(delegate { ReplyButtonFunc(0); });
        False.onClick.AddListener(delegate { ReplyButtonFunc(1); });

        Answer1.onClick.AddListener(delegate { ReplyButtonFunc(0); });
        Answer2.onClick.AddListener(delegate { ReplyButtonFunc(1); });
        Answer3.onClick.AddListener(delegate { ReplyButtonFunc(2); });
        Answer4.onClick.AddListener(delegate { ReplyButtonFunc(3); });

        StartButton.gameObject.SetActive(false);
        GameModeButton1.gameObject.SetActive(false);
        GameModeButton2.gameObject.SetActive(false);
        GameModeButton3.gameObject.SetActive(false);

        Category.gameObject.SetActive(false);
        Difficulty.gameObject.SetActive(false);
        QuestionCountField.gameObject.SetActive(false);

        SetButtons(false); 

        GM.enabled = false;
        CAT.enabled = false;
        Count.enabled = false;
        Diff.enabled = false;
        Star.enabled = false;
        Que.enabled = false;
        Ans.enabled = false;
    }
    private void FixedUpdate()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;

                SetButtons(false);
            }
            Timer.text = "Time remaining: " + timeRemaining;
        }
    }

    void NewGame()
    {
        Debug.Log("New Game!");
        StartCoroutine(GetToken(tokenStorage)); //Coroutine for IEnumerator
        SCORE = 0;
        GM.enabled = true;
        GameModeButton1.gameObject.SetActive(true);
        GameModeButton2.gameObject.SetActive(true);
        GameModeButton3.gameObject.SetActive(true);
    }
    public void tokenStorage(string fromweb)
    {
        myToken = JsonUtility.FromJson<TokenClass>(fromweb); //Parse JSON into object
        TokenDebug = myToken.token; //take string from JSON object
    }
    void SetCategory()
    {
        StartCoroutine(GetCategory(categoryStorage)); 
    }
    public void categoryStorage(string fromweb)
    {
        //Debug.Log(":\nString: " + fromweb);
        List<category> tempCat = new List<category>();
        AllCategories = JsonUtility.FromJson<Categories>(fromweb);
        tempCat = AllCategories.trivia_categories;

        cats = new List<string>();
        foreach (category cat in tempCat)
        {
            cats.Add(cat.name);            
        }

        Category.ClearOptions();
        Category.AddOptions(cats);
    }

    void gameType(int i) //multiple clics could corrupt the WWWForm
    {
        if (i == 0) typePH = "multiple";//ParamForm.AddField("type", "multiple");
        else if (i == 1) typePH = "boolean"; //ParamForm.AddField("type", "bool");
        else typePH = "";

        CAT.enabled = true;
        Count.enabled = true;
        Diff.enabled = true;

        Category.gameObject.SetActive(true);
        Difficulty.gameObject.SetActive(true);
        QuestionCountField.gameObject.SetActive(true);

        Star.enabled = true;
        StartButton.gameObject.SetActive(true);
    }
    void setCount(int i)
    {
        roundCount = i;
        if (i <= 0)
        {
            roundCount = 1;
        }
        else if (i >= 50)
        {
            roundCount = 50;
        }      
    }
    void setCategoryForm(int i) 
    {
        /*
        //get ID from string. AllCategories has the stuff
        //ParamForm.AddField("category", i);
        foreach (category cat in AllCategories.trivia_categories) //go through list of categories with string/int pairs from API
        {
            if (cat.name == i) //compare selected string from dropdown to each string in list of categories.
            {
                categoryPH = cat.id; // API understands categories as int/id, get ID of selected category.
                //ParamForm.AddField("category", cat.id); 
                break; //end loop
            }
        } //no field for "category" is added if no recognized category is selected, open trivia acts on default
        */
        categoryPH = i; //honestly, I thought I would need this var, I'll keep it for no reason.
    }
    void setDifficultyForm(int i) 
    {
        diffNumber = i; 
        //ParamForm.AddField("difficulty", i);
    }

    void StartGame()
    {
        Debug.Log("Start Game!");
        ParamForm.AddField("token", TokenDebug); //pre token for sending in the form

        //ParamForm.AddField("category", categoryPH);
        
        foreach (category cat in AllCategories.trivia_categories) //go through list of categories with string/int pairs from API
        {
            if (cat.name == Category.itemText.text) //compare selected string from dropdown to each string in list of categories.
            {
                ParamForm.AddField("category", cat.id); // API understands categories as int/id, get ID of selected category.
                break; //end loop
            }
        } //no field for "category" is added if no recognized category is selected, open trivia acts on default

        switch (diffNumber) //Difficulties cannot be pulled from API like categories, requires hard definitions for each element in dropdown.
        {
            case 0: //No field is added if none is selected, open trivia acts on default
                //ParamForm.AddField("difficulty", "");
                break;
            case 1:
                diffText = "easy";
                ParamForm.AddField("difficulty", "easy");
                break;
            case 2:
                diffText = "medium";
                ParamForm.AddField("difficulty", "medium");
                break;
            case 3:
                diffText = "hard";
                ParamForm.AddField("difficulty", "hard");
                break;
        }
        ParamForm.AddField("amount", roundCount);
        ParamForm.AddField("type", typePH);

        StartCoroutine(SendParam(StartReturn));
        
        //SetButtons(true, typeSwitch);
        
    }


    void StartReturn(string fromweb)
    {
        questionData data = new questionData();
        data = JsonUtility.FromJson<questionData>(fromweb);
        questionsDataList = data;
        QueRemain = questionsDataList.results.Count;
        Debug.Log(fromweb);
        gameSetUp();
        QuestionsLeft.text = "Questions Remaining: " + QueRemain;
        timerIsRunning = true;
        timeRemaining = 60;
    }
    void SetButtons(bool to)
    {
        Que.enabled = to;
        Ans.enabled = to;

        Question.gameObject.SetActive(to);

        Answer.gameObject.SetActive(to);
        Submit.gameObject.SetActive(to);

        True.gameObject.SetActive(to);
        False.gameObject.SetActive(to);
        
        Answer1.gameObject.SetActive(to);
        Answer2.gameObject.SetActive(to);
        Answer3.gameObject.SetActive(to);
        Answer4.gameObject.SetActive(to);

    }
    void SetButtons(bool to, bool GM)
    {
        Que.enabled = to;
        Ans.enabled = to;

        Question.gameObject.SetActive(to);

        if (GM)
        {
            //Answer.gameObject.SetActive(to);
            //Submit.gameObject.SetActive(to);
            Answer1.gameObject.SetActive(GM);
            Answer2.gameObject.SetActive(GM);
            Answer3.gameObject.SetActive(GM);
        }
        else
        {
            True.gameObject.SetActive(to);
            False.gameObject.SetActive(to);
        }
    }

    void ReplyButtonFunc(int but)
    {

        TheGame(but, CurrentRound);
        CurrentRound++;
        try
        {
            questionListElement data = questionsDataList.results[CurrentRound];
            gameSetUp(CurrentRound);
        }catch (ArgumentOutOfRangeException)
        {
            Debug.Log("Out of questions");

            True.gameObject.SetActive(false);
            False.gameObject.SetActive(false);

            Answer1.gameObject.SetActive(false);
            Answer2.gameObject.SetActive(false);
            Answer3.gameObject.SetActive(false);
            Answer4.gameObject.SetActive(false);
            QueRemain = 0;
            QuestionsLeft.text = "Questions Remaining: " + QueRemain;
        }
        QueRemain--;
        QuestionsLeft.text = "Questions Remaining: " + QueRemain;
    }
    void TheGame(int input, int iteration)
    {
        questionListElement element = questionsDataList.results[iteration];
        if (element.type == "boolean")
        {            
            if (element.correct_answer == "True" && input == 0 || element.correct_answer == "False" && input == 1)
            {
                //corr
                SCORE++;
            }
            else
            {
                //incorr
            }
        }
        else if (element.type == "multiple")
        {
            string corr = element.correct_answer;
            string answ = butts[input].text;
            if (answ == corr) SCORE++ ;//corr

        }
        Score.text = "Score: " + SCORE;

    }

    string[] theShuffle(string[] shuff)
    {
        for (int i = 0; i < shuff.Length; i++)
        {
            string tmp = shuff[i];
            int r = UnityEngine.Random.Range(i, shuff.Length);
            shuff[i] = shuff[r];
            shuff[r] = tmp;
        }
        return shuff;
    }
    void GameRound(int iteration)
    {
        string questA = questionsDataList.results[iteration].question;
        string questB = "Question " + iteration + ": " + questA;
        Question.text = questB;
    }
    void gameSetUp()
    {
        CurrentRound = 0;
        Score.text = "Score: " + SCORE;
        Que.enabled = true;
        Ans.enabled = true;

        Question.gameObject.SetActive(true);
        GameRound(0);
        questionListElement element = questionsDataList.results[0];
        if(element.type == "boolean")
        {
            True.gameObject.SetActive(true);
            False.gameObject.SetActive(true);

            Answer1.gameObject.SetActive(false);
            Answer2.gameObject.SetActive(false);
            Answer3.gameObject.SetActive(false);
            Answer4.gameObject.SetActive(false);


        }
        else if (element.type == "multiple")
        {
            True.gameObject.SetActive(false);
            False.gameObject.SetActive(false);

            Answer1.gameObject.SetActive(true);
            Answer2.gameObject.SetActive(true);
            Answer3.gameObject.SetActive(true);
            Answer4.gameObject.SetActive(true);

            string[] options = new string[4];
            options[0] = element.correct_answer;
            options[1] = element.incorrect_answers[0];
            options[2] = element.incorrect_answers[1];
            options[3] = element.incorrect_answers[2];
            options = theShuffle(options);
            //GameRound(0, butts, options);

            for (int i = 0; i < 4; i++)
            {
                butts[i].text = options[i];
            }
        }
    }
    void gameSetUp(int iteration)
    {
        //CurrentRound = 0;
        Que.enabled = true;
        Ans.enabled = true;

        Question.gameObject.SetActive(true);
        GameRound(iteration);
        questionListElement element = questionsDataList.results[iteration];
        if (element.type == "boolean")
        {
            True.gameObject.SetActive(true);
            False.gameObject.SetActive(true);

            Answer1.gameObject.SetActive(false);
            Answer2.gameObject.SetActive(false);
            Answer3.gameObject.SetActive(false);
            Answer4.gameObject.SetActive(false);


        }
        else if (element.type == "multiple")
        {
            True.gameObject.SetActive(false);
            False.gameObject.SetActive(false);

            Answer1.gameObject.SetActive(true);
            Answer2.gameObject.SetActive(true);
            Answer3.gameObject.SetActive(true);
            Answer4.gameObject.SetActive(true);

            string[] options = new string[4];
            options[0] = element.correct_answer;
            options[1] = element.incorrect_answers[0];
            options[2] = element.incorrect_answers[1];
            options[3] = element.incorrect_answers[2];
            options = theShuffle(options);
            //GameRound(0, butts, options);

            for (int i = 0; i < 4; i++)
            {
                butts[i].text = options[i];
            }
        }
    }

    IEnumerator GetToken(System.Action<string> gibtoken) //With callback
    {
        using (UnityWebRequest tokenRequest = UnityWebRequest.Get("https://opentdb.com/api_token.php?command=request")) //Get from token generating address
        {
            yield return tokenRequest.SendWebRequest();

            switch (tokenRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + tokenRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + tokenRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string returnString = tokenRequest.downloadHandler.text;
                    //Debug.Log(":\nReceived: " + returnString);
                    gibtoken(returnString); //Use callback to retrieve aquired token
                    //ThisToken = JsonUtility.FromJson<TokenClass>(returnString);
                    break;
            }
        }
    }

    IEnumerator SendParam(System.Action<string> gibAny) //WIP
    {
        string sendToken = TokenDebug;
       
        string uriA = "https://opentdb.com/api.php?";

        //Scuffed uri generation, just patching strings together.
        string uriB = uriA + "&" +"amount="+ roundCount;
        if (categoryPH != 0)
        {
            uriB = uriB + "&" + "category=" + categoryPH;
        }
        if (diffText != "")
        {
            uriB = uriB + "&" + "difficulty=" + diffText;
        }
        if (typePH != "")
        {
            uriB = uriB + "&" + "type=" + typePH;
        }
        uriB = uriB + "&" + "type=" + typePH + "&" + "token=" + TokenDebug;
        Debug.Log(uriB);        
        using (UnityWebRequest www = UnityWebRequest.Get(uriB)) //Let Unity handle formatting for passing parameters of game
        {
            yield return www.SendWebRequest();

            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + www.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + www.error);
                    break;
                case UnityWebRequest.Result.Success:

                    Debug.Log("Upload complete!");
                    /*
                    Debug.Log("Text lenth:"+www.downloadHandler.text.Length);
                    Debug.Log("Data:" + www.downloadHandler.data);
                    Debug.Log("Text:" + www.downloadHandler.text.ToString());
                    Debug.Log("Request object:" + www);
                    Debug.Log("Result:" + www.result);
                    Debug.Log("URI:" + www.uri);
                    Debug.Log("URL:" + www.url);
                    */
                    string returnString = www.downloadHandler.text;
                    //Debug.Log(":\nReceived: " + returnString);
                    gibAny(returnString);
                    break;
            }           

        }

    }

    IEnumerator GetCategory(System.Action<string> gibcat)
    {
        using (UnityWebRequest tokenRequest = UnityWebRequest.Get("https://opentdb.com/api_category.php")) //Get JSON of all categories
        {
            yield return tokenRequest.SendWebRequest();

            switch (tokenRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + tokenRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + tokenRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string returnString = tokenRequest.downloadHandler.text;
                    //Debug.Log(":\nReceived: " + returnString);
                    gibcat(returnString);
                    break;
            }
        }
    }
}
