using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ProjectPalladium.Items;
using ProjectPalladium.UI;
namespace ProjectPalladium
{
    public class Inventory
    {
        public static int INVENTORY_SIZE = 30; // zero indexed
        private List<Item> inventory = new List<Item>(INVENTORY_SIZE);
        public InventoryUI ui;
        public Inventory(InventoryUI ui)
        {
            this.ui = ui;
            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                inventory.Add(Item.none);
            }
            // inventory[3] = Item.Items["ectoplasmic gem"].Clone();

        }
        public int Size()
        {
            return inventory.Count;
        }

        // TODO: Implement item stacking
        public bool AddItem(Item item, int amount)
        {
            int firstEmptyInv = FindItem(Item.none);
            if (firstEmptyInv == -1) { return false; } // the player's inventory is full

            // find the first non-full item stack
            int index = -1;
            do
            {
                index = FindItem(item, index + 1); // find next item stack

            }
            while (index != -1 && inventory[index].quantity == item.stackSize);

            if (index == -1) // player does not have a non-full stack
            {
                Item newItem = item.Clone();
                newItem.quantity = amount;
                inventory[firstEmptyInv] = newItem;
                
            }
            else
            {
                if (inventory[index].quantity + amount <= item.stackSize) { inventory[index].quantity += amount; }
                else
                {
                    int remainder = item.stackSize - inventory[index].quantity;
                    AddItem(item, remainder);
                    AddItem(item, amount - remainder);
                }
            }
            ui.UpdateInventory();
            
            return true;
        }

        public int FindItem(Item item, int index = 0)
        {
            // Debug.WriteLine(index);
            return inventory.FindIndex(index, new Predicate<Item>(i =>
            {
                return i.name == item.name;
            }));
        }

        public bool RemoveItem(Item item, int amount)
        {
            int index = FindItem(item);
            if (index == -1) return false; // the player does not have the item


            int totalItems = 0;
            do
            {
                totalItems += inventory[index].quantity;
                Debug.WriteLine(totalItems);
                index = FindItem(item, index + 1);
            }

            while (totalItems < amount && index != -1);

            if (totalItems < amount) { ui.UpdateInventory(); return false; } // the player does not have enough items

            index = FindItem(item); // get location of first stack
            int remainder = amount - inventory[index].quantity;
            int numRemoved = inventory[index].quantity;

            if (remainder <= 0) { 
                inventory[index].quantity -= amount; 
                if (inventory[index].quantity == 0) { inventory[index] = Item.none; ui.UpdateInventory();}

                return true; } // the player has enough in this stack, so just remove it and move on

            // otherwise, we need to recursively remove from other stacks

            inventory[index] = Item.none;
            ui.UpdateInventory();

            // if there are still items to be removed, call recursively
            RemoveItem(item, amount - numRemoved);
            

            return true;
        }

        public Item GetAtIndex(int index)
        {
            if (index >= inventory.Count || index < 0) return Item.none;
            else return inventory[index];
        }

        public void SwapItems(int index1, int index2)
        {
            Item tmp = inventory[index1];
            inventory[index1] = inventory[index2];
            inventory[index2] = tmp;
            ui.UpdateInventory();
        }
    }
}
