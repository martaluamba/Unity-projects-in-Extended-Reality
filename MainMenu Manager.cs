using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public string passiveGameScene = "Passive Game";
    public string multimodalGameScene = "Multimodal Game";
    public string activeGameScene = "Active Game";
    // Call this function from the button
    //I implemented a public method load passive game() to switch from the current scene to the passive game 
    public void LoadPassiveGame()
    {
        SceneManager.LoadScene(passiveGameScene);
    }

    //same as above but with multimodal
    public void LoadMultimodalGame()
    {
        SceneManager.LoadScene(multimodalGameScene);
    }
    //same as above with active 
    public void LoadActiveGame()
    {
        SceneManager.LoadScene(activeGameScene);
    }

}
