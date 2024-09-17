using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    public enum TYPE
    {
        PLAY,
        EXIT
    }
    public TYPE type;
    public Vector2 position;
    public bool selected;
    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        position = rect.anchoredPosition;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.PLAY:
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                Actions.Begin.Invoke();
                break;
            case TYPE.EXIT:
                Application.Quit();
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
}
