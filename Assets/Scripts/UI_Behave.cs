using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class UI_Behave : MonoBehaviour
{
    public Button NewGameButton,StartButton,GameModeButton1, GameModeButton2;
    public Dropdown Category, Difficulty;
    public InputField QuestionCountField;

    
    public string TokenDebug;
    public List<category> catDebug;
    private List<string> cats;
    //private WWWForm ParamForm = new WWWForm();
    TokenClass myToken = new TokenClass();
    Categories AllCategories = new Categories();
    private void Awake()
    {   //Webfomr to contain game parameters later, amount of games, difficulty, selected category.
        WWWForm ParamForm = new WWWForm();
    }
    void Start()
    {
        Category.ClearOptions(); //Clear defaults from dropdown 
        catDebug = new List<category>();
        cats = new List<string>();
        //StartCoroutine(GetCategory(categoryStorage));

        Difficulty.ClearOptions();
        NewGameButton.onClick.AddListener(NewGame);
        StartButton.onClick.AddListener(StartGame);
        GameModeButton1.onClick.AddListener(SetCategory); //Dropdown population starts here, bound to GM select for debugging
        GameModeButton2.onClick.AddListener(SetCategory);
    }

    void NewGame()
    {
        Debug.Log("New Game!");
        StartCoroutine(GetToken(tokenStorage)); //Coroutine for IEnumerator
        
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
    void StartGame()
    {
        Debug.Log("Start Game!");
        //GetToken();
        tokenBugger();
        catBugger();
        //StartCoroutine(SendParam());
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
                    Debug.Log(":\nReceived: " + returnString);
                    gibtoken(returnString); //Use callback to retrieve aquired token
                    //ThisToken = JsonUtility.FromJson<TokenClass>(returnString);
                    break;
            }
        }
    }

    IEnumerator SendParam() //WIP
    {
        string sendToken = TokenDebug;
        WWWForm form = new WWWForm();
        form.AddField("token", sendToken);
        form.AddField("amount", 7);

        using (UnityWebRequest www = UnityWebRequest.Post("https://opentdb.com/api.php?", form)) //Let Unity handle formatting for passing parameters of game
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
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
                    Debug.Log(":\nReceived: " + returnString);
                    gibcat(returnString);
                    break;
            }
        }
    }
    void tokenBugger()
    {
        Debug.Log(TokenDebug);

    }
    void catBugger()
    {
        Debug.Log(AllCategories);
        Debug.Log(AllCategories.trivia_categories);

        foreach (category cat in catDebug)
        {
            Debug.Log(cat.name);
        }
        //Debug.Log(catDebug);
    }
}
