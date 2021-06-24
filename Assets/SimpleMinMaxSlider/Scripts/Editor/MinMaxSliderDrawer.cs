using UnityEngine;
using UnityEditor;
using GD.MinMaxSlider;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){

        var minMaxAttribute = (MinMaxSliderAttribute)attribute;
        var propertyType = property.propertyType;

        label.tooltip = minMaxAttribute.min.ToString("F2") + " to " + minMaxAttribute.max.ToString("F2");

        bool isVector2 = propertyType == SerializedPropertyType.Vector2;
        if (!isVector2 && propertyType != SerializedPropertyType.Vector2Int)
        {
            Debug.LogError(property.propertyPath + " is not combatible with MinMaxSlider Property Drawer!");
            return;
        }

        //PrefixLabel returns the rect of the right part of the control. It leaves out the label section. We don't have to worry about it. Nice!
        Rect controlRect = EditorGUI.PrefixLabel(position, label);
        
        Rect[] splittedRect = SplitRect(controlRect,3);

        EditorGUI.BeginChangeCheck();

        Vector2 vector = isVector2 ? property.vector2Value : property.vector2IntValue;
        float minVal = vector.x;
        float maxVal = vector.y;

        //F2 limits the float to two decimal places (0.00).
        minVal = EditorGUI.FloatField(splittedRect[0], float.Parse(minVal.ToString("F2")));
        maxVal = EditorGUI.FloatField(splittedRect[2], float.Parse(maxVal.ToString("F2")));

        EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal,
        minMaxAttribute.min,minMaxAttribute.max);

        minVal = Mathf.Max(minMaxAttribute.min, minVal);
        maxVal = Mathf.Min(minMaxAttribute.max, maxVal);

        vector.x = minVal > maxVal ? maxVal : minVal;
        vector.y = maxVal;

        if(EditorGUI.EndChangeCheck()){
            if(isVector2)
                property.vector2Value = vector;
            else
                property.vector2IntValue = Vector2Int.RoundToInt(vector);
        }
    }

    Rect[] SplitRect(Rect rectToSplit, int n){


        Rect[] rects = new Rect[n];

        for(int i = 0; i < n; i++){

            rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n, rectToSplit.height);
        
        }

        int padding = (int)rects[0].width - 40;
        int space = 5;

        rects[0].width -= padding + space;
        rects[2].width -= padding + space;

        rects[1].x -= padding;
        rects[1].width += padding * 2;

        rects[2].x += padding + space;
        

        return rects;

    }
    
} 
