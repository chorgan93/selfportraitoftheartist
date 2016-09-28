using UnityEngine;
using System.Collections;

public class ItemS : MonoBehaviour {

	public int itemCost;
	public Sprite itemIcon;
	public Sprite itemIconUsed;
	public float itemCooldown = 1f;

	private float _itemCooldownCount;
	public float itemCooldownCount { get { return _itemCooldownCount; } }

	public virtual void UseItem(){

		_itemCooldownCount = itemCooldown;

	}
}
