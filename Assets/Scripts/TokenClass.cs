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
public class category
{
    /* This format contained above         
                {"id":9,"name":"General Knowledge"}              
    */
    public int id;
    public string name;
}