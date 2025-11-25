using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; //New input system for Unity

[RequireComponent(typeof(InputField))]
public class TabBetween : MonoBehaviour
{
    public InputField nextField;
    InputField myField;

    void Start()
    {
        if (nextField == null)
        {
            Destroy(this);
            return;
        }
        myField = GetComponent<InputField>();
    }

    void Update()
    {
        if (myField.isFocused && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            nextField.Select();
            nextField.ActivateInputField();
        }
    }
}
