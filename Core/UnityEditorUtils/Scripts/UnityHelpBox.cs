namespace Insthync.UnityEditorUtils
{
    [System.Serializable]
    public class UnityHelpBox
    {
        public enum Type
        {
            None,
            Info,
            Warning,
            Error
        }

        [System.NonSerialized]
        public string text;

        [System.NonSerialized]
        public Type type;

        public UnityHelpBox(string text, Type type = Type.Info)
        {
            this.text = text;
            this.type = type;
        }
    }
}
