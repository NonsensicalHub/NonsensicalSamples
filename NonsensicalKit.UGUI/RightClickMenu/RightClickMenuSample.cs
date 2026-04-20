using NonsensicalKit.Core;
using NonsensicalKit.Tools.InputTool;
using NonsensicalKit.UGUI.Table;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class RightClickMenuSample : NonsensicalMono
    {
        [SerializeField] private Text m_txt_Show;
        [SerializeField] private Sprite m_sampleSprite1;
        [SerializeField] private Sprite m_sampleSprite2;

        private InputHub _ic;
        private SpriteManager _sm;
        private List<RightClickMenuItem> _lcmis;

        private void Awake()
        {
            _sm = SpriteManager.Instance;
            _lcmis = new List<RightClickMenuItem>();

            _sm.SetSprite("sampleSprite1", () => { return m_sampleSprite1; });
            _sm.SetSprite("sampleSprite2", () => { return m_sampleSprite2; });
            _lcmis.Add(new RightClickMenuItem(null, "First Button", () => { m_txt_Show.text = "press first button"; }));
            _lcmis.Add(new RightClickMenuItem("sampleSprite1", "Second Button", () => { m_txt_Show.text = "press second button"; }));
            _lcmis.Add(new RightClickMenuItem("sampleSprite2", "Third Button", () => { m_txt_Show.text = "press third button"; }));
            _lcmis.Add(new RightClickMenuItem("sampleSprite2", "Fourth Button", () => { m_txt_Show.text = "press fourth button"; }));
        }

        private void Start()
        {
            _ic = InputHub.Instance;
            _ic.OnMouseRightButtonDown += OnMouseRightButtonDown;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _ic.OnMouseRightButtonDown -= OnMouseRightButtonDown;
        }

        private void OnMouseRightButtonDown()
        {
            Publish("OpenRightClickMenu", _lcmis);
        }
    }
}
