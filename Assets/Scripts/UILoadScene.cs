using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoadScene : MonoBehaviour
{
    public void LoadByName(string loadscene)
    {
        SceneManager.LoadScene(loadscene);
    }
    
    public void LoadByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}