using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MenuController : MonoBehaviour
{
    public PagesData pagesData;
    
    [SerializeField]
    public string StartPageName;

    [SerializeField]
    public List<GameObject> pagesobjects;

    private Page currPage;

    void Start()
    {
        bool was = false;
        foreach(var page in pagesData.pages)
        {
            if(CompareStrings(page.Name, StartPageName))
            {
                currPage = page;
                was = true;
            }
            else
            {
                pagesobjects[page.objectindex].SetActive(false);
            }
        }
        if(!was)
        {
            currPage = pagesData.pages[0];
        }
        pagesobjects[currPage.objectindex].SetActive(true);
    }

    void Update()
    {
        
    }

    public void SetPage(string pageName)
    {
        foreach (var page in pagesData.pages)
        {
            if (CompareStrings(page.Name, pageName))
            {
                if (currPage.transitions != null && currPage.transitions.Count > 0)
                {
                    bool hide = false;
                    foreach (var transition in currPage.transitions)
                    {
                        if (CompareStrings(transition, page.Name))
                        {
                            hide = true;
                            break;
                        }
                    }
                    if (hide)
                    {
                        pagesobjects[currPage.objectindex].SetActive(false);
                    }
                }
                currPage = page;
                pagesobjects[currPage.objectindex].SetActive(true);
                break;
            }
        }
    }

    private bool CompareStrings(string string1, string string2)
    {
        return string1.ToLowerInvariant().Replace(" ", "").Equals(string2.ToLowerInvariant().Replace(" ", ""), 
            System.StringComparison.CurrentCultureIgnoreCase);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
