using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThaiText))]
public class ThaiTextEditor : Editor
{
    //private SerializedObject targatSerializedObject;
    MonoScript script = null;

    void OnEnable()
    {
        // Setup the SerializedProperties
        //targatSerializedObject = new SerializedObject(target);
        ThaiText thaiText = target as ThaiText;
        script = MonoScript.FromMonoBehaviour((ThaiText)target);

        alignmentButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
        alignmentButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
        alignmentButtonRight = new GUIStyle(EditorStyles.miniButtonRight);

        m_LeftAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left", "Left Align");
        m_CenterAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center", "Center Align");
        m_RightAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right", "Right Align");
        m_LeftAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left_active", "Left Align");
        m_CenterAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center_active", "Center Align");
        m_RightAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right_active", "Right Align");

        m_TopAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top", "Top Align");
        m_MiddleAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center", "Middle Align");
        m_BottomAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom", "Bottom Align");
        m_TopAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top_active", "Top Align");
        m_MiddleAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center_active", "Middle Align");
        m_BottomAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom_active", "Bottom Align");

        //    EditorGUIUtility.IconContent("TextAsset Icon");
        FixAlignmentButtonStyles(alignmentButtonLeft, alignmentButtonMid, alignmentButtonRight);
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        ThaiText thaiText = target as ThaiText;

        GUI.enabled = false;
        script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
        GUI.enabled = true;

        EditorGUILayout.LabelField("Text");
        thaiText.text =  EditorGUILayout.TextArea(thaiText.DefaultText, GUILayout.MinHeight(48));

        EditorGUILayout.LabelField("Character", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        thaiText.font = EditorGUILayout.ObjectField("Font", thaiText.font, typeof(UnityEngine.Font), false) as Font;
        thaiText.fontStyle = (UnityEngine.FontStyle)EditorGUILayout.EnumPopup("Font Style", thaiText.fontStyle);
        thaiText.fontSize = EditorGUILayout.IntField("Font Size", thaiText.fontSize);
        thaiText.lineSpacing = EditorGUILayout.FloatField("Line Spacing", thaiText.lineSpacing);
        thaiText.supportRichText = EditorGUILayout.Toggle(new GUIContent("Rich Text", "For emoticons and  colors"), thaiText.supportRichText);
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Paragraph", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        thaiText.alignment = DrawGUITextAlignment(thaiText.alignment);
        thaiText.alignByGeometry = EditorGUILayout.Toggle("Align By Geometry", thaiText.alignByGeometry);
        thaiText.horizontalOverflow = (HorizontalWrapMode)EditorGUILayout.EnumPopup("Horizontal Overflow", thaiText.horizontalOverflow);
        thaiText.verticalOverflow = (VerticalWrapMode)EditorGUILayout.EnumPopup("Vertical Overflow", thaiText.verticalOverflow);
        thaiText.resizeTextForBestFit = EditorGUILayout.Toggle("Best Fit", thaiText.resizeTextForBestFit);
        if (thaiText.resizeTextForBestFit)
        {
            thaiText.resizeTextMinSize = EditorGUILayout.IntField("Min Size", thaiText.resizeTextMinSize);
            thaiText.resizeTextMaxSize = EditorGUILayout.IntField("Max Size", thaiText.resizeTextMaxSize);
        }
        EditorGUI.indentLevel--;
        thaiText.color = EditorGUILayout.ColorField("Color", thaiText.color);
        thaiText.material = EditorGUILayout.ObjectField("Material", thaiText.material, typeof(UnityEngine.Material), false) as Material;
        thaiText.raycastTarget = EditorGUILayout.Toggle("Raycast Target", thaiText.raycastTarget);
         
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        } 
    }

    public static GUIStyle alignmentButtonLeft;
    public static GUIStyle alignmentButtonMid;
    public static GUIStyle alignmentButtonRight;

    public static GUIContent m_LeftAlignText;
    public static GUIContent m_CenterAlignText;
    public static GUIContent m_RightAlignText;
    public static GUIContent m_TopAlignText;
    public static GUIContent m_MiddleAlignText;
    public static GUIContent m_BottomAlignText;

    public static GUIContent m_LeftAlignTextActive;
    public static GUIContent m_CenterAlignTextActive;
    public static GUIContent m_RightAlignTextActive;
    public static GUIContent m_TopAlignTextActive;
    public static GUIContent m_MiddleAlignTextActive;
    public static GUIContent m_BottomAlignTextActive;


