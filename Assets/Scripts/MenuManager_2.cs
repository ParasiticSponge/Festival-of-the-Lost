using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager_2 : MonoBehaviour
{
    public Vector3 position;

    public GameObject mainMenu;
    public GameObject settings;

    PointerEventData pointerEventData;
    public EventSystem eventSystem;
    GameObject selected;
    public List<Animator> animator;

    private void OnEnable()
    {
        Actions.Begin += PlayAnimation;
        Actions.Settings += Move;
    }
    private void OnDisable()
    {
        Actions.Begin -= PlayAnimation;
        Actions.Settings -= Move;
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
            if (raycastResults[0].gameObject.GetComponent<MenuButton>() != null)
            {
                if (!raycastResults[0].gameObject.GetComponent<MenuButton>().selected)
                {
                    selected = raycastResults[0].gameObject;
                    selected.GetComponent<MenuButton>().selected = true;
                }
            }
            else
            {
                if (selected)
                {
                    selected.GetComponent<MenuButton>().selected = false;
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
        switch (selected.GetComponent<MenuButton>().type)
        {
            case MenuButton.TYPE.PLAY:
                animator[0].Play("pop");
                break;
            case MenuButton.TYPE.EXIT:
                break;
        }
        //play gate open anim
        Actions.MenuBeginSound.Invoke();
        yield return new WaitForSeconds(.5f);
        animator[2].enabled = true;
        animator[2].Play("HideMenuUI");
        animator[2].Play("OpenGate");
        yield return new WaitForSeconds(2f);
        animator[1].Play("MenuSelectOption");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Move(bool isOn)
    {
        Vector3 left = new Vector3(-700, 0, 0);
        Vector3 centre = new Vector3(0, 0, 0);
        Vector3 right = new Vector3(700, 0, 0);
        switch (isOn)
        {
            case true:
                StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, left, value => mainMenu.GetComponent<RectTransform>().localPosition = value));
                StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, centre, value => settings.GetComponent<RectTransform>().localPosition = value));
                break;
            case false:
                StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, centre, value => mainMenu.GetComponent<RectTransform>().localPosition = value));
                StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, right, value => settings.GetComponent<RectTransform>().localPosition = value));
                break;
        }
    }
}
