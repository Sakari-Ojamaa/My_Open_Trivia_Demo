using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class UI_Behave : MonoBehaviour
{

    //This will be scuffed, sorry

    public Button NewGameButton,StartButton,GameModeButton1, GameModeButton2;
    public Dropdown Category, Difficulty;
    public InputField QuestionCountField;

    public Button True, False, Submit, Answer1, Answer2, Answer3;
    public InputField Question, Answer;

    public Text GM, CAT, Count, Diff, Star, Que, Ans;
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
    private bool typeSwitch = true;

    //https://opentdb.com/api.php?amount=7&category=9&difficulty=medium&type=multiple
    private void Awake()
    {   //Webfomr to contain game parameters later, amount of games, difficulty, selected category.
        ParamForm = new WWWForm();
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
        GameModeButton1.onClick.AddListener(delegate { gameType(0); }); 
        GameModeButton2.onClick.AddListener(delegate { gameType(1); });

        QuestionCountField.onValueChanged.AddListener(delegate { setCount(int.Parse(QuestionCountField.text)); });
        //{ setCategoryForm(Category.itemText.text); });
        Category.onValueChanged.AddListener(delegate { setCategoryForm(Category.value); }); //Listener will pass index selected item
        Difficulty.onValueChanged.AddListener(delegate { setDifficultyForm(Difficulty.value); });

        StartButton.gameObject.SetActive(false);
        GameModeButton1.gameObject.SetActive(false);
        GameModeButton2.gameObject.SetActive(false);

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

    void NewGame()
    {
        Debug.Log("New Game!");
        StartCoroutine(GetToken(tokenStorage)); //Coroutine for IEnumerator
        GM.enabled = true;
        GameModeButton1.gameObject.SetActive(true);
        GameModeButton2.gameObject.SetActive(true);
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
        else typePH = "boolean"; //ParamForm.AddField("type", "bool");

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
        SetButtons(true, typeSwitch);
    }
    void StartReturn(string fromweb)
    {
        questionData data = new questionData();
        data = JsonUtility.FromJson<questionData>(fromweb);
        questionsDataList = data;
        Debug.Log(fromweb);
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
