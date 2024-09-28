using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image powerBar;
    public GameObject fade;
    public GameObject textBoxPrefab;
    //public CharacterController2D character;
    public GameObject character;
    public Animator switchScreen;
    public Animator gameCanvas;
    public GameObject UI;
    public List<GameObject> darts;
    public List<GameObject> balloons;
    public Text scoreDartsText;
    public Text scoreTicketsText;
    int scoreDarts = 0;
    int tickets = 0;

    Sprite initialSprite;
    Sprite balloon;

    public GameObject backgrounds;
    public GameObject foregrounds;
    List<GameObject> background = new List<GameObject>();
    List<GameObject> foreground = new List<GameObject>();
    public List<BoxCollider2D> doors = new List<BoxCollider2D>();
    Sprite dart;

    float dartDistanceFromCam = -2;
    int currentRoom = 0;
    float initialGravity;
    bool hold;
    int startingDart;
    [SerializeField] private int maxPower = 100;

    private void Awake()
    {
        initialSprite = character.gameObject.GetComponent<SpriteRenderer>().sprite;

        GameObject f2 = foregrounds.transform.GetChild(1).gameObject;
        foreach (Transform t2 in f2.transform)
        {
            if (t2.gameObject.name.Contains("Balloon"))
            {
                balloons.Add(t2.gameObject);
            }
            if (t2.gameObject.name.Contains("ThrowDart"))
            {
                darts.Add(t2.gameObject);
            }
        }

        balloon = balloons[0].GetComponent<SpriteRenderer>().sprite;
        dart = darts[startingDart].GetComponent<SpriteRenderer>().sprite;
    }
    private void OnEnable()
    {
        //var addCar = new Action<string, decimal>((number, test) => { } );
        Actions.EnterRoom += SwitchRoom;
        Actions.isOverDoor += DoorAnim;
        Actions.isOverDoor += DoorAnim;
        Actions.Back += showUI;
        Actions.Hold += Hold;
        Actions.Release += Release;
        Actions.Shot += SwitchDart;
        Actions.HitBalloon += ScoreDarts;
    }
    private void OnDisable()
    {
        Actions.EnterRoom -= SwitchRoom;
        Actions.isOverDoor -= DoorAnim;
        Actions.Back -= showUI;
        Actions.Hold -= Hold;
        Actions.Release -= Release;
        Actions.Shot -= SwitchDart;
        Actions.HitBalloon -= ScoreDarts;
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
        //StartCoroutine(Intro());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);

        TextBox.Text("???", "What is your name?", 0.05f);
        TextBox.Text();
        TextBox.Text("???", "...", 0.2f);
        //TextBox.Text($"Oh! Your name is {character.charName}?", 0.05f, true);
        //I* ouputs the input of the player
        TextBox.Text("I*", "Mum? Dad? Where did you go?", 0.02f);
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

        character.GetComponent<CharacterController2D>().enabled = false;
        //mouseInteract.enabled = true;
        character.GetComponent<Rigidbody2D>().gravityScale = 0;

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
                switch (currentRoom)
                {
                    case 1:
                        character.transform.localPosition = new Vector3(-2.9f, -3.06f, 0);
                        break;
                }
                currentRoom = 0;

                character.GetComponent<CharacterController2D>().enabled = true;
                //mouseInteract.enabled = false;
                character.GetComponent<Rigidbody2D>().gravityScale = initialGravity;
                character.GetComponent<Animator>().SetBool("dart", false);
                Camera.main.GetComponent<CameraFollow>().enabled = true;
                character.GetComponent<MouseController2D>().enabled = false;
                UI.SetActive(false);
                break;
            case 1:
                ResetDartsGame();
                switchScreen.StartPlayback();
                switchScreen.speed = -1;
                switchScreen.Play("MenuSelectOption", -1, float.NegativeInfinity);

                background[currentRoom].SetActive(false);
                foreground[currentRoom].SetActive(false);
                currentRoom = 1;

                Vector3 room = background[currentRoom].transform.position;
                character.transform.position = new Vector3(room.x, room.y - 12, 0);
                Vector3 pos = character.transform.localPosition;
                character.transform.localPosition = new Vector3(pos.x, pos.y, dartDistanceFromCam);

                character.GetComponent<BoxCollider2D>().enabled = true;
                character.GetComponent<SpriteRenderer>().sprite = initialSprite;
                character.GetComponent<MouseController2D>().enabled = true;
                character.GetComponent<Animator>().SetBool("dart", true);
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
    public void DoorAnim(GameObject obj, bool visible)
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
            if (!hold) { Actions.Power.Invoke(i/100); break; }
            powerBar.fillAmount = i/100;
            yield return null;
        }
        if (hold)
        {
            for (float i = 100; i > -1; i--)
            {
                if (!hold) { Actions.Power.Invoke(i / 100); break; }
                powerBar.fillAmount = i / 100;
                yield return null;
            }
            if (hold) StartCoroutine(PowerBar());
        }
    }

    public void SwitchDart()
    {
        if (startingDart < darts.Count)
        {
            //Play Animation of switching darts
            StartCoroutine(MoveDart());
        }
    }
    IEnumerator MoveDart()
    {
        //next dart to starting position
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenToWorld.y = darts[startingDart].transform.position.y;
        screenToWorld.z = darts[startingDart].transform.position.z;
        yield return StartCoroutine(Functions.Move(darts[startingDart].transform.position, screenToWorld, (value => darts[startingDart].transform.position = value)));

        Sprite thrown = character.GetComponent<SpriteRenderer>().sprite;
        darts[startingDart].transform.position = character.transform.position;
        darts[startingDart].GetComponent<SpriteRenderer>().sprite = thrown;

        Vector3 room = background[currentRoom].transform.position;
        character.transform.position = new Vector3(room.x, room.y - 12, 0);
        Vector3 pos = character.transform.localPosition;
        character.transform.localPosition = new Vector3(pos.x, pos.y, dartDistanceFromCam);

        character.GetComponent<SpriteRenderer>().sprite = dart;
        character.GetComponent<Animator>().Play("dartIdle");

        character.GetComponent<MouseController2D>().fire = false;
        startingDart++;
    }
    public void ResetDartsGame()
    {
        scoreDarts = 0;
        character.GetComponent<MouseController2D>().fire = false;
        startingDart = 0;
        for (int i = 1; i < darts.Count + 1; i++)
        {
            darts[i - 1].transform.localPosition = new Vector3(i + 1.5f, -3, dartDistanceFromCam);
            darts[i - 1].GetComponent<SpriteRenderer>().sprite = dart;
        }
        for (int i = 0; i < balloons.Count; i++)
        {
            balloons[i].GetComponent<SpriteRenderer>().sprite = balloon;
            balloons[i].GetComponent<CircleCollider2D>().enabled = true;
        }
    }
    public void ScoreDarts()
    {
        tickets++;
        scoreDarts++;
        scoreDartsText.text = scoreDarts.ToString();
        scoreTicketsText.text = tickets.ToString();
    }
}
