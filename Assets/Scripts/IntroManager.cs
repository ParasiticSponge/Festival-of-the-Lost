using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public GameObject text;
    public Sprite testingSprite;
    public Sprite testingSpriteDark;
    public Canvas canvas;

    List<Animator> animators = new List<Animator>();
    List<SpriteRenderer> sprites = new List<SpriteRenderer>(); 
    int dial = 0;

    GameObject character;
    Sprite charAppearance;
    CharacterController2D controller;

    GameObject selectionBorder;
    GameObject selectionInner;
    List<GameObject> characters = new List<GameObject>();
    RuntimeAnimatorController[] controllers = new RuntimeAnimatorController[2];
    public static RuntimeAnimatorController chosenController;
    GameObject characterSelection;
    bool hasChanged;
    bool looking;

    GameObject scene1;
    GameObject scene2;
    PhysicsManager physicsManager;
    float textBoxSpeed = 0.005f;
    GameObject fade;

    public List<GameObject> door = new List<GameObject>();

    private void Awake()
    {
        controllers[0] = Resources.Load<AnimatorController>("dude");
        controllers[1] = Resources.Load<AnimatorOverrideController>("dude_blanket");
        scene1 = transform.GetChild(0).gameObject;
        scene2 = transform.GetChild(1).gameObject;
        scene1.SetActive(true);
        scene2.SetActive(false);
        //scene1
        foreach (Transform child in scene1.transform)
        {
            if (child.name == "Doors")
                door.Add(child.gameObject);
        }
        //scene1 canvas
        foreach (Transform _canvas in canvas.transform.GetChild(1))
        {
            if (_canvas.name == "SelectPanel")
            {
                characterSelection = _canvas.gameObject;
                foreach (Transform child in _canvas.transform)
                {
                    if (child.name == "SelectionChars")
                    {
                        for (int i = 0; i < child.childCount; i++)
                        {
                            characters.Add(child.GetChild(i).gameObject);
                        }
                    }
                    if (child.name == "SelectionInner")
                    {
                        selectionInner = child.gameObject;
                    }
                    if (child.name == "SelectionBorder")
                    {
                        selectionBorder = child.gameObject;
                    }
                }
            }
        }
        foreach (Transform _canvas in canvas.transform)
        {
            if (_canvas.name == "Fade")
                fade = _canvas.gameObject;
        }
        characterSelection.SetActive(false);

        physicsManager = FindObjectOfType<PhysicsManager>();
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.GetComponent<Animator>())
                animators.Add(child.GetComponent<Animator>());
            if (child.GetComponent<SpriteRenderer>())
                sprites.Add(child.GetComponent<SpriteRenderer>());
        }
        //StartCoroutine(Intro());
        if (MenuManager_2.textBoxColourLight == null)
        {
            MenuManager_2.textBoxColourLight = testingSprite;
            MenuManager_2.textBoxColourDark = testingSpriteDark;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Initiate();
        //scene1.SetActive(false);
        //scene2.SetActive(true);
    }

    void OnEnable()
    {
        Actions.ClickedDialogue += Initiate;
        Actions.isOverDoor += DoorAnim;
        Actions.EnterRoom += SwitchRoom;
        Actions.ChooseCharacter += SetBox;
    }
    void OnDisable()
    {
        Actions.ClickedDialogue -= Initiate;
        Actions.isOverDoor -= DoorAnim;
        Actions.EnterRoom -= SwitchRoom;
        Actions.ChooseCharacter -= SetBox;
    }
    IEnumerator Intro()
    {
        yield return StartCoroutine(Functions.Fade(text, 0, 1, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(Functions.Fade(text, 1, 0, 1));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public IEnumerator MumCall()
    {
        yield return StartCoroutine(Functions.Fade(fade, 1, 0, 1));
        fade.SetActive(false);
        yield return new WaitForSeconds(4);
        TextBox.Text(null, "mum", "Its time to get up! We've got a big day ahead of us! If we don't leave now we'll be late to the grand opening of the new circus in town!", textBoxSpeed);
    }
    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        animators[1].Play("dot_anim");
        animators[0].GetCurrentAnimatorClipInfo(0)[0].clip.wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(2);
        TextBox.Text(Resources.Load<Sprite>("IntroImages/PlayerBlanketIdle_1"), "You", "ugggghhhh...", textBoxSpeed);
    }
    public void GetUp()
    {
        Destroy(animators[1].gameObject);
        animators.RemoveAt(0);
        sprites[1].GetComponent<Animator>().enabled = false;
        sprites[1].sprite = Resources.Load<Sprite>("IntroImages/bed_5");
        character = Instantiate(Resources.Load<GameObject>("character"));
        controller = character.GetComponent<CharacterController2D>();
        charAppearance = controller.appearance;
        character.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("dude_blanket");
        character.transform.position = new Vector3(-6.8f, -3, -1f);
        physicsManager.UpdateCollision();
    }

    public void Initiate()
    {
        switch(dial)
        {
            case 0:
                StartCoroutine(MumCall());
                break;
            case 1:
                StartCoroutine(Wait());
                break;
            case 2:
                GetUp();
                break;
        }
        dial++;
    }

    public void DoorAnim(GameObject obj, bool visible)
    {
        obj.transform.GetChild(0).gameObject.SetActive(visible);
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
            case 1:
                switch (looking)
                {
                    case false:
                        looking = true;
                        hasChanged = true;
                        controller.canMove = 0;
                        characterSelection.SetActive(true);
                        Functions.PlayAnimation(characterSelection.GetComponent<Animator>(), "characterSelect", false);
                        door[0].gameObject.SetActive(false);
                        break;
                    case true:
                        looking = false;
                        controller.canMove = 1;
                        Functions.PlayAnimation(characterSelection.GetComponent<Animator>(), "characterSelect", true);
                        door[0].gameObject.SetActive(true);
                        yield return new WaitForSeconds(0.7f);
                        characterSelection.SetActive(false);
                        break;
                }
                /*yield return StartCoroutine(Transition());
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

                Camera.main.GetComponent<CameraFollow>().enabled = true;*/
                break;
            case 2:
                if (!hasChanged)
                {
                    TextBox.Text(Resources.Load<Sprite>("IntroImages/PlayerBlanketIdle_1"), controller.charName, "I should probably change first...", textBoxSpeed);
                    yield break;
                }

                Destroy(door[0]);
                yield return null;
                physicsManager.UpdateCollision();

                fade.SetActive(true);
                yield return StartCoroutine(Functions.Fade(fade, 0, 1.1f, 1));
                scene1.SetActive(false);
                scene2.SetActive(true);
                yield return null;
                physicsManager.UpdateCollision();
                yield return StartCoroutine(Functions.Fade(fade, 1, 0, 1));
                //yield return StartCoroutine(Transition());
                /*character.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                currentRoom = 2;
                tickets -= ticketsToEnterTent;
                scoreTicketsText.text = tickets.ToString();
                character.transform.localPosition = new Vector3(0, -33.5f, -1);
                controller.canJump = true;
                character.GetComponent<BossDartController>().enabled = true;
                Vector3 component = Camera.main.GetComponent<CameraFollow>().offset;
                Camera.main.transform.position = new Vector3(character.transform.position.x + component.x, character.transform.position.y + component.y, Camera.main.transform.position.z);
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                StartCoroutine(SetupBossFight());*/
                break;
        }

        /*switchScreen.StartPlayback();
        switchScreen.speed = -1;
        switchScreen.Play("MenuSelectOption", -1, float.NegativeInfinity);

        yield return new WaitForSeconds(1);
        switchScreen.gameObject.SetActive(false);
        masks[1].SetActive(false);*/
    }
    /*IEnumerator Transition()
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
    }*/

    public void SetBox(int number)
    {
        if (number == Enum.GetValues(typeof(CharacterSelection.TYPE)).Length - 1)
        {
            StartCoroutine(SwitchRooms(1));
        }
        else
        {
            selectionBorder.transform.position = characters[number].transform.position;
            selectionInner.transform.position = characters[number].transform.position;
            chosenController = controllers[number];
            character.GetComponent<Animator>().runtimeAnimatorController = chosenController;
        }
    }
}
