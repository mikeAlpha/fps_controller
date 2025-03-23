using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string ItemName { get; set; }
    string ItemDescription { get; set; }
    Sprite ItemIcon { get; set; }
    void Use();
}