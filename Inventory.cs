using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            inventory[0] = Item.GetItemFromRegistry("wand");
            inventory[4] = Item.GetItemFromRegistry("ectoplasmic gem");
            inventory[4].quantity = 20;
            inventory[5] = Item.GetItemFromRegistry("cauldron");
            inventory[5].quantity = 2;
            inventory[6] = Item.GetItemFromRegistry("magmarock");
            inventory[6].quantity = 30;
            inventory[7] = Item.GetItemFromRegistry("mana melon seed");
            inventory[7].quantity = 30;

            
            

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

        // force add an item, very dangerous b/c no checking.
        public void AddItemAtIndex(int index, Item item)
        {
            inventory[index] = item;
            ui.UpdateInventory();
        }
        // force remove an item, very dangerous b/c no checking.
        public void RemoveItemAtIndex(int index, Item item)
        {
            inventory[index] = Item.none;
            ui.UpdateInventory();
        }
       
        public void RemoveCurrentItem(int quantity)
        {
            
            RemoveItemAtIndex(Game1.player.ActiveItemIndex, quantity);
        }

        public void RemoveItemAtIndex(int index, int quantity)
        {
            if (inventory[index].quantity <= quantity)
            {
                if (Game1.player.ActiveItem.IsSameItemStack(inventory[index]))
                {
                    Game1.player.ActiveItem = Item.none;
                }
                inventory[index] = Item.none;
            }
            else
            {
                inventory[index].quantity -= quantity;
            }

            ui.UpdateInventory();
        }

        public int FindItem(Item item, int index = 0)
        {
            if (item == null)
            {
                return -1;
            }
            return inventory.FindIndex(index, new Predicate<Item>(i =>
            {
                return i == item;
            }));
        }

        public int FindExactItemStack(Item item)
        {
            return inventory.FindIndex(new Predicate<Item>(i =>
            {
                return i.IsSameItemStack(item);
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
                index = FindItem(item, index + 1);
            }

            while (totalItems < amount && index != -1);

            if (totalItems < amount) { ui.UpdateInventory(); return false; } // the player does not have enough items

            index = FindItem(item); // get location of first stack
            int remainder = amount - inventory[index].quantity;
            int numRemoved = inventory[index].quantity;

            if (remainder <= 0) { 
                inventory[index].quantity -= amount; 
                if (inventory[index].quantity == 0) { 
                    if (inventory[index] == Game1.player.ActiveItem) { Game1.player.ActiveItem = Item.none; }
                    inventory[index] = Item.none; 
                    ui.UpdateInventory();
                }

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

        public void SwapItems(int index2)
        {
            
            Item tmp = inventory[index2].Clone();
            inventory[index2] = ui.ghostItem.item;

            ui.CreateGhostItem(tmp);

            ui.UpdateInventory();
        }
    }
}
