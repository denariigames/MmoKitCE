using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DropdownWrapper : MonoBehaviour
{
    public Dropdown unityDropdown;
    public TMP_Dropdown textMeshDropdown;

    public bool interactable
    {
        get
        {
            if (unityDropdown != null)
                return unityDropdown.interactable;
            if (textMeshDropdown != null)
                return textMeshDropdown.interactable;
            return false;
        }
    }

    public int value
    {
        get
        {
            if (unityDropdown != null)
                return unityDropdown.value;
            if (textMeshDropdown != null)
                return textMeshDropdown.value;
            return 0;
        }

        set
        {
            if (unityDropdown != null)
                unityDropdown.value = value;
            if (textMeshDropdown != null)
                textMeshDropdown.value = value;
        }
    }

    public virtual List<OptionData> options
    {
        get
        {
            if (unityDropdown != null)
            {
                if (unityDropdown.options == null)
                    return null;
                List<OptionData> options = new List<OptionData>();
                foreach (Dropdown.OptionData entry in unityDropdown.options)
                {
                    options.Add(new OptionData(entry.text, entry.image, Color.white));
                }
                return options;
            }
            if (textMeshDropdown != null)
            {
                if (textMeshDropdown.options == null)
                    return null;
                var options = new List<OptionData>();
                foreach (var entry in textMeshDropdown.options)
                {
#if UNITY_6000_0_OR_NEWER
                    options.Add(new OptionData(entry.text, entry.image, entry.color));
#else
                    options.Add(new OptionData(entry.text, entry.image, Color.white));
#endif
                }
                return options;
            }
            return null;
        }

        set
        {
            if (unityDropdown != null)
            {
                if (value == null)
                    unityDropdown.options = null;
                else
                {
                    List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                    foreach (OptionData entry in value)
                    {
                        options.Add(new Dropdown.OptionData(entry.text, entry.image));
                    }
                    unityDropdown.options = options;
                }
            }
            if (textMeshDropdown != null)
            {
                if (value == null)
                    textMeshDropdown.options = null;
                else
                {
                    var options = new List<TMP_Dropdown.OptionData>();
                    foreach (var entry in value)
                    {
#if UNITY_6000_0_OR_NEWER
                        options.Add(new TMP_Dropdown.OptionData(entry.text, entry.image, entry.color));
#else
                        options.Add(new TMP_Dropdown.OptionData(entry.text, entry.image));
#endif
                    }
                    textMeshDropdown.options = options;
                }
            }
        }
    }

    public virtual UnityEvent<int> onValueChanged
    {
        get
        {
            if (unityDropdown != null) return unityDropdown.onValueChanged;
            if (textMeshDropdown != null) return textMeshDropdown.onValueChanged;
            return null;
        }

        set
        {
            if (unityDropdown != null) unityDropdown.onValueChanged = value as Dropdown.DropdownEvent;
            if (textMeshDropdown != null) textMeshDropdown.onValueChanged = value as TMP_Dropdown.DropdownEvent;
        }
    }

    void Awake()
    {
        if (unityDropdown == null) unityDropdown = GetComponent<Dropdown>();
        if (textMeshDropdown == null) textMeshDropdown = GetComponent<TMP_Dropdown>();
    }

    private void OnDestroy()
    {
        unityDropdown = null;
        textMeshDropdown = null;
    }

    public void SetGameObjectActive(bool isActive)
    {
        if (unityDropdown != null)
            unityDropdown.gameObject.SetActive(isActive);
        if (textMeshDropdown != null)
            textMeshDropdown.gameObject.SetActive(isActive);
        gameObject.SetActive(isActive);
    }

    [ContextMenu("Set Attached Dropdown Component To Field")]
    public void SetAttachedDropdownComponentToField()
    {
        unityDropdown = GetComponent<Dropdown>();
    }

    [ContextMenu("Set Attached Text Mesh Dropdown Component To Field")]
    public void SetAttachedTextMeshDropdownComponentToField()
    {
        textMeshDropdown = GetComponent<TMP_Dropdown>();
    }

    public class OptionData
    {
        public OptionData() : this(string.Empty, null, Color.white)
        {
        }

        public OptionData(string text) : this(text, null, Color.white)
        {

        }

        public OptionData(Sprite image) : this(string.Empty, image, Color.white)
        {

        }

        public OptionData(string text, Sprite image, Color color)
        {
            this.text = text;
            this.image = image;
            this.color = color;
        }


        public string text { get; set; }
        public Sprite image { get; set; }
        public Color color { get; set; }
    }
}
