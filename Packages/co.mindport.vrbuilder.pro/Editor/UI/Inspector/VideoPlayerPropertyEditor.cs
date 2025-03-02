using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using VRBuilder.Pro.Properties;

namespace VRBuilder.Pro.Editor.UI.Inspector
{
    /// <summary>
    /// Editor UI for <see cref="VideoPlayerProperty"/>.
    /// </summary>
    [CustomEditor(typeof(VideoPlayerProperty))]
    public class VideoPlayerPropertyEditor : UnityEditor.Editor
    {
        private const string renderTexture360AssetName = "RenderTextures/360VideoRenderTexture";

        /// <inheritdoc/>        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Set up for equirectangular 360° videos"))
            {
                SetupEquirectangular360();
            }
        }

        private void SetupEquirectangular360()
        {
            VideoPlayerProperty property = (VideoPlayerProperty)target;

            property.VideoPlayer.source = VideoSource.Url;
            property.VideoPlayer.playOnAwake = false;
            property.VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            property.VideoPlayer.targetTexture = Resources.Load<RenderTexture>(renderTexture360AssetName);
            EditorUtility.SetDirty(property.VideoPlayer);

            property.Is360Video = true;
            EditorUtility.SetDirty(property);
        }
    }
}