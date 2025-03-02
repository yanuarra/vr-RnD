using System;
using System.Linq;
using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Editor.UI;
using VRBuilder.Core.Editor.UI.Drawers;
using VRBuilder.Pro.Behaviors;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Editor.UI.Drawers
{
    /// <summary>
    /// Custom drawer for <see cref="SetStateBehavior"/>.
    /// </summary>    
    [DefaultProcessDrawer(typeof(SetStateBehavior.EntityData))]
    public class SetStateDrawer : NameableDrawer
    {
        public override Rect Draw(Rect rect, object currentValue, Action<object> changeValueCallback, GUIContent label)
        {
            rect = base.Draw(rect, currentValue, changeValueCallback, label);

            float height = DrawLabel(rect, currentValue, changeValueCallback, label);

            height += EditorDrawingHelper.VerticalSpacing;

            Rect nextPosition = new Rect(rect.x, rect.y + height, rect.width, rect.height);

            SetStateBehavior.EntityData data = currentValue as SetStateBehavior.EntityData;

            nextPosition = DrawerLocator.GetDrawerForValue(data.DataProperties, typeof(MultipleScenePropertyReference<IDataProperty<int>>)).Draw(nextPosition, data.DataProperties, (value) => UpdateDataProperty(value, data, changeValueCallback), "Data Properties");
            height += nextPosition.height;
            height += EditorDrawingHelper.VerticalSpacing;
            nextPosition.y = rect.y + height;

            if (data.DataProperties.HasValue())
            {
                Type stateType = data.DataProperties.Values.FirstOrDefault()?.StateType;
                bool allHaveSameStateType = true;

                foreach (StateDataPropertyBase stateDataProperty in data.DataProperties.Values)
                {
                    allHaveSameStateType &= stateDataProperty.StateType == stateType;
                }

                if (stateType != null && allHaveSameStateType)
                {
                    Enum newValue = (Enum)Enum.ToObject(stateType, data.NewValue);
                    nextPosition = DrawerLocator.GetDrawerForValue(newValue, stateType).Draw(nextPosition, newValue, (value) => UpdateState(value, data, changeValueCallback), "State");
                    height += nextPosition.height;
                    height += EditorDrawingHelper.VerticalSpacing;
                    nextPosition.y = rect.y + height;
                }
            }

            rect.height = height;
            return rect;
        }

        private void UpdateDataProperty(object value, SetStateBehavior.EntityData data, Action<object> changeValueCallback)
        {
            MultipleScenePropertyReference<StateDataPropertyBase> newProperty = (MultipleScenePropertyReference<StateDataPropertyBase>)value;
            MultipleScenePropertyReference<StateDataPropertyBase> oldProperty = data.DataProperties;

            if (newProperty != oldProperty)
            {
                data.DataProperties = newProperty;
                changeValueCallback(data);
            }
        }

        private void UpdateState(object value, SetStateBehavior.EntityData data, Action<object> changeValueCallback)
        {
            int oldValue = data.NewValue;
            int newValue = (int)value;

            if (newValue != oldValue)
            {
                data.NewValue = newValue;
                changeValueCallback(data);
            }
        }
    }
}