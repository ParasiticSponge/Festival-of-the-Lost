using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject character;
    Sprite charAppearance;
    string charName;
    System.Random random = new System.Random();

    public Image powerBar;
    public GameObject fade;
    public GameObject textBoxPrefab;
    public Animator switchScreen;
    public GameObject animationCanvas;
    public Animator gameCanvas;
    public GameObject UI;
    GameObject f1_UI;
    public List<GameObject> darts;
    public List<GameObject> balloons;
    public Text scoreDartsText;
    public Text scoreTicketsText;
    int scoreDarts = 0;
    [SerializeField] int tickets = 0;
    int ticketsToEnterTent = 10;

    Sprite initialSprite;
    Sprite balloon;
    [SerializeField] Sprite[] tentSheet;

    public GameObject backgrounds;
    public GameObject foregrounds;
    List<GameObject> background = new List<GameObject>();
    List<GameObject> foreground = new List<GameObject>();
    public GameObject doors;
    List<Collision2D> door = new List<Collision2D>();
    List<GameObject> NPCs = new List<GameObject>();
    List<Animator> anims = new List<Animator>();
    //List<BoxCollider2D> door2 = new List<BoxCollider2D>();
    Sprite dart;
    float dartDistanceFromCam = -2;
    int currentRoom = 0;
    float initialGravity;
    bool hold;
    int startingDart;
    GameObject circusTent;

    GameObject noticeBoard;
    bool isLookingAtBoard;
    public float timeLooking;
    public bool dialogueExists;

    [SerializeField] private int maxPower = 100;

    public Sprite testingSprite;
    private void Awake()
    {
        character = FindObjectOfType<CharacterController2D>().gameObject;
        CharacterController2D controller = character.GetComponent<CharacterController2D>();
        charAppearance = controller.appearance;
        charName = controller.charName;
        
        //tentSheet = Resources.LoadAll<Sprite>("Circus_Sheet");
        if (MenuManager_2.textBoxColourLight == null) MenuManager_2.textBoxColourLight = testingSprite;
        initialSprite = character.gameObject.GetComponent<SpriteRenderer>().sprite;
        GameObject f1 = foregrounds.transform.GetChild(0).gameObject;
        GameObject f2 = foregrounds.transform.GetChild(1).gameObject;
        foreach (Transform t1 in f1.transform)
        {
            if (t1.gameObject.name.Contains("Tent"))
            {
                circusTent = t1.gameObject;
            }
            if (t1.gameObject.name.Contains("NoticeBoard"))
            {
                noticeBoard = t1.gameObject;
            }
            if (t1.gameObject.name.Contains("NPC"))
            {
                NPCs.Add(t1.gameObject);
            }
            if (t1.gameObject.name.Contains("Canvas"))
            {
                f1_UI = t1.GetChild(0).gameObject;
            }
        }
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
        foreach (Transform child in animationCanvas.transform)
        {
            if (child.GetComponent<Animator>())
                anims.Add(child.gameObject.GetComponent<Animator>());
        }

        balloon = balloons[0].GetComponent<SpriteRenderer>().sprite;
        dart = darts[startingDart].GetComponent<SpriteRenderer>().sprite;
        scoreTicketsText.text = tickets.ToString();
    }
    private void OnEnable()
    {
        //var addCar = new Action<string, decimal>((number, test) => { } );
        Actions.EnterRoom += SwitchRoom;
        Actions.isOverDoor += DoorAnim;
        Actions.Back += showUI;
        Actions.Hold += Hold;
        Actions.Release += Release;
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
        Actions.HitBalloon -= ScoreDarts;
        Actions.Talk -= Talk;
    }
    // Start is called before the first frame update
    void Start()
    {
        initialGravity = character.GetComponent<Rigidbody2D>().gravityScale;
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
        TextBox.Text(charAppearance, "I*", "Mum? Dad? Where did you go?", 0.02f);
    }

    public void SwitchRoom(int n)
    {
        StartCoroutine(SwitchRooms(n));
    }

    IEnumerator SwitchRooms(int n)
    {
        if (n == 3)
        {
            isLookingAtBoard = true;
            StartCoroutine(BoardLooking());
            StartCoroutine(Functions.Fade(f1_UI, 1));
            StartCoroutine(Functions.Fade(character, 1));
            character.GetComponent<CharacterController2D>().enabled = false;
            foreach (GameObject npc in NPCs)
            {
                StartCoroutine(Functions.Fade(npc, 1));
                npc.GetComponent<NPC_AI>().canMove = 0;
            }
            door[2].gameObject.SetActive(false);
            Vector3 pos = new Vector3(noticeBoard.transform.position.x, noticeBoard.transform.position.y, -10);
            Camera.main.GetComponent<CameraFollow>().enabled = false;
            StartCoroutine(Functions.Move(Camera.main.transform.position, pos, (value => Camera.main.transform.position = value)));
            StartCoroutine(Functions.Zoom(Camera.main, -9));
            UI.transform.GetChild(0).GetComponent<GameButtons>().type = GameButtons.TYPE.exitBoard;
            UI.SetActive(true);
            yield break;
        }
        if (n == 2 && tickets < ticketsToEnterTent)
        {
            TextBox.Text(charAppearance, charName, "It appears I don't have enough tickets...", 0.02f);
            yield break;
        }
        character.GetComponent<MouseController2D>().fire = true;
        switchScreen.speed = 1;
        switchScreen.Play("MenuSelectOption", 0, 0);
        foreach (Collision2D collider in door) { collider.enabled = false; }

        yield return new WaitForSeconds(1);
        Camera.main.GetComponent<CameraFollow>().enabled = true;
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
                if (tickets >= ticketsToEnterTent)
                    StartCoroutine(Sad());
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
            case GameButtons.TYPE.exitBoard:
                isLookingAtBoard = false;
                timeLooking = 0;
                StartCoroutine(Functions.Fade(f1_UI, 0));
                StartCoroutine(Functions.Fade(character, 0));
                //TODO: when moving before completely zoomed out, camera snaps rather than smooths to position 
                character.GetComponent<CharacterController2D>().enabled = true;
                foreach (GameObject npc in NPCs)
                {
                    StartCoroutine(Functions.Fade(npc, 0));
                    npc.GetComponent<NPC_AI>().canMove = 1;
                }
                door[2].gameObject.SetActive(true);
                CameraFollow component = Camera.main.GetComponent<CameraFollow>();
                Vector3 pos = new Vector3(character.transform.position.x, character.transform.position.y, 0) + component.offset;
                component.enabled = true;
                StartCoroutine(Functions.Move(Camera.main.transform.position, pos, (value => Camera.main.transform.position = value)));
                StartCoroutine(Functions.Zoom(Camera.main, 9));
                UI.transform.GetChild(0).GetComponent<GameButtons>().type = GameButtons.TYPE.back;
                UI.SetActive(false);
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
        powerBar.fillAmount = 0;
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
    public void ScoreDarts(bool hit)
    {
        anims[1].enabled = true;
        switch (hit)
        {
            case true:
                anims[1].Play("Nice", 0, 0);
                tickets++;
                scoreDarts++;
                scoreDartsText.text = scoreDarts.ToString();
                scoreTicketsText.text = tickets.ToString();
                break;
            case false:
                if (startingDart < darts.Count)
                    anims[1].Play("TryAgain", 0, 0);
                break;
        }

        if (tickets >= ticketsToEnterTent)
        {
            circusTent.GetComponent<SpriteRenderer>().sprite = tentSheet[1];
        }

        if (startingDart < darts.Count)
        {
            //Play Animation of switching darts
            StartCoroutine(MoveDart());
        }
        else
        {
            anims[1].Play("GameOverMinigame", 0, 0);
        }
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
    IEnumerator Sad()
    {
        yield return new WaitForSeconds(1.5f);
        TextBox.Text(charAppearance, charName, "Mum? Dad? Where did you go?", 0.02f);
    }

    IEnumerator BoardLooking()
    {
        while (isLookingAtBoard)
        {
            timeLooking += Time.deltaTime;
            if (timeLooking > 10 && !dialogueExists)
            {
                int number = random.Next(3);
                switch (number)
                {
                    case 0:
                        TextBox.Text(charAppearance, charName, "I didn't come here to look at paper all day!", 0.02f);
                        break;
                    case 1:
                        TextBox.Text(charAppearance, charName, "The board is filled with many things...", 0.02f);
                        break;
                    case 2:
                        TextBox.Text(charAppearance, charName, "I should get going", 0.02f);
                        break;
                }
            }
            yield return null;
        }
    }
}
