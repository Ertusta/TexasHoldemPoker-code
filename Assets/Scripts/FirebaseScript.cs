using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using Firebase.Extensions;
using TMPro;
using Google.MiniJSON;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

//database e y�klencek class
[Serializable]
public class Data
{   //databese y�klenecek de�i�kenler
    [HideInInspector]
    public int  Money;
    [HideInInspector]
    public string Nickname;

}



[Serializable]
public class Leader
{


    public List<string> NameList = new List<string>() { "John","Emily", "Michael", "Sophia", "William", "Olivia", "James", "Emma", "Benjamin", "Ava" };
    public List<int> PointList = new List<int>() { 19987, 19234, 17843, 16789, 15567, 13456, 12976, 10678, 10342,10000 };
}







public class FirebaseScript : MonoBehaviour
{


    public TMP_InputField EmailText;
    public TMP_InputField PasswordText;
    public TMP_InputField NicknameText;



    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference db;

    [SerializeField]
    public int  Money=10000;
    [SerializeField]
    public string Nickname;

    public Data Dataram;
    public Leader Lead;

    public TMPro.TextMeshProUGUI[] NameList;
    public TMPro.TextMeshProUGUI[] PointList;

    public GameObject Auth;
    public GameObject Menu;

    public static FirebaseScript Instance;


    

    


    //database ve kullan�c�lara eri�im
    private void Awake()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        Instance = this;
    }

    //istatistik toplama
    private void Start()
    {

        

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {


            var app = Firebase.FirebaseApp.DefaultInstance;


        });
    }

    //kullan�c� kaydetme
    public void SignUp()
    {
        Auth.SetActive(false);
        Menu.SetActive(true);
        //email kaydedilir
        auth.CreateUserWithEmailAndPasswordAsync(EmailText.text, PasswordText.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {

                Debug.Log("Bir�eyler ters gitti :(");
                return;
            }
            if (task.IsFaulted)
            {

                Debug.Log("Sign up failed. Check your email and password.");
                return;
            }

            Nickname = NicknameText.text;
            //kullan�c� Id si olu�turulur
            user = task.Result.User;
            Debug.Log("Sign up successful!");

            

            SaveData();

        });



    }

    //kullan�c� Id sine verileri kaydedilir
    public void SaveData()
    {

        
        Dataram.Money = Money;
        Dataram.Nickname = Nickname;


        //veriler jsona �eviirilir y�klenir
        string json = JsonUtility.ToJson(Dataram);
        db.Child(user.UserId).SetRawJsonValueAsync(json);



    }



    //kullan�c� giri�i
    public void Login()
    {
        Auth.SetActive(false);
        Menu.SetActive(true);

        auth.SignInWithEmailAndPasswordAsync(EmailText.text, PasswordText.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {

                Debug.Log("Bir�eyler ters gitti :(");
                return;
            }
            if (task.IsFaulted)
            {

                Debug.Log("Login failed please try again");
                return;
            }

            //Id al�n�r
            user = task.Result.User;
            Debug.Log("Log in successful!");

            

            //Id deki veriler classa aktar�l�r
            var serverData = db.Child(user.UserId).GetValueAsync();
            string json = serverData.Result.GetRawJsonValue();
            Dataram = JsonUtility.FromJson<Data>(json);

            


            //class de�i�kenlere aktar�l�r
            if (Dataram != null)
            {
                
                Money = Dataram.Money;
                Nickname = Dataram.Nickname;

                PlayerPrefs.SetFloat("Money", Money);
                PlayerPrefs.SetString("Nickname", Nickname);
            }
            else
            {//yeni kullan�c� giri�i oldu�unda veri olmad��� i�in 0 atan�r
                
                Money = 10000;
                Nickname = null;

            }
        });
    }





    //Firebaseden veri �ekmek biraz uzun s�r�yor o y�zden Ienumerator �a��r�yoruz
    public void Leaderboard()
    {
        StartCoroutine(Writeboard());

    }

    public void Refreshboard()
    {
        StartCoroutine(Refresh());

    }




    //kullan�c�y� leaderboarda kaydetme
   public  IEnumerator Writeboard()
    {
        Debug.Log("hello");
        //class al
        var data1 = db.Child("LeaderBoard").GetValueAsync();
        yield return new WaitUntil(predicate: () => data1.IsCompleted);
        string tson = data1.Result.GetRawJsonValue();
        Lead = JsonUtility.FromJson<Leader>(tson);

        int number = 0;

        

        //mevcut puan� best 10 ki�iyle k�yasla
        while (number < 10)
        {//ayn� isim yazilmas�n diye �nlem
            if (Lead.PointList[number] < Money )
            {

                if (number == 0)
                {

                    Lead.PointList[0] = Money;
                    Lead.NameList[0] = Nickname;


                    

                    //keydet
                    string json = JsonUtility.ToJson(Lead);
                    db.Child("LeaderBoard").SetRawJsonValueAsync(json);


                }
                else if(Lead.PointList[number - 1] > Money && Lead.NameList[number-1] != Nickname)
                {

                    //listeye ekle 
                    Lead.PointList.Insert(number, Money);
                    Lead.NameList.Insert(number, Nickname);


                    //geriye d��eni sil
                    Lead.PointList.RemoveAt(9);
                    Lead.NameList.RemoveAt(9);

                    //keydet
                    string json = JsonUtility.ToJson(Lead);
                    db.Child("LeaderBoard").SetRawJsonValueAsync(json);

                    


                }


                break;


            }
            number++;

        }
    }


    //best 10 ki�i ve puan� listeye yazd�r
    IEnumerator Refresh()
    {
        //class� al
        var data1 = db.Child("LeaderBoard").GetValueAsync();
        yield return new WaitUntil(predicate: () => data1.IsCompleted);
        string tson = data1.Result.GetRawJsonValue();
        Lead = JsonUtility.FromJson<Leader>(tson);

        int number = 0;

        //teker teker yazd�r
        while (number < 10)
        {
            
            NameList[number].text = Lead.NameList[number];
            PointList[number].text = Lead.PointList[number].ToString();
            number++;

        }

    }

    


}