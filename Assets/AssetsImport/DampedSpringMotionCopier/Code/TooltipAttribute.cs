//Eugene: made out of http://forum.unity3d.com/threads/182621-Inspector-Tooltips

using UnityEngine;

#if UNITY_EDITOR
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using UnityEditor;
    using Debug = System.Diagnostics.Debug;
#else
	using System.Collections;
#endif

public class Tooltip : PropertyAttribute
{
    public string EditorTooltip;

    public Tooltip(string EditorTooltip)
    {
        this.EditorTooltip = EditorTooltip;
    }
}

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Tooltip))]
    public class TooltipDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Tooltip tooltipAttribute = attribute as Tooltip;
     
            if (property.propertyType == SerializedPropertyType.AnimationCurve)
            {
                property.animationCurveValue = EditorGUI.CurveField(position, new GUIContent(label.text, tooltipAttribute.EditorTooltip), property.animationCurveValue);
            }
     
            if (property.propertyType == SerializedPropertyType.Boolean)
            {
                property.boolValue = EditorGUI.Toggle(position, new GUIContent(label.text, tooltipAttribute.EditorTooltip), property.boolValue);
            }
     
            if (property.propertyType == SerializedPropertyType.Bounds)
            {
                property.boundsValue = EditorGUI.BoundsField(position, new GUIContent(label.text, tooltipAttribute.EditorTooltip), property.boundsValue);
            }
     
            if (property.propertyType == SerializedPropertyType.Color)
            {
                property.colorValue = EditorGUI.ColorField(position, new GUIContent(label.text, tooltipAttribute.EditorTooltip),
                    property.colorValue);
            }
     
            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.FloatField(position,
                    new GUIContent(label.text, tooltipAttribute.EditorTooltip), property.floatValue);
            }
     
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntField(position, new GUIContent(label.text, tooltipAttribute.EditorTooltip), property.intValue);
            }
           
            if (property.propertyType == SerializedPropertyType.Rect)
            {
                property.rectValue = EditorGUI.RectField(position, new GUIContent(label.text, tooltipAttribute.EditorTooltip),
                    property.rectValue);
            }
     
            if (property.propertyType == SerializedPropertyType.String)
            {
                property.stringValue = EditorGUI.TextField(position,
                    new GUIContent(label.text, tooltipAttribute.EditorTooltip), property.stringValue);
            }

			if(property.propertyType == SerializedPropertyType.Vector3)
			{
				property.vector3Value = EditorGUI.Vector3Field(position,
					new GUIContent(label.text, tooltipAttribute.EditorTooltip), property.vector3Value);
			}
		}
    }
#endif //#if UNITY_EDITOR
