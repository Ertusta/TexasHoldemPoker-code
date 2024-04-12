using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public TMPro.TextMeshProUGUI Name;
    public TMPro.TextMeshProUGUI Money;

    public int money;

    public GameObject Menu;
    public GameObject Board;

    //para ve isim yazdirir
    private void OnEnable()
    {

        Invoke("Write", 2);


    }

    private void Write()
    {

        Name.text  = "Nickname:" + FirebaseScript.Instance.Nickname;
        Money.text = "Money:" + FirebaseScript.Instance.Money;
    }
    //cikis
    public void Quit()
    {
        Application.Quit();
    }

    //leaderboard acar
    public void Leaderboard()
    {
        Board.SetActive(true);

        FirebaseScript.Instance.Leaderboard();
        Invoke("Refresh", 2);

        
    }
    //oyun baslar
    public void Play()
    {
        Menu.SetActive(false);
        SceneManager.LoadScene("Game");

    }
    //leaderboard kapat
    public void CloseBoard()
    {
        Menu.SetActive(true);
        Board.SetActive(false);
    }
    //listeyi yenile
    void Refresh()
    {
        FirebaseScript.Instance.Refreshboard();


    }


}
