using System;
using System.Collections.Generic;
using VRBuilder.Core.Editor.Configuration;
using VRBuilder.Core.Editor.UI.MenuItems.Behaviors;

namespace VRBuilder.Pro.Editor.Configuration
{
    public class AnimationsConfigurationExtension : IEditorConfigurationExtension
    {
        public IEnumerable<Type> RequiredMenuItems
        {
            get
            {
                return new Type[0];
            }
        }

        public IEnumerable<Type> DisabledMenuItems
        {
            get
            {
                return new[] { typeof(MoveObjectMenuItem) };
            }
        }
    }
}
