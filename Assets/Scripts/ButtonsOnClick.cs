using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsOnClick : MonoBehaviour
{
    public bool isContinueButton;

    private void Awake()
    {
        if(isContinueButton && !PlayerPrefs.HasKey("lastLevel"))
        {
            GetComponent<Image>().color = Color.gray;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    public void ContinueGame()
    {
        if(PlayerPrefs.HasKey("lastLevel"))
        {
            string temp = PlayerPrefs.GetString("lastLevel");
            SceneManager.LoadScene(temp, LoadSceneMode.Single);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetString("lastLevel", GameController.instance.currentLevelName);
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(GameController.instance.currentLevelName, LoadSceneMode.Single);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(GameController.instance.nextLevelName, LoadSceneMode.Single);
    }

}
