using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using static Functions;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.HID;
//using UnityEditor.Animations;

public class GameManager : MonoBehaviour
{
    // ------------------MANAGEMENT---------------------
    PhysicsManager physics;
    float initialGravity;
    public bool paused;
    public bool canPause = true;
    bool settings;
    int currentRoom = 0;
    int stage = 0;
    bool mood = false;
    // ------------------CHARACTER---------------------
    protected CharacterController2D controller;
    protected GameObject character;
    Sprite charAppearance;
    GameObject boss;
    System.Random random = new System.Random();
    // ---------------------NPCS------------------------
    List<GameObject> NPCs = new List<GameObject>();
    GameObject mum;
    // ------------------OVERALL UI---------------------
    public GameObject UI;
    List<GameObject> masks = new List<GameObject>();
    GameObject fade;
    public GameObject textBoxPrefab;
    public Sprite testingSprite;
    public Sprite testingSpriteDark;
    public float textBoxSpeed = 0.005f;
    bool dialogueExists;
    public Animator switchScreen;
    public GameObject canvas;
    List<Animator> anims = new List<Animator>();
    List<Animator> anims2 = new List<Animator>();
    GameObject retryButton;

    [SerializeField] Sprite[] tentSheet;
    // ---------------------LEVEL------------------------
    public GameObject backgrounds;
    public GameObject foregrounds;
    List<GameObject> background = new List<GameObject>();
    protected List<GameObject> foreground = new List<GameObject>();
    // ---------------------LEVEL1------------------------
    public Sprite darkBg;
    List<Light2D> lights = new List<Light2D>();
    GameObject f1_UI;
    GameObject circusTent;
    public GameObject doors;
    List<Collision2D> door = new List<Collision2D>();
    List<SpriteRenderer> trees = new List<SpriteRenderer>();
    Sprite treeStump1;
    GameObject noticeBoard;
    bool isLookingAtBoard;
    [SerializeField] bool hasLookedAtBoard;
    float timeLooking;
    [SerializeField] int tickets = 0;
    int ticketsToEnterTent = 10;
    Text scoreTicketsText;
    bool waitingForCart;
    bool ridingCart;
    int plushiesFound = 0;
    float[,] plushiePos = { {-5.5f, 4}, {-1.833f, 4}, {1.834f, 4}, {5.5f, 4} };
    // ---------------------LEVEL2------------------------
    GameObject f2_UI;
    GameObject stars;
    List<Animator> star = new List<Animator>();
    List<GameObject> balloons = new List<GameObject>();
    Sprite balloon;
    Image powerBar;
    List<GameObject> darts = new List<GameObject>();
    Sprite dart;
    int scoreDarts = 0;
    Text scoreDartsText;
    float dartDistanceFromCam = 2;
    float charDartDistanceFromCam = -2;
    bool hold;
    int startingDart;
    GameObject crosshair;
    Vector3 desiredPos;
    float rapidTime;
    float slowTime;
    // ---------------------LEVEL3------------------------
    GameObject f3_UI;
    GameObject bow;
    Text countDartsText;
    [SerializeField] int pocketDarts = 0;
    bool centrePlatform;
    GameObject bossPlatform;
    Image powerBarBoss;
    Image bossHealthBar;
    float bossHealth;
    // -------------------SPRITE LIST----------------------
    public List<Sprite> darkSprites = new List<Sprite>();
    public List<SpriteRenderer> lightSprites = new List<SpriteRenderer>();

