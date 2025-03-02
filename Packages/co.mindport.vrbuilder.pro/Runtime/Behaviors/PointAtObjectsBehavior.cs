using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Pro.Utils;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Behavior that displays one or more pointers pointing at specified objects.
    /// </summary>
    [DataContract(IsReference = true)]
    public class PointAtObjectsBehavior : Behavior<PointAtObjectsBehavior.EntityData>, IOptional
    {
        private const string defaultPointerPrefab = "CircleArrow";

        /// <summary>
        /// "Outline object" behavior's data.
        /// </summary>        
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Target scene objects to point at.
            /// </summary>
            [DataMember]
            [DisplayName("Objects")]
            public MultipleSceneObjectReference TargetObjects { get; set; }

            /// <summary>
            /// Resources path of the pointer prefab to use.
            /// </summary>
            [DataMember]
            [DisplayName("Pointer Resource")]
            [UsesSpecificProcessDrawer("ObjectPointerResourceDrawer")]
            public string PointerObjectPrefabPath { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <summary>
            /// Used to cache spawned instances of the pointer object.
            /// </summary>
            [IgnoreDataMember]
            public List<ObjectPointer> PointerInstances { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name => $"Point at {TargetObjects}";
        }

        private class ActivatingProcess : InstantProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                Data.PointerInstances.Clear();
                ObjectPointer prefab = Resources.Load<ObjectPointer>(Data.PointerObjectPrefabPath);

                foreach (ISceneObject sceneObject in Data.TargetObjects.Values)
                {
                    ObjectPointer pointer = GameObject.Instantiate(prefab);
                    Data.PointerInstances.Add(pointer);
                    pointer.StartTracking(sceneObject.GameObject.transform);
                }
            }
        }

        private class DeactivatingProcess : InstantProcess<EntityData>
        {
            public DeactivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                foreach (ObjectPointer pointer in Data.PointerInstances)
                {
                    pointer.StopTracking();
                    GameObject.Destroy(pointer.gameObject);
                }
            }
        }

        [JsonConstructor, Preserve]
        public PointAtObjectsBehavior()
        {
            Data.TargetObjects = new MultipleSceneObjectReference();
            Data.PointerObjectPrefabPath = defaultPointerPrefab;
            Data.PointerInstances = new List<ObjectPointer>();
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new DeactivatingProcess(Data);
        }
    }
}
