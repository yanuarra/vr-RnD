using UnityEngine;
using UnityEngine.Video;
using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Property that controls a <see cref="VideoPlayer"/>.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoPlayerProperty : ProcessSceneObjectProperty, IVideoPlayerProperty
    {
        private const string renderTextureMaterialPath = "Materials/360VideoSkyboxMaterial";
        private Material renderTextureMaterial;
        private VideoPlayer videoPlayer;
        private Skybox skybox;

        [SerializeField]
        private bool is360Video;

        /// <summary>
        /// True if the video should be played as a 360° video.
        /// </summary>
        public bool Is360Video
        {
            get { return is360Video; }
            set { is360Video = value; }
        }

        /// <inheritdoc/>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// The <see cref="VideoPlayer"/> associated with this property.
        /// </summary>
        public VideoPlayer VideoPlayer
        {
            get
            {
                if (videoPlayer == null)
                {
                    videoPlayer = GetComponent<VideoPlayer>();
                }

                return videoPlayer;
            }
        }

        private void Start()
        {
            base.OnEnable();
            renderTextureMaterial = Resources.Load<Material>(renderTextureMaterialPath);
        }

        private void OnLoopPointReached(VideoPlayer source)
        {
            videoPlayer.loopPointReached -= OnLoopPointReached;
            Stop();
        }

        private void OnPrepareCompleted(VideoPlayer source)
        {
            videoPlayer.prepareCompleted -= OnPrepareCompleted;

            if (Is360Video)
            {
                skybox = Camera.main.gameObject.AddComponent<Skybox>();
                skybox.material = renderTextureMaterial;
            }

            VideoPlayer.Play();
        }

        /// <inheritdoc/>
        public void Play()
        {
            VideoPlayer.Prepare();
            VideoPlayer.prepareCompleted += OnPrepareCompleted;
            VideoPlayer.loopPointReached += OnLoopPointReached;
            IsPlaying = true;
        }

        /// <inheritdoc/>
        public void Stop()
        {
            VideoPlayer.Stop();
            IsPlaying = false;

            if (skybox != null)
            {
                Destroy(skybox);
            }
        }

        /// <inheritdoc/>
        public void SetVideoURL(string url)
        {
            VideoPlayer.source = VideoSource.Url;
            VideoPlayer.url = url;
        }

        /// <inheritdoc/>
        public void SetVideoResourcePath(string path)
        {
            VideoPlayer.source = VideoSource.VideoClip;
            VideoPlayer.clip = Resources.Load<VideoClip>(path);
        }
    }
}