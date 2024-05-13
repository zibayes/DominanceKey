using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public class StandardCaseViewPopup : MonoBehaviour
    {
        [SerializeField] StandardCaseView standardCaseView;
        [SerializeField] RawImage icon;
        [SerializeField] StandardButton closeButton;

        [SerializeField] RectTransform sizeSampleTarget;
        [SerializeField] RectTransform sizeTarget;
        [SerializeField] Vector2 sizeTargetOffset;

        public StandardCaseView StandardCaseView => standardCaseView;

        protected virtual StandardAssetLoader Loader { get; set; } = new StandardAssetLoader();

        public virtual void Open(IStandardCaseCellData caseData, Action onCloseButton)
        {
            standardCaseView.Apply(caseData.CaseData);
            icon.texture = caseData.image.texture;
            closeButton.SetCallback(() => onCloseButton());

            // wait for relayout
            StartCoroutine(DelayFrame(() => sizeTarget.sizeDelta = sizeSampleTarget.rect.size + sizeTargetOffset));
        }

        IEnumerator DelayFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}
