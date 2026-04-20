using UnityEngine;

namespace NonsensicalKit.UGUI.Media.Samples
{
    public class AudioManagerDemo : MonoBehaviour
    {
        [SerializeField] private AudioManager m_audioManager;
        [SerializeField] private string m_audioUrl = "https://music.163.com/song/media/outer/url?id=1492276411.mp3";

        public void PlayTest()
        {
            m_audioManager.PlayAudio(m_audioUrl);
        }
    }
}
