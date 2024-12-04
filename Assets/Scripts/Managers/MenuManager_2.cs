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
    GameObject dev;
    GameObject titleRender;
    GameObject floor;

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
    public static bool wiggleCross = false;
    public static Int32 test = 0;

    [SerializeField] GameObject scrollContent;
    List<GameObject> boxes = new List<GameObject>();

    GameObject ferrisBase;
    List<FerrisCart> carts = new List<FerrisCart>();

    GameObject border;
    GameObject selection;

    float difference;
    private void Awake()
    {
        mainMenu = canvas.transform.GetChild(0).gameObject;
        settings = canvas.transform.GetChild(1).gameObject;
        dev = canvas.transform.GetChild(2).gameObject;
        eventSystem = EventSystem.current;

        //mask
        animator.Add(maskCanvas);
        //gate
        animator.Add(canvas.GetComponent<Animator>());
        //circus
        animator.Add(mainMenu.transform.GetChild(13).GetComponent<Animator>());
        foreach (Transform m in mainMenu.transform)
        {
            if (m.name == "TitleRender")
                titleRender = m.gameObject;
            if (m.name == "Floor")
                floor = m.gameObject;
        }
        foreach (Transform s in settings.transform)
        {
            if (s.name == "Options")
            {
                foreach (Transform child in s)
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
            }
        }
        for (int i = 0; i < scrollContent.transform.childCount; i++)
        {
            boxes.Add(scrollContent.transform.GetChild(i).gameObject);
        }

        ScreenResolution();

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
        Actions.Dev += MoveDev;
        //Actions.TextBoxColour += value => textBox = value;
        Actions.TextBoxColour += SetBox;
        Actions.Toggles += SwitchBool;
    }
    private void OnDisable()
    {
        Actions.Begin -= PlayAnimation;
        Actions.Settings -= Move;
        Actions.Dev -= MoveDev;
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
                StartCoroutine(Functions.MoveCubic(mainMenu.GetComponent<RectTransform>().localPosition, left, value => mainMenu.GetComponent<RectTransform>().localPosition = value, 1));
                StartCoroutine(Functions.MoveCubic(settings.GetComponent<RectTransform>().localPosition, centre, value => settings.GetComponent<RectTransform>().localPosition = value, 1));
                StartCoroutine(FerrisCartMove(left));
                //StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, left));
                //StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, centre));
                break;
            case false:
                StartCoroutine(Functions.MoveCubic(mainMenu.GetComponent<RectTransform>().localPosition, centre, value => mainMenu.GetComponent<RectTransform>().localPosition = value, 1));
                StartCoroutine(Functions.MoveCubic(settings.GetComponent<RectTransform>().localPosition, right, value => settings.GetComponent<RectTransform>().localPosition = value, 1));
                StartCoroutine(FerrisCartMove(centre));
                //StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, centre));
                //StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, right));
                break;
        }
    }
    public void MoveDev(bool isOn)
    {
        Vector3 up = new Vector3(0, 1080 * difference, 0);
        Vector3 centre = new Vector3(0, 0, 0);
        Vector3 down = new Vector3(0, -1080 * difference, 0);
        switch (isOn)
        {
            case true:
                StartCoroutine(Functions.MoveCubic(mainMenu.GetComponent<RectTransform>().localPosition, up, value => mainMenu.GetComponent<RectTransform>().localPosition = value, 1));
                StartCoroutine(Functions.MoveCubic(dev.GetComponent<RectTransform>().localPosition, centre, value => dev.GetComponent<RectTransform>().localPosition = value, 1));
                StartCoroutine(FerrisCartMove(up));
                //StartCoroutine(Functions.Move(mainMenu.GetComponent<RectTransform>().localPosition, left));
                //StartCoroutine(Functions.Move(settings.GetComponent<RectTransform>().localPosition, centre));
                break;
            case false:
                StartCoroutine(Functions.MoveCubic(mainMenu.GetComponent<RectTransform>().localPosition, centre, value => mainMenu.GetComponent<RectTransform>().localPosition = value, 1));
                StartCoroutine(Functions.MoveCubic(dev.GetComponent<RectTransform>().localPosition, down, value => dev.GetComponent<RectTransform>().localPosition = value, 1));
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

        border.transform.position = boxes[textBox].transform.position;
        selection.transform.position = boxes[textBox].transform.position;
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

    public void ChangeTest(Dropdown dropdown)
    {
        test = dropdown.value;
    }

    public void ScreenResolution()
    {
        //float desiredScaleX = ((float)Screen.currentResolution.width / 1920);
        //float desiredScaleY = ((float)Screen.currentResolution.height / 1080);
        //print(Screen.dpi);

        /*//object[] obj = GameObject.FindSceneObjectsOfType(typeof(GameObject));
        Image[] img = GameObject.FindObjectsOfType<Image>();
        foreach (Image child in img)
        {
            Debug.Log(child.name);
        }*/
        //mainMenu.transform.localScale = new Vector3(scale.x * desiredScaleX, 1.1f, 1);
        /*Vector3 scale = mainMenu.transform.localScale;
        mainMenu.transform.localScale = new Vector3(scale.x * desiredScaleX, 1.1f, 1);
        scale = settings.transform.localScale;
        settings.transform.localScale = new Vector3(scale.x * desiredScaleX, scale.y * desiredScaleY, 1);
        scale = dev.transform.localScale;
        dev.transform.localScale = new Vector3(scale.x * desiredScaleX, scale.y * desiredScaleY, 1);*/



        /*Vector3 deviceScreenResolution = new Vector3(Screen.width, Screen.height, 1);
        float deviceScreen = deviceScreenResolution.x / deviceScreenResolution.y;
        Camera.main.aspect = deviceScreen;
        float camHeight = 100 * Camera.main.orthographicSize * 2;
        float camWidth = camHeight * deviceScreen;

        Image[] img = GameObject.FindObjectsOfType<Image>();
        foreach (Image child in img)
        {
            float sprHeight = child.sprite.rect.height;
            float sprWidth = child.sprite.rect.width;
            float scaleRatioWidth = camWidth / sprWidth;
            float scaleRatioHeight = camHeight / sprHeight;
            child.transform.localScale = new Vector3(scaleRatioWidth, scaleRatioHeight, 1);
        }*/

        float ratioX = (float)Screen.currentResolution.width / 1920;
        float desiredRatioY = 1080 * ratioX;
        difference = (float)Screen.currentResolution.height / desiredRatioY;

        Vector3 scale = mainMenu.transform.GetChild(0).GetComponent<RectTransform>().localScale;
        mainMenu.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y * difference, 1);
        mainMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        scale = settings.transform.GetChild(0).GetComponent<RectTransform>().localScale;
        settings.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y * difference, 1);
        settings.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);

        scale = dev.transform.GetChild(0).GetComponent<RectTransform>().localScale;
        dev.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y * difference, 1);
        dev.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080 * difference);

        scale = floor.GetComponent<RectTransform>().localScale;
        floor.GetComponent<RectTransform>().localScale = new Vector3(scale.x, (scale.y * difference) + ((scale.y * difference) / 2), 1);
        scale = floor.GetComponent<RectTransform>().localScale;
        float halfScreen = -1080 * difference / 2;
        print(halfScreen);
        float halfSpr = -floor.GetComponent<RectTransform>().sizeDelta.y * scale.y / 2;
        print(-floor.GetComponent<RectTransform>().sizeDelta.y);
        print(scale.y);
        scale = floor.GetComponent<RectTransform>().anchoredPosition;
        floor.GetComponent<RectTransform>().anchoredPosition = new Vector2(scale.x, halfScreen - halfSpr);

        //Vector3 scale = mainMenu.GetComponent<RectTransform>().localScale;
        //mainMenu.GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y * difference, 1);
        //scale = settings.GetComponent<RectTransform>().localScale;
        //settings.GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y * difference, 1);
        //scale = dev.GetComponent<RectTransform>().localScale;
        //dev.GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y * difference, 1);
        //float posY = dev.GetComponent<RectTransform>().anchoredPosition.y;
        //dev.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY * difference);
        scale = maskCanvas.GetComponent<RectTransform>().sizeDelta;
        maskCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(scale.x * difference, scale.y * difference);
        scale = titleRender.GetComponent<RectTransform>().anchoredPosition;
        titleRender.GetComponent<RectTransform>().anchoredPosition = new Vector2(scale.x * difference, scale.y * difference);
    }
}
