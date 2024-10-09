using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor.Animations;

public class GameManager : MonoBehaviour
{
    public PhysicsManager physics;
    GameObject character;
    Sprite charAppearance;
    string charName;
    System.Random random = new System.Random();

    public Image powerBar;
    List<GameObject> masks = new List<GameObject>();
    GameObject fade;
    public GameObject textBoxPrefab;
    public Animator switchScreen;
    public GameObject animationCanvas;
    public Animator gameCanvas;
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
    GameObject mum;
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

    bool mood = false;

    public Sprite testingSprite;
    public bool paused;
    public bool canPause = true;
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
                circusTent = t1.gameObject;
            if (t1.gameObject.name.Contains("NoticeBoard"))
                noticeBoard = t1.gameObject;
            if (t1.gameObject.name.Contains("NPC"))
            {
                NPCs.Add(t1.gameObject);
                if (t1.gameObject.name.Contains("Mum"))
                    mum = t1.gameObject;
            }
            if (t1.gameObject.name.Contains("Canvas"))
                f1_UI = t1.GetChild(0).gameObject;
        }
        foreach (Transform t2 in f2.transform)
        {
            if (t2.gameObject.name.Contains("Balloon"))
                balloons.Add(t2.gameObject);
            if (t2.gameObject.name.Contains("ThrowDart"))
                darts.Add(t2.gameObject);
        }
        foreach (Transform t in doors.transform)
        {
            door.Add(t.GetComponent<Collision2D>());
            //door2.Add(t.GetComponent<BoxCollider2D>());
        }
        foreach (Transform child in animationCanvas.transform)
        {
            if (child.gameObject.name == "GameOver")
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
        Actions.FinishTalk += DoAction;
        Actions.Pause += Pause;
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
        Actions.FinishTalk -= DoAction;
        Actions.Pause -= Pause;
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

        switchScreen.gameObject.SetActive(false);
        masks[1].SetActive(false);
        fade.SetActive(true);
        StartCoroutine(Functions.Fade(fade, 1));
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);

        /*TextBox.Text(null, "???", "What is your name?", 0.05f);
        TextBox.Text();
        TextBox.Text(null, "???", "...", 0.2f);
        //TextBox.Text($"Oh! Your name is {character.charName}?", 0.05f, true);
        //I* ouputs the input of the player
        TextBox.Text(charAppearance, "I*", "Mum? Dad? Where did you go?", 0.02f);*/
        fade.SetActive(false);
    }

    public void SwitchRoom(int n)
    {
        StartCoroutine(SwitchRooms(n));
    }

    IEnumerator SwitchRooms(int n)
    {
        if (n == 3)
        {
            ExitBoard(isLookingAtBoard);
            yield break;
        }
        if (n == 2 && tickets < ticketsToEnterTent)
        {
            TextBox.Text(charAppearance, charName, "It appears I don't have enough tickets...", 0.02f);
            yield break;
        }
        switchScreen.gameObject.SetActive(true);
        masks[1].SetActive(true);
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
                if (tickets >= ticketsToEnterTent && !mood)
                {
                    Destroy(mum);
                    physics.UpdateCollision();
                    NPCs.Remove(mum);
                    StartCoroutine(Sad());
                }
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

        yield return new WaitForSeconds(1);
        switchScreen.gameObject.SetActive(false);
        masks[1].SetActive(false);
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
                canPause = true;
                Time.timeScale = 1;
                StartCoroutine(SwitchRooms(0));
                PlayAnimation(anims[1], "ScoreSheetShow", true);
                break;
            case GameButtons.TYPE.pauseBack:
                paused = true;
                Time.timeScale = 0;
                anims[4].enabled = true;
                PlayAnimation(anims[4], "WarningShow", false);
                break;
            case GameButtons.TYPE.replayMini:
                canPause = true;
                Time.timeScale = 1;
                PlayAnimation(anims[1], "ScoreSheetShow", true);
                ResetDartsGame();
                break;
            case GameButtons.TYPE.resetMini:
                Pause();
                ResetDartsGame();
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
                PlayAnimation(anims[4], "WarningShow", true);
                StartCoroutine(SwitchRooms(0));
                break;
            case GameButtons.TYPE.pauseBackNo:
                //get rid of warning but don't unpause
                print(anims[4].gameObject.name);
                PlayAnimation(anims[4], "WarningShow", true);
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
        Vector3 screenToWorld;
        Vector3 desiredPos;
        GameObject cross = character.GetComponent<MouseController2D>().crosshair;
        for (float i = 0; i < 101; i++)
        {
            if (!hold) { Actions.Power.Invoke(i/100); break; }
            powerBar.fillAmount = i/100;
            screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * powerBar.fillAmount, 0));
            desiredPos = new Vector3(cross.transform.position.x, screenToWorld.y, cross.transform.position.z);
            if (cross.activeSelf)
                cross.transform.position = desiredPos;
            yield return null;
        }
        if (hold)
        {
            for (float i = 100; i > -1; i--)
            {
                if (!hold) { Actions.Power.Invoke(i / 100); break; }
                powerBar.fillAmount = i / 100;
                screenToWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * powerBar.fillAmount, 0));
                desiredPos = new Vector3(cross.transform.position.x, screenToWorld.y, cross.transform.position.z);
                if (cross.activeSelf)
                    cross.transform.position = desiredPos;
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
        powerBar.fillAmount = 0;

        //reset y to middle of screen
        GameObject cross = character.GetComponent<MouseController2D>().crosshair;
        Vector3 desiredPos = new Vector3(cross.transform.localPosition.x, 0, cross.transform.localPosition.z);
        if (cross.activeSelf)
            cross.transform.localPosition = desiredPos;
    }
    public void ResetDartsGame()
    {
        powerBar.fillAmount = 0;
        scoreDarts = 0;
        scoreDartsText.text = scoreDarts.ToString();
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
        Vector3 room = background[currentRoom].transform.position;
        character.transform.position = new Vector3(room.x, room.y - 12, 0);
        Vector3 pos = character.transform.localPosition;
        character.transform.localPosition = new Vector3(pos.x, pos.y, dartDistanceFromCam);
        character.GetComponent<Animator>().Play("dartIdle", 0, 0);

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
        Vector3 position = Camera.main.WorldToScreenPoint(new Vector3(character.transform.position.x, character.transform.position.y + 3, 0));
        anims[0].transform.parent.GetComponent<RectTransform>().position = new Vector3(position.x, position.y, 0);
        switch (hit)
        {
            case true:
                anims[0].Play("Nice", 0, 0);
                tickets++;
                scoreDarts++;
                scoreDartsText.text = scoreDarts.ToString();
                scoreTicketsText.text = tickets.ToString();
                break;
            case false:
                if (startingDart < darts.Count)
                    anims[0].Play("TryAgain", 0, 0);
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
        yield return new WaitForSeconds(1.5f);
        TextBox.Text(charAppearance, charName, "Mum? Dad? Where did you go?", 0.02f);
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
                        TextBox.Text(charAppearance, charName, "I didn't come here to look at paper all day!", 0.02f);
                        break;
                    case 1:
                        TextBox.Text(charAppearance, charName, "The board is filled with many things...", 0.02f);
                        break;
                    case 2:
                        TextBox.Text(charAppearance, charName, "I should get going", 0.02f);
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

    void ExitBoard(bool exit)
    {
        Vector3 pos;
        switch (exit)
        {
            case false:
                isLookingAtBoard = true;
                StartCoroutine(BoardLooking());
                StartCoroutine(Functions.Fade(f1_UI, 1));
                StartCoroutine(Functions.Fade(scoreTicketsText.gameObject, 1));
                StartCoroutine(Functions.Fade(character, 1));
                character.GetComponent<CharacterController2D>().enabled = false;
                foreach (GameObject npc in NPCs)
                {
                    StartCoroutine(Functions.Fade(npc, 1));
                    npc.GetComponent<NPC_AI>().canMove = 0;
                }
                door[2].gameObject.SetActive(false);
                pos = new Vector3(noticeBoard.transform.position.x, noticeBoard.transform.position.y, -10);
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                StartCoroutine(Functions.Move(Camera.main.transform.position, pos, (value => Camera.main.transform.position = value)));
                StartCoroutine(Functions.Zoom(Camera.main, -9));
                break;
            case true:
                isLookingAtBoard = false;
                timeLooking = 0;
                StartCoroutine(Functions.Fade(f1_UI, 0));
                StartCoroutine(Functions.Fade(scoreTicketsText.gameObject, 0));
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
                pos = new Vector3(character.transform.position.x, character.transform.position.y, 0) + component.offset;
                component.enabled = true;
                StartCoroutine(Functions.Move(Camera.main.transform.position, pos, (value => Camera.main.transform.position = value)));
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
        }
    }

    void Pause()
    {
        switch (currentRoom)
        {
            case 0:
                anims[2].enabled = true;
                if (!paused)
                {
                    PlayAnimation(anims[2], "PauseGameShow", false);
                    paused = true;
                    Time.timeScale = 0;
                }
                else
                {
                    PlayAnimation(anims[2], "PauseGameShow", true);
                    paused = false;
                    Time.timeScale = 1;
                }
                break;
            case 1:
                anims[3].enabled = true;
                if (!paused)
                {
                    PlayAnimation(anims[3], "PauseMinigameShow", false);
                    paused = true;
                    Time.timeScale = 0;
                }
                else
                {
                    PlayAnimation(anims[3], "PauseMinigameShow", true);
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
        StartCoroutine(Functions.Fade(fade, 0));
    }

    IEnumerator GameOverDarts()
    {
        canPause = false;
        anims[1].enabled = true;
        /*yield return StartCoroutine(Functions.WaitFor(() => 
        { anims[0].Play("GameOverMinigame", 0, 0); 
        }));
        yield return StartCoroutine(Functions.WaitFor(() =>
        {
            anims[1].Play("ScoreSheetShow", 0, 0);
        }));*/
        anims[0].transform.parent.transform.localScale = Vector3.one;
        anims[0].transform.parent.localPosition = Vector3.zero;
        anims[0].Play("GameOverMinigame", 0, 0);
        anims[1].transform.GetChild(5).gameObject.GetComponent<Text>().text = "0";
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 0;
        anims[1].speed = 1;
        PlayAnimation(anims[1], "ScoreSheetShow", false);
        yield return new WaitForSecondsRealtime(1);
        for (int i = 0; i <= scoreDarts; i++)
        {
            //Play ticket collecting sound
            anims[1].transform.GetChild(5).gameObject.GetComponent<Text>().text = i.ToString();
            yield return new WaitForSecondsRealtime(0.3f);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        switch (scoreDarts)
        {
            case 0:
                break;
            case int a when a > 0 && a <= 2:
                anims[1].Play("OneStar", 0, 0);
                break;
            case int a when a > 2 && a <= 4:
                anims[1].Play("TwoStar", 0, 0);
                break;
            case 5:
                anims[1].Play("ThreeStar", 0, 0);
                break;
        }
    }

    void PlayAnimation(Animator animator, string name, bool reversed)
    {
        bool count = false;
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
            AnimatorController controller = (animator.runtimeAnimatorController as AnimatorController);
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
        }

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
}