    private void Awake()
    {
        physics = gameObject.GetComponent<PhysicsManager>();
        character = FindObjectOfType<CharacterController2D>().gameObject;
        controller = character.GetComponent<CharacterController2D>();
        charAppearance = controller.appearance;

        //tentSheet = Resources.LoadAll<Sprite>("Circus_Sheet");
        if (MenuManager_2.textBoxColourLight == null)
        {
            MenuManager_2.textBoxColourLight = testingSprite;
            MenuManager_2.textBoxColourDark = testingSpriteDark;
        }

        foreach (Transform child in backgrounds.transform)
            background.Add(child.gameObject);
        foreach (Transform child in foregrounds.transform)
            foreground.Add(child.gameObject);

        foreground[2].SetActive(true);
        boss = FindObjectOfType<Boss>().gameObject;
        foreground[2].SetActive(false);

        foreach (Transform t1 in foreground[0].transform)
        {
            if (t1.name.Contains("Tent"))
                circusTent = t1.gameObject;
            if (t1.name.Contains("NoticeBoard"))
                noticeBoard = t1.gameObject;
            if (t1.name.Contains("NPC"))
            {
                NPCs.Add(t1.gameObject);
                if (t1.name.Contains("Mum"))
                    mum = t1.gameObject;
            }
            if (t1.GetComponent<Canvas>())
                f1_UI = t1.gameObject;
            if (t1.name.Contains("Lights"))
            {
                foreach (Transform child in t1)
                    lights.Add(child.gameObject.GetComponent<Light2D>());
            }
            if (t1.name.Contains("Trees"))
            {
                foreach (Transform child in t1)
                {
                    trees.Add(child.GetComponent<SpriteRenderer>());
                    if (child.name.Contains("TreeStump1"))
                        treeStump1 = child.GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
        foreach (Transform t2 in foreground[1].transform)
        {
            if (t2.name.Contains("Balloon"))
                balloons.Add(t2.gameObject);
            if (t2.name.Contains("ThrowDart"))
                darts.Add(t2.gameObject);
            if (t2.GetComponent<Canvas>())
                f2_UI = t2.gameObject;
        }
        foreach (Transform t3 in foreground[2].transform)
        {
            if (t3.name.Contains("Platform"))
                bossPlatform = t3.gameObject;
            if (t3.GetComponent<Canvas>())
                f3_UI = t3.gameObject;
        }
        foreach (Transform t in doors.transform)
        {
            door.Add(t.GetComponent<Collision2D>());
            //door2.Add(t.GetComponent<BoxCollider2D>());
        }
        foreach (Transform child in canvas.transform.GetChild(0))
        {
            if (child.name == "GameOver")
                anims.Add(child.GetChild(0).gameObject.GetComponent<Animator>());
            if (child.GetComponent<Animator>())
                anims.Add(child.gameObject.GetComponent<Animator>());
            switch (child.gameObject.name)
            {
                case string a when a.Contains("Mask"):
                    masks.Add(child.gameObject);
                    break;
                case string a when a.Contains("Fade"):
                    fade = child.gameObject;
                    break;
            }
        }
        foreach (Transform child in canvas.transform.GetChild(1))
        {
            if (child.GetComponent<Animator>())
                anims2.Add(child.gameObject.GetComponent<Animator>());
        }

        stars = anims[1].transform.GetChild(3).gameObject;
        foreach (Transform child in stars.transform)
        {
            if (child.GetComponent<Animator>())
                star.Add(child.gameObject.GetComponent<Animator>());
        }

        crosshair = character.GetComponent<MouseController2D>().crosshair;
        balloon = balloons[0].GetComponent<SpriteRenderer>().sprite;
        dart = darts[startingDart].GetComponent<SpriteRenderer>().sprite;
        bow = character.transform.GetChild(2).gameObject;

        scoreTicketsText = UI.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        countDartsText = UI.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        scoreDartsText = f2_UI.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        powerBar = f2_UI.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
        powerBarBoss = f3_UI.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        bossHealthBar = f3_UI.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();

        scoreTicketsText.text = tickets.ToString();
        retryButton = anims[1].gameObject.transform.GetChild(0).gameObject;
        anims2[3].transform.GetChild(1).GetChild(1).gameObject.GetComponent<Slider>().value = MenuManager_2.musicVol;
        anims2[3].transform.GetChild(2).GetChild(1).gameObject.GetComponent<Slider>().value = MenuManager_2.sfxVol;
        anims2[3].transform.GetChild(3).GetChild(1).gameObject.GetComponent<Toggle>().isOn = MenuManager_2.crossAssist;
        anims2[3].transform.GetChild(4).GetChild(1).gameObject.GetComponent<Toggle>().isOn = MenuManager_2.wiggleCross;
        anims[1].transform.GetChild(4).gameObject.GetComponent<Text>().text = tickets.ToString();

        //global volume
        foreground[4].SetActive(false);

        darkSprites.Add(Resources.Load<Sprite>("Images/Misc/BenchDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/PopcornDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Backgrounds/BalloonBackDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/BalloonDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/NoticeDark"));
        darkSprites.Add(Resources.Load<Sprite>(""));
        darkSprites.Add(Resources.Load<Sprite>("Images/UI/Design/Ticket_and_Dart_Display_Board"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/Circus_Dark_Version_curtain_opened"));

        background[1].SetActive(true);
        lightSprites.Add(GameObject.FindGameObjectWithTag("ParkBench").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("PopcornCart").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("BalloonBackground").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("BalloonGame").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("Noticeboard").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("PlushieStand").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("Tickets").GetComponent<SpriteRenderer>());
        background[1].SetActive(false);
    }
    private void Update()
    {
        MenuManager_2.musicVol = anims2[3].transform.GetChild(1).GetChild(1).gameObject.GetComponent<Slider>().value;
        MenuManager_2.sfxVol = anims2[3].transform.GetChild(2).GetChild(1).gameObject.GetComponent<Slider>().value;
        anims2[3].transform.GetChild(1).GetChild(2).gameObject.GetComponent<Text>().text = (MenuManager_2.musicVol * 100).ToString();
        anims2[3].transform.GetChild(2).GetChild(2).gameObject.GetComponent<Text>().text = (MenuManager_2.sfxVol * 100).ToString();
    }
    private void OnEnable()
    {
        //var addCar = new Action<string, decimal>((number, test) => { } );
        Actions.EnterRoom += SwitchRoom;
        Actions.isOverDoor += DoorAnim;
        Actions.Back += showUI;
        Actions.Hold += Hold;
        Actions.Release += Release;
        Actions.BalloonType += value => { tickets += value; scoreDarts += value; };
        Actions.HitBalloon += ScoreDarts;
        Actions.Talk += Talk;
        Actions.FinishTalk += DoAction;
        Actions.Pause += Pause;
        Actions.RideCart += RideCart;
        Actions.BulletHit += BulletHit;
        Actions.BossPhase += BossPhase;
        Actions.FoundPlushie += FoundPlushie;
    }
    private void OnDisable()
    {
        Actions.EnterRoom -= SwitchRoom;
        Actions.isOverDoor -= DoorAnim;
        Actions.Back -= showUI;
        Actions.Hold -= Hold;
        Actions.Release -= Release;
        Actions.BalloonType -= value => { tickets += value; scoreDarts += value; };
        Actions.HitBalloon -= ScoreDarts;
        Actions.Talk -= Talk;
        Actions.FinishTalk -= DoAction;
        Actions.Pause -= Pause;
        Actions.RideCart -= RideCart;
        Actions.BulletHit -= BulletHit;
        Actions.BossPhase -= BossPhase;
        Actions.FoundPlushie -= FoundPlushie;
    }
    // Start is called before the first frame update
    void Start()
    {
        initialGravity = character.GetComponent<Rigidbody2D>().gravityScale;

        switchScreen.gameObject.SetActive(false);
        masks[1].SetActive(false);
        fade.SetActive(true);
        StartCoroutine(Functions.Fade(fade, 1, 0, 1));
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);

        TextBox.Text(null, "???", "What is your name?", textBoxSpeed);
        TextBox.Text();
        //TextBox.Text($"Oh! Your name is {character.charName}?", 0.05f, true);
        //I* ouputs the input of the player
        //TextBox.Text(charAppearance, "I*", "Mum? Dad? Where did you go?", textBoxSpeed);
        TextBox.Text(mum.GetComponent<NPC_AI>().appearance, mum.GetComponent<NPC_AI>().charName, "We finally made it to the circus. Go play some minigames I*. The minigames are probably rigged so if you're having trouble, be sure to press ESCAPE and change some of the settings!", textBoxSpeed);
        StartCoroutine(DisableCollisions());
        fade.SetActive(false);
    }

    public void SwitchRoom(int n)
    {
        StartCoroutine(SwitchRooms(n));
    }

    IEnumerator SwitchRooms(int n)
    {
        switch (n)
        {
            //this option brings user back to main circus
            case 0:
                yield return StartCoroutine(Transition());
                star[0].Rebind();
                star[1].Rebind();
                star[2].Rebind();
                foreach (Collision2D collider in door) { collider.enabled = true; }
                switch (currentRoom)
                {
                    case 1:
                        character.transform.localPosition = new Vector3(18.7f, -3.06f, -1);
                        break;
                    case 2:
                        character.transform.localPosition = new Vector3(17, -3.06f, -1);
                        break;
                }
                currentRoom = 0;
                character.GetComponent<Rigidbody2D>().gravityScale = initialGravity;
                character.GetComponent<CharacterController2D>().enabled = true;
                character.GetComponent<MouseController2D>().enabled = false;
                character.GetComponent<Animator>().SetBool("dart", false);
                character.GetComponent<CircleCollider2D>().enabled = true;

                character.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                character.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 1.6f);

                Camera.main.GetComponent<CameraFollow>().enabled = true;
                break;
            case 1:
                yield return StartCoroutine(Transition());
                currentRoom = 1;
                //default layer mask
                character.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                UI.SetActive(false);
                ResetDartsGame(true);
                break;
            case 2:
                if (!hasLookedAtBoard)
                {
                    TextBox.Text(charAppearance, controller.charName, "The flyer says: 'Go check the notice board!'...", textBoxSpeed);
                    yield break;
                }
                else if (tickets < ticketsToEnterTent)
                {
                    TextBox.Text(charAppearance, controller.charName, "It appears I don't have enough tickets...", textBoxSpeed);
                    yield break;
                }
                yield return StartCoroutine(Transition());
                character.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                currentRoom = 2;
                tickets -= ticketsToEnterTent;
                scoreTicketsText.text = tickets.ToString();
                character.transform.localPosition = new Vector3(0, -33.5f, -1);
                controller.canJump = true;
                character.GetComponent<BossDartController>().enabled = true;
                Vector3 component = Camera.main.GetComponent<CameraFollow>().offset;
                Camera.main.transform.position = new Vector3(character.transform.position.x + component.x, character.transform.position.y + component.y, Camera.main.transform.position.z);
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                StartCoroutine(SetupBossFight());
                break;
            case 3:
                ExitBoard(isLookingAtBoard);
                yield break;
        }

        background[currentRoom].SetActive(true);
        foreground[currentRoom].SetActive(true);
        if (n == 0 && tickets >= ticketsToEnterTent && !mood)
        {
            mood = true;
            Destroy(mum);
            NPCs.Remove(mum);
            ChangeMood();
            StartCoroutine(Sad());
            yield return null;
            physics.UpdateCollision();
        }

        switchScreen.StartPlayback();
        switchScreen.speed = -1;
        switchScreen.Play("MenuSelectOption", -1, float.NegativeInfinity);

        yield return new WaitForSeconds(1);
        switchScreen.gameObject.SetActive(false);
        masks[1].SetActive(false);
    }
    IEnumerator Transition()
    {
        switchScreen.gameObject.SetActive(true);
        masks[1].SetActive(true);
        character.GetComponent<MouseController2D>().fire = true;
        switchScreen.speed = 1;
        switchScreen.Play("MenuSelectOption", 0, 0);
        foreach (Collision2D collider in door) { collider.enabled = false; }

        yield return new WaitForSeconds(1);
        //UIFront layer mask
        character.GetComponent<SpriteRenderer>().sortingLayerName = "UIFront";
        UI.SetActive(true);
        background[currentRoom].SetActive(false);
        foreground[currentRoom].SetActive(false);
    }
    public void DoorAnim(GameObject obj, bool visible)
    {
        obj.transform.GetChild(0).gameObject.SetActive(visible);
    }
    public void showUI(GameButtons.TYPE type)
    {
        //back is only called from 
        switch (type)
        {
            case GameButtons.TYPE.scoreSheetBack:
                anims[1].updateMode = AnimatorUpdateMode.UnscaledTime;
                canPause = true;
                Time.timeScale = 1;
                StartCoroutine(SwitchRooms(0));
                PlayAnimation(anims[1], "ScoreSheetShow", true);
                break;
            case GameButtons.TYPE.pauseBack:
                paused = true;
                Time.timeScale = 0;
                anims2[2].enabled = true;
                PlayAnimation(anims2[2], "WarningShow", false);
                break;
            case GameButtons.TYPE.replayMini:
                anims[1].updateMode = AnimatorUpdateMode.UnscaledTime;
                canPause = true;
                Time.timeScale = 1;
                PlayAnimation(anims[1], "ScoreSheetShow", true);
                ResetDartsGame(false);
                break;
            case GameButtons.TYPE.resetMini:
                Pause();
                ResetDartsGame(true);
                break;
            case GameButtons.TYPE.exitToMenu:
                canPause = true;
                Time.timeScale = 1;
                StartCoroutine(ExitToMenu());
                break;
            case GameButtons.TYPE.pauseBackYes:
                //get rid of warning and unpause
                Pause();
                tickets -= scoreDarts;
                scoreTicketsText.text = tickets.ToString();
                PlayAnimation(anims2[2], "WarningShow", true);
                StartCoroutine(SwitchRooms(0));
                break;
            case GameButtons.TYPE.pauseBackNo:
                //get rid of warning but don't unpause
                PlayAnimation(anims2[2], "WarningShow", true);
                break;
            case GameButtons.TYPE.settings:
                anims2[3].enabled = true;
                switch (settings)
                {
                    case true:
                        settings = false;
                        PlayAnimation(anims2[3], "SettingsShow", true);
                        Pause();
                        break;
                    case false:
                        settings = true;
                        PlayAnimation(anims2[3], "SettingsShow", false);
                        break;
                }
                break;
            case GameButtons.TYPE.cross:
                switch (MenuManager_2.crossAssist)
                {
                    case true:
                        crosshair.SetActive(true);
                        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Vector3.zero);
                        Vector3 desiredPos = new Vector3(crosshair.transform.localPosition.x, 0, crosshair.transform.localPosition.z);
                        crosshair.transform.localPosition = desiredPos;
                        MenuManager_2.crossAssist = false;
                        break;
                    case false:
                        crosshair.SetActive(false);
                        MenuManager_2.crossAssist = true;
                        break;
                }
                break;
            case GameButtons.TYPE.wiggle:
                switch (MenuManager_2.wiggleCross)
                {
                    case true:
                        MenuManager_2.wiggleCross = false;
                        break;
                    case false:
                        MenuManager_2.wiggleCross = true;
                        break;
                }
                break;
        }
    }
    public void Hold()
    {
        hold = true;
        powerBar.gameObject.SetActive(true);
        powerBarBoss.gameObject.SetActive(true);
        timeLooking = 0;
        //number from 0.5 to 1
        rapidTime = (random.Next(2) / 2) + 0.5f;
        //number from 1 to 2.5
        slowTime = (random.Next(4) / 2) + 1;
        StartCoroutine(PowerBar());
    }
    public void Release()
    {
        timeLooking = 0;
        hold = false;
    }
    IEnumerator PowerBar()
    {
        for (float i = 0; i < 101; i++)
        {
            if (!hold)
            {
                if (currentRoom == 1)
                    Actions.Power.Invoke(desiredPos);
                if (currentRoom == 2)
                    Bow(i);
                //mouseController has x position updated to match mouse, overide this
                crosshair.transform.position = desiredPos;
                break;
            }
            WiggleCrosshair(i);
            yield return new WaitForSeconds(0.01f);
        }
        if (hold)
        {
            for (float i = 100; i > -1; i--)
            {
                if (!hold)
                {
                    if (currentRoom == 1)
                        Actions.Power.Invoke(desiredPos);
                    if (currentRoom == 2)
                        Bow(i);
                    crosshair.transform.position = desiredPos;
                    break;
                }
                WiggleCrosshair(i);
                yield return new WaitForSeconds(0.01f);
            }
        }
        if (hold) StartCoroutine(PowerBar());
    }

    void WiggleCrosshair(float i)
    {
        float radius = 0;
        if (timeLooking <= rapidTime)
            radius = 3.5f;
        else if (timeLooking <= rapidTime + slowTime)
        {
            radius = 0.5f;
            rapidTime = (random.Next(2) / 2) + 0.5f;
        }
        else
        {
            radius = 3.5f;
            timeLooking = 0;
            slowTime = (random.Next(4) / 2) + 1;
        }

        //reuse variable for other purposes
        timeLooking += 0.01f;

        Vector3 screenToWorld;

        powerBar.fillAmount = i / 100;
        powerBarBoss.fillAmount = i / 100;
        screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * powerBar.fillAmount, 0));
        //position along bar
        desiredPos = new Vector3(crosshair.transform.position.x, screenToWorld.y, crosshair.transform.position.z);
        if (MenuManager_2.wiggleCross)
        {
            float randomAngleX = random.Next(721) / 2;
            float randomAngleY = random.Next(721) / 2;
            Vector3 randomPosition = new Vector3(Mathf.Cos(randomAngleX) * radius, Mathf.Sin(randomAngleY) * radius, 0);
            desiredPos = desiredPos + randomPosition;
        }

        if (crosshair.activeSelf)
            crosshair.transform.position = desiredPos;
    }
    IEnumerator MoveDart()
    {
        //next dart to starting position
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenToWorld.y = darts[startingDart].transform.position.y;
        screenToWorld.z = darts[startingDart].transform.position.z;
        yield return StartCoroutine(Functions.Move(darts[startingDart].transform.position, screenToWorld, (value => darts[startingDart].transform.position = value)));
        //yield return StartCoroutine(Functions.Move(arg, screenToWorld));

        Sprite thrown = character.GetComponent<SpriteRenderer>().sprite;
        darts[startingDart].transform.position = character.transform.position;
        darts[startingDart].GetComponent<SpriteRenderer>().sprite = thrown;

        Vector3 room = background[currentRoom].transform.position;
        character.transform.position = new Vector3(room.x, room.y - 8, 0);
        Vector3 pos = character.transform.localPosition;
        character.transform.localPosition = new Vector3(pos.x, pos.y, charDartDistanceFromCam);

        character.GetComponent<SpriteRenderer>().sprite = dart;
        character.GetComponent<Animator>().Play("dartIdle");

        character.GetComponent<MouseController2D>().fire = false;
        startingDart++;
        powerBar.fillAmount = 0;

        //reset y to middle of screen
        GameObject cross = character.GetComponent<MouseController2D>().crosshair;
        Vector3 desiredPos = new Vector3(cross.transform.localPosition.x, 0, cross.transform.localPosition.z);
        if (cross.activeSelf)
            cross.transform.localPosition = desiredPos;
    }
    public void ResetDartsGame(bool fullReset)
    {
        retryButton.SetActive(true);
        Vector3 room = background[currentRoom].transform.position;
        character.transform.position = new Vector3(room.x, room.y - 8, 0);
        Vector3 pos = character.transform.localPosition;
        character.transform.localPosition = new Vector3(pos.x, pos.y, charDartDistanceFromCam);
        character.GetComponent<Animator>().Play("dartIdle", 0, 0);

        character.GetComponent<CharacterController2D>().enabled = false;
        character.GetComponent<MouseController2D>().enabled = true;
        character.GetComponent<Rigidbody2D>().gravityScale = 0;
        character.GetComponent<Animator>().SetBool("dart", true);
        character.GetComponent<CircleCollider2D>().enabled = false;

        character.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.03451007f);
        character.GetComponent<BoxCollider2D>().size = new Vector2(0.06f, 0.07310224f);

