using UnityEngine;

namespace NonsensicalKit.UGUI.Media.Samples
{
    public class VideoManagerDemo : MonoBehaviour
    {
        [SerializeField] private VideoManager m_videoManager;
        [SerializeField] private string m_videoUrl = "http://vjs.zencdn.net/v/oceans.mp4";

        public void PlayTest()
        {
            m_videoManager.PlayVideo(m_videoUrl);
        }
    }
}
