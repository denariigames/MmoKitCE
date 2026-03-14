using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if ENABLE_PURCHASING
using UnityEditor.Purchasing;
using UnityEngine.Purchasing;
#endif

namespace MultiplayerARPG
{
    [CustomEditor(typeof(CashPackage))]
    [CanEditMultipleObjects]
    public class CashPackageEditor : Editor
    {
        private const string kNoProduct = "<None>";

#if ENABLE_PURCHASING
        private List<string> m_ValidIDs = new List<string>();
#endif
        private SerializedProperty m_ProductIDProperty;

        public void OnEnable()
        {
            m_ProductIDProperty = serializedObject.FindProperty("productId");
        }

        public override void OnInspectorGUI()
        {
            CashPackage package = (CashPackage)target;

            serializedObject.Update();

            EditorGUILayout.LabelField(new GUIContent("Product ID:", "Select a product from the IAP catalog"));

#if ENABLE_PURCHASING
            var catalog = ProductCatalog.LoadDefaultCatalog();

            m_ValidIDs.Clear();
            m_ValidIDs.Add(kNoProduct);
            foreach (var product in catalog.allProducts)
            {
                m_ValidIDs.Add(product.id);
            }

            int currentIndex = string.IsNullOrEmpty(package.ProductId) ? 0 : m_ValidIDs.IndexOf(package.ProductId);
            int newIndex = EditorGUILayout.Popup(currentIndex, m_ValidIDs.ToArray());
            if (newIndex > 0 && newIndex < m_ValidIDs.Count)
            {
                m_ProductIDProperty.stringValue = m_ValidIDs[newIndex];
            }
            else
            {
                m_ProductIDProperty.stringValue = string.Empty;
            }

            if (GUILayout.Button("IAP Catalog..."))
            {
                ProductCatalogEditor.ShowWindow();
            }
#else
            var defaultColor = GUI.color;
            GUI.color = Color.red;
            GUILayout.Label("You must install Unity Purchasing");
            GUI.color = defaultColor;
#endif

            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });

            serializedObject.ApplyModifiedProperties();
        }
    }
}
