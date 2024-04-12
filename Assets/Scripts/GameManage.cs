using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class GameManage : MonoBehaviour
{
    //kart havuzlarý
    public Transform Deck;
    public Transform Table;


    public int Money;

    int cardCount;
    int chooseCount;
    int scriptCount = 0;
    int playerCount = 7;

    int control = 0;
    int vector = 0;

    int turn = 0;
    int bet = 1;
    int totalBet;

    bool task = false;

    int playerPoint;
    int botPoint=-25;

    public List<GameObject> HandList;
    public List<TMPro.TextMeshProUGUI> StateList;
    public List<TMPro.TextMeshProUGUI> BetList;
    public List<GameObject> ButtonList;
    public List<GameObject> ChildList;
    public List<CardScript> CardList;

    public GameObject Menu;

    //tum konumlar
    List<Vector3> CardPositionList = new List<Vector3>() { new Vector3(-5, -4, 0), new Vector3(-6, -4, 0), new Vector3(-8, -0, 0), new Vector3(-7, 0, 0), new Vector3(-5, 4, 0), new Vector3(-6, 4, 0), new Vector3(-0.5f, 4, 0), new Vector3(0.5f, 4, 0), new Vector3(5, 4, 0), new Vector3(6, 4, 0), new Vector3(8, 0, 0), new Vector3(7, 0, 0), new Vector3(5, -4, 0), new Vector3(6, -4, 0) };

    List<bool> botList = new List<bool>() { true, true, true, true, true, true, true };

    //para degiskenini alir yazdirir
    public void Start()
    {

        Money = FirebaseScript.Instance.Money;
        BetList[9].text = "Money:" + Money;
        State1();

    }

    //oyunucu olmayan yonetici
    void State0()
    {
        if (turn == 0)
        {//bahis turu
            task = true;
            StateList[7].text = "State: Play";

        }//3 kart dagitir
        else if (turn == 1)
        {
            task = true;
            BetList[8].text = "Total bet:" + totalBet;

            //rastgele karti alýp ekler
            cardCount = Deck.childCount;
            int randomNumber = Random.Range(0, cardCount - 1);
            Transform theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(-3f, 1, 0);
            theChild.transform.SetParent(Table.transform);

            cardCount = Deck.childCount;
            randomNumber = Random.Range(0, cardCount - 1);
            theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(-2f, 1, 0);
            theChild.transform.SetParent(Table.transform);

            cardCount = Deck.childCount;
            randomNumber = Random.Range(0, cardCount - 1);
            theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(-1f, 1, 0);
            theChild.transform.SetParent(Table.transform);


            StateList[7].text = "State: Play";

        }
        else if (turn == 2)
        {//tek kart dagitir
            task = true;
            BetList[8].text = "Total bet:" + totalBet;
            StateList[7].text = "State: Play";

            cardCount = Deck.childCount;
            int randomNumber = Random.Range(0, cardCount - 1);
            Transform theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(0f, 1, 0);
            theChild.transform.SetParent(Table.transform);

        }
        else if (turn == 3)
        {//tek kart dagitir
            task = true;
            BetList[8].text = "Total bet:" + totalBet;
            StateList[7].text = "State: Play";

            cardCount = Deck.childCount;
            int randomNumber = Random.Range(0, cardCount - 1);
            Transform theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(1f, 1, 0);
            theChild.transform.SetParent(Table.transform);


        }
        else if (turn == 4)
        {//3 kart sectirir
            BetList[8].text = "Total bet:" + totalBet;
            StateList[7].text = "State: Choose 3 card";

            ButtonList[0].SetActive(true);
            ButtonList[1].SetActive(true);
            ButtonList[2].SetActive(true);
            ButtonList[3].SetActive(true);
            ButtonList[4].SetActive(true);


        }
        else if (turn == 5)
        {//puanlarý karsilastirir para ayarlar
            if (botPoint < playerPoint)
            {
                StateList[7].text = "kazandýn";
                Money += totalBet;
            }
            else if (botPoint == playerPoint)
            {
                Money += totalBet / playerCount;
                StateList[7].text = "Berabere";
            }
            else
            {

                StateList[7].text = "Kaybettin";
            }

            turn++;

            Invoke("Finish", 3);


        }





    }// ana oyuncu
    void State1()
    {

        if (turn == 0)
        {//kart alýr

            cardCount = Deck.childCount;
            int randomNumber = Random.Range(0, cardCount - 1);
            Transform theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(-0.5f, -4, 0);
            theChild.transform.SetParent(HandList[7].transform);

            cardCount = Deck.childCount;
            randomNumber = Random.Range(0, cardCount - 1);
            theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(0.5f, -4, 0);
            theChild.transform.SetParent(HandList[7].transform);

        }
        else if (turn <= 4)
        {//her tur bahis yapar
            BetList[0].text = "bet:" + bet;
            Money -= bet;
            BetList[9].text = "Money:" + Money;
            totalBet += bet;

        }
        else if (turn == 5)
        {//sectigi 3 karta elindeki 2 karti ekler Result fonksiyonu deger dondurur
            ChildList[scriptCount] = HandList[7].transform.GetChild(0).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            ChildList[scriptCount] = HandList[7].transform.GetChild(1).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            playerPoint = Result();
            scriptCount = 0;

            if (playerPoint == 9)
            {
                StateList[7].text = "Royal Flush";
            }
            else if (playerPoint == 8)
            {
                StateList[7].text = "Straight Flush";
            }
            else if (playerPoint == 7)
            {
                StateList[7].text = "Quad";
            }
            else if (playerPoint == 6)
            {
                StateList[7].text = "Full House";
            }
            else if (playerPoint == 5)
            {
                StateList[7].text = "Flush";
            }
            else if (playerPoint == 4)
            {
                StateList[7].text = "Straight";
            }
            else if (playerPoint == 3)
            {
                StateList[7].text = "Three of a kind";
            }
            else if (playerPoint == 2)
            {
                StateList[7].text = "Two pair";
            }
            else if (playerPoint == 1)
            {
                StateList[7].text = "One pair";
            }
            else if (playerPoint == 0)
            {
                StateList[7].text = "High card";
            }

        }



        Invoke("State2", 1);

    }//bot hareketleri dongu halinde 7 botu oynatir
    void State2()
    {
        if (turn == 0)
        {//kart ekler

            cardCount = Deck.childCount;
            int randomNumber = Random.Range(0, cardCount - 1);
            Transform theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(CardPositionList[vector].x, CardPositionList[vector].y, CardPositionList[vector].z);
            vector++;
            theChild.transform.SetParent(HandList[control].transform);

            cardCount = Deck.childCount;
            randomNumber = Random.Range(0, cardCount - 1);
            theChild = Deck.GetChild(randomNumber);
            theChild.position = new Vector3(CardPositionList[vector].x, CardPositionList[vector].y, CardPositionList[vector].z);
            vector++;
            theChild.transform.SetParent(HandList[control].transform);

        }//rastgele bahis yada cekilmek
        else if (turn <= 4 && botList[control])
        {
            int randomNumber = Random.Range(0, 3);

            if (randomNumber == 0)
            {
                bet *= 2;
                BetList[control + 1].text = "bet:" + bet;
                StateList[control].text = "State:Raise";
                totalBet += bet;
            }
            else if (randomNumber == 1)
            {
                BetList[control + 1].text = "bet:" + bet;
                StateList[control].text = "State:Call";
                totalBet += bet;

            }
            else if (randomNumber == 2)
            {//eli temizler
                playerCount--;
                botList[control] = false;
                StateList[control].text = "State:Fold";

                Transform theChild = HandList[control].transform.GetChild(0);
                theChild.transform.SetParent(Deck.transform);
                theChild.position = new Vector3(-600, -4, 0);
                theChild = HandList[control].transform.GetChild(0);
                theChild.transform.SetParent(Deck.transform);
                theChild.position = new Vector3(-600, -4, 0);
            }

        }//rastgele 3 karta kendi elini ekler Result calýsýr
        else if (turn == 5 && botList[control])
        {

            ChildList[scriptCount] = HandList[control].transform.GetChild(0).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            ChildList[scriptCount] = HandList[control].transform.GetChild(1).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            int randomNumber1 = Random.Range(0, 5);
            ChildList[scriptCount] = Table.transform.GetChild(randomNumber1).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            int randomNumber2 = Random.Range(0, 5);
            while (randomNumber2 == randomNumber1)
            {
                randomNumber2 = Random.Range(0, 5);
            }
            ChildList[scriptCount] = Table.transform.GetChild(randomNumber2).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            int randomNumber3 = Random.Range(0, 5);

            while (randomNumber3 == randomNumber1 || randomNumber3 == randomNumber2)
            {
                randomNumber3 = Random.Range(0, 5);
            }
            ChildList[scriptCount] = Table.transform.GetChild(randomNumber3).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            int botRam = Result();
            if (botRam == 9)
            {
                StateList[control].text = "Royal Flush";
            }
            else if (botRam == 8)
            {
                StateList[control].text = "Straight Flush";
            }
            else if (botRam == 7)
            {
                StateList[control].text = "Quad";
            }
            else if (botRam == 6)
            {
                StateList[control].text = "Full House";
            }
            else if (botRam == 5)
            {
                StateList[control].text = "Flush";
            }
            else if (botRam == 4)
            {
                StateList[control].text = "Straight";
            }
            else if (botRam == 3)
            {
                StateList[control].text = "Three of a kind";
            }
            else if (botRam == 2)
            {
                StateList[control].text = "Two pair";
            }
            else if (botRam == 1)
            {
                StateList[control].text = "One pair";
            }
            else if (botRam == 0)
            {
                StateList[control].text = "High card";
            }


            //en yuksek botun puanu kaydolur
            if (botPoint < botRam)
            {
                botPoint = botRam;
            }
            scriptCount = 0;



        }
        



        Invoke("State3", 0);
    }
    void State3()
    {// botlar icin dongu saglar

        if(playerCount == 0)
        {
            turn = 5;
            Invoke("State0", 1);
        }
        else if (control < 6)
        {
            Invoke("State2", 1);
            control++;
        }
        else
        {
            if (turn != 6)
            {
                control = 0;
                Invoke("State0", 1);
            }

        }
    }


    public void Raise()
    {
        if (task && turn < 4)
        {//bahsi 2 ye katlar
            bet *= 2;
            task = false;
            turn++;
            StateList[7].text = "State: Wait";
            Invoke("State1", 0);

        }

    }

    public void Call()
    {
        if (task && turn < 4)
        {//ayni bahis

            task = false;
            turn++;
            StateList[7].text = "State: Wait";
            Invoke("State1", 0);
        }
    }


    public void Fold()
    {//cekilir
        FirebaseScript.Instance.Money = Money;
        SceneManager.LoadScene("Menu");
        FirebaseScript.Instance.SaveData();
    }


    //5 kart secerkenki butonlar secilen kart eklenir
    public void FirstCard()
    {
        chooseCount++;
        if (chooseCount < 3)
        {

            ChildList[scriptCount] = Table.GetChild(0).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;
            ButtonList[0].SetActive(false);
        }
        else
        {
            ChildList[scriptCount] = Table.GetChild(0).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            ButtonList[0].SetActive(false);
            ButtonList[1].SetActive(false);
            ButtonList[2].SetActive(false);
            ButtonList[3].SetActive(false);
            ButtonList[4].SetActive(false);
            turn++;
            Invoke("State1", 0);
        }

    }

    public void SecondCard()
    {
        chooseCount++;
        if (chooseCount < 3)
        {

            ChildList[scriptCount] = Table.GetChild(1).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;
            ButtonList[1].SetActive(false);
        }
        else
        {
            ChildList[scriptCount] = Table.GetChild(1).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            ButtonList[0].SetActive(false);
            ButtonList[1].SetActive(false);
            ButtonList[2].SetActive(false);
            ButtonList[3].SetActive(false);
            ButtonList[4].SetActive(false);
            turn++;
            Invoke("State1", 0);
        }
    }

    public void ThirdCard()
    {
        chooseCount++;
        if (chooseCount < 3)
        {

            ChildList[scriptCount] = Table.GetChild(2).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;
            ButtonList[2].SetActive(false);
        }
        else
        {
            ChildList[scriptCount] = Table.GetChild(2).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            ButtonList[0].SetActive(false);
            ButtonList[1].SetActive(false);
            ButtonList[2].SetActive(false);
            ButtonList[3].SetActive(false);
            ButtonList[4].SetActive(false);
            turn++;
            Invoke("State1", 0);
        }
    }

    public void FourthCard()
    {
        chooseCount++;
        if (chooseCount < 3)
        {

            ChildList[scriptCount] = Table.GetChild(3).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;
            ButtonList[3].SetActive(false);
        }
        else
        {
            ChildList[scriptCount] = Table.GetChild(3).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            ButtonList[0].SetActive(false);
            ButtonList[1].SetActive(false);
            ButtonList[2].SetActive(false);
            ButtonList[3].SetActive(false);
            ButtonList[4].SetActive(false);
            turn++;
            Invoke("State1", 0);

        }
    }

    public void FifthCard()
    {
        chooseCount++;
        if (chooseCount < 3)
        {

            ChildList[scriptCount] = Table.GetChild(4).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;
            ButtonList[4].SetActive(false);
        }
        else
        {
            ChildList[scriptCount] = Table.GetChild(4).gameObject;
            CardList[scriptCount] = ChildList[scriptCount].GetComponent<CardScript>();
            scriptCount++;

            ButtonList[0].SetActive(false);
            ButtonList[1].SetActive(false);
            ButtonList[2].SetActive(false);
            ButtonList[3].SetActive(false);
            ButtonList[4].SetActive(false);
            turn++;
            Invoke("State1", 0);
        }
    }

    int Result()
    {//elin degeri doner
        int result = 0;
        int counter2 = 0;
        int pair = 0;

        CardList.Sort((x, y) => x.Number.CompareTo(y.Number));

        int type = CardList[4].Type;
        int number = CardList[4].Number;

        List<int> sameCard = new List<int>() { 0, 0, 0, 0, 0 };

        //sirf full house  icin
        //ayný kart sayýmý
        foreach (CardScript obj in CardList)
        {
            int counter = 0;

            foreach (CardScript obj2 in CardList)
            {
                if (obj.Number == obj2.Number)
                {
                    counter++;
                }

            }
            sameCard[counter2] = counter;
            counter2++;
        }
        counter2 = 0;

        //cift sayýmý
        foreach (int obj in sameCard)
        {
            if (obj == 2)
            {
                pair++;
            }

        }




        //one pair
        if (pair == 2)
        {
            result = 1;

        }

        //two pair
        if (pair == 4)
        {
            result = 2;
        }

        //three of a kind
        if (sameCard[4] == 3 || sameCard[3] == 3 || sameCard[2] == 3 || sameCard[1] == 3 || sameCard[0] == 3)
        {
            result = 3;
        }

        //straigt
        if (CardList[3].Number == number - 1 && CardList[2].Number == number - 2 && CardList[1].Number == number - 3 && CardList[0].Number == number - 4)
        {
            result = 4;
        }

        //flush

        if (CardList[3].Type == type && CardList[2].Type == type && CardList[1].Type == type && CardList[2].Type == type)
        {
            result = 5;
        }

        //full house 
        if (sameCard[4] == 3 || sameCard[3] == 3 || sameCard[2] == 3 || sameCard[1] == 3 || sameCard[0] == 3)
        {
            if (pair > 0)
            {
                result = 6;
            }

        }

        //quad
        if (sameCard[4] == 4 || sameCard[3] == 4 || sameCard[2] == 4 || sameCard[1] == 4 || sameCard[0] == 4)
        {
            result = 7;

        }

        //bu fonksiyona girdigi icn else iflere girmiyodu bende tersden yazdim
        if (CardList[3].Type == type && CardList[2].Type == type && CardList[1].Type == type && CardList[2].Type == type)
        {

            //royal flush için
            if (CardList[3].Number == 12 && CardList[2].Number == 11 && CardList[1].Number == 10 && CardList[0].Number == 1)
            {
                result = 10;

            }//straigt flush için
            else if (CardList[3].Number == number - 1 && CardList[2].Number == number - 2 && CardList[1].Number == number - 3 && CardList[0].Number == number - 4)
            {
                result = 9;

            }
        }


        return result;
    }

    //parayý kaydet 
    void Finish()
    {
        FirebaseScript.Instance.Money = Money;


        FirebaseScript.Instance.SaveData();

        SceneManager.LoadScene("Menu");



    }







}