    static void FixAlignmentButtonStyles(params GUIStyle[] styles)
    {
        foreach (GUIStyle style in styles)
        {
            style.padding.left = 2;
            style.padding.right = 2;
        }
    }

    enum Verticle { top, middle, bottom}
    enum Horizontal { left, center, right }
    Verticle vAlign;
    Horizontal hAlign;

    public TextAnchor DrawGUITextAlignment(TextAnchor alignment)
    {
      //  Debug.Log("alignment: "+ alignment);

        EditorGUIUtility.SetIconSize(new Vector2(15, 15));

        if (alignment == TextAnchor.UpperLeft || alignment == TextAnchor.MiddleLeft || alignment == TextAnchor.LowerLeft)
        {
            hAlign = Horizontal.left;
        }
        else if (alignment == TextAnchor.UpperCenter || alignment == TextAnchor.MiddleCenter || alignment == TextAnchor.LowerCenter)
        {
            hAlign = Horizontal.center;
        }
        else{
            hAlign = Horizontal.right;
        }

        if (alignment == TextAnchor.UpperLeft || alignment == TextAnchor.UpperCenter || alignment == TextAnchor.UpperRight)
        {
            vAlign = Verticle.top;
        }
        else if (alignment == TextAnchor.MiddleLeft || alignment == TextAnchor.MiddleCenter || alignment == TextAnchor.MiddleRight)
        {
            vAlign = Verticle.middle;
        }
        else
        {
            vAlign = Verticle.bottom;
        }

        GUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Alignment");

        GUILayout.Space(-8);

        if (GUILayout.Toggle(hAlign == Horizontal.left,
            hAlign == Horizontal.left ? m_LeftAlignTextActive : m_LeftAlignText,
            alignmentButtonLeft,
            GUILayout.Width(20),
            GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            hAlign = Horizontal.left;
        }

        if (GUILayout.Toggle(hAlign == Horizontal.center,
            hAlign == Horizontal.center? m_CenterAlignTextActive : m_CenterAlignText,
            alignmentButtonMid,
            GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            hAlign = Horizontal.center;
        }

        if (GUILayout.Toggle(hAlign == Horizontal.right,
            hAlign == Horizontal.right ? m_RightAlignTextActive : m_RightAlignText,
            alignmentButtonRight,
            GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            hAlign = Horizontal.right;
        }

        GUILayout.Space(20);

        if (GUILayout.Toggle(vAlign == Verticle.top,
            vAlign == Verticle.top ? m_TopAlignTextActive : m_TopAlignText,
            alignmentButtonLeft,
            GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            vAlign = Verticle.top;
        }

        if (GUILayout.Toggle(vAlign == Verticle.middle,
             vAlign == Verticle.middle ? m_MiddleAlignTextActive : m_MiddleAlignText,
            alignmentButtonMid,
            GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            vAlign = Verticle.middle;
        }

        if (GUILayout.Toggle(vAlign == Verticle.bottom,
             vAlign == Verticle.bottom ? m_BottomAlignTextActive : m_BottomAlignText,
            alignmentButtonRight,
            GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            vAlign = Verticle.bottom;
        }

        GUILayout.EndHorizontal();

        if (hAlign == Horizontal.left && vAlign == Verticle.top) {
            return TextAnchor.UpperLeft;
        }
        else if (hAlign == Horizontal.left && vAlign == Verticle.middle)
        {
            return TextAnchor.MiddleLeft;
        }
        else if (hAlign == Horizontal.left && vAlign == Verticle.bottom)
        {
            return TextAnchor.LowerLeft;
        }
        else if (hAlign == Horizontal.center && vAlign == Verticle.top)
        {
            return TextAnchor.UpperCenter;
        }
        else if (hAlign == Horizontal.center && vAlign == Verticle.middle)
        {
            return TextAnchor.MiddleCenter;
        }
        else if (hAlign == Horizontal.center && vAlign == Verticle.bottom)
        {
            return TextAnchor.LowerCenter;
        }
        else if (hAlign == Horizontal.right && vAlign == Verticle.top)
        {
            return TextAnchor.UpperRight;
        }
        else if (hAlign == Horizontal.right && vAlign == Verticle.middle)
        {
            return TextAnchor.MiddleRight;
        }
        else if (hAlign == Horizontal.right && vAlign == Verticle.bottom)
        {
            return TextAnchor.LowerRight;
        }
        return alignment;
    }
}
