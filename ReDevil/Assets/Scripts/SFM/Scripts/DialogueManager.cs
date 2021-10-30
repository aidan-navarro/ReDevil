using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager instance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion
    [SerializeField]
    private TextMeshProUGUI dialogueText;
    [SerializeField]
    private Animator dialogueBoxAnimator;
    [SerializeField]
    private GameObject continueTextObject;
    [SerializeField]
    private ResponseHandler responseHandler;
    [SerializeField]
    private bool instantText = true;
    [SerializeField]
    private bool isOpen;
    [SerializeField]
    private bool isWriting;
    [SerializeField]
    private float letterRate = 10.0f;
    private DialogueObject currentDialogue;

    private Queue<string> sentences = new Queue<string>();
    private string currentSentence;

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char> {'.', '!', '?'}, 0.6f ),
        new Punctuation(new HashSet<char> {',', ';', ';'}, 0.6f )
    };

    public void StartDialogue(DialogueObject dialogueObject)
    {
        dialogueBoxAnimator.SetBool("IsOpen", true);
        isOpen = true;
        sentences.Clear();

        foreach (string sentence in dialogueObject.Dialogue)
        {
            sentences.Enqueue(sentence);
        }
        currentDialogue = dialogueObject;

        AdvanceSentence();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    public void AdvanceSentence()
    {
        //if (!instantText)
        //{

        //    StopAllCoroutines();
        //    if (isWriting)
        //    {
        //        dialogueText.text = currentSentence;
        //        isWriting = false;
        //        continueTextObject.SetActive(true);
        //    }

        //    else
        //    {
        //        if (sentences.Count == 0)
        //        {
        //            EndDialogue();
        //            return;
        //        }
        //        currentSentence = sentences.Dequeue();
        //        continueTextObject.SetActive(false);
        //        StartCoroutine(TypeSentence(currentSentence));
        //    }
        //}
        //else
        //{
        //    if (sentences.Count == 0)
        //    {
        //        EndDialogue();
        //        return;
        //    }
        //    else
        //    {
        //        currentSentence = sentences.Dequeue();
        //        PrintSentence(currentSentence);
        //    }
        //}

        if (isWriting)
        {
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isWriting = false;
            continueTextObject.SetActive(true);
        }
        else if (sentences.Count == 0)
        {
            if (currentDialogue.HasResponses)
            {
                {
                    responseHandler.ShowResponses(currentDialogue.Responses);
                }
            }
            else
            {
                EndDialogue();
            }
        }
        else 
        {
            currentSentence = sentences.Dequeue();
            if (instantText)
            {
                continueTextObject.SetActive(false);
                StartCoroutine(TypeSentence(currentSentence));
            }
            else
            {
                PrintSentence(currentSentence);
            }
        }
    }


    IEnumerator TypeSentence(string sentence)
    {
        isWriting = true;
        dialogueText.text = "";
        char[] sentenceToType = sentence.ToCharArray();
        //foreach (char letter in sentence.ToCharArray())
        //{
        //    dialogueText.text += letter;
        //    yield return new WaitForSeconds(1.0f / letterRate);
        //}
        for (int i = 0; i < sentenceToType.Length; i++)
        {
            bool isLast = i >= sentenceToType.Length - 1;
            dialogueText.text += sentenceToType[i];
            if (IsPunctuation(sentenceToType[i], out float waitTime) && !isLast && !IsPunctuation(sentenceToType[i + 1], out _))
            {
                yield return new WaitForSeconds(1.0f / waitTime);
            }
            else
            {
                yield return new WaitForSeconds(1.0f / letterRate);
            }
        }
        continueTextObject.SetActive(true);
        isWriting = false;
    }

    private void PrintSentence(string sentence)
    {
        dialogueText.text = sentence;
        continueTextObject.SetActive(true);
    }

    public void EndDialogue()
    {
        dialogueBoxAnimator.SetBool("IsOpen", false);
        isOpen = false;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = default;
        return false;
    }

    public readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> puncations, float waitTime)
        {
            Punctuations = puncations;
            WaitTime = waitTime;
        }
    }

}
