using UnityEngine;

public class DecoManager : MonoBehaviour
{
    [SerializeField] public bool isDecoTime = false;

    [SerializeField] private GameObject circleBurnDecor;
    [SerializeField] private GameObject circlenormalDecor;
    [SerializeField] private GameObject circlelessDecor;

    [SerializeField] private GameObject heartBurnDecor;
    [SerializeField] private GameObject heartnormalDecor;
    [SerializeField] private GameObject heartlessDecor;

    [SerializeField] private GameObject starBurnDecor;
    [SerializeField] private GameObject starnormalDecor;
    [SerializeField] private GameObject starlessDecor;

    [SerializeField] private GameObject cookieManBurnDecor;
    [SerializeField] private GameObject cookieMannormalDecor;
    [SerializeField] private GameObject cookieManlessDecor;

    private void Update()
    {
        // Circle
        if (CheckItemManager.Instance.IsItemUsed(ItemName.Circle))
        {
            if (CheckItemManager.Instance.IsItemUsed(ItemName.burn))
            {
                circleBurnDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.normal))
            {
                circlenormalDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.less))
            {
                circlelessDecor.SetActive(true);
                isDecoTime = true;
            }
        }

        // Heart
        if (CheckItemManager.Instance.IsItemUsed(ItemName.Heart))
        {
            if (CheckItemManager.Instance.IsItemUsed(ItemName.burn))
            {
                heartBurnDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.normal))
            {
                heartnormalDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.less))
            {
                heartlessDecor.SetActive(true);
                isDecoTime = true;
            }
        }

        // Star
        if (CheckItemManager.Instance.IsItemUsed(ItemName.Star))
        {
            if (CheckItemManager.Instance.IsItemUsed(ItemName.burn))
            {
                starBurnDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.normal))
            {
                starnormalDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.less))
            {
                starlessDecor.SetActive(true);
                isDecoTime = true;
            }
        }

        // CookieMan
        if (CheckItemManager.Instance.IsItemUsed(ItemName.CookieMan))
        {
            if (CheckItemManager.Instance.IsItemUsed(ItemName.burn))
            {
                cookieManBurnDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.normal))
            {
                cookieMannormalDecor.SetActive(true);
                isDecoTime = true;
            }
            else if (CheckItemManager.Instance.IsItemUsed(ItemName.less))
            {
                cookieManlessDecor.SetActive(true);
                isDecoTime = true;
            }
        }
    }
}
