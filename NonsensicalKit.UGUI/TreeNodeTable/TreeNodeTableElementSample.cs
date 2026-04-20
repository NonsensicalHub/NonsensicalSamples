using NonsensicalKit.Tools;
using NonsensicalKit.UGUI.Table;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit.UGUI.Samples.Table
{
    public class TreeNodeTableElementSample : TreeNodeTableElementDraggableBase<TreeNodeClassSample>
    {
        [SerializeField] private Text m_txt_NodeName;

        [SerializeField] private Button m_btn_Visible;
        [SerializeField] private Button m_btn_Name;

        [SerializeField] private Image m_img_VisibleState;

        [SerializeField] private Sprite m_sp_Visible;
        [SerializeField] private Sprite m_sp_Invisible;

        public bool IsVisible { get { return ElementData.IsVisible; } set { ElementData.IsVisible = value; } }

        protected override void Awake()
        {
            base.Awake();
            m_btn_Visible.onClick.AddListener(OnVisibleButtonClick);
            m_btn_Name.onClick.AddListener(OnNameClick);
        }

        public override void SetValue(TreeNodeClassSample elementData)
        {
            base.SetValue(elementData);
            elementData.Belong = gameObject;
            ChangeVisible(elementData.IsVisible);
            m_txt_NodeName.text = elementData.NodeName;
        }

        private void OnNameClick()
        {
            Debug.Log(ElementData.NodeName + " Click");
        }

        private void OnVisibleButtonClick()
        {
            ChangeVisible(!IsVisible);
        }

        private void ChangeVisible(bool isVisible)
        {
            IsVisible = isVisible;
            if (IsVisible)
            {
                m_img_VisibleState.sprite = m_sp_Visible;
            }
            else
            {
                m_img_VisibleState.sprite = m_sp_Invisible;
            }
        }

        public override void OnFocus()
        {
            base.OnFocus();
            GetComponent<RectTransform>().DoLocalScale(Vector3.one * 1.2f, 0.3f).OnCompleteEvent += () => GetComponent<RectTransform>().DoLocalScale(Vector3.one, 0.3f);
        }
    }
}
