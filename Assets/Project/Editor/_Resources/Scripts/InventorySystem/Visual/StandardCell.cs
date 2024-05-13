using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public class StandardCell : VariableInventoryCell
    {
        [SerializeField] Vector2 defaultCellSize;
        [SerializeField] Vector2 margineSpace;

        [SerializeField] RectTransform sizeRoot;
        [SerializeField] RectTransform target;
        [SerializeField] Graphic background;
        [SerializeField] RawImage cellImage;
        [SerializeField] Graphic highlight;
        [SerializeField] TextMeshProUGUI amount;
        [SerializeField] TextMeshProUGUI ammoCount;

        [SerializeField] StandardButton button;

        public override Vector2 DefaultCellSize => defaultCellSize;
        public override Vector2 MargineSpace => margineSpace;
        protected override IVariableInventoryCellActions ButtonActions => button;
        protected virtual StandardAssetLoader Loader { get; set; }

        protected bool isSelectable = true;
        protected Sprite currentImage;

        public Vector2 GetCellSize()
        {
            var width = ((CellData?.Width ?? 1) * (defaultCellSize.x + margineSpace.x)) - margineSpace.x;
            var height = ((CellData?.Height ?? 1) * (defaultCellSize.y + margineSpace.y)) - margineSpace.y;
            return new Vector2(width, height);
        }

        public Vector2 GetRotateCellSize()
        {
            var isRotate = CellData?.IsRotate ?? false;
            var cellSize = GetCellSize();
            if (isRotate)
            {
                var tmp = cellSize.x;
                cellSize.x = cellSize.y;
                cellSize.y = tmp;
            }

            return cellSize;
        }

        public override void SetSelectable(bool value)
        {
            ButtonActions.SetActive(value);
            isSelectable = value;
        }

        public virtual void SetHighLight(bool value)
        {
            highlight.gameObject.SetActive(value);
        }

        protected override void OnApply()
        {
            SetHighLight(false);
            target.gameObject.SetActive(CellData != null);
            ApplySize();

            if (CellData == null)
            {
                cellImage.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
            }
            else
            {
                // update cell image
                if (currentImage != CellData.image)
                {
                    currentImage = CellData.image;

                    cellImage.gameObject.SetActive(false);

                    cellImage.texture = CellData.image.texture;
                }

                if (CellData.IsStackable && CellData.currentAmount > 1)
                {
                    amount.gameObject.SetActive(true);
                    amount.text = CellData.currentAmount + "";
                }
                else
                {
                    amount.gameObject.SetActive(false);
                }
                if (CellData.type == "weapon")
                {
                    ammoCount.gameObject.SetActive(true);
                    ammoCount.text = CellData.currentAmmo + "/" + CellData.magSize;
                }
                else
                {
                    ammoCount.gameObject.SetActive(false);
                }

                background.gameObject.SetActive(true && isSelectable);
            }
        }

        protected virtual void ApplySize()
        {
            sizeRoot.sizeDelta = GetRotateCellSize();
            target.sizeDelta = GetCellSize();
            target.localEulerAngles = Vector3.forward * (CellData?.IsRotate ?? false ? 90 : 0);
        }
    }
}
