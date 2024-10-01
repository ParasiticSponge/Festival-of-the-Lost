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
    public GameObject doors;
    List<Collision2D> door = new List<Collision2D>();
    //List<BoxCollider2D> door2 = new List<BoxCollider2D>();
    Sprite dart;
    float dartDistanceFromCam = -2;
    int currentRoom = 0;
    float initialGravity;
    bool hold;
    int startingDart;
    [SerializeField] private int maxPower = 100;

    public Sprite testingSprite;
    private void Awake()
    {
        MenuManager_2.textBoxColourLight = testingSprite;
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
        foreach (Transform t in doors.transform)
        {
            door.Add(t.GetComponent<Collision2D>());
            //door2.Add(t.GetComponent<BoxCollider2D>());
        }

        balloon = balloons[0].GetComponent<SpriteRenderer>().sprite;
        dart = darts[startingDart].GetComponent<SpriteRenderer>().sprite;
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
        Actions.HitBalloon += ScoreDarts;
        Actions.Talk += Talk;
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
        Actions.Talk -= Talk;
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

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);

        TextBox.Text(null, "???", "What is your name?", 0.05f);
        TextBox.Text();
        TextBox.Text(null, "???", "...", 0.2f);
        //TextBox.Text($"Oh! Your name is {character.charName}?", 0.05f, true);
        //I* ouputs the input of the player
        TextBox.Text(character.GetComponent<CharacterController2D>().appearance, "I*", "Mum? Dad? Where did you go?", 0.02f);
    }

    public void SwitchRoom(int n)
    {
        StartCoroutine(SwitchRooms(n));
    }

    IEnumerator SwitchRooms(int n)
    {
        if (n == 2 && tickets < 10)
        {
            TextBox.Text(character.GetComponent<CharacterController2D>().appearance, character.name, "It appears I don't have enough tickets...", 0.02f);
            yield break;
        }
        character.GetComponent<MouseController2D>().fire = true;
        switchScreen.speed = 1;
        switchScreen.Play("MenuSelectOption", 0, 0);
        foreach (Collision2D collider in door) { collider.enabled = false; }

        yield return new WaitForSeconds(1);
        background[currentRoom].SetActive(false);
        foreground[currentRoom].SetActive(false);
        switch (n)
        {
            //this option brings user back to main circus
            case 0:
                foreach (Collision2D collider in door) { collider.enabled = true; }
                switch (currentRoom)
                {
                    case 1:
                        character.transform.localPosition = new Vector3(-2.9f, -3.06f, 0);
                        break;
                    case 2:
                        character.transform.localPosition = new Vector3(17, -3.06f, 0);
                        break;
                }
                currentRoom = 0;

                character.GetComponent<Rigidbody2D>().gravityScale = initialGravity;
                character.GetComponent<CharacterController2D>().enabled = true;
                character.GetComponent<MouseController2D>().enabled = false;
                character.GetComponent<Animator>().SetBool("dart", false);
                character.GetComponent<CircleCollider2D>().enabled = true;
                Camera.main.GetComponent<CameraFollow>().enabled = true;
                UI.SetActive(false);
                break;
            case 1:
                ResetDartsGame();
                currentRoom = 1;

                Vector3 room = background[currentRoom].transform.position;
                character.transform.position = new Vector3(room.x, room.y - 12, 0);
                Vector3 pos = character.transform.localPosition;
                character.transform.localPosition = new Vector3(pos.x, pos.y, dartDistanceFromCam);

                character.GetComponent<CharacterController2D>().enabled = false;
                character.GetComponent<MouseController2D>().enabled = true;
                character.GetComponent<Rigidbody2D>().gravityScale = 0;
                character.GetComponent<SpriteRenderer>().sprite = initialSprite;
                character.GetComponent<Animator>().SetBool("dart", true);
                character.GetComponent<CircleCollider2D>().enabled = false;
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                Camera.main.transform.position = new Vector3(room.x, room.y, -10);
                UI.SetActive(true);
                break;
            case 2:
                currentRoom = 2;

                character.transform.localPosition = new Vector3(0, -33.5f, 0);
                break;
        }
        switchScreen.StartPlayback();
        switchScreen.speed = -1;
        switchScreen.Play("MenuSelectOption", -1, float.NegativeInfinity);

        background[currentRoom].SetActive(true);
        foreground[currentRoom].SetActive(true);
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

    public void Talk(GameObject obj)
    {
        print("talking!!!");
        obj.transform.GetChild(0).gameObject.SetActive(false);
        TextBox.Text(obj.GetComponent<NPC_AI>().appearance, obj.GetComponent<NPC_AI>().charName, "Hello!", 0.02f);
        obj.GetComponent<NPC_AI>().canMove = 0;
        StartCoroutine(Talking(obj));
    }
    public IEnumerator Talking(GameObject obj)
    {
        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }
        obj.GetComponent<NPC_AI>().canMove = 1;
    }
}
