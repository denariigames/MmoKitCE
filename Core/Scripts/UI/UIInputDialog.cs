using UnityEngine;
using UnityEngine.UI;

public class UIInputDialog : UIBase
{
    public TextWrapper uiTextTitle;
    public TextWrapper uiTextDescription;
    public InputFieldWrapper uiInputField;
    public Button buttonConfirm;
    public bool hideOnConfirm = true;
    private System.Action<string> _onConfirmText;
    private System.Action<int> _onConfirmInteger;
    private System.Action<float> _onConfirmDecimal;
    private int _intDefaultAmount;
    private int? _intMinAmount;
    private int? _intMaxAmount;
    private float _floatDefaultAmount;
    private float? _floatMinAmount;
    private float? _floatMaxAmount;
    private string _defaultPlaceHolderText;

    public string Title
    {
        get
        {
            return uiTextTitle == null ? string.Empty : uiTextTitle.text;
        }
        set
        {
            if (uiTextTitle != null) uiTextTitle.text = value;
        }
    }

    public string Description
    {
        get
        {
            return uiTextDescription == null ? string.Empty : uiTextDescription.text;
        }
        set
        {
            if (uiTextDescription != null) uiTextDescription.text = value;
        }
    }

    public string InputFieldText
    {
        get
        {
            return uiInputField == null ? string.Empty : uiInputField.text;
        }
        set
        {
            if (uiInputField != null) uiInputField.text = value;
        }
    }

    public string PlaceHolderText
    {
        get
        {

            if (uiInputField != null)
            {
                if (uiInputField.placeholder is Text)
                    return (uiInputField.placeholder as Text).text;
                if (uiInputField.placeholder is TMPro.TMP_Text)
                    return (uiInputField.placeholder as TMPro.TMP_Text).text;
            }
            return string.Empty;
        }
        set
        {
            if (uiInputField != null)
            {
                if (uiInputField.placeholder is Text)
                {
                    if (string.IsNullOrEmpty(_defaultPlaceHolderText))
                        _defaultPlaceHolderText = (uiInputField.placeholder as Text).text;
                    (uiInputField.placeholder as Text).text = !string.IsNullOrEmpty(value) ? value : _defaultPlaceHolderText;
                }
                if (uiInputField.placeholder is TMPro.TMP_Text)
                {
                    if (string.IsNullOrEmpty(_defaultPlaceHolderText))
                        _defaultPlaceHolderText = (uiInputField.placeholder as TMPro.TMP_Text).text;
                    (uiInputField.placeholder as TMPro.TMP_Text).text = !string.IsNullOrEmpty(value) ? value : _defaultPlaceHolderText;
                }
            }
        }
    }

    public InputField.ContentType ContentType
    {
        get
        {
            return uiInputField == null ? InputField.ContentType.Standard : uiInputField.contentType;
        }
        set
        {
            if (uiInputField != null) uiInputField.contentType = value;
        }
    }

