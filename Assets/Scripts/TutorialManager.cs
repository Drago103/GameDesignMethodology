using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject[] tutorialPages;
    [SerializeField] GameObject Player;

    private int currentPage = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowPage(currentPage);
    }

    void ShowPage(int index)
    {
        for(int i = 0; i < tutorialPages.Length; i++)
            tutorialPages[i].SetActive(i==index);
    }

    public void NextPage()
    {
        currentPage++;

        if (currentPage < tutorialPages.Length)
        {
            ShowPage(currentPage);
        }
        else
        {
            EndTutorial();
        }
    }

    void EndTutorial()
    {

        foreach(GameObject page in tutorialPages)
        {
            page.SetActive(false);
        }

        foreach(MonoBehaviour script in Player.GetComponents<MonoBehaviour>())
        {
            script.enabled = true;
        }

        gameObject.SetActive(false);   
    }
}
