using UnityEngine;

namespace Insthync.UnityEditorUtils
{
    public class CategoryAttribute : PropertyAttribute
    {
        public string category { get; private set; }
        public bool isFoldoutByDefault { get; private set; }
        public CategoryAttribute(int order, string category, bool isFoldoutByDefault = true)
        {
            this.category = category;
            this.order = order;
            this.isFoldoutByDefault = isFoldoutByDefault;
        }

        public CategoryAttribute(string category, bool isFoldoutByDefault = true) : this(0, category, isFoldoutByDefault)
        {
        }
    }
}
