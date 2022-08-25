using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardMenu : MonoBehaviour
{
    public void Board1()
    {
        BoardControl.selectedBoard = 1;
        StartCoroutine(LoadScene("Game"));
    }
    public void Board2()
    {
        BoardControl.selectedBoard = 2;
        StartCoroutine(LoadScene("Game"));
    }
    public void Board3()
    {
        BoardControl.selectedBoard = 3;
        StartCoroutine(LoadScene("Game"));
    }
    public void Board4()
    {
        BoardControl.selectedBoard = 4;
        StartCoroutine(LoadScene("Game"));
    }
    public void Board5()
    {
        BoardControl.selectedBoard = 5;
        StartCoroutine(LoadScene("Game"));
    }
    IEnumerator LoadScene(string name)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(name);
    }

}
