using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.NEW_CHARACTER_SETTING_FILE, menuName = GameDataMenuConsts.NEW_CHARACTER_SETTING_MENU, order = GameDataMenuConsts.NEW_CHARACTER_SETTING_ORDER)]
    public partial class NewCharacterSetting : ScriptableObject
    {
        [Header("New Character Configs")]
        [Tooltip("Amount of gold that will be added to character when create new character")]
        public int startGold = 0;
        [Tooltip("Items that will be added to character when create new character")]
        [ArrayElementTitle("item")]
        public ItemAmount[] startItems = new ItemAmount[0];

#if UNITY_EDITOR
        public GameDatabase sourceDatabase;

        [InspectorButton(nameof(AddAllAsStartItems), "Add All Start Items")]
        public bool btnAddAllAsStartItems;

        public void AddAllAsStartItems()
        {
            if (sourceDatabase == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a source database", "OK");
                return;
            }

            List<ItemAmount> startItems = new List<ItemAmount>(this.startItems);
            foreach (var item in sourceDatabase.items)
            {
                startItems.Add(new ItemAmount()
                {
                    item = item,
                    amount = item.MaxStack,
                });
            }
            this.startItems = startItems.ToArray();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
