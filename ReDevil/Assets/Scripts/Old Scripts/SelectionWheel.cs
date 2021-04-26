using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//how power ups were going to be selected.  I didn't program this.  ask dave about this in NCGamedev discord for questions
public class SelectionWheel : MonoBehaviour {

    public int numSlots = 1; //The number of slots in the selection wheel
    public int maxSlots = 32;
    public GameObject selectionSlot; //The slot game object for visualization
    public float slotRadius = 1.0f;

    public delegate void SelectAction(int index); //The event for when a slot on the selection wheel is selected
    public static event SelectAction OnSelected;

    private GameObject[] slots; //An array of all the slots
    
    private int selectedSlot = 0; //Index of the selected option
    private int lastSelected = 1;

    private Vector2 inputVector;
    private float angleRadians;
    private float angleDegrees;

    private int slotCount = 0;

    void Start()
    {
        slots = new GameObject[maxSlots];
        CreateSelectionWheel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Time.timeScale = 0.1f;
        }
        
        if (Input.GetKeyUp(KeyCode.J))
        {
            Time.timeScale = 1f;

            if (OnSelected != null)
            {
                OnSelected(selectedSlot);
            }

            Debug.Log(selectedSlot);
        }

        if (numSlots > slotCount)
        {
            AddSlot();
        }
        else if (numSlots < slotCount)
        {
            RemoveSlot();
        }

        GetInputVariables();

        if (Mathf.Abs(inputVector.x) > 0.5f || Mathf.Abs(inputVector.y) > 0.5f)
        {
            angleRadians = Mathf.Atan2(inputVector.x, inputVector.y);

            angleDegrees = angleRadians * 180 / Mathf.PI;
            angleDegrees = (angleDegrees + 360) % 360;

            selectedSlot = Mathf.FloorToInt(angleDegrees / (360 / slotCount));
        }
        else
        {
            selectedSlot = lastSelected;
        }

        if (selectedSlot != lastSelected)
        {
            slots[selectedSlot].GetComponent<SpriteRenderer>().color = Color.red;
            slots[lastSelected].GetComponent<SpriteRenderer>().color = Color.white;
            lastSelected = selectedSlot;
        }
    }

    //create wheel
    private void CreateSelectionWheel()
    {
        for (int i = 0; i < numSlots; i++)
        {
            slots[i] = (GameObject)Instantiate(selectionSlot, transform);
            slotCount++;
        }

        SetSlotPositions();
    }

    //add slot
    private void AddSlot()
    {
        if (slotCount < maxSlots)
        {
            slots[slotCount] = (GameObject)Instantiate(selectionSlot, transform);
            slotCount++;
        }

        SetSlotPositions();
    }

    //remove slot
    private void RemoveSlot()
    {
        if (slotCount > 0)
        {
            Destroy(slots[slotCount - 1]);
            slots[slotCount - 1] = null;
            slotCount--;
        }

        SetSlotPositions();
    }

    //set slot positions
    private void SetSlotPositions()
    {
        float slotDistance = 0f;
        for (int i = 2; i <= slotCount; i++)
        {
            slotDistance += (1f / i);
        }

        for (int i = 0; i < slotCount; i++)
        {
            float slotAngle = (360 / slotCount * i) + (360 / slotCount / 2);
            slots[i].transform.position = GetSlotPosition(transform.position, slotRadius * slotDistance, slotAngle);
        }
    }

    //get position on wheel circle
    private Vector3 GetSlotPosition(Vector3 center, float distance, float angle)
    {
        Vector3 pos;
        pos.x = center.x + (distance * Mathf.Sin(angle * Mathf.Deg2Rad));
        pos.y = center.y + (distance * Mathf.Cos(angle * Mathf.Deg2Rad));
        pos.z = center.z;
        return pos;
    }

    private void DeleteSelectionWheel()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Destroy(slots[i]);
            slots[i] = null;
        }
    }

    private void GetInputVariables()
    {
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        inputVector = Vector2.ClampMagnitude(inputVector, 1.0f);
    }
}