        Camera.main.GetComponent<CameraFollow>().enabled = false;
        Camera.main.transform.position = new Vector3(room.x, room.y, -10);

        star[0].Rebind();
        star[1].Rebind();
        star[2].Rebind();
        powerBar.fillAmount = 0;
        scoreDarts = 0;
        scoreDartsText.text = scoreDarts.ToString();
        character.GetComponent<MouseController2D>().fire = false;
        startingDart = 0;
        for (int i = 1; i < darts.Count + 1; i++)
        {
            darts[i - 1].transform.localPosition = new Vector3(i + 1.5f, -2.5f, dartDistanceFromCam);
            darts[i - 1].GetComponent<SpriteRenderer>().sprite = dart;
        }
        if (fullReset)
        {
            for (int i = 0; i < balloons.Count; i++)
            {
                balloons[i].GetComponent<SpriteRenderer>().sprite = balloon;
                balloons[i].GetComponent<Animator>().Rebind();
                balloons[i].GetComponent<CircleCollider2D>().enabled = true;
            }
        }

        //reset y to middle of screen
        GameObject cross = character.GetComponent<MouseController2D>().crosshair;
        Vector3 desiredPos = new Vector3(cross.transform.localPosition.x, 0, cross.transform.localPosition.z);
        if (cross.activeSelf)
            cross.transform.localPosition = desiredPos;
    }
    public void ScoreDarts(bool hit)
    {
        anims[0].enabled = true;
        anims[0].transform.parent.localScale = new Vector3(0.3f, 0.3f, 1);
        Vector3 position = Camera.main.WorldToScreenPoint(new Vector3(character.transform.position.x, character.transform.position.y + 2, 0));
        anims[0].transform.parent.GetComponent<RectTransform>().position = new Vector3(position.x, position.y, 0);
        switch (hit)
        {
            case true:
                //reuse variables
                slowTime = random.Next(2);
                if (slowTime == 0) anims[0].Play("Nice", 0, 0);
                else anims[0].Play("Great", 0, 0);
                //Making sure BalloonType in mouseController was invoked before HitBalloon which calls this function
                scoreDartsText.text = scoreDarts.ToString();
                scoreTicketsText.text = tickets.ToString();
                int count = 0;
                for (int i = 0; i < balloons.Count; i++)
                {
                    if (balloons[i].GetComponent<CircleCollider2D>().enabled)
                        count++;
                }
                if (count == 0)
                {
                    retryButton.SetActive(false);
                    StartCoroutine(GameOverDarts());
                }
                break;
            case false:
                if (startingDart < darts.Count)
                {
                    slowTime = random.Next(2);
                    if (slowTime == 0) anims[0].Play("TryAgain", 0, 0);
                    else anims[0].Play("Missed", 0, 0);
                }
                break;
        }

        if (startingDart < darts.Count)
        {
            //Play Animation of switching darts
            StartCoroutine(MoveDart());
        }
        else
        {
            StartCoroutine(GameOverDarts());
        }
    }

    public void Talk(GameObject obj)
    {
        TextBox text = FindObjectOfType<TextBox>();
        if (text == null)
        {
            obj.transform.GetChild(0).gameObject.SetActive(false);
            obj.GetComponent<NPC_AI>().Talk();
            obj.GetComponent<NPC_AI>().canMove = 0;
            StartCoroutine(Talking(obj));
        }
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
        //circusTent.GetComponent<SpriteRenderer>().sprite = tentSheet[1];

        yield return new WaitForSeconds(1.5f);
        TextBox.Text(NPCs[1].GetComponent<NPC_AI>().appearance, NPCs[1].GetComponent<NPC_AI>().charName, "Wow, nice job kiddo! Here, take some darts as a souvenir", textBoxSpeed);

        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }
        //Play animation
        anims[0].Play("Aquire", 0, 0);
        pocketDarts = 5;
        countDartsText.text = pocketDarts.ToString();

        yield return new WaitForSeconds(2.5f);
        TextBox.Text(charAppearance, controller.charName, "Thanks!", textBoxSpeed);
        TextBox.Text(charAppearance, controller.charName, "...", textBoxSpeed);
        TextBox.Text(charAppearance, controller.charName, "Mum? Dad? Where did you go?", textBoxSpeed);
    }
    public void ChangeMood()
    {
        fade.SetActive(true);
        StartCoroutine(moodyFade());
        //other static stuff
        background[0].gameObject.GetComponent<SpriteRenderer>().sprite = darkBg;
        foreach (Light2D child in lights)
            child.color = Color.red;
        foreach (SpriteRenderer child in trees)
        {
            if (child.gameObject.name.Contains("TreeWhole"))
                child.sprite = treeStump1;
        }
        MenuManager_2.textBoxColourLight = MenuManager_2.textBoxColourDark;

        //mum npc removed so -1 in array
        NPCs[4].GetComponent<Animator>().Play("BoyPointDarkAnim");

        for (int i = 0; i < lightSprites.Count; i++)
            lightSprites[i].sprite = darkSprites[i];

        circusTent.GetComponent<SpriteRenderer>().sprite = darkSprites[2];

        foreground[4].SetActive(true);
    }
    IEnumerator moodyFade()
    {
        if (mood)
        {
            yield return StartCoroutine(Functions.Fade(fade, fade.GetComponent<Image>().color.a, 0.2f, 0.2f));
            yield return StartCoroutine(Functions.Fade(fade, fade.GetComponent<Image>().color.a, 0, 0.2f));
            StartCoroutine(moodyFade());
        }
    }

    //TODO: could use DoAction to check if textbox is not there
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
                        TextBox.Text(charAppearance, controller.charName, "I didn't come here to look at paper all day!", textBoxSpeed);
                        break;
                    case 1:
                        TextBox.Text(charAppearance, controller.charName, "The board is filled with many things...", textBoxSpeed);
                        break;
                    case 2:
                        TextBox.Text(charAppearance, controller.charName, "I should get going", textBoxSpeed);
                        break;
                }
                dialogueExists = true;
                StartCoroutine(WaitBoard());
            }
            yield return null;
        }
    }

    IEnumerator WaitBoard()
    {
        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }
        dialogueExists = false;
        timeLooking = 0;
    }

    /*Func<Vector3, Vector3> arg = input =>
    {
        Camera.main.transform.position += input;
        return Camera.main.transform.position;
    };
    public delegate Vector3 MethodNameDelegate(ref Vector3 a);*/
