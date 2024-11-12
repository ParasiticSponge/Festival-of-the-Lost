using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Linq;
using static UnityEngine.UI.Image;

public partial class MenuManager_2 : MonoBehaviour
{
    public GameObject canvas;
    public Animator maskCanvas;
    GameObject mainMenu;
    GameObject settings;

    PointerEventData pointerEventData;
    EventSystem eventSystem;
    GameObject selected;
    List<Animator> animator = new List<Animator>();
    public AudioSource audioSource;

    Dropdown resolutionBar;
    Slider musicSlider;
    Text musicNumber;
    Slider sfxSlider;
    Text sfxNumber;

    public static int textBox = 0;
    public static Sprite textBoxColourLight;
    public static Sprite textBoxColourDark;
    public static int resolution;
    public static float musicVol = 1;
    public static float sfxVol = 1;
    public static bool crossAssist = true;
    public static bool wiggleCross = true;

    [SerializeField] GameObject scrollContent;
    List<GameObject> boxes = new List<GameObject>();

    GameObject ferrisBase;
    List<FerrisCart> carts = new List<FerrisCart>();

    GameObject border;
    GameObject selection;

    private void Awake()
    {
        mainMenu = canvas.transform.GetChild(0).gameObject;
        settings = canvas.transform.GetChild(1).gameObject;
        eventSystem = EventSystem.current;

        //mask
        animator.Add(maskCanvas);
        //gate
        animator.Add(canvas.GetComponent<Animator>());
        //circus
        animator.Add(mainMenu.transform.GetChild(10).GetComponent<Animator>());

        foreach (Transform child in settings.transform)
        {
            switch (child.name)
            {
                case "MUSIC":
                    musicSlider = child.GetChild(1).gameObject.GetComponent<Slider>();
                    musicNumber = child.GetChild(2).gameObject.GetComponent<Text>();
                    break;
                case "SFX":
                    sfxSlider = child.GetChild(1).gameObject.GetComponent<Slider>();
                    sfxNumber = child.GetChild(2).gameObject.GetComponent<Text>();
                    break;
                case "Scroll":
                    border = child.GetChild(0).GetChild(0).gameObject;
                    selection = child.GetChild(0).GetChild(2).gameObject;
                    break;
                case "RESOLUTION":
                    resolutionBar = child.GetChild(1).gameObject.GetComponent<Dropdown>();
                    break;
            }
        }
        for (int i = 0; i < scrollContent.transform.childCount; i++)
        {
            boxes.Add(scrollContent.transform.GetChild(i).gameObject);
        }
        audioSource.volume = musicSlider.value;
        musicVol = musicSlider.value;
        sfxVol = sfxSlider.value;
        resolution = resolutionBar.value;
        textBoxColourLight = boxes[textBox].transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        textBoxColourDark = boxes[textBox].transform.GetChild(1).gameObject.GetComponent<Image>().sprite;
        carts = FindObjectsOfType<FerrisCart>().ToList();
        ferrisBase = GameObject.FindGameObjectWithTag("ferrisBase");
    }
    private void OnEnable()
    {
        Actions.Begin += PlayAnimation;
        Actions.Settings += Move;
        //Actions.TextBoxColour += value => textBox = value;
        Actions.TextBoxColour += SetBox;
        Actions.Toggles += SwitchBool;
    }
    private void OnDisable()
    {
        Actions.Begin -= PlayAnimation;
        Actions.Settings -= Move;
        //Actions.TextBoxColour -= value => textBox = value;
        Actions.TextBoxColour -= SetBox;
        Actions.Toggles -= SwitchBool;
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

        border.transform.position = boxes[textBox].transform.position;
        selection.transform.position = boxes[textBox].transform.position;

        audioSource.volume = musicSlider.value;
        musicVol = musicSlider.value;
        musicNumber.text = (Mathf.RoundToInt(musicVol*100)).ToString();
        sfxVol = sfxSlider.value;
        sfxNumber.text = (Mathf.RoundToInt(sfxVol * 100)).ToString();
        resolution = resolutionBar.value;
    }

    public void PlayAnimation()
    {
        StartCoroutine(playAnimAndLoad());
    }
    IEnumerator playAnimAndLoad()
    {
        yield return new WaitForSeconds(.5f);
        animator[1].enabled = true;
        animator[1].Play("HideMenuUI");
        animator[1].Play("OpenGate");
        animator[2].Play("OpenCircus");
        yield return new WaitForSeconds(2f);
        animator[0].Play("MenuSelectOption");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Move(bool isOn)
    {
        Vector3 left = new Vector3(-1920, 0, 0);
        Vector3 centre = new Vector3(0, 0, 0);
        Vector3 right = new Vector3(1920, 0, 0);
        switch (isOn)
        {
            case true:
                StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, left, (value => mainMenu.GetComponent<RectTransform>().localPosition = value)));
                StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, centre, (value => settings.GetComponent<RectTransform>().localPosition = value)));
                StartCoroutine(FerrisCartMove(left));
                //StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, left));
                //StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, centre));
                break;
            case false:
                StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, centre, (value => mainMenu.GetComponent<RectTransform>().localPosition = value)));
                StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, right, (value => settings.GetComponent<RectTransform>().localPosition = value)));
                StartCoroutine(FerrisCartMove(centre));
                //StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, centre));
                //StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, right));
                break;
        }
    }
    public void SetBox(int number)
    {
        textBox = number;
        textBoxColourLight = boxes[textBox].transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        textBoxColourDark = boxes[textBox].transform.GetChild(1).gameObject.GetComponent<Image>().sprite;
    }

    //can't pass ref to actions :(
    public void SwitchBool(bool condition, Action<bool> OP)
    {
        switch (condition)
        {
            case true:
                OP.Invoke(false);
                break;
            case false:
                OP.Invoke(true);
                break;
        }
    }

    IEnumerator FerrisCartMove(Vector3 target)
    {
        while (mainMenu.GetComponent<RectTransform>().localPosition != target)
        {
            foreach (var cart in carts)
            {
                if (ferrisBase.GetComponent<RectTransform>())
                    cart.TargetOrigin = ferrisBase.GetComponent<RectTransform>().transform.position;
                else
                    cart.TargetOrigin = ferrisBase.transform.position;
            }
            yield return null;
        }
    }
}
