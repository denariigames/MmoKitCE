using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class InputFieldWrapper : MonoBehaviour
{
    public InputField unityInputField;
    public TMP_InputField textMeshInputField;
    private string _textValue = null;
    public virtual string text
    {
        get
        {
            if (unityInputField != null) return unityInputField.text;
            if (textMeshInputField != null) return textMeshInputField.text;
            return _textValue;
        }

        set
        {
            _textValue = value;
            if (unityInputField != null) unityInputField.text = value;
            if (textMeshInputField != null) textMeshInputField.text = value;
        }
    }

    public virtual bool interactable
    {
        get
        {
            if (unityInputField != null) return unityInputField.interactable;
            if (textMeshInputField != null) return textMeshInputField.interactable;
            return false;
        }

        set
        {
            if (unityInputField != null) unityInputField.interactable = value;
            if (textMeshInputField != null) textMeshInputField.interactable = value;
        }
    }

    public virtual bool multiLine
    {
        get
        {
            if (unityInputField != null) return unityInputField.multiLine;
            if (textMeshInputField != null) return textMeshInputField.multiLine;
            return false;
        }
    }

    public bool isFocused
    {
        get
        {
            bool result = false;
            if (unityInputField != null) result = unityInputField.isFocused;
            if (textMeshInputField != null) result = result || textMeshInputField.isFocused;
            return result;
        }
    }

    public virtual UnityEvent<string> onValueChanged
    {
        get
        {
            if (unityInputField != null) return unityInputField.onValueChanged;
            if (textMeshInputField != null) return textMeshInputField.onValueChanged;
            return null;
        }

        set
        {
            if (unityInputField != null) unityInputField.onValueChanged = value as InputField.OnChangeEvent;
            if (textMeshInputField != null) textMeshInputField.onValueChanged = value as TMP_InputField.OnChangeEvent;
        }
    }

    public virtual int characterLimit
    {
        get
        {
            if (unityInputField != null) return unityInputField.characterLimit;
            if (textMeshInputField != null) return textMeshInputField.characterLimit;
            return 0;
        }
        set
        {
            if (unityInputField != null) unityInputField.characterLimit = value;
            if (textMeshInputField != null) textMeshInputField.characterLimit = value;
        }
    }

    public virtual InputField.ContentType contentType
    {
        get
        {
            if (unityInputField != null) return unityInputField.contentType;
            if (textMeshInputField != null)
            {
                switch (textMeshInputField.contentType)
                {
                    case TMP_InputField.ContentType.Standard:
                        return InputField.ContentType.Standard;
                    case TMP_InputField.ContentType.Autocorrected:
                        return InputField.ContentType.Autocorrected;
                    case TMP_InputField.ContentType.IntegerNumber:
                        return InputField.ContentType.IntegerNumber;
                    case TMP_InputField.ContentType.DecimalNumber:
                        return InputField.ContentType.DecimalNumber;
                    case TMP_InputField.ContentType.Alphanumeric:
                        return InputField.ContentType.Alphanumeric;
                    case TMP_InputField.ContentType.Name:
                        return InputField.ContentType.Name;
                    case TMP_InputField.ContentType.EmailAddress:
                        return InputField.ContentType.EmailAddress;
                    case TMP_InputField.ContentType.Password:
                        return InputField.ContentType.Password;
                    case TMP_InputField.ContentType.Pin:
                        return InputField.ContentType.Pin;
                    case TMP_InputField.ContentType.Custom:
                        return InputField.ContentType.Custom;
                }
            }
            return InputField.ContentType.Standard;
        }

        set
        {
            if (unityInputField != null) unityInputField.contentType = value;
            if (textMeshInputField != null)
            {
                switch (value)
                {
                    case InputField.ContentType.Standard:
                        textMeshInputField.contentType = TMP_InputField.ContentType.Standard;
                        break;
                    case InputField.ContentType.Autocorrected:
                        textMeshInputField.contentType = TMP_InputField.ContentType.Autocorrected;
                        break;
                    case InputField.ContentType.IntegerNumber:
                        textMeshInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                        break;
                    case InputField.ContentType.DecimalNumber:
                        textMeshInputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                        break;
                    case InputField.ContentType.Alphanumeric:
                        textMeshInputField.contentType = TMP_InputField.ContentType.Alphanumeric;
                        break;
                    case InputField.ContentType.Name:
                        textMeshInputField.contentType = TMP_InputField.ContentType.Name;
                        break;
                    case InputField.ContentType.EmailAddress:
                        textMeshInputField.contentType = TMP_InputField.ContentType.EmailAddress;
                        break;
                    case InputField.ContentType.Password:
                        textMeshInputField.contentType = TMP_InputField.ContentType.Password;
                        break;
                    case InputField.ContentType.Pin:
                        textMeshInputField.contentType = TMP_InputField.ContentType.Pin;
                        break;
                    case InputField.ContentType.Custom:
                        textMeshInputField.contentType = TMP_InputField.ContentType.Custom;
                        break;
                }
            }
        }
    }

    public virtual InputField.InputType inputType
    {
        get
        {
            if (unityInputField != null) return unityInputField.inputType;
            if (textMeshInputField != null)
            {
                switch (textMeshInputField.inputType)
                {
                    case TMP_InputField.InputType.Standard:
                        return InputField.InputType.Standard;
                    case TMP_InputField.InputType.AutoCorrect:
                        return InputField.InputType.AutoCorrect;
                    case TMP_InputField.InputType.Password:
                        return InputField.InputType.Password;
                }
            }
            return InputField.InputType.Standard;
        }

        set
        {
            if (unityInputField != null) unityInputField.inputType = value;
            if (textMeshInputField != null)
            {
                switch (value)
                {
                    case InputField.InputType.Standard:
                        textMeshInputField.inputType = TMP_InputField.InputType.Standard;
                        break;
                    case InputField.InputType.AutoCorrect:
                        textMeshInputField.inputType = TMP_InputField.InputType.AutoCorrect;
                        break;
                    case InputField.InputType.Password:
                        textMeshInputField.inputType = TMP_InputField.InputType.Password;
                        break;
                }
            }
        }
    }

    public virtual InputField.LineType lineType
    {
        get
        {
            if (unityInputField != null) return unityInputField.lineType;
            if (textMeshInputField != null)
            {
                switch (textMeshInputField.lineType)
                {
                    case TMP_InputField.LineType.SingleLine:
                        return InputField.LineType.SingleLine;
                    case TMP_InputField.LineType.MultiLineSubmit:
                        return InputField.LineType.MultiLineSubmit;
                    case TMP_InputField.LineType.MultiLineNewline:
                        return InputField.LineType.MultiLineNewline;
                }
            }
            return InputField.LineType.SingleLine;
        }

        set
        {
            if (unityInputField != null) unityInputField.lineType = value;
            if (textMeshInputField != null)
            {
                switch (value)
                {
                    case InputField.LineType.SingleLine:
                        textMeshInputField.lineType = TMP_InputField.LineType.SingleLine;
                        break;
                    case InputField.LineType.MultiLineSubmit:
                        textMeshInputField.lineType = TMP_InputField.LineType.MultiLineSubmit;
                        break;
                    case InputField.LineType.MultiLineNewline:
                        textMeshInputField.lineType = TMP_InputField.LineType.MultiLineNewline;
                        break;
                }
            }
        }
    }

    public Graphic placeholder
    {
        get
        {
            if (unityInputField != null)
                return unityInputField.placeholder;
            if (textMeshInputField != null)
                return textMeshInputField.placeholder;
            return null;
        }
    }

    private void Awake()
    {
        if (unityInputField == null) unityInputField = GetComponent<InputField>();
        if (textMeshInputField == null) textMeshInputField = GetComponent<TMP_InputField>();
        if (_textValue != null)
            text = _textValue;
    }

    private void OnDestroy()
    {
        unityInputField = null;
        textMeshInputField = null;
        _textValue = null;
    }

    public void SetGameObjectActive(bool isActive)
    {
        if (unityInputField != null)
            unityInputField.gameObject.SetActive(isActive);
        if (textMeshInputField != null)
            textMeshInputField.gameObject.SetActive(isActive);
        gameObject.SetActive(isActive);
    }

    public void DeactivateInputField()
    {
        if (unityInputField != null) unityInputField.DeactivateInputField();
        if (textMeshInputField != null) textMeshInputField.DeactivateInputField();
    }

    public void Select()
    {
        if (unityInputField != null) unityInputField.Select();
        if (textMeshInputField != null) textMeshInputField.Select();
    }

    public void ActivateInputField()
    {
        if (unityInputField != null) unityInputField.ActivateInputField();
        if (textMeshInputField != null) textMeshInputField.ActivateInputField();
    }

    public void MoveTextStart(bool shift)
    {
        if (unityInputField != null) unityInputField.MoveTextStart(shift);
        if (textMeshInputField != null) textMeshInputField.MoveTextStart(shift);
    }

    public void MoveTextEnd(bool shift)
    {
        if (unityInputField != null) unityInputField.MoveTextEnd(shift);
        if (textMeshInputField != null) textMeshInputField.MoveTextEnd(shift);
    }

    public void SetTextWithoutNotify(string text)
    {
        if (unityInputField != null) unityInputField.SetTextWithoutNotify(text);
        if (textMeshInputField != null) textMeshInputField.SetTextWithoutNotify(text);
    }

    [ContextMenu("Set Attached Input Field Component To Field")]
    public void SetAttachedInputFieldComponentToField()
    {
        unityInputField = GetComponent<InputField>();
    }

    [ContextMenu("Set Attached Text Mesh Input Field Component To Field")]
    public void SetAttachedTextMeshInputFieldComponentToField()
    {
        textMeshInputField = GetComponent<TMP_InputField>();
    }
}
