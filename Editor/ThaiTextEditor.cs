using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace com.dgn.ThaiText.Editor
{
    [CustomEditor(typeof(ThaiText), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Text Component.
    /// Extend this class to write a custom editor for a component derived from Text.
    /// </summary>
    public class ThaiTextEditor : GraphicEditor
    {
        SerializedProperty defaultText;
        SerializedProperty m_Text;
        SerializedProperty m_FontData;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            defaultText = serializedObject.FindProperty("defaultText");
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            ThaiText thaiText = target as ThaiText;
            
            EditorGUILayout.PropertyField(defaultText, new GUIContent("Text"));
            EditorGUILayout.PropertyField(m_FontData);

            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();
            
            if (EditorGUI.EndChangeCheck())
            {
                m_Text.stringValue = ThaiText.TextAdjust(defaultText.stringValue, thaiText.GetBoxWidth(),
                    thaiText.horizontalOverflow, thaiText.GetFontData);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}