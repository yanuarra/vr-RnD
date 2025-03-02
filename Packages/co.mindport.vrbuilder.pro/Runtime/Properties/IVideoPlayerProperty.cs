using VRBuilder.Core.Properties;

namespace VRBuilder.Pro.Properties
{
    /// <summary>
    /// Property that controls a video player.
    /// </summary>
    public interface IVideoPlayerProperty : ISceneObjectProperty
    {
        /// <summary>
        /// True while the video is playing.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Starts playback of the selected video.
        /// </summary>
        void Play();

        /// <summary>
        /// Stops playback of the currently playing video.
        /// </summary>
        void Stop();

        /// <summary>
        /// Sets the source video to the specified URL.
        /// </summary>        
        void SetVideoURL(string url);

        /// <summary>
        /// Sets the source video to the specified Resources path.
        /// </summary>        
        void SetVideoResourcePath(string path);
    }
}