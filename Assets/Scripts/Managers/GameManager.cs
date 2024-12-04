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
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.U2D;
using System.IO;
//using UnityEditor.Animations;

public class GameManager : MonoBehaviour
{
    // ------------------MANAGEMENT---------------------
    public Transform environment;
    List<Volume> volume = new List<Volume>();
    AudioSource audioSource;
    AudioSource ambience;
    Play_Menu_Sounds audioManager;
    PhysicsManager physics;
    float initialGravity;
    public bool paused;
    public bool canPause = true;
    bool settings;
    int currentRoom = 0;
    int stage = 0;
    bool mood = false;
    int dialoguesClicked;
    // ------------------CHARACTER---------------------
    protected CharacterController2D controller;
    protected GameObject character;
    Sprite charAppearance;
    GameObject boss;
    System.Random random = new System.Random();
    // ---------------------NPCS------------------------
    List<GameObject> NPCs = new List<GameObject>();
    List<GameObject> operatorNPCs = new List<GameObject>();
    GameObject mum;
    // ------------------OVERALL UI---------------------
    public GameObject UI;
    public Image TicketsUI;
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
    List<SpriteRenderer> flagTickets = new List<SpriteRenderer>();
    int ticketsToEnterTent = 22;
    Text scoreTicketsText;
    bool waitingForCart;
    bool ridingCart;
    int plushiesFound = 0;
    float[,] plushiePos = { {-5.5f, 4}, {-1.833f, 4}, {1.834f, 4}, {5.5f, 4} };
    List<AnimatorOverrideController> NPCDarkAnim = new List<AnimatorOverrideController>();
    // ---------------------LEVEL2------------------------
    GameObject f2_UI;
    GameObject stars;
    List<Animator> star = new List<Animator>();
    List<GameObject> balloons = new List<GameObject>();
    Sprite balloon;
    Image powerBar;
    GameObject powerBttnPhone;
    List<GameObject> darts = new List<GameObject>();
    Sprite dart;
    public int scoreDarts = 0;
    Text scoreDartsText;
    float dartDistanceFromCam = 2;
    float charDartDistanceFromCam = -2;
    bool hold;
    int startingDart;
    GameObject crosshair;
    public Vector3 desiredPos;
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
    public int bossPhase = 1;
    List<GameObject> bossLights = new List<GameObject>();
    GameObject fallingDart;
    GameObject bossPlushie;
    AudioSource reelAudio;
    // --------------------LEVEL 4-------------------------
    GameObject f4_UI;
    Image powerBarFish;
    Transform fishes;
    public GameObject bobber;
    public SplineContainer bobberSpline;
    int misses = 0;
    int tries = 0;
    public int scoreFish = 0;
    Text scoreFishText;
    public GameObject fishReelUI;
    bool startFishTimer;
    Text fishTimerText;
    float fishTimer = 1;
    GameObject bucket;
    public static bool hitNose;

    // -------------------SPRITE LIST----------------------
    public List<Sprite> darkSprites = new List<Sprite>();
    public List<SpriteRenderer> lightSprites = new List<SpriteRenderer>();

    public enum TEST
    {
        none,
        balloons,
        fish,
        boss,
        darkMode,
        skipINTRO
    }
    public TEST test = TEST.none;
    private void Awake()
    {
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            if (audio.gameObject.name == "MainAudio")
                audioSource = audio;
            else if (audio.gameObject.name == "AmbAudio")
                ambience = audio;
        }

        audioManager = GetComponent<Play_Menu_Sounds>();

        foreach (Transform child in environment)
        {
            if (child.GetComponent<Volume>())
            {
                volume.Add(child.GetComponent<Volume>());
                child.gameObject.SetActive(false);
            }
        }
        volume[1].gameObject.SetActive(true);

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
        else
            test = (TEST)MenuManager_2.test;

        float ratioX = (float)Screen.currentResolution.width / 1920;
        float desiredRatioY = 1080 * ratioX;
        float difference = (float)Screen.currentResolution.height / desiredRatioY;
        Vector3 scale = Vector3.zero;
        foreach (Transform child in backgrounds.transform)
        {
            background.Add(child.gameObject);
            scale = child.localScale;
            child.localScale = new Vector3(scale.x, scale.y * difference, 1);
        }
        foreach (Transform child in foregrounds.transform)
            foreground.Add(child.gameObject);

