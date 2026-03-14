using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWrapper : MonoBehaviour
{
    public Text unityText;
    public TextMeshProUGUI textMeshText;
    private string _textValue = null;
    public virtual string text
    {
        get
        {
            if (unityText != null) return unityText.text;
            if (textMeshText != null) return textMeshText.text;
            return _textValue;
        }

        set
        {
            _textValue = value;
            if (unityText != null) unityText.text = value;
            if (textMeshText != null) textMeshText.text = value;
        }
    }

    public virtual Color color
    {
        get
        {
            if (unityText != null) return unityText.color;
            if (textMeshText != null) return textMeshText.color;
            return Color.clear;
        }

        set
        {
            if (unityText != null) unityText.color = value;
            if (textMeshText != null) textMeshText.color = value;
        }
    }

    private void Awake()
    {
        if (unityText == null) unityText = GetComponent<Text>();
        if (textMeshText == null) textMeshText = GetComponent<TextMeshProUGUI>();
        if (_textValue != null)
            text = _textValue;
    }

    private void OnDestroy()
    {
        unityText = null;
        textMeshText = null;
        _textValue = null;
    }

    public void SetGameObjectActive(bool isActive)
    {
        if (unityText != null)
            unityText.gameObject.SetActive(isActive);
        if (textMeshText != null)
            textMeshText.gameObject.SetActive(isActive);
        gameObject.SetActive(isActive);
    }

    [ContextMenu("Set Attached Text Component To Field")]
    public void SetAttachedTextComponentToField()
    {
        unityText = GetComponent<Text>();
    }

    [ContextMenu("Set Attached Text Mesh Text Component To Field")]
    public void SetAttachedTextMeshTextComponentToField()
    {
        textMeshText = GetComponent<TextMeshProUGUI>();
    }
}
