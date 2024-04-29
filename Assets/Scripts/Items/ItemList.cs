using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList
{
    public Item item;
    public string itemName;
    public int stacks;

    public ItemList(Item newItem, string newName, int newStacks)
    {
        item = newItem;
        itemName = newName;
        stacks = newStacks;
    }
}
