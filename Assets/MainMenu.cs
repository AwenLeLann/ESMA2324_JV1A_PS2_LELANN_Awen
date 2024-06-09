using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGAme(){
        Debug.Log("QUIT");
        Application.Quit();
    }

    public void Part1()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Part2()
    {
        SceneManager.LoadScene("Part 2");
    }
    public void Part3()
    {
        SceneManager.LoadScene("Part 3");

    }
    public void Fin()
    {
        SceneManager.LoadScene("Fin");
    }

    

}
