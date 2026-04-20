using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UGUI.Table.Sample
{
    public class ScrollTableSampleCell : ScrollTableCell
    {
        [SerializeField] private Button m_btn_click;

        private string CatchGuid => _catchGuid ??= Guid.NewGuid().ToString().Substring(0,8);

        private string _catchGuid;

        private void Awake()
        {
            m_btn_click.onClick.AddListener(OnButtonClick);
        }

        protected override void SetText(string text)
        {
            base.SetText(text);
            m_txt_content.text = text + "_" + CatchGuid;
        }

        private void OnButtonClick()
        {
            Debug.Log(_catchGuid+" Clicked");
        }
    }
}
