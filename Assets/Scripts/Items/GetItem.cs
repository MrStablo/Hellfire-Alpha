using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    public Item item;
    public Items itemDrop;

    public Item AssignItem(Items ItemToAssign)
    {
        switch (ItemToAssign)
        {
            case Items.TestItem:
                return new TestItem();
            default:
                return null;
        }
    }

    public void AddItem(PlayerStats stats)
    {
        foreach(ItemList i in stats.items)
        {
            if(i.itemName == item.GiveName())
            {
                i.stacks += 1;
                return;
            }
        }
        stats.items.Add(new ItemList(item, item.GiveName(), 1));
    }

    public enum Items
    {
        TestItem,
    }
}
