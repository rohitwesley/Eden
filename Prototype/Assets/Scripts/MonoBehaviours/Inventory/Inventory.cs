using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] Image[] itemImages;// = new Image[numItemSlots];    // The Image components that display the Items.
    [SerializeField] Text[] itemText;// = new Text[numItemSlots];    // The Text components that display the Items.
    // [SerializeField] int[] itemCount;// = new int[numItemSlots];    // The Image components that display the Items.
    [SerializeField] Item[] items;// = new Item[numItemSlots];           // The Items that are carried by the player.


    [SerializeField] int numItemSlots = 5;                      // The number of items that can be carried.  This is a constant so that the number of Images and Items are always the same.

    private void Start()
    {
        // itemImages = new Image[numItemSlots];
        // itemText = new Text[numItemSlots];
        // itemCount = new int[numItemSlots];
        // items = new Item[numItemSlots];           // The Items that are carried by the player.
        for (int i = 0; i < items.Length; i++)
        {
            UpdateItem(items[i]);
        }

    }

    private void Update()
    {
        
        for (int i = 0; i < items.Length; i++)
        {
            UpdateItem(items[i]);
        }

    }

    // This function is called by the PickedUpItemReaction in order to add an item to the inventory.
    public void UpdateItem(Item itemToAdd)
    {
        // Go through all the item slots...
        for (int i = 0; i < items.Length; i++)
        {
            // ... if the item slot is has the item to add...
            if (items[i] == itemToAdd)
            {
                // ... set it to the picked up item and set the image component to display the item's sprite.
                items[i] = itemToAdd;
                items[i].textPlaceHolder = itemText[i];
                items[i].textPlaceHolder.text = " " + items[i].Value;
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;
                return;
            }
            // ... if the item slot is empty...
            else if (items[i] == null)
            {
                // ... set it to the picked up item and set the image component to display the item's sprite.
                items[i] = itemToAdd;
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;
                return;
            }
        }
    }

    // This function is called by the PickedUpItemReaction in order to add an item to the inventory.
    public void AddItem(Item itemToAdd)
    {
        // Go through all the item slots...
        for (int i = 0; i < items.Length; i++)
        {
            // ... if the item slot is has the item to add...
            if (items[i] == itemToAdd)
            {
                // ... set it to the picked up item and set the image component to display the item's sprite.
                items[i].Value++;
                items[i] = itemToAdd;
                items[i].textPlaceHolder = itemText[i];
                items[i].textPlaceHolder.text = " " + items[i].Value;
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;
                return;
            }
            // ... if the item slot is empty...
            else if (items[i] == null)
            {
                // ... set it to the picked up item and set the image component to display the item's sprite.
                items[i].Value++;
                items[i] = itemToAdd;
                itemImages[i].sprite = itemToAdd.sprite;
                itemImages[i].enabled = true;
                return;
            }
        }
    }


    // This function is called by the LostItemReaction in order to remove an item from the inventory.
    public void RemoveItem (Item itemToRemove)
    {
        // Go through all the item slots...
        for (int i = 0; i < items.Length; i++)
        {
            // ... if the item slot has the item to be removed...
            if (items[i] == itemToRemove && items[i].Value>0)
            {
                // ... set the item slot to null and set the image component to display nothing.
                items[i].Value--;// = null;
                items[i].textPlaceHolder = itemText[i];
                items[i].textPlaceHolder.text = " " + items[i].Value;
                // itemImages[i].sprite = null;
                // itemImages[i].enabled = false;
                return;
            }
        }
    }
}
