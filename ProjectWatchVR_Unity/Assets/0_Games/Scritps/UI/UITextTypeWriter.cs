using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
// attach to UI Text component (with the full text already there)

public class UITextTypeWriter : MonoBehaviour
{

    Text txt;
    string story;

    public float durationPerText = 0.125f;
    [SerializeField]
    UnityEvent OnStart;

    [SerializeField]
    UnityEvent OnComplete;
    void Awake()
    {
        txt = GetComponent<Text>();
      
    }
    public Coroutine StartShow(string text)
    {
        // TODO: add optional delay when to start
        return StartCoroutine(PlayText(text));
    }
    
    IEnumerator PlayText(string text)
    {
        if (OnStart != null)
        {
            OnStart.Invoke();
        }
        story = text;
        txt.text = "";
        yield return new WaitForSeconds(0.3f);
        foreach (char c in story)
        {
            txt.text += c;
            yield return new WaitForSeconds(durationPerText);
        }

        if (OnComplete != null)
        {
            OnComplete.Invoke();
        }
    }

}