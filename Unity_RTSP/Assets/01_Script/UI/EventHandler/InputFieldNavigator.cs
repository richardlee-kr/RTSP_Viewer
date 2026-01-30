using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InputFieldNavigator : MonoBehaviour
{
    [SerializeField] private bool focusOnStart;
    [SerializeField] private List<TMP_InputField> inputFields;
    private int currentIndex = 0;

    void Start()
    {
        if(focusOnStart)
        {
            inputFields[0].ActivateInputField();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            MoveNext();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                MovePrev();
            else
                MoveNext();
        }
    }

    void MoveNext()
    {
        if(currentIndex >= inputFields.Count - 1)
            return;

        currentIndex = (currentIndex + 1) % inputFields.Count;
        SelectInputField();
    }

    void MovePrev()
    {
        if(currentIndex <= 0)
            return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = inputFields.Count - 1;

        SelectInputField();
    }

    void SelectInputField()
    {
        EventSystem.current.SetSelectedGameObject(inputFields[currentIndex].gameObject);
        inputFields[currentIndex].ActivateInputField();
    }
}