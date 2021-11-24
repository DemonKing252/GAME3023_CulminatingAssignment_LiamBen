using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public UnityEvent dialogueEndEvent = new UnityEvent();

    private static DialogueManager sInstance;
    public static DialogueManager GetInstance()
    {
        return sInstance;
    }

    private Text dialogueText;

    // Start is called before the first frame update
    void Start()
    {
        sInstance = this;
        dialogueText = GetComponentInChildren<Text>();
    }


    public void StartNewDialogue(string text, float durationPerLetter = 0.05f)
    {
        StartCoroutine(AddLetter(text, durationPerLetter));
    }

    private IEnumerator AddLetter(string text, float duration)
    {
        dialogueText.text = "";

        int index = 0;
        while (index < text.Length)
        {
            // Wait for the delay we specified between letters
            yield return new WaitForSeconds(duration);
            dialogueText.text += text[index];
            index++;

        }
        dialogueEndEvent.Invoke();
    }

}
