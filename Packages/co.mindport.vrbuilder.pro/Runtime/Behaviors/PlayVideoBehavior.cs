using Newtonsoft.Json;
using System.Collections;
using System.Runtime.Serialization;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.UI.SelectableValues;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Behaviors
{
    /// <summary>
    /// Plays a videoclip and ends when the clip is over.
    /// </summary>
    public class PlayVideoBehavior : Behavior<PlayVideoBehavior.EntityData>
    {
        /// <summary>
        /// Entity data for <see cref="PlayVideoBehavior"/>.
        /// </summary>
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Video asset source. First value is an asset path, second value is a URL.
            /// </summary>
            [DataMember]
            [UsesSpecificProcessDrawer("StringsSelectableValueDrawer")]
            [DisplayName("Video clip")]
            public VideoClipOrURLSelectableValue VideoSource { get; set; }

            /// <summary>
            /// Video player which will play the video.
            /// </summary>
            [DataMember]
            [DisplayName("Video player")]
            public SingleScenePropertyReference<IVideoPlayerProperty> VideoPlayer { get; set; }

            [DataMember]
            public Metadata Metadata { get; set; }

            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string assetName = string.IsNullOrEmpty(VideoSource.FirstValue) ? "[NULL]" : VideoSource.FirstValue.Remove(0, VideoSource.FirstValue.LastIndexOf('/') + 1);
                    string videoSource = VideoSource.IsFirstValueSelected ? assetName : "from URL";
                    return $"Play video {videoSource}";
                }
            }
        }

        public class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            public override void End()
            {
            }

            public override void FastForward()
            {
                Data.VideoPlayer.Value.Stop();
            }

            public override void Start()
            {
                if (Data.VideoSource.IsFirstValueSelected)
                {
                    Data.VideoPlayer.Value.SetVideoResourcePath(Data.VideoSource.FirstValue);
                }
                else
                {
                    Data.VideoPlayer.Value.SetVideoURL(Data.VideoSource.SecondValue);
                }

                Data.VideoPlayer.Value.Play();
            }

            public override IEnumerator Update()
            {
                while (Data.VideoPlayer.Value.IsPlaying)
                {
                    yield return null;
                }
            }
        }

        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        [JsonConstructor]
        public PlayVideoBehavior()
        {
            Data.VideoPlayer = new SingleScenePropertyReference<IVideoPlayerProperty>();
            Data.VideoSource = new VideoClipOrURLSelectableValue();
        }
    }
}