using System.Collections.Generic;
using UnityEngine;

public partial class PlayerControllerV2
{
    public Inventory<IInventoryItem> inventory = new Inventory<IInventoryItem>();
    private IInventoryItem currentItem;

    public List<EquippableItem> inventory_items = new List<EquippableItem>();

    public void OnInitInventory()
    {
        if(inventory != null && inventory_items.Count > 0)
        {
            foreach (var weapon in inventory_items)
            {
                EquipWeapon(weapon as BaseWeapon);
            }
            
            //This is Temporary
            currentItem = inventory_items[0];
            
            UseItem();
        }
    }

    public void EquipWeapon(BaseWeapon weapon)
    {
        inventory.AddItem(weapon);
    }

    public void RemoveWeapon(BaseWeapon weapon)
    {
        inventory.RemoveItem(weapon);
    }

    public void UseItem()
    {
        if (currentItem != null)
        {

            if(currentItem is BaseWeapon)
            {
                var wp_config = currentItem as BaseWeapon;
                var wp = Instantiate(wp_config.DisplayObject, playerIK.RightHand);
                wp.transform.localPosition = new Vector3(-0.0769999996f, 0.230399996f, 0.0412000008f);
                wp.transform.localRotation = Quaternion.Euler(-180, -90, 90);
                var wp_controller = wp.GetComponent<WeaponController>();
                ApplyWeaponIK(wp_controller,wp_config);
                wp_controller.InitWeapon(wp_config,this);
                //playerLookSettings.MagPos = wc.DisplayObject.GetComponent<WeaponController>().MagPoint;
            }
            currentItem.Use();
        }
    }
}

public class Inventory<T> where T : IInventoryItem
{
    List<T> items = new List<T>();

    public void AddItem(T item)
    {
        if(items.Contains(item))
        {
            items.Add(item);
        }
    }

    public void RemoveItem(T item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }
}