    public int CharacterLimit
    {
        get
        {
            return uiInputField == null ? 0 : uiInputField.characterLimit;
        }
        set
        {
            if (uiInputField != null) uiInputField.characterLimit = value;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        uiTextTitle = null;
        uiTextDescription = null;
        uiInputField = null;
        buttonConfirm = null;
        _onConfirmText = null;
        _onConfirmInteger = null;
        _onConfirmDecimal = null;
    }

    protected virtual void OnEnable()
    {
        if (buttonConfirm != null)
        {
            buttonConfirm.onClick.RemoveListener(OnClickConfirm);
            buttonConfirm.onClick.AddListener(OnClickConfirm);
        }
    }

    public void Show(string title,
        string description,
        System.Action<string> onConfirmText,
        string defaultText = "",
        InputField.ContentType contentType = InputField.ContentType.Standard,
        int characterLimit = 0,
        string placeHolder = "")
    {
        Title = title;
        Description = description;
        InputFieldText = defaultText;
        ContentType = contentType;
        CharacterLimit = characterLimit;
        PlaceHolderText = placeHolder;
        _onConfirmText = onConfirmText;
        Show();
    }

    public void Show(string title,
        string description,
        System.Action<int> onConfirmInteger,
        int? minAmount = null,
        int? maxAmount = null,
        int defaultAmount = 0,
        string placeHolder = "")
    {
        Title = title;
        Description = description;
        PlaceHolderText = placeHolder;
        SetupForIntegerInput(onConfirmInteger, minAmount, maxAmount, defaultAmount);
        Show();
    }

    public void SetupForIntegerInput(
        System.Action<int> onConfirmInteger,
        int? minAmount = null,
        int? maxAmount = null,
        int defaultAmount = 0)
    {
        if (!minAmount.HasValue)
            minAmount = int.MinValue;
        if (!maxAmount.HasValue)
            maxAmount = int.MaxValue;

        _intDefaultAmount = defaultAmount;
        _intMinAmount = minAmount;
        _intMaxAmount = maxAmount;

        if (uiInputField != null)
        {
            if (minAmount.Value > maxAmount.Value)
            {
                minAmount = null;
                Debug.LogWarning("min amount is more than max amount");
            }
            uiInputField.onValueChanged.RemoveListener(ValidateIntegerAmount);
            uiInputField.onValueChanged.RemoveListener(ValidateDecimalAmount);
            uiInputField.onValueChanged.AddListener(ValidateIntegerAmount);
        }
        InputFieldText = defaultAmount.ToString();
        ContentType = InputField.ContentType.IntegerNumber;
        CharacterLimit = 0;
        _onConfirmInteger = onConfirmInteger;
    }

    protected void ValidateIntegerAmount(string result)
    {
        int amount = _intDefaultAmount;
        if (int.TryParse(result, out amount))
        {
            if (_intMinAmount.HasValue && amount < _intMinAmount.Value)
                amount = _intMinAmount.Value;
            if (_intMaxAmount.HasValue && amount > _intMaxAmount.Value)
                amount = _intMaxAmount.Value;
            uiInputField.SetTextWithoutNotify(amount.ToString());
        }
    }

    public void Show(string title,
        string description,
        System.Action<float> onConfirmDecimal,
        float? minAmount = null,
        float? maxAmount = null,
        float defaultAmount = 0f,
        string placeHolder = "")
    {
        Title = title;
        Description = description;
        PlaceHolderText = placeHolder;
        SetupForDecimalInput(onConfirmDecimal, minAmount, maxAmount, defaultAmount);
        Show();
    }

    public void SetupForDecimalInput(
        System.Action<float> onConfirmDecimal,
        float? minAmount = null,
        float? maxAmount = null,
        float defaultAmount = 0f)
    {
        if (!minAmount.HasValue)
            minAmount = float.MinValue;
        if (!maxAmount.HasValue)
            maxAmount = float.MaxValue;

        _floatDefaultAmount = defaultAmount;
        _floatMinAmount = minAmount;
        _floatMaxAmount = maxAmount;

        if (uiInputField != null)
        {
            if (minAmount.Value > maxAmount.Value)
            {
                minAmount = null;
                Debug.LogWarning("min amount is more than max amount");
            }
            uiInputField.onValueChanged.RemoveListener(ValidateIntegerAmount);
            uiInputField.onValueChanged.RemoveListener(ValidateDecimalAmount);
            uiInputField.onValueChanged.AddListener(ValidateDecimalAmount);
        }
        InputFieldText = defaultAmount.ToString();
        ContentType = InputField.ContentType.DecimalNumber;
        CharacterLimit = 0;
        _onConfirmDecimal = onConfirmDecimal;
    }

    protected void ValidateDecimalAmount(string result)
    {
        float amount = _floatDefaultAmount;
        if (float.TryParse(result, out amount))
        {
            if (_floatMinAmount.HasValue && amount < _floatMinAmount.Value)
                amount = _floatMinAmount.Value;
            if (_floatMaxAmount.HasValue && amount > _floatMaxAmount.Value)
                amount = _floatMaxAmount.Value;
            uiInputField.SetTextWithoutNotify(amount.ToString());
        }
    }

    public void OnClickConfirm()
    {
        if (!IsVisible())
            return;
        if (hideOnConfirm)
            Hide();
        switch (ContentType)
        {
            case InputField.ContentType.IntegerNumber:
                int intAmount = int.Parse(InputFieldText);
                if (_onConfirmInteger != null)
                    _onConfirmInteger.Invoke(intAmount);
                break;
            case InputField.ContentType.DecimalNumber:
                float floatAmount = float.Parse(InputFieldText);
                if (_onConfirmDecimal != null)
                    _onConfirmDecimal.Invoke(floatAmount);
                break;
            default:
                string text = InputFieldText;
                if (_onConfirmText != null)
                    _onConfirmText.Invoke(text);
                break;
        }
    }

    public void SetToMinAmount()
    {
        switch (ContentType)
        {
            case InputField.ContentType.IntegerNumber:
                if (_intMinAmount.HasValue)
                    InputFieldText = _intMinAmount.Value.ToString();
                break;
            case InputField.ContentType.DecimalNumber:
                if (_floatMinAmount.HasValue)
                    InputFieldText = _floatMinAmount.Value.ToString();
                break;
        }
    }

    public void SetToMaxAmount()
    {
        switch (ContentType)
        {
            case InputField.ContentType.IntegerNumber:
                if (_intMaxAmount.HasValue)
                    InputFieldText = _intMaxAmount.Value.ToString();
                break;
            case InputField.ContentType.DecimalNumber:
                if (_floatMaxAmount.HasValue)
                    InputFieldText = _floatMaxAmount.Value.ToString();
                break;
        }
    }

    public void ChangeAmount(int step)
    {
        switch (ContentType)
        {
            case InputField.ContentType.IntegerNumber:
                if (!int.TryParse(InputFieldText, out int amount))
                    amount = 0;
                amount -= step;
                if (_intMinAmount.HasValue && amount < _intMinAmount.Value)
                    amount = _intMinAmount.Value;
                if (_intMaxAmount.HasValue && amount > _intMaxAmount.Value)
                    amount = _intMaxAmount.Value;
                uiInputField.SetTextWithoutNotify(amount.ToString());
                break;
            case InputField.ContentType.DecimalNumber:
                if (!float.TryParse(InputFieldText, out float amountDecimal))
                    amountDecimal = 0f;
                amountDecimal -= step;
                if (_floatMinAmount.HasValue && amountDecimal < _floatMinAmount.Value)
                    amountDecimal = _floatMinAmount.Value;
                if (_floatMaxAmount.HasValue && amountDecimal > _floatMaxAmount.Value)
                    amountDecimal = _floatMaxAmount.Value;
                uiInputField.SetTextWithoutNotify(amountDecimal.ToString());
                break;
        }
    }

    public void ChangeAmountDecimal(float step)
    {
        switch (ContentType)
        {
            case InputField.ContentType.IntegerNumber:
                if (!int.TryParse(InputFieldText, out int amount))
                    amount = 0;
                amount -= (int)step;
                if (_intMinAmount.HasValue && amount < _intMinAmount.Value)
                    amount = _intMinAmount.Value;
                if (_intMaxAmount.HasValue && amount > _intMaxAmount.Value)
                    amount = _intMaxAmount.Value;
                uiInputField.SetTextWithoutNotify(amount.ToString());
                break;
            case InputField.ContentType.DecimalNumber:
                if (!float.TryParse(InputFieldText, out float amountDecimal))
                    amountDecimal = 0f;
                amountDecimal -= step;
                if (_floatMinAmount.HasValue && amountDecimal < _floatMinAmount.Value)
                    amountDecimal = _floatMinAmount.Value;
                if (_floatMaxAmount.HasValue && amountDecimal > _floatMaxAmount.Value)
                    amountDecimal = _floatMaxAmount.Value;
                uiInputField.SetTextWithoutNotify(amountDecimal.ToString());
                break;
        }
    }

    public void AppendText(string text)
    {
        uiInputField.text += text;
    }

    public void DeleteLastCharacter()
    {
        uiInputField.text = uiInputField.text.Substring(0, uiInputField.text.Length - 1);
    }
}
