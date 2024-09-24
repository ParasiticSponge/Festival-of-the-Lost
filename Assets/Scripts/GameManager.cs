using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image powerBar;
    public GameObject fade;
    public GameObject textBoxPrefab;
    public CharacterController2D character;
    public Animator switchScreen;
    public Animator gameCanvas;
    public GameObject UI;
    public List<GameObject> darts;

    Sprite initialSprite;
    public GameObject backgrounds;
    public GameObject foregrounds;
    List<GameObject> background = new List<GameObject>();
    List<GameObject> foreground = new List<GameObject>();
    public List<BoxCollider2D> doors = new List<BoxCollider2D>();

    int currentRoom = 0;
    float initialGravity;
    bool hold;
    int startingDart;
    [SerializeField] private int maxPower = 100;

    private void Awake()
    {
        initialSprite = character.gameObject.GetComponent<SpriteRenderer>().sprite;
    }
    private void OnEnable()
    {
        //var addCar = new Action<string, decimal>((number, test) => { } );
        Actions.EnterRoom += SwitchRoom;
        Actions.isOverDoor += DoorAnim;
        Actions.Back += showUI;
        Actions.Hold += Hold;
        Actions.Release += Release;
        Actions.Shot += SwitchDart;
    }
    private void OnDisable()
    {
        Actions.EnterRoom -= SwitchRoom;
        Actions.isOverDoor -= DoorAnim;
        Actions.Back -= showUI;
        Actions.Hold -= Hold;
        Actions.Release -= Release;
        Actions.Shot -= SwitchDart;
    }
    // Start is called before the first frame update
    void Start()
    {
        initialGravity = character.gameObject.GetComponent<Rigidbody2D>().gravityScale;
        foreach (Transform child in backgrounds.transform)
        {
            background.Add(child.gameObject);
        }
        foreach (Transform child in foregrounds.transform)
        {
            foreground.Add(child.gameObject);
        }

        StartCoroutine(Functions.Fade(fade, 1));
        StartCoroutine(Intro());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);

        TextBox.Text("What is your name?", 0.02f);
        TextBox.Text();
        TextBox.Text("...", 0.2f);
        //TextBox.Text($"Oh! Your name is {character.charName}?", 0.05f, true);
        //I* ouputs the input of the player
        TextBox.Text("I*: Mum? Dad? Where did you go?", 0.02f);
    }

    public void SwitchRoom(int n)
    {
        StartCoroutine(SwitchRooms(n));
    }

    IEnumerator SwitchRooms(int n)
    {
        switchScreen.speed = 1;
        switchScreen.Play("MenuSelectOption", 0, 0);
        foreach (BoxCollider2D collider in doors) { collider.enabled = false; }

        character.enabled = false;
        //mouseInteract.enabled = true;
        character.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;

        yield return new WaitForSeconds(1);
        switch (n)
        {
            //this option brings user back to main circus
            case 0:
                switchScreen.StartPlayback();
                switchScreen.speed = -1;
                switchScreen.Play("MenuSelectOption", -1, float.NegativeInfinity);

                background[currentRoom].SetActive(false);
                foreground[currentRoom].SetActive(false);
                foreach (BoxCollider2D collider in doors) { collider.enabled = true; }
                currentRoom = 0;

                character.enabled = true;
                //mouseInteract.enabled = false;
                character.gameObject.GetComponent<Rigidbody2D>().gravityScale = initialGravity;
                character.gameObject.GetComponent<Animator>().SetBool("dart", false);
                Camera.main.GetComponent<CameraFollow>().enabled = true;
                character.gameObject.GetComponent<MouseController2D>().enabled = false;
                UI.SetActive(false);
                break;
            case 1:
                switchScreen.StartPlayback();
                switchScreen.speed = -1;
                switchScreen.Play("MenuSelectOption", -1, float.NegativeInfinity);

                background[currentRoom].SetActive(false);
                foreground[currentRoom].SetActive(false);
                currentRoom = 1;

                Vector3 room = background[currentRoom].transform.position;
                character.gameObject.transform.position = new Vector3(room.x, room.y - 12, 0);
                character.gameObject.GetComponent<SpriteRenderer>().sprite = initialSprite;
                character.gameObject.GetComponent<MouseController2D>().enabled = true;
                character.gameObject.GetComponent<Animator>().SetBool("dart", true);
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                UI.SetActive(true);
                Camera.main.transform.position = new Vector3(room.x, room.y, -10);
                break;
            case 2:
                break;
        }

        background[currentRoom].SetActive(true);
        foreground[currentRoom].SetActive(true);

        yield return new WaitForSeconds(2);
        switchScreen.StopPlayback();
    }
    public void DoorAnim(Collider2D obj, bool visible)
    {
        obj.transform.GetChild(0).gameObject.SetActive(visible);
    }
    public void showUI(GameButtons.TYPE type)
    {
        switch (type)
        {
            case GameButtons.TYPE.back:
                StartCoroutine(SwitchRooms(0));
                break;
        }
    }
    public void Hold()
    {
        hold = true;
        powerBar.gameObject.SetActive(true);
        StartCoroutine(PowerBar());
    }
    public void Release()
    {
        hold = false;
    }
    IEnumerator PowerBar()
    {
        for (float i = 0; i < 101; i++)
        {
            if (!hold) { print("this is 1"); Actions.Power.Invoke(i/100); break; }
            powerBar.fillAmount = i/100;
            yield return null;
        }
        if (hold)
        {
            for (float i = 100; i > -1; i--)
            {
                if (!hold) { print("this is 2"); Actions.Power.Invoke(i / 100); break; }
                powerBar.fillAmount = i / 100;
                yield return null;
            }
            if (hold) StartCoroutine(PowerBar());
        }
    }

    public void SwitchDart()
    {
        Sprite dart = darts[startingDart].GetComponent<SpriteRenderer>().sprite;
        Sprite thrown = character.GetComponent<SpriteRenderer>().sprite;

        //Play Animation of switching darts

        darts[startingDart].transform.position = character.gameObject.transform.position;
        darts[startingDart].GetComponent<SpriteRenderer>().sprite = thrown;

        Vector3 room = background[currentRoom].transform.position;
        character.gameObject.transform.position = new Vector3(room.x, room.y - 12, 0);
        character.gameObject.GetComponent<SpriteRenderer>().sprite = dart;

        character.gameObject.GetComponent<MouseController2D>().fire = false;
        startingDart++;
    }
    public void ResetDarts()
    {
    }
}
