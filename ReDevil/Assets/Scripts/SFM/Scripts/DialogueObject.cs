using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField]
    [TextArea(3, 10)]
    private string[] dialogue;
    public string[] Dialogue => dialogue;
    [SerializeField]
    private ResponseOptions[] responseOptions;
    public ResponseOptions[] Responses => responseOptions;

    public bool HasResponses => Responses != null && Responses.Length > 0;
}

[System.Serializable]
public struct ResponseOptions
{
    [SerializeField]
    private string responseText;
    [SerializeField]
    private DialogueObject dialogueObject;

    public string ResponseText => responseText;
    public DialogueObject DialogueObject => dialogueObject;
}

