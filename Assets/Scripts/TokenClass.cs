using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenClass
{
    /* Format of token request response
        {
            "response_code":0,
            "response_message":"Token Generated Successfully!",
            "token":"c3486a5da8acadac8fcecdb1ec8528b96f49f690476cae384d4a6d5be83745b4"    
        }
    */
    public int response_code;
    public string response_message;
    public string token;

    //public TokenClass NewToken = new TokenClass();
}
[System.Serializable]
public class Categories
{
    /* Format of category request response
        {
            "trivia_categories":
            [
                {"id":9,"name":"General Knowledge"},
                {"id":10,"name":"Entertainment: Books"},
                {"id":11,"name":"Entertainment: Film"},
                ***
                ***
            ]    
        }
    */
    public List<category> trivia_categories;
}
[System.Serializable]
public class category
{
    /* This format contained above         
                {"id":9,"name":"General Knowledge"}              
    */
    public int id;
    public string name;
}
[System.Serializable]
public class questionData
{
    /* Format:
            Text:
    {
	    "response_code":0,
	    "results":[
		    {
			    "category":"Art",
			    "type":"boolean",
			    "difficulty":"easy",
			    "question":"Leonardo da Vinci&#039;s Mona Lisa does not have eyebrows.",
			    "correct_answer":"True",
			    "incorrect_answers":[
				    "False"
				    ]
		    },
		    {
			    "category":"History",
			    "type":"boolean",
			    "difficulty":"easy",
			    "question":"The United States of America declared their independence from the British Empire on July 4th, 1776.",
			    "correct_answer":"True",
			    "incorrect_answers":[
				    "False"
				    ]
		    },
		    {
			    "category":"Entertainment: Television","
			    type":"boolean",
			    "difficulty":"medium",
			    "question":"An episode of &quot;The Simpsons&quot; is dedicated to Moe Szyslak&#039;s bar rag.",
			    "correct_answer":"True",
			    "incorrect_answers":[
				    "False"
				    ]
		    }
	    ]
    }
     */
    public int response_code;
    public List<questionListElement> results;
  
}
[System.Serializable]
public class questionListElement
{
    /*Format, contained above:
     {
        "category":"Art",
		"type":"boolean",
		"difficulty":"easy",
		"question":"Leonardo da Vinci&#039;s Mona Lisa does not have eyebrows.",
		"correct_answer":"True",
		"incorrect_answers":[
			"False"
			]
    },
     */

    public string category;
    public string type;
    public string difficulty;
    public string question;
    public string correct_answer;
    public List<string> incorrect_answers;
}
