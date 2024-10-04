using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public partial class MenuManager_2 : MonoBehaviour
{
    public Vector3 position;

    public GameObject mainMenu;
    public GameObject settings;

    PointerEventData pointerEventData;
    public EventSystem eventSystem;
    GameObject selected;
    public List<Animator> animator;
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
    public static float musicVol;
    public static float sfxVol;

    [SerializeField] GameObject scrollContent;
    List<GameObject> boxes = new List<GameObject>();

    GameObject border;
    GameObject selection;

    private void Awake()
    {
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
    }
    private void OnEnable()
    {
        Actions.Begin += PlayAnimation;
        Actions.Settings += Move;
        //Actions.TextBoxColour += value => textBox = value;
        Actions.TextBoxColour += SetBox;
    }
    private void OnDisable()
    {
        Actions.Begin -= PlayAnimation;
        Actions.Settings -= Move;
        //Actions.TextBoxColour -= value => textBox = value;
        Actions.TextBoxColour -= SetBox;
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
        yield return new WaitForSeconds(2f);
        animator[0].Play("MenuSelectOption");
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
    public void SetBox(int number)
    {
        textBox = number;
        textBoxColourLight = boxes[textBox].transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
        textBoxColourDark = boxes[textBox].transform.GetChild(1).gameObject.GetComponent<Image>().sprite;
    }
}