/*    Func<Vector3, Vector3> arg = input =>
    {
        return input + value;
    };
    public Vector3 Delegate(ref Vector3 a)
    {
        return a;
    }*/
    void ExitBoard(bool exit)
    {
        Vector3 pos;
        switch (exit)
        {
            case false:
                isLookingAtBoard = true;
                hasLookedAtBoard = true;
                StartCoroutine(BoardLooking());
                StartCoroutine(Functions.Fade(UI.transform.GetChild(0).gameObject, 1, 0, 1));
                StartCoroutine(Functions.Fade(scoreTicketsText.gameObject, 1, 0, 1));
                StartCoroutine(Functions.Fade(character, 1, 0, 1));
                controller.canMove = 0;
                //character.GetComponent<CharacterController2D>().enabled = false;
                foreach (GameObject npc in NPCs)
                {
                    StartCoroutine(Functions.Fade(npc, 1, 0, 1));
                    if (npc.GetComponent<NPC_AI>()) npc.GetComponent<NPC_AI>().canMove = 0;
                }
                door[2].gameObject.SetActive(false);
                pos = new Vector3(noticeBoard.transform.position.x, noticeBoard.transform.position.y, -10);
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                StartCoroutine(Functions.Move(Camera.main.transform.position, pos, (value => Camera.main.transform.position = value)));
                //StartCoroutine(Functions.Move(arg => Camera.main.transform.position = arg, pos));
                StartCoroutine(Functions.Zoom(Camera.main, -9));
                break;
            case true:
                isLookingAtBoard = false;
                timeLooking = 0;
                StartCoroutine(Functions.Fade(UI.transform.GetChild(0).gameObject, 0, 1, 1));
                StartCoroutine(Functions.Fade(scoreTicketsText.gameObject, 0, 1, 1));
                StartCoroutine(Functions.Fade(character, 0, 1, 1));
                //TODO: when moving before completely zoomed out, camera snaps rather than smooths to position 
                controller.canMove = 1;
                //character.GetComponent<CharacterController2D>().enabled = true;
                foreach (GameObject npc in NPCs)
                {
                    StartCoroutine(Functions.Fade(npc, 0, 1, 1));
                    if (npc.GetComponent<NPC_AI>()) npc.GetComponent<NPC_AI>().canMove = 1;
                }
                door[2].gameObject.SetActive(true);
                CameraFollow component = Camera.main.GetComponent<CameraFollow>();
                pos = new Vector3(character.transform.position.x, character.transform.position.y, 0) + component.offset;
                component.enabled = true;
                StartCoroutine(Functions.Move(Camera.main.transform.position, pos, (value => Camera.main.transform.position = value)));
                //StartCoroutine(Functions.Move(arg, pos));
                StartCoroutine(Functions.Zoom(Camera.main, 9));
                break;
        }
    }

    void DoAction(string name)
    {
        switch (name)
        {
            case string a when a.Contains("Balloon"):
                StartCoroutine(SwitchRooms(1));
                break;
            case string a when a.Contains("Wheel"):
                if (!ridingCart && !waitingForCart)
                {
                    waitingForCart = true;
                    controller.canMove = 0;
                }
                break;
        }
    }

    void Pause()
    {
        switch (currentRoom)
        {
            case 0:
                anims2[0].enabled = true;
                if (!paused)
                {
                    PlayAnimation(anims2[0], "PauseGameShow", false);
                    paused = true;
                    Time.timeScale = 0;
                }
                else
                {
                    PlayAnimation(anims2[0], "PauseGameShow", true);
                    paused = false;
                    Time.timeScale = 1;
                }
                break;
            case 1:
                anims2[1].enabled = true;
                if (!paused)
                {
                    PlayAnimation(anims2[1], "PauseMinigameShow", false);
                    paused = true;
                    Time.timeScale = 0;
                }
                else
                {
                    PlayAnimation(anims2[1], "PauseMinigameShow", true);
                    paused = false;
                    Time.timeScale = 1;
                }
                break;
        }
    }

    IEnumerator ExitToMenu()
    {
        switchScreen.speed = 1;
        switchScreen.Play("MenuSelectOption", 0, 0);
        fade.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        StartCoroutine(Functions.Fade(fade, 0, 1, 1));
    }

    IEnumerator GameOverDarts()
    {
        canPause = false;

        anims[0].transform.parent.transform.localScale = Vector3.one;
        anims[0].transform.parent.localPosition = Vector3.zero;
        PlayAnimation(anims[0], "GameOverMinigame", false);
        //anims[1].transform.GetChild(4).gameObject.GetComponent<Text>().text = "0";
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 0;
        anims[1].speed = 1;
        PlayAnimation(anims[1], "ScoreSheetShow", false);
        yield return new WaitForSecondsRealtime(1);
        float temp = tickets - scoreDarts;
        for (int i = 0; i <= scoreDarts; i++)
        {
            //Play ticket collecting sound
            anims[1].transform.GetChild(4).gameObject.GetComponent<Text>().text = (temp + i).ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }

        //parent animator affects scaling of children, so disable change to physics
        //anims[1].updateMode = AnimatorUpdateMode.AnimatePhysics;

        switch (scoreDarts)
        {
            case 0:
                break;
            case int a when a > 0 && a <= 4:
                //anims[1].Play("OneStar", 0, 0);
                PlayAnimation(star[0], "StarIndividual", false);
                break;
            case int a when a > 4 && a <= 11:
                //anims[1].Play("TwoStar", 0, 0);
                PlayAnimation(star[0], "StarIndividual", false);
                yield return new WaitForSecondsRealtime(0.5f);
                PlayAnimation(star[1], "StarIndividual", false);
                break;
            case int a when a > 11 && a <= 15:
                //anims[1].Play("ThreeStar", 0, 0);
                PlayAnimation(star[0], "StarIndividual", false);
                yield return new WaitForSecondsRealtime(0.5f);
                PlayAnimation(star[1], "StarIndividual", false);
                yield return new WaitForSecondsRealtime(0.5f);
                PlayAnimation(star[2], "StarIndividual", false);
                break;
        }
        //i set the menu animator back to unscaled time when the user presses the button
    }

    void PlayAnimation(Animator animator, string name, bool reversed)
    {
        /*bool count = false;
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name == "Speed")
            {
                count = true;
                break;
            }
        }
        //TODO: get correct layer for state
        if (!count)
        {
            //get controller of animator at runtime
            //AnimatorController controller = (animator.runtimeAnimatorController as AnimatorController);
            var controller = animator.runtimeAnimatorController;
            AnimatorControllerParameter paramater = new AnimatorControllerParameter();
            paramater.name = "Speed";
            paramater.type = AnimatorControllerParameterType.Float;
            paramater.defaultFloat = 1;
            controller.AddParameter(paramater);

            ChildAnimatorState[] states = controller.layers[0].stateMachine.states;
            AnimatorState state = states[0].state;

            //get correct state provided by name
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state.name == name)
                    state = controller.layers[0].stateMachine.states[i].state;
            }
            //state.speed = animator.GetFloat("Speed");
            print(state.name);
            state.speedParameterActive = true;
            state.speedParameter = "Speed";
        }*/
        if (!animator.enabled) animator.enabled = true;

        switch (reversed)
        {
            case false:
                animator.SetFloat("Speed", 1);
                animator.Play(name, 0, 0);
                break;
            case true:
                animator.SetFloat("Speed", -1);
                animator.Play(name, -1, 1);
                break;
        }
    }
    
    void BrokenFunctions()
    {
        /*yield return StartCoroutine(Functions.WaitFor(() => 
        { anims[0].Play("GameOverMinigame", 0, 0); 
        }));
        yield return StartCoroutine(Functions.WaitFor(() =>
        {
            anims[1].Play("ScoreSheetShow", 0, 0);
        }));*/
    }

    void Insta(float power)
    {
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * power, 0));
        GameObject cross = character.GetComponent<MouseController2D>().crosshair;
        Vector3 desiredPos = new Vector3(cross.transform.position.x, screenToWorld.y, -2);
        //Instantiate(circle, desiredPos, Quaternion.identity);
    }

    IEnumerator DisableCollisions()
    {
        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            foreach (GameObject child in NPCs)
            {
                if (child.transform.childCount > 0) 
                    child.transform.GetChild(0).gameObject.SetActive(false);
            }
            text = FindObjectOfType<TextBox>();
            yield return null;
        }
    }
    //alternatively setup a trigger boxCollider2D and invoke an action to begin boss fight
    IEnumerator SetupBossFight()
    {
        while (!centrePlatform)
        {
            if (character.transform.localPosition.x >= -1 && character.transform.localPosition.x <= 1)
                centrePlatform = true;
            yield return null;
        }
        StartCoroutine(BeginBossFight());
    }

    //TODO: find more efficient way
    IEnumerator BeginBossFight()
    {
        boss.GetComponent<Animator>().Play("fall", 0, 0);
        yield return new WaitForSeconds(2f);
        boss.transform.localPosition = new Vector3(boss.transform.localPosition.x, 9, boss.transform.localPosition.z);
        Vector3 backPos = background[2].transform.localPosition + new Vector3(0, -3.8f, 0);
        Vector3 platPos = bossPlatform.transform.localPosition + new Vector3(0, 1.6f, 0);
        Vector3 bossPos = boss.transform.localPosition + new Vector3(0, -4.7f, 0);
        Vector3 groundPos = foreground[2].transform.GetChild(0).localPosition + new Vector3(0, -3.8f, 0);
        StartCoroutine(Functions.Move(background[2].transform.localPosition, backPos, value => background[2].transform.localPosition = value));
        StartCoroutine(Functions.Move(bossPlatform.transform.localPosition, platPos, value => bossPlatform.transform.localPosition = value));
        StartCoroutine(Functions.Move(boss.transform.localPosition, bossPos, value => boss.transform.localPosition = value));
        StartCoroutine(Functions.Move(foreground[2].transform.GetChild(0).localPosition, groundPos, value => foreground[2].transform.GetChild(0).localPosition = value));
        yield return new WaitForSeconds(1);
        boss.GetComponent<Animator>().Play("idle", 0, 0);
    }

    public void RideCart(GameObject cart)
    {
        if (waitingForCart)
        {
            ridingCart = true;
            character.GetComponent<Rigidbody2D>().gravityScale = 0;
            mum.GetComponent<NPC_AI>().canMove = 0;
            character.transform.parent = cart.transform;
            character.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            character.transform.localRotation = Quaternion.identity;
            character.transform.localPosition = new Vector3(0, -3.5f, 1);
            controller.enabled = false;
            waitingForCart = false;
            StartCoroutine(RideTime());
        }
        if (ridingCart && timeLooking > 45)
        {
            ridingCart = false;
            character.GetComponent<Rigidbody2D>().gravityScale = controller.gravity;
            character.transform.rotation = Quaternion.identity;
            mum.GetComponent<NPC_AI>().canMove = 1;
            controller.canMove = 1;
            character.transform.parent = backgrounds.transform.parent.parent;
            character.GetComponent<SpriteRenderer>().sortingLayerName = "UIFront";
            character.transform.localPosition = new Vector3(character.transform.localPosition.x, character.transform.localPosition.y, -1);
            controller.enabled = true;
            timeLooking = 0;
        }
    }
    IEnumerator RideTime()
    {
        while (ridingCart)
        {
            timeLooking += Time.deltaTime;
            print(timeLooking);
            yield return null;
        }
    }

    void Bow(float power)
    {
        if (currentRoom == 2 && pocketDarts > 0)
        {
            pocketDarts--;
            countDartsText.text = pocketDarts.ToString();
            GameObject go = new();
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            Rigidbody2D rigid = go.AddComponent<Rigidbody2D>();
            Bullet bullet = go.AddComponent<Bullet>();
            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
            go.transform.position = character.transform.position;
            go.transform.localRotation = bow.transform.rotation;
            renderer.sprite = dart;
            rigid.velocity = go.transform.up * (power/2);
            collider.size = new Vector2(0.2f, 0.3f);
            collider.offset = new Vector2(0, 0.1f);
            LayerMask mask = LayerMask.GetMask("Player", "Default");
            rigid.excludeLayers += mask;
            collider.excludeLayers += mask;
            
            
            Destroy(go, 5);
        }
    }

    void BulletHit(float damage)
    {
        bossHealthBar.fillAmount = bossHealthBar.fillAmount - (damage/100);
    }

    void BossPhase(int phase)
    {
        switch (phase)
        {
            case 1:
                boss.GetComponent<Animator>().Play("");
                break;
            case 2:
                boss.GetComponent<Animator>().Play("");
                break;
        }
    }

    void FoundPlushie(GameObject plushie)
    {
        Destroy(plushie.transform.GetChild(0).gameObject);
        //plushie stand
        plushie.transform.parent = lightSprites[5].gameObject.transform;
        plushie.transform.localPosition = new Vector3(plushiePos[plushiesFound, 0], plushiePos[plushiesFound, 1], plushie.transform.localPosition.z);
        plushiesFound++;
        print("move");
    }
}
