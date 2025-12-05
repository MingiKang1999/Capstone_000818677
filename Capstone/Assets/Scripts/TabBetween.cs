/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; //New input system for Unity

// Tab Between Inputfields
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
