using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public enum TYPE
    {
        PLAY,
        SETTINGS,
        EXIT,
        BOXOPTION1,
        BOXOPTION2, 
        BOXOPTION3,
        BOXOPTION4,
        MENU,
        SFX_TEST,
        CROSS,
        WIGGLE
    }
    public TYPE type;
    public Vector2 position;
    public bool selected;
    RectTransform rect;
    public bool shouldFloat;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (transform.childCount > 0) animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        position = rect.anchoredPosition;
    }
    void Update()
    {
        if (shouldFloat) rect.anchoredPosition = new Vector2(position.x, position.y + Mathf.Sin(Time.timeSinceLevelLoad) * 10);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.PLAY:
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                animator.Play("pop");
                Actions.MenuBeginSound.Invoke();
                Actions.Begin.Invoke();
                break;
            case TYPE.SETTINGS:
                StartCoroutine(SettingsPop());
                break;
            case TYPE.EXIT:
                Application.Quit();
                break;
            case TYPE.BOXOPTION1:
                Actions.TextBoxColour(0);
                break;
            case TYPE.BOXOPTION2:
                Actions.TextBoxColour(1);
                break;
            case TYPE.BOXOPTION3:
                Actions.TextBoxColour(2);
                break;
            case TYPE.BOXOPTION4:
                Actions.TextBoxColour(3);
                break;
            case TYPE.MENU:
                Actions.Settings.Invoke(false);
                break;
            case TYPE.SFX_TEST:
                Actions.MenuBeginSound.Invoke();
                break;
            case TYPE.CROSS:
                Actions.Toggles.Invoke(MenuManager_2.crossAssist, (value => MenuManager_2.crossAssist = value));
                break;
            case TYPE.WIGGLE:
                Actions.Toggles.Invoke(MenuManager_2.wiggleCross, (value => MenuManager_2.wiggleCross = value));
                break;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    IEnumerator Move(Vector2 endPosition, float speed)
    {
        Vector2 currentPosition = rect.anchoredPosition;
        Vector2 distance = (endPosition - currentPosition);

        for (float i = 0; i <= 1; i += 0.01f * speed)
        {
            rect.anchoredPosition = currentPosition + distance * i;
            yield return null;
        }
    }

    public IEnumerator Selected()
    {
        yield return StartCoroutine(Move(new Vector2(position.x + 70, position.y), 1));
        yield return StartCoroutine(Move(new Vector2(position.x + 37, position.y), 1));
    }

    public void End() 
    {
        StartCoroutine(Move(position, 1));
    }

    IEnumerator SettingsPop()
    {
        animator.Play("pop");
        Actions.Settings.Invoke(true);
        yield return new WaitForSeconds(1);
        animator.Rebind();
    }
}
