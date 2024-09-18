using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager_2 : MonoBehaviour
{
    public Vector3 position;

    PointerEventData pointerEventData;
    public EventSystem eventSystem;
    GameObject selected;
    public List<Animator> animator;

    private void OnEnable()
    {
        Actions.Begin += PlayAnimation;
    }
    private void OnDisable()
    {
        Actions.Begin -= PlayAnimation;
    }

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        //method 1
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            if (raycastResults[0].gameObject.GetComponent<MenuButton>())
            {
                if (!raycastResults[0].gameObject.GetComponent<MenuButton>().selected)
                {
                    selected = raycastResults[0].gameObject;
                    selected.GetComponent<MenuButton>().selected = true;
                    StartCoroutine(selected.GetComponent<MenuButton>().Selected());
                }
            }
            else
            {
                if (selected)
                {
                    selected.GetComponent<MenuButton>().selected = false;
                    selected.GetComponent<MenuButton>().End();
                }
            }
        }
    }

    public void PlayAnimation()
    {
        StartCoroutine(playAnimAndLoad());
    }
    IEnumerator playAnimAndLoad()
    {
        animator[0].Play("pop");
        yield return new WaitForSeconds(.5f);
        animator[1].Play("MenuSelectOption");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
