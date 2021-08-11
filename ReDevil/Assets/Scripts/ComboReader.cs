using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComboReader : MonoBehaviour
{
    [SerializeField]
    private List<KeyCode> comboKeys = new List<KeyCode>();
    [SerializeField]
    private float waitTimeBetweenKeys = 2.0f;
    private Queue<KeyCode> comboQueue = new Queue<KeyCode>();
    private KeyCode currentKey;
    private float time;
    private float comboProgress;
    private bool comboStarted = false;

    public UnityEvent ComboStarted;
    public UnityEvent ComboFailed;
    public UnityEvent ComboFinished;
    // Start is called before the first frame update
    void Start()
    {
        ResetComboQueue();
    }

    private void FillComboQueue()
    {
        comboQueue.Clear();
        List<KeyCode> comboList = comboKeys;
        foreach (KeyCode comboKey in comboList)
        {
            comboQueue.Enqueue(comboKey);
        }
    }

    private void ResetComboQueue()
    {
        FillComboQueue();
        EnqueueNextComboKey();
        comboStarted = false;
    }

    private void EnqueueNextComboKey()
    {
        time = 0;
        currentKey = comboQueue.Dequeue();
    }

    // Update is called once per frame
    void Update()
    {
        if (comboStarted)
        {
            time += Time.deltaTime;
            if (Input.GetKeyDown(currentKey))
            {
                if (comboQueue.Count > 0)
                {
                    Debug.Log(currentKey);
                    EnqueueNextComboKey();
                }
                else
                {
                    ComboFinished?.Invoke();
                    Debug.Log("ComboFinished");
                    ResetComboQueue();
                }
            }
            else if (time >= waitTimeBetweenKeys)
            {
                ComboFailed?.Invoke();
                Debug.Log("ComboFailed");
                ResetComboQueue();    
            }
        }
        else
        {
            if (Input.GetKeyDown(currentKey))
            {
                comboStarted = true;
                ComboStarted?.Invoke();
                EnqueueNextComboKey();
                Debug.Log("ComboStarted");
            }
        }
    }
}
