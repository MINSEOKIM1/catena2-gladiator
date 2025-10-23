using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shop
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] shopItems;

        private void Start()
        {
            // TODO: Load shop items
        }

        private void SetItemInfo(int index, Sprite icon, string itemName, string description)
        {
            var itemSlotTransform = shopItems[index].transform;
            itemSlotTransform.GetChild(0).GetComponent<Image>().sprite = icon;
            itemSlotTransform.GetChild(1).GetComponent<TMP_Text>().text = itemName;
            itemSlotTransform.GetChild(2).GetComponent<TMP_Text>().text = description;
        }
        
        public void QuitShop()
        {
            SceneManager.LoadScene("Scenes/Map");
        }
    }
}