        background[1].transform.position += new Vector3(0, -2, 0);
        foreground[2].SetActive(true);
        boss = FindObjectOfType<Boss>().gameObject;
        foreground[2].SetActive(false);
        foreground[1].SetActive(true);
        foreach (Transform t1 in foreground[0].transform)
        {
            if (t1.name.Contains("Tent"))
                circusTent = t1.gameObject;
            if (t1.name.Contains("NoticeBoard"))
                noticeBoard = t1.gameObject;
            //if (t1.name.Contains("NPC"))
            if (t1.name == "NPCs")
            {
                foreach (Transform child in t1)
                {
                    if (child.name.Contains("Employee"))
                        operatorNPCs.Add(child.gameObject);
                    else if (child.name.Contains("Mum"))
                        mum = child.gameObject;
                    else
                        NPCs.Add(child.gameObject);
                }
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
            if (t1.name == "Plushie (Boss)")
                bossPlushie = t1.gameObject;
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
            if (t3.name == "Lights")
            {
                foreach (Transform child in t3)
                {
                    bossLights.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
            if (t3.name == "Bucket")
                bucket = t3.gameObject;
        }
        foreach (Transform t4 in foreground[3].transform)
        {
            if (t4.GetComponent<Canvas>())
                f4_UI = t4.gameObject;
            if (t4.name == "Bobber")
                bobber = t4.gameObject;
            if (t4.name == "Fishes")
                fishes = t4;
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
        foreach (Transform transform in circusTent.transform)
        {
            flagTickets.Add(transform.GetChild(0).GetComponent<SpriteRenderer>());
        }

        crosshair = character.GetComponent<MouseController2D>().crosshair;
        balloon = balloons[0].GetComponent<SpriteRenderer>().sprite;
        dart = darts[startingDart].GetComponent<SpriteRenderer>().sprite;
        bow = character.transform.GetChild(2).gameObject;

        scoreTicketsText = UI.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        countDartsText = UI.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Text>();

        scoreDartsText = f2_UI.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        powerBar = f2_UI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        powerBttnPhone = f2_UI.transform.GetChild(4).gameObject;
        if (Application.platform == RuntimePlatform.Android)
            powerBttnPhone.SetActive(true);
        else
            powerBttnPhone.SetActive(false);

        powerBarBoss = f3_UI.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        bossHealthBar = f3_UI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();

        fishTimerText = f4_UI.transform.GetChild(6).GetChild(0).GetComponent<Text>();
        fishReelUI = f4_UI.transform.GetChild(5).gameObject;
        fishReelUI.SetActive(false);
        scoreFishText = f4_UI.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        powerBarFish = f4_UI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        powerBttnPhone = f4_UI.transform.GetChild(4).gameObject;
        if (Application.platform == RuntimePlatform.Android)
            powerBttnPhone.SetActive(true);
        else
            powerBttnPhone.SetActive(false);
        string minutes = Mathf.Floor(fishTimer).ToString();
        string seconds = Mathf.Floor(((fishTimer) - Mathf.Floor(fishTimer)) * 60).ToString();
        Int32.TryParse(seconds, out int s);
        if (s < 10) seconds = "0" + seconds.ToString();
        fishTimerText.text = minutes + ":" + seconds;
        bobberSpline = bobber.transform.GetChild(0).GetComponent<SplineContainer>();

        scoreTicketsText.text = tickets.ToString();
        retryButton = anims[1].gameObject.transform.GetChild(0).gameObject;
        anims2[3].transform.GetChild(1).GetChild(1).gameObject.GetComponent<Slider>().value = MenuManager_2.musicVol;
        anims2[3].transform.GetChild(2).GetChild(1).gameObject.GetComponent<Slider>().value = MenuManager_2.sfxVol;
        anims2[3].transform.GetChild(3).GetChild(1).gameObject.GetComponent<Toggle>().isOn = MenuManager_2.crossAssist;
        anims2[3].transform.GetChild(4).GetChild(1).gameObject.GetComponent<Toggle>().isOn = MenuManager_2.wiggleCross;
        anims[1].transform.GetChild(4).gameObject.GetComponent<Text>().text = tickets.ToString();

        darkSprites.Add(Resources.Load<Sprite>("Images/Misc/BenchDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/PopcornDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Backgrounds/BalloonBackDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/BalloonDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/NoticeDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Misc/PlushieStandDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Fauna/LionDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/FOTLGateDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/FOTLPostDark"));
        darkSprites.Add(Resources.Load<Sprite>("Images/UI/Design/Ticket_and_Dart_Display_Board"));
        darkSprites.Add(Resources.Load<Sprite>("Images/Booths/Dark/Circus_Dark_Version_curtain_opened"));

        background[1].SetActive(true);
        lightSprites.Add(GameObject.FindGameObjectWithTag("ParkBench").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("PopcornCart").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("BalloonBackground").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("BalloonGame").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("Noticeboard").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("PlushieStand").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("LionCage").GetComponent<SpriteRenderer>());
        lightSprites.Add(GameObject.FindGameObjectWithTag("Gate").GetComponent<SpriteRenderer>());
        lightSprites.Add(lightSprites[7].transform.GetChild(0).GetComponent<SpriteRenderer>());
        TicketsUI = GameObject.FindGameObjectWithTag("Tickets").GetComponent<Image>();
        background[1].SetActive(false);

        fallingDart = Resources.Load<GameObject>("FallingDart");

        foreground[1].SetActive(false);
        foreground[3].SetActive(false);
        foreground[4].SetActive(false);
        background[3].SetActive(false);

        NPCDarkAnim.Add(Resources.Load<AnimatorOverrideController>("Controllers/CandyGirlDarkAnim"));
        NPCDarkAnim.Add(Resources.Load<AnimatorOverrideController>("Controllers/BalloonBoyDarkAnim"));
        //if (Application.platform == RuntimePlatform.Android)

    }
    void ScreenResolution()
    {
        float ratioX = (float)Screen.currentResolution.width / 1920;
        float desiredRatioY = 1080 * ratioX;
        float difference = (float)Screen.currentResolution.height / desiredRatioY;

        Vector3 scale = background[1].transform.localScale;
        background[1].transform.localScale = new Vector3(scale.x, scale.y * difference, 1);
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
        Actions.EnterRoom += value => { StartCoroutine(SwitchRooms(value)); };
        Actions.isOverDoor += DoorAnim;
        Actions.Back += showUI;
        Actions.Hold += Hold;
        Actions.Release += () => { timeLooking = 0; hold = false; StartCoroutine(Play_Menu_Sounds.PlayClip(4, MenuManager_2.sfxVol)); };
        Actions.BalloonType += value => { scoreDarts += value; };
        Actions.HitBalloon += ScoreDarts;
        Actions.Talk += Talk;
        Actions.FinishTalk += DoAction;
        Actions.Pause += Pause;
        Actions.RideCart += RideCart;
        Actions.BulletHit += BulletHit;
        Actions.BossPhase += BossPhase;
        Actions.FoundPlushie += FoundPlushie;
        Actions.ClickedDialogue += () => { dialoguesClicked++; };
        Actions.Reel += value => { StartCoroutine(FishMinigame(value)); };
        Actions.missedReel += () => { misses++; };
    }
    private void OnDisable()
    {
        Actions.EnterRoom -= value => { StartCoroutine(SwitchRooms(value)); };
        Actions.isOverDoor -= DoorAnim;
        Actions.Back -= showUI;
        Actions.Hold -= Hold;
        Actions.Release -= () => { timeLooking = 0; hold = false; };
        Actions.BalloonType -= value => { scoreDarts += value; };
        Actions.HitBalloon -= ScoreDarts;
        Actions.Talk -= Talk;
        Actions.FinishTalk -= DoAction;
        Actions.Pause -= Pause;
        Actions.RideCart -= RideCart;
        Actions.BulletHit -= BulletHit;
        Actions.BossPhase -= BossPhase;
        Actions.FoundPlushie -= FoundPlushie;
        Actions.ClickedDialogue -= () => { dialoguesClicked++; };
        Actions.Reel -= value => { StartCoroutine(FishMinigame(value)); };
        Actions.missedReel -= () => { misses++; };
    }
    // Start is called before the first frame update
    void Start()
    {
        initialGravity = character.GetComponent<Rigidbody2D>().gravityScale;

        switchScreen.gameObject.SetActive(false);
        masks[1].SetActive(false);
        fade.SetActive(true);
        StartCoroutine(Functions.Fade(fade, 1, 0, 1));
        Test();
    }
    void Test()
    {
        switch (test)
        {
            case TEST.none:
                StartCoroutine(Intro());
                break;
            case TEST.balloons:
                StartCoroutine(SwitchRooms(1));
                break;
            case TEST.fish:
                StartCoroutine(SwitchRooms(4));
                break;
            case TEST.boss:
                tickets = 22;
                scoreTicketsText.text = tickets.ToString();
                TransferParent();

                StartCoroutine(SwitchRooms(2));
                break;
            case TEST.darkMode:
                tickets = 22;
                scoreTicketsText.text = tickets.ToString();
                StartCoroutine(SwitchRooms(0));
                break;
        }
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);

        character.GetComponent<CharacterController2D>().canMove = 0;

        TextBox.Text(null, "???", "What is your name?", textBoxSpeed);
        TextBox.Text();
        //TextBox.Text($"Oh! Your name is {character.charName}?", 0.05f, true);
        //I* ouputs the input of the player
        //TextBox.Text(charAppearance, "I*", "Mum? Dad? Where did you go?", textBoxSpeed);
        TextBox.Text(mum.GetComponent<NPC_AI>().appearance, mum.GetComponent<NPC_AI>().charName, "We finally made it to the circus. Go play some minigames I*", textBoxSpeed);

        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        Vector3 pos = new Vector3(-8.5f, character.transform.localPosition.y, character.transform.localPosition.z);
        Vector3 dir = pos - character.transform.localPosition;
        float magnitude = dir.magnitude;
        if (Mathf.Sign(dir.x) == -1)
            character.transform.localScale = new Vector3(-1, 1, 1);
        StartCoroutine(Functions.Move(character.transform.localPosition, pos, value => character.transform.localPosition = value, 0.5f));
        while (magnitude > 0.1f)
        {
            dir = pos - character.transform.localPosition;
            magnitude = dir.magnitude;
            character.GetComponent<Animator>().SetFloat("velocity", 100);
            yield return null;
        }
        character.GetComponent<Animator>().SetFloat("velocity", 0);

        TextBox.Text(charAppearance, controller.charName, "Hm? What's this!?", textBoxSpeed);
        text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        //plushie stand
        bossPlushie.transform.parent = lightSprites[5].gameObject.transform;
        bossPlushie.transform.localPosition = new Vector3(plushiePos[plushiesFound, 0], plushiePos[plushiesFound, 1], bossPlushie.transform.localPosition.z);
        plushiesFound++;
        TextBox.Text(null, "", "You take the plushie from its place... but something feels wrong...", textBoxSpeed);

        character.GetComponent<CharacterController2D>().canMove = 1;
        StartCoroutine(DisableCollisions());
        fade.SetActive(false);
    }

    IEnumerator SwitchRooms(int n)
    {
        switch (n)
        {
            //this option brings user back to main circus
            case 0:
                yield return StartCoroutine(Transition());
                foreach (Collision2D collider in door) { collider.enabled = true; }
                switch (currentRoom)
                {
                    case 1:
                        character.transform.localPosition = new Vector3(18.7f, -3.06f, -1);
                        break;
                    case 2:
                        character.transform.localPosition = new Vector3(17, -3.06f, -1);
                        break;
                    case 3:
                        character.transform.localPosition = new Vector3(27.8f, -3.06f, -1);
                        break;
                }

                ambience.gameObject.SetActive(true);
                if (!mood)
                {
                    Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[0], MenuManager_2.musicVol);
                    Play_Menu_Sounds.ChangeClip(ambience, audioManager.musicClips[1], MenuManager_2.musicVol);
                }
                else
                {
                    Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[2], MenuManager_2.musicVol);
                    Play_Menu_Sounds.ChangeClip(ambience, audioManager.musicClips[3], MenuManager_2.musicVol);
                }

                currentRoom = 0;
                character.transform.localRotation = Quaternion.identity;
                character.GetComponent<Rigidbody2D>().gravityScale = initialGravity;
                character.GetComponent<CharacterController2D>().enabled = true;
                character.GetComponent<Animator>().SetBool("dart", false);
                character.GetComponent<Animator>().SetBool("rod", false);
                character.GetComponent<CircleCollider2D>().enabled = true;
                character.GetComponent<BoxCollider2D>().enabled = true;

                character.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                character.GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 1.6f);

                Camera.main.GetComponent<CameraFollow>().enabled = true;
                break;
            case 1:
                yield return StartCoroutine(Transition());
                Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[4], MenuManager_2.musicVol);
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
                    StartCoroutine(Play_Menu_Sounds.PlayClip(8, MenuManager_2.sfxVol));
                    TextBox.Text(charAppearance, controller.charName, "It appears I don't have enough tickets...", textBoxSpeed);
                    yield break;
                }
                yield return StartCoroutine(Transition());
                TicketsUI.sprite = darkSprites[darkSprites.Count - 2];
                countDartsText.text = pocketDarts.ToString();

                StartCoroutine(Functions.Fade(UI.transform.GetChild(0).gameObject, 1, 0, 2));
                StartCoroutine(Functions.Fade(UI.transform.GetChild(0).GetChild(0).gameObject, 1, 0, 2));
                StartCoroutine(Functions.Fade(UI.transform.GetChild(0).GetChild(1).gameObject, 1, 0, 2));

                StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(0).gameObject, 1, 0, 2));
                StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(0).GetChild(0).gameObject, 1, 0, 2));
                StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(1).gameObject, 1, 0, 2));

                StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(0).gameObject, 1, 0, 2));
                StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject, 1, 0, 2));
                StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(1).gameObject, 1, 0, 2));

                character.GetComponent<SpriteRenderer>().sortingLayerName = "Boss";
                audioSource.Stop();

                currentRoom = 2;
                tickets -= ticketsToEnterTent;
                scoreTicketsText.text = tickets.ToString();
                character.transform.localPosition = new Vector3(0, -33.5f, -1);
                //controller.canJump = true;
                Vector3 component = Camera.main.GetComponent<CameraFollow>().offset;
                Camera.main.transform.position = new Vector3(character.transform.position.x + component.x, character.transform.position.y + component.y, Camera.main.transform.position.z);
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                StartCoroutine(BeginBossFight());
                break;
            case 3:
                ExitBoard(isLookingAtBoard);
                yield break;
            case 4:
                yield return StartCoroutine(Transition());
                Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[5], MenuManager_2.musicVol);
                currentRoom = 3;
                character.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

                UI.SetActive(false);
                ResetFishingGame(true);
                break;
        }

        background[currentRoom].SetActive(true);
        foreground[currentRoom].SetActive(true);
        if (n == 0 && tickets >= ticketsToEnterTent && !mood)
        {
            mood = true;
            Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[2], MenuManager_2.musicVol);
            Play_Menu_Sounds.ChangeClip(ambience, audioManager.musicClips[3], MenuManager_2.musicVol);

            //Destroy(mum);
            //NPCs.Remove(mum);
            //yield return null;

            TransferParent();

            //destroy other NPCs
            physics.UpdateCollision();
            ChangeMood();
            StartCoroutine(Sad());
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
        ambience.gameObject.SetActive(false);
        audioSource.loop = true;

        switchScreen.gameObject.SetActive(true);
        masks[1].SetActive(true);
        character.GetComponent<MouseController2D>().fire = true;
        switchScreen.speed = 1;
        switchScreen.Play("MenuSelectOption", 0, 0);
        foreach (Collision2D collider in door) { collider.enabled = false; }

        character.GetComponent<BossDartController>().enabled = false;
        character.GetComponent<CharacterController2D>().enabled = false;
        character.GetComponent<MouseController2D>().enabled = false;
        character.GetComponent<RodController>().enabled = false;

        yield return new WaitForSeconds(1);
        //UIFront layer mask
        character.GetComponent<SpriteRenderer>().sortingLayerName = "UIFront";
        UI.SetActive(true);
        background[currentRoom].SetActive(false);
        foreground[currentRoom].SetActive(false);
    }
    void TransferParent()
    {
        mum.transform.parent = foreground[2].transform;
        mum.transform.localPosition = new Vector3(11, 3.5f, -1);
        mum.SetActive(false);
        mum.layer = 9;
        mum.GetComponent<SpriteRenderer>().sortingLayerName = "Boss";
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
            case GameButtons.TYPE.pauseBackFromGameToMenu:
                //re-use boolean to switch whether user is going back to menu or main game in other function
                settings = true;
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
                switch (currentRoom)
                {
                    case 1:
                        ResetDartsGame(false);
                        break;
                    case 3:
                        ResetFishingGame(false);
                        break;
                }
                break;
            case GameButtons.TYPE.resetMini:
                Pause();
                switch (currentRoom)
                {
                    case 1:
                        ResetDartsGame(true);
                        break;
                    case 3:
                        ResetFishingGame(true);
                        break;
                }
                break;
            case GameButtons.TYPE.pauseBackYes:
                canPause = true;
                //get rid of warning and unpause
                Pause();
                PlayAnimation(anims2[2], "WarningShow", true);
                switch (currentRoom)
                {
                    case int a when a != 0:
                        if (settings)
                        {
                            settings = false;
                            StartCoroutine(ExitToMenu());
                        }
                        else
                        {
                            StartCoroutine(SwitchRooms(0));
                        }
                        break;
                    case 0:
                        StartCoroutine(ExitToMenu());
                        break;
                }
                break;
            case GameButtons.TYPE.pauseBackNo:
                settings = false;
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
            case GameButtons.TYPE.PAUSE:
                {
                    Pause();
                    break;
                }
        }
    }
    public void Hold()
    {
        hold = true;
        powerBar.gameObject.SetActive(true);
        powerBarFish.gameObject.SetActive(true);
        powerBarBoss.gameObject.SetActive(true);
        timeLooking = 0;
        //number from 0.5 to 1
        rapidTime = (random.Next(2) / 2) + 0.5f;
        //number from 1 to 2.5
        slowTime = (random.Next(4) / 2) + 1;
        StartCoroutine(PowerBar());
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
                    Bullet(i);
                if (currentRoom == 3)
                    CastRod(i);
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
                        Bullet(i);
                    if (currentRoom == 3)
                        CastRod(i);
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
        powerBarFish.fillAmount = i / 100;
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
        yield return StartCoroutine(Functions.MoveCubic(darts[startingDart].transform.position, screenToWorld, value => darts[startingDart].transform.position = value, 1));
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
        Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[4], MenuManager_2.musicVol);

        retryButton.SetActive(true);
        Vector3 room = background[currentRoom].transform.position;
        character.transform.position = new Vector3(room.x, room.y - 8, 0);
        Vector3 pos = character.transform.localPosition;
        character.transform.localPosition = new Vector3(pos.x, pos.y, charDartDistanceFromCam);
        character.GetComponent<Animator>().Play("dartIdle", 0, 0);

        character.GetComponent<MouseController2D>().enabled = true;
        character.GetComponent<Rigidbody2D>().gravityScale = 0;
        character.GetComponent<Animator>().SetBool("dart", true);
        character.GetComponent<CircleCollider2D>().enabled = false;
        character.GetComponent<BoxCollider2D>().enabled = true;

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
                int count = 0;
                for (int i = 0; i < balloons.Count; i++)
                {
                    if (balloons[i].GetComponent<CircleCollider2D>().enabled)
                        count++;
                }
                if (count == 0)
                {
                    retryButton.SetActive(false);
                    StartCoroutine(GameOver(0));
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
            StartCoroutine(GameOver(0));
        }
    }
    void Bullet(float power)
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
            renderer.sortingLayerName = "Boss";
            rigid.velocity = go.transform.up * (power / 2);
            collider.size = new Vector2(0.2f, 0.3f);
            collider.offset = new Vector2(0, 0.1f);
            LayerMask mask = LayerMask.GetMask("Player", "Default");
            rigid.excludeLayers += mask;
            collider.excludeLayers += mask;
            Actions.BulletTarget.Invoke(boss);
            if (pocketDarts == 0)
            {
                Vector3 position = bucket.transform.localPosition + new Vector3(0, -8.2f, 0);
                StartCoroutine(Functions.Move(bucket.transform.localPosition, position, value => bucket.transform.localPosition = value, 1));
            }

            Destroy(go, 5);
        }
    }
    void CastRod(float power)
    {
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * (power / 100), 0));
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 vector = screenToWorld - origin;
        Vector2 vector2 = new Vector2(vector.x, vector.y);

        string path = Application.persistentDataPath + "/Pond_without_lilypads.png";
        try
        {
            ImageHeader.GetDimensions(path);
        }
        catch
        {
            path = @"C:\Users\123sj\Documents\GitHub\idk-what-to-call-this-game\Assets\Resources\Images\Backgrounds\Pond_without_lilypads.png";
        }
        
        Vector2Int imgSize = ImageHeader.GetDimensions(path);
        float bottomSection = 369/(float)imgSize.y;
        float middleSection = 4232/(float)imgSize.y;
        print(bottomSection);
        print(middleSection);
        float sus = middleSection * vector.y + (bottomSection * vector.y);
        print(vector.y);
        print(sus);
        vector2.y = sus;

        if (!startFishTimer)
        {
            startFishTimer = true;
            StartCoroutine(FishTimer());
        }

        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouse = mouseToWorld - character.transform.position;

        float angle = Mathf.Atan2(mouse.y, mouse.x) * Mathf.Rad2Deg - 90;
        Vector2 rotation = Functions.RotateVector2(vector2, angle);
        vector = new Vector3(rotation.x, rotation.y, 0);

        Vector3 rodTip = character.transform.position + (mouse * 0.55f);
        desiredPos = (character.transform.position + vector);

        //BezierKnot knot = bobberSpline.Spline.ToArray()[bobberSpline.Spline.Count - 1];
        //StartCoroutine(MoveLine());
        //IEnumerator MoveLine()
        //{
        //    for (float i = 0; i <= 1; i += Time.deltaTime)
        //    {
        //        bobber.transform.position = (rodTip + ((desiredPos - rodTip) * EasingFunctions.EaseOutCubic(i)));

        //        knot = bobberSpline.Spline.ToArray()[bobberSpline.Spline.Count - 1];
        //        knot.Position = bobberSpline.transform.InverseTransformPoint(rodTip);
        //        knot.Rotation = Quaternion.Inverse(bobberSpline.transform.rotation) * Quaternion.Euler(0, 0, angle);
        //        bobberSpline.Spline.SetKnot(bobberSpline.Spline.Count - 1, knot);

        //        yield return null;
        //    }
        //}

        StartCoroutine(Move());
        IEnumerator Move()
        {
            yield return StartCoroutine(Functions.MoveCubic(rodTip, desiredPos, value => bobber.transform.position = value, 1.5f));
            yield return StartCoroutine(Play_Menu_Sounds.PlayClip(11, MenuManager_2.sfxVol));
        }
        //bobber.transform.position = desiredPos;

    }
    IEnumerator FishTimer()
    {
        for (float i = fishTimer * 60; i >= 0; i-= Time.deltaTime)
        {
            string minutes = Mathf.Floor(i / 60).ToString();
            string seconds = Mathf.Floor(((i / 60) - Mathf.Floor(i / 60)) * 60).ToString();
            Int32.TryParse(seconds, out int t);
            if (t < 10) seconds = "0" + seconds.ToString();
            fishTimerText.text = minutes + ":" + seconds;
            yield return null;
        }
        startFishTimer = false;
        StartCoroutine(GameOver(1));
    }
    IEnumerator FishMinigame(FishAI.TYPE type)
    {
        reelAudio = Play_Menu_Sounds.CreateClipReturn(6, MenuManager_2.musicVol);
        float speed = 1;
        character.GetComponent<RodController>().hasFish = true;
        character.GetComponent<RodController>().fire = true;

        Transform fish = bobber.transform.GetChild(1);
        Vector3 vector = character.transform.position - bobber.transform.position;
        Vector3 away = (bobber.transform.position - character.transform.position).normalized;
        Vector3 lift = Vector3.zero;
        float magnitude = vector.magnitude;
        SwitchFishType(type, ref speed);
        fishReelUI.SetActive(true);

        while (magnitude > 4)
        {
            vector = character.transform.position - bobber.transform.position;
            magnitude = vector.magnitude;
            bobber.transform.position += vector.normalized * Time.deltaTime * speed;
            //fish moves away
            if (misses >= 3)
            {
                Destroy(reelAudio.gameObject);
                fish.parent = fishes;
                fish.GetComponent<FishAI>().hooked = false;
                fish.GetComponent<FishAI>().state = FishAI.STATES.idleSwim;
                fish.GetComponent<FishAI>().Switch();
                fish.GetComponent<CircleCollider2D>().enabled = true;
                character.GetComponent<RodController>().hasFish = false;
                bobber.transform.localPosition = new Vector3(0, -6, -2);
                fishReelUI.SetActive(false);
                character.GetComponent<RodController>().fire = false;
                misses = 0;
                tries = 0;
                yield break;
            }
            else if (tries != misses)
            {
                tries++;
                lift = bobber.transform.position + away * 2;
                fishReelUI.SetActive(false);
                StartCoroutine(SwimAway(lift, type));
            }
            yield return null;
        }

        Destroy(reelAudio.gameObject);
        scoreFish += (int)type;
        tickets += (int)type;
        misses = 0;
        tries = 0;
        scoreFishText.text = scoreFish.ToString();
        scoreTicketsText.text = tickets.ToString();

        fish.transform.localPosition = new Vector3(10, 0, 0);
        fish.GetComponent<FishAI>().enabled = false;
        fish.parent = fishes;

        fishReelUI.SetActive(false);
        bobber.transform.position += new Vector3(0, -6, -2);
        character.GetComponent<RodController>().hasFish = false;
        character.GetComponent<RodController>().fire = false;
        StartCoroutine(SpawnFish(fish));
    }
    IEnumerator SwimAway(Vector3 lift, FishAI.TYPE type)
    {
        yield return StartCoroutine(Functions.Move(bobber.transform.position, lift, value => bobber.transform.position = value, 1));
        fishReelUI.SetActive(true);
    }
    IEnumerator SpawnFish(Transform fish)
    {
        yield return new WaitForSeconds(3);
        // -6 => 6
        int randomX = Functions.random.Next(13) - 6;
        // -2 => 2
        int randomY = Functions.random.Next(5) - 2;
        fish.transform.localPosition = new Vector3(randomX, randomY, 0);
        fish.GetComponent<FishAI>().hooked = false;
        fish.GetComponent<FishAI>().enabled = true;
        fish.GetComponent<FishAI>().state = FishAI.STATES.idleSwim;
        fish.GetComponent<FishAI>().Switch();
        fish.GetComponent<CircleCollider2D>().enabled = true;
    }
    void SwitchFishType(FishAI.TYPE type, ref float speed)
    {
        switch (type)
        {
            case FishAI.TYPE.angler:
                fishReelUI.GetComponent<FishReeler>().speed = 2;
                fishReelUI.GetComponent<FishReeler>().sprite = 0;
                speed = 2;
                break;
            case FishAI.TYPE.bass:
                fishReelUI.GetComponent<FishReeler>().speed = 2.5f;
                fishReelUI.GetComponent<FishReeler>().sprite = 1;
                speed = 1.5f;
                break;
            case FishAI.TYPE.clown:
                fishReelUI.GetComponent<FishReeler>().speed = 3;
                fishReelUI.GetComponent<FishReeler>().sprite = 2;
                speed = 1;
                break;
        }
    }
    public void ResetFishingGame(bool fullReset)
    {
        startFishTimer = false;
        StopCoroutine(FishTimer());
        Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[5], MenuManager_2.musicVol);

        string minutes = Mathf.Floor(fishTimer).ToString();
        string seconds = Mathf.Floor(((fishTimer) - Mathf.Floor(fishTimer)) * 60).ToString();
        Int32.TryParse(seconds, out int s);
        if (s < 10) seconds = "0" + seconds.ToString();
        fishTimerText.text = minutes + ":" + seconds;

        bobber.transform.position += new Vector3(0, -6, -2);
        retryButton.SetActive(true);
        Vector3 room = background[currentRoom].transform.position;
        character.transform.position = new Vector3(room.x, room.y - 14.5f, 0);
        Vector3 pos = character.transform.localPosition;
        character.transform.localPosition = new Vector3(pos.x, pos.y, charDartDistanceFromCam);
        character.GetComponent<Animator>().Play("rodIdle", 0, 0);

        character.GetComponent<RodController>().enabled = true;
        character.GetComponent<Animator>().SetBool("rod", true);
        character.GetComponent<Rigidbody2D>().gravityScale = 0;
        character.GetComponent<CircleCollider2D>().enabled = false;
        character.GetComponent<BoxCollider2D>().enabled = false;

        /*        character.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.03451007f);
                character.GetComponent<BoxCollider2D>().size = new Vector2(0.06f, 0.07310224f);*/

        Camera.main.GetComponent<CameraFollow>().enabled = false;
        Camera.main.transform.position = new Vector3(room.x, room.y, -10);

        star[0].Rebind();
        star[1].Rebind();
        star[2].Rebind();
        powerBarFish.fillAmount = 0;
        scoreFish = 0;
        scoreFishText.text = scoreFish.ToString();
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

        /*TextBox.Text(operatorNPCs[0].GetComponent<NPC_AI>().appearance, operatorNPCs[0].GetComponent<NPC_AI>().charName, "Wow, nice job kiddo! Here, take some darts as a souvenir", textBoxSpeed);
        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }
        //Play animation
        anims[0].Play("Aquire", 0, 0);
        pocketDarts = 20;
        countDartsText.text = pocketDarts.ToString();*/
        //yield return new WaitForSeconds(2.5f);
        //TextBox.Text(charAppearance, controller.charName, "Thanks!", textBoxSpeed);

        TextBox.dialogueAudio = false;
        TextBox.Text(charAppearance, controller.charName, "...", textBoxSpeed);
        TextBox textBox = FindObjectOfType<TextBox>();
        while (textBox == null)
        {
            yield return null;
        }
        TextBox.dialogueAudio = true;
        TextBox.Text(charAppearance, controller.charName, "Dad? Where did you go?", textBoxSpeed);
    }
    public void ChangeMood()
    {
        charAppearance = Resources.Load<Sprite>("Images/Appearance/PlayerAppearanceSad");
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

        for (int i = 0; i < lightSprites.Count; i++)
        {
            lightSprites[i].sprite = darkSprites[i];
        }
        circusTent.GetComponent<SpriteRenderer>().sprite = darkSprites[darkSprites.Count - 1];

        NPCs[0].SetActive(false);
        NPCs[1].SetActive(false);
        NPCs[2].SetActive(false);
        NPCs[6].SetActive(false);
        NPCs[8].SetActive(false);

        NPCs[3].GetComponent<Animator>().Play("GirlEatDarkAnim");
        NPCs[4].GetComponent<Animator>().Play("BoyPointDarkAnim");

        NPCs[5].GetComponent<Animator>().runtimeAnimatorController = NPCDarkAnim[0];
        NPCs[7].GetComponent<Animator>().runtimeAnimatorController = NPCDarkAnim[1];

        volume[0].gameObject.SetActive(true);
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
                StartCoroutine(Functions.Fade(countDartsText.gameObject, 1, 0, 1));
                StartCoroutine(Functions.Fade(character, 1, 0, 1));
                StartCoroutine(Functions.Fade(mum, 1, 0, 1));
                controller.canMove = 0;
                //character.GetComponent<CharacterController2D>().enabled = false;
                foreach (GameObject npc in NPCs)
                {
                    StartCoroutine(Functions.Fade(npc, 1, 0, 1));
                    if (npc.GetComponent<NPC_AI>()) npc.GetComponent<NPC_AI>().canMove = 0;
                }
                door[1].gameObject.SetActive(false);
                pos = new Vector3(noticeBoard.transform.position.x, noticeBoard.transform.position.y, -10);
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                StartCoroutine(Functions.MoveCubic(Camera.main.transform.position, pos, value => Camera.main.transform.position = value, 1));
                //StartCoroutine(Functions.Move(arg => Camera.main.transform.position = arg, pos));
                StartCoroutine(Functions.Zoom(Camera.main, -9));
                break;
            case true:
                isLookingAtBoard = false;
                timeLooking = 0;
                StartCoroutine(Functions.Fade(UI.transform.GetChild(0).gameObject, 0, 1, 1));
                StartCoroutine(Functions.Fade(scoreTicketsText.gameObject, 0, 1, 1));
                StartCoroutine(Functions.Fade(countDartsText.gameObject, 0, 1, 1));
                StartCoroutine(Functions.Fade(character, 0, 1, 1));
                StartCoroutine(Functions.Fade(mum, 0, 1, 1));
                //TODO: when moving before completely zoomed out, camera snaps rather than smooths to position 
                controller.canMove = 1;
                //character.GetComponent<CharacterController2D>().enabled = true;
                foreach (GameObject npc in NPCs)
                {
                    StartCoroutine(Functions.Fade(npc, 0, 1, 1));
                    if (npc.GetComponent<NPC_AI>()) npc.GetComponent<NPC_AI>().canMove = 1;
                }
                door[1].gameObject.SetActive(true);
                CameraFollow component = Camera.main.GetComponent<CameraFollow>();
                pos = new Vector3(character.transform.position.x, character.transform.position.y, 0) + component.offset;
                component.enabled = true;
                StartCoroutine(Functions.MoveCubic(Camera.main.transform.position, pos, value => Camera.main.transform.position = value, 1));
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
            case string a when a.Contains("Fish"):
                StartCoroutine(SwitchRooms(4));
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
                    StartCoroutine(PauseTrue());
                }
                else
                {
                    PlayAnimation(anims2[0], "PauseGameShow", true);
                    StartCoroutine(PauseFalse());
                }
                break;
            case int a when a == 1 || a == 2 || a == 3:
                anims2[1].enabled = true;
                if (!paused)
                {
                    PlayAnimation(anims2[1], "PauseMinigameShow", false);
                    StartCoroutine(PauseTrue());
                }
                else
                {
                    PlayAnimation(anims2[1], "PauseMinigameShow", true);
                    StartCoroutine(PauseFalse());
                }
                break;
        }
    }
    IEnumerator PauseTrue()
    {
        paused = true;
        Time.timeScale = 0;
        UnityEngine.Rendering.Universal.ColorAdjustments colour;
        volume[1].profile.TryGet(out colour);
        bool check = colour.saturation.value < 0;
        float increment = check ? 1 : -1;
        for (float i = colour.saturation.value; check ? i <= 0 : i >= -100; i += increment)
        {
            colour.saturation.value = i;
            yield return null;
        }
    }
    IEnumerator PauseFalse()
    {
        paused = false;
        Time.timeScale = 1;
        UnityEngine.Rendering.Universal.ColorAdjustments colour;
        volume[1].profile.TryGet(out colour);
        bool check = colour.saturation.value < 0;
        float increment = check ? 1 : -1;
        for (float i = colour.saturation.value; check ? i <= 0 : i >= -100; i += increment)
        {
            colour.saturation.value = i;
            yield return null;
        }
    }

    IEnumerator ExitToMenu()
    {
        switchScreen.gameObject.SetActive(true);
        masks[1].SetActive(true);
        PlayAnimation(switchScreen, "MenuSelectOption", false);
        yield return null;
        yield return new WaitForSecondsRealtime(CurrentClipLength(switchScreen));
        SceneManager.LoadScene(0);
    }

    IEnumerator GameOver(int scoringSystem)
    {
        if (reelAudio != null)
        {
            Destroy(reelAudio.gameObject);
        }

        character.GetComponent<MouseController2D>().enabled = false;
        character.GetComponent<RodController>().enabled = false;

        float count = tickets;
        tickets += scoreDarts;
        scoreTicketsText.text = (tickets).ToString();

        int scoring = 0;
        switch (scoringSystem)
        {
            case 0:
                scoring = scoreDarts;
                star[0].Rebind();
                star[1].Rebind();
                star[2].Rebind();
                break;
            case 1:
                startFishTimer = false;
                StopCoroutine(FishTimer());
                scoring = scoreFish;
                break;
        }
        audioSource.loop = false;
        if (scoring == 0)
            Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[7], MenuManager_2.musicVol);
        else
            Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[8], MenuManager_2.musicVol);
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
        for (int i = 0; i < scoring; i++)
        {
            count++;
            //Play ticket collecting sound
            StartCoroutine(Play_Menu_Sounds.PlayClip(7, MenuManager_2.sfxVol));
            anims[1].transform.GetChild(4).gameObject.GetComponent<Text>().text = (count).ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }

        //parent animator affects scaling of children, so disable change to physics
        //anims[1].updateMode = AnimatorUpdateMode.AnimatePhysics;

        switch (scoring)
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
        CrossOutTickets();
        //i set the menu animator back to unscaled time when the user presses the button
    }

    void CrossOutTickets()
    {
        if (flagTickets[10].gameObject.activeSelf == false)
        {
            if (tickets <= 22)
            {
                for (int i = 0; i < Mathf.Floor((float)tickets / 2); i++)
                {
                    flagTickets[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < 11; i++)
                {
                    flagTickets[i].gameObject.SetActive(true);
                }
            }
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
            foreach (GameObject child in operatorNPCs)
            {
                if (child.transform.childCount > 0)
                    child.transform.GetChild(0).gameObject.SetActive(false);
            }
            if (mum.transform.childCount > 0)
                mum.transform.GetChild(0).gameObject.SetActive(false);
            text = FindObjectOfType<TextBox>();
            yield return null;
        }
    }
    //alternatively setup a trigger boxCollider2D and invoke an action to begin boss fight
    public void ResetBossFight()
    {
        StartCoroutine(Functions.Fade(UI.transform.GetChild(0).gameObject, 1, 0, 2));
        StartCoroutine(Functions.Fade(UI.transform.GetChild(0).GetChild(0).gameObject, 1, 0, 2));
        StartCoroutine(Functions.Fade(UI.transform.GetChild(0).GetChild(1).gameObject, 1, 0, 2));

        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(0).gameObject, 1, 0, 2));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(0).GetChild(0).gameObject, 1, 0, 2));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(1).gameObject, 1, 0, 2));

        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(0).gameObject, 1, 0, 2));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject, 1, 0, 2));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(1).gameObject, 1, 0, 2));

        character.transform.localPosition = new Vector3(0, -33.5f, -1);

        background[2].transform.localPosition = new Vector3(0, -29, 0);
        bossPlatform.transform.localPosition = new Vector3(0, -4.4f, 0);
        boss.transform.localPosition = new Vector3(5.7f, 18, -1);
        foreground[2].transform.GetChild(0).localPosition = new Vector3(0, 1.53f, 0);
    }

    //TODO: find more efficient way
    IEnumerator BeginBossFight()
    {
        yield return new WaitForSeconds(1);
        TextBox.Text(charAppearance, controller.name, "It's really dark in here!", textBoxSpeed);

        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.7f);

        StartCoroutine(Play_Menu_Sounds.PlayClip(13, MenuManager_2.sfxVol));
        TextBox.dialogueAudio = false;
        TextBox.Text(null, "???", "Help!", textBoxSpeed);

        text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        //spotlights on
        yield return new WaitForSeconds(1);
        bossLights[0].SetActive(true);
        bossLights[3].SetActive(true);

        StartCoroutine(Play_Menu_Sounds.PlayClip(2, MenuManager_2.sfxVol));
        bossLights[3].GetComponent<Light2D>().intensity = 0.0025f;

        yield return new WaitForSeconds(1);
        bossLights[1].SetActive(true);

        StartCoroutine(Play_Menu_Sounds.PlayClip(2, MenuManager_2.sfxVol));
        bossLights[3].GetComponent<Light2D>().intensity = 0.005f;

        yield return new WaitForSeconds(1);
        bossLights[2].SetActive(true);

        StartCoroutine(Play_Menu_Sounds.PlayClip(2, MenuManager_2.sfxVol));
        bossLights[3].GetComponent<Light2D>().intensity = 0.05f;

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(Functions.Fade(bossLights[3], 0.05f, 0.4f, 1));

        TextBox.dialogueAudio = true;
        TextBox.Text(charAppearance, controller.name, "Dad! Is That you!?!", textBoxSpeed);
        text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }
        //wheel parents away
        GameObject wheel = GameObject.FindGameObjectWithTag("TrappedParents");
        Vector3 wheelPos = wheel.transform.localPosition + new Vector3(10, 0, 0);
        yield return StartCoroutine(Functions.Move(wheel.transform.localPosition, wheelPos, value => wheel.transform.localPosition = value, 0.7f));

        boss.GetComponent<Animator>().Play("fall", 0, 0);
        yield return new WaitForSeconds(2f);
        boss.transform.localPosition = new Vector3(boss.transform.localPosition.x, 9, boss.transform.localPosition.z);
        Vector3 position = background[2].transform.localPosition + new Vector3(0, -3.8f, 0);
        StartCoroutine(Functions.MoveCubic(background[2].transform.localPosition, position, value => background[2].transform.localPosition = value, 1));
        position = bossPlatform.transform.localPosition + new Vector3(0, 1.6f, 0);
        StartCoroutine(Functions.MoveCubic(bossPlatform.transform.localPosition, position, value => bossPlatform.transform.localPosition = value, 1));
        position = boss.transform.localPosition + new Vector3(0, -6.7f, 0);
        StartCoroutine(Functions.MoveCubic(boss.transform.localPosition, position, value => boss.transform.localPosition = value, 1));
        position = foreground[2].transform.GetChild(0).localPosition + new Vector3(0, -3.8f, 0);
        StartCoroutine(Functions.MoveCubic(foreground[2].transform.GetChild(0).localPosition, position, value => foreground[2].transform.GetChild(0).localPosition = value, 1));
        yield return new WaitForSeconds(1);
        boss.GetComponent<Animator>().Play("taunt", 0, 0);
        StartCoroutine(Play_Menu_Sounds.PlayClip(9, MenuManager_2.sfxVol));
        TextBox.dialogueAudio = false;
        TextBox.Text(null, "Poppy", "Why are you here? You should be happy i took your father away. Now you can play at the circus all the time!", textBoxSpeed);

        text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        //drop dart on boss
        StartCoroutine(DropDart());
    }
    IEnumerator DropDart()
    {
        Vector3 increment = new Vector3(-1, 0, 0);
        Vector3 position = new Vector3(-4.5f, bucket.transform.localPosition.y, bucket.transform.localPosition.z);
        float magnitude = (position - bucket.transform.localPosition).magnitude;
        bool hasDropped = false;

        while (magnitude > 0.1f)
        {
            magnitude = (position - bucket.transform.localPosition).magnitude;
            bucket.transform.localPosition += increment * Time.deltaTime * 1.5f;
            if (bucket.transform.localPosition.x <= 5.8f && !hasDropped)
            {
                hasDropped = true;
                GameObject go = new();
                SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                Rigidbody2D rigid = go.AddComponent<Rigidbody2D>();
                Bullet bullet = go.AddComponent<Bullet>();
                BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                go.transform.position = bucket.transform.position;
                go.transform.localRotation = bow.transform.rotation;
                renderer.sprite = dart;
                renderer.sortingLayerName = "Boss";
                collider.size = new Vector2(0.2f, 0.3f);
                collider.offset = new Vector2(0, 0.1f);
                LayerMask mask = LayerMask.GetMask("Player", "Default");
                rigid.excludeLayers += mask;
                collider.excludeLayers += mask;

                Actions.BulletTarget.Invoke(boss);
                Destroy(go, 5);
            }
            yield return null;
        }

        position = bucket.transform.localPosition + new Vector3(0, -5.8f, 0);
        StartCoroutine(Functions.Move(bucket.transform.localPosition, position, value => bucket.transform.localPosition = value, 1));

        magnitude = (position - bucket.transform.localPosition).magnitude;
        while (magnitude > 0.1f)
        {
            magnitude = (position - bucket.transform.localPosition).magnitude;
            yield return null;
        }

        StartCoroutine(Functions.Fade(UI.transform.GetChild(0).gameObject, 0, 1, 1));
        StartCoroutine(Functions.Fade(UI.transform.GetChild(0).GetChild(0).gameObject, 0, 1, 1));
        StartCoroutine(Functions.Fade(UI.transform.GetChild(0).GetChild(1).gameObject, 0, 1, 1));

        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(0).gameObject, 0, 1, 1));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(0).GetChild(0).gameObject, 0, 1, 1));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(0).GetChild(1).gameObject, 0, 1, 1));

        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(0).gameObject, 0, 1, 1));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject, 0, 1, 1));
        StartCoroutine(Functions.Fade(f3_UI.transform.GetChild(1).GetChild(1).gameObject, 0, 1, 1));

        character.GetComponent<CharacterController2D>().enabled = true;
        character.GetComponent<BossDartController>().enabled = true;
        Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[6], MenuManager_2.musicVol);
        boss.GetComponent<Animator>().Play("idle", 0, 0);
    }
    IEnumerator PhaseTwo()
    {
        bossPhase = 2;
        character.GetComponent<BossDartController>().enabled = false;
        PlayAnimation(boss.GetComponent<Animator>(), "taunt", false);
        StartCoroutine(Play_Menu_Sounds.PlayClip(9, MenuManager_2.sfxVol));
        TextBox.Text(null, "Poppy", "What an ungrateful child, Did you really think it was going to be that easy!?", textBoxSpeed);

        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        character.GetComponent<BossDartController>().enabled = true;
        PlayAnimation(boss.GetComponent<Animator>(), "headSpin", false);
        StartCoroutine(FallingDarts(false));
    }
    IEnumerator PhaseThree()
    {
        bossPhase = 3;
        character.GetComponent<BossDartController>().enabled = false;
        PlayAnimation(boss.GetComponent<Animator>(), "idle", false);
        TextBox.Text(null, "Poppy", "You're seriously bursting my balloon. I'll make sure you and every other child never leave the circus!", textBoxSpeed);

        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        character.GetComponent<BossDartController>().enabled = true;
        PlayAnimation(boss.GetComponent<Animator>(), "headSpin", false);
        //juggole head
        StartCoroutine(FallingDarts(false));
    }
    public void LooseDarts()
    {
        //play animation
        if (pocketDarts > 0)
        {
            anims[3].transform.GetChild(0).GetComponent<Text>().text = "-2";
            PlayAnimation(anims[3], "LooseDarts", false);
            pocketDarts -= 2;
            countDartsText.text = pocketDarts.ToString();
        }
    }
    public void GainDarts()
    {
        if (pocketDarts <= 0)
        {
            anims[3].transform.GetChild(0).GetComponent<Text>().text = "+5";
            PlayAnimation(anims[3], "LooseDarts", false);
            pocketDarts += 5;
            countDartsText.text = pocketDarts.ToString();

            Vector3 position = bucket.transform.localPosition + new Vector3(0, 8.2f, 0);
            StartCoroutine(Functions.Move(bucket.transform.localPosition, position, value => bucket.transform.localPosition = value, 1));
        }
    }
    public IEnumerator RestartBoss()
    {
        anims[0].transform.parent.transform.localScale = Vector3.one;
        anims[0].transform.parent.localPosition = Vector3.zero;
        PlayAnimation(anims[0], "GameOverMinigame", false);
        yield return new WaitForSeconds(3);
        fade.SetActive(true);
        StartCoroutine(Fade(fade, 0, 1, 0.5f));
        yield return new WaitForSeconds(3);

        ResetBossFight();
        StartCoroutine(BeginBossFight());
    }
    IEnumerator BossDeath()
    {
        bossPhase = 4;
        character.GetComponent<BossDartController>().enabled = false;
        PlayAnimation(boss.GetComponent<Animator>(), "Spin", false);
        TextBox.Text(null, "Poppy", "GAHH!!", textBoxSpeed);

        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        //boss goes to centre
        Vector3 bossPos = boss.transform.localPosition + new Vector3(0, -9f, 0);
        yield return StartCoroutine(Functions.MoveCubic(boss.transform.localPosition, bossPos, value => boss.transform.localPosition = value, 1));
        Destroy(boss);
        yield return new WaitForSeconds(1);

        Vector3 backPos = background[2].transform.localPosition + new Vector3(0, 3.8f, 0);
        Vector3 platPos = bossPlatform.transform.localPosition + new Vector3(0, -1.6f, 0);
        Vector3 groundPos = foreground[2].transform.GetChild(0).localPosition + new Vector3(0, 3.8f, 0);
        StartCoroutine(Functions.MoveCubic(background[2].transform.localPosition, backPos, value => background[2].transform.localPosition = value, 1));
        StartCoroutine(Functions.MoveCubic(bossPlatform.transform.localPosition, platPos, value => bossPlatform.transform.localPosition = value, 1));
        yield return StartCoroutine(Functions.MoveCubic(foreground[2].transform.GetChild(0).localPosition, groundPos, value => foreground[2].transform.GetChild(0).localPosition = value, 1));

        StartCoroutine(Ending());
    }
    IEnumerator Ending()
    {
        canPause = false;
        foreground[4].SetActive(true);
        fade.SetActive(true);
        character.GetComponent<CharacterController2D>().enabled = false;
        character.GetComponent<BossDartController>().enabled = false;
        Vector3 centre = new Vector3(foreground[currentRoom].transform.position.x, character.transform.position.y, character.transform.position.z);
        Vector3 dir = centre - character.transform.position;
        float magnitude = dir.magnitude;
        if (Mathf.Sign(dir.x) == -1)
            character.transform.localScale = new Vector3(-1, 1, 1);
        character.GetComponent<Animator>().SetFloat("velocity", 100);
        StartCoroutine(Functions.Move(character.transform.position, centre, value => character.transform.position = value, 1));
        while (magnitude > 0.1)
        {
            dir = centre - character.transform.position;
            magnitude = dir.magnitude;
            yield return null;
        }
        character.GetComponent<Animator>().SetFloat("velocity", 0);

        TextBox.dialogueAudio = true;
        TextBox.Text(charAppearance, controller.charName, "Dad!?", textBoxSpeed);
        TextBox text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        Play_Menu_Sounds.ChangeClip(audioSource, audioManager.musicClips[8], MenuManager_2.musicVol);
        mum.SetActive(true);
        yield return new WaitForSeconds(3);

        float velocity = mum.GetComponent<Animator>().GetFloat("velocity");
        while (velocity > 0.1)
        {
            velocity = mum.GetComponent<Animator>().GetFloat("velocity");
            yield return null;
        }

        NPC_AI ai = mum.GetComponent<NPC_AI>();
        TextBox.Text(ai.appearance, ai.charName, controller.charName + "! Are you hurt!? Thank you for saving me.. I think that's enough circus for one night, how about we go home.", textBoxSpeed);
        text = FindObjectOfType<TextBox>();
        while (text != null)
        {
            yield return null;
        }

        EndFade();
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void EndFade()
    {
        Vector3 camPos = new Vector3(Camera.main.transform.position.x, -78, Camera.main.transform.position.z);
        StartCoroutine(Functions.Move(Camera.main.transform.position, camPos, value => Camera.main.transform.position = value, 0.25f));
        StartCoroutine(Functions.Fade(fade, 0, 1, 0.75f));
    }

    IEnumerator FallingDarts(bool hard)
    {
        yield return new WaitForSeconds(10);

        PlayAnimation(boss.GetComponent<Animator>(), "Spin", false);
        float initialX = boss.transform.localPosition.x;
        //boss goes to centre
        Vector3 bossPos = boss.transform.localPosition + new Vector3(0, -9, 0);
        yield return StartCoroutine(Functions.MoveCubic(boss.transform.localPosition, bossPos, value => boss.transform.localPosition = value, 1));
        boss.transform.localPosition = new Vector3(0, boss.transform.localPosition.y, boss.transform.localPosition.z);
        bossPos = boss.transform.localPosition + new Vector3(0, 9, 0);
        yield return StartCoroutine(Functions.MoveCubic(boss.transform.localPosition, bossPos, value => boss.transform.localPosition = value, 1));

        boss.GetComponent<Animator>().Play("removeHead", 0, 0);
        yield return new WaitForSeconds(CurrentClipLength(boss.GetComponent<Animator>()));
        boss.GetComponent<Animator>().Play("getItems", 0, 0);
        yield return new WaitForSeconds(CurrentClipLength(boss.GetComponent<Animator>()));
        yield return new WaitForSeconds(1);
        boss.GetComponent<Animator>().Play("juggleHeadTran");
        yield return new WaitForSeconds(0.2f);
        boss.GetComponent<Animator>().Play("juggleHead");

        float time = 0;
        bool init = false;
        while (time <= 15)
        {
            if (!hard)
            {
                if (Mathf.Floor(time) % 2 == 0 && init == false)
                {
                    init = true;
                    Instantiate(fallingDart, foreground[2].transform);
                }
                if ((Mathf.Floor(time) - 1) % 2 == 0)
                    init = false;
            }
            else
            {
                if (Mathf.Floor(time) % 2 == 0 && init == false)
                {
                    init = true;
                    Instantiate(fallingDart, foreground[2].transform);
                }
                if ((Mathf.Floor(time) - 1) % 2 == 0)
                {
                    init = false;
                    Instantiate(fallingDart, foreground[2].transform);
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        //boss goes back
        bossPos = boss.transform.localPosition + new Vector3(0, -9, 0);
        yield return StartCoroutine(Functions.MoveCubic(boss.transform.localPosition, bossPos, value => boss.transform.localPosition = value, 1));
        boss.transform.localPosition = new Vector3(initialX, boss.transform.localPosition.y, boss.transform.localPosition.z);
        bossPos = boss.transform.localPosition + new Vector3(0, 9, 0);
        yield return StartCoroutine(Functions.MoveCubic(boss.transform.localPosition, bossPos, value => boss.transform.localPosition = value, 1));

        PlayAnimation(boss.GetComponent<Animator>(), "juggleHeadTran", true);
        yield return new WaitForSeconds(0.2f);
        PlayAnimation(boss.GetComponent<Animator>(), "getItems", true);
        yield return new WaitForSeconds(CurrentClipLength(boss.GetComponent<Animator>()));
        PlayAnimation(boss.GetComponent<Animator>(), "removeHead", true);
        yield return new WaitForSeconds(CurrentClipLength(boss.GetComponent<Animator>()));

        switch (bossPhase)
        {
            case 2:
                PlayAnimation(boss.GetComponent<Animator>(), "headSpin", false);
                break;
            case 3:
                PlayAnimation(boss.GetComponent<Animator>(), "headSpin", false);
                break;
        }
    }

    void BulletHit(float damage)
    {
        bossHealthBar.fillAmount = bossHealthBar.fillAmount - (damage / 100);
    }

    void BossPhase(int phase)
    {
        switch (phase)
        {
            case 2:
                StartCoroutine(PhaseTwo());
                break;
            case 3:
                StartCoroutine(PhaseThree());
                break;
            case 4:
                StopAllCoroutines();
                StartCoroutine(BossDeath());
                break;
        }
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
            operatorNPCs[2].transform.GetChild(0).gameObject.SetActive(false);
            waitingForCart = false;
            StartCoroutine(RideTime());
        }
        if (ridingCart && timeLooking > 20 && !hold)
        {
            //re-use bool
            hold = true;
            tickets += 3;
            scoreTicketsText.text = tickets.ToString();
            TextBox.Text(null, "", "The height of the cart bolsters the wind and you catch flying tickets", textBoxSpeed);
        }
        if (ridingCart && timeLooking > 45)
        {
            hold = false;
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
            /*if (hold)
            {
                //stop and wait
                TextBox text = FindObjectOfType<TextBox>();
                while (text != null)
                {
                    yield return null;
                }
            }*/
            timeLooking += Time.deltaTime;
            yield return null;
        }
    }

    void FoundPlushie(GameObject plushie)
    {
        Destroy(plushie.transform.GetChild(0).gameObject);
        //plushie stand
        plushie.transform.parent = lightSprites[5].gameObject.transform;
        plushie.transform.localPosition = new Vector3(plushiePos[plushiesFound, 0], plushiePos[plushiesFound, 1], plushie.transform.localPosition.z);
        plushiesFound++;
        tickets += 3;
        scoreTicketsText.text = tickets.ToString();
        if (plushiesFound == 1) TextBox.Text(null, "", "You've found a plushie! You grab the tickets the plushie was holding and neatly fold it in your pocket. Any plushies you find will be placed on the plushie stand", textBoxSpeed);
    }
}
