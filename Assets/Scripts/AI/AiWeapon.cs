using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;

public class AiWeapon : BaseWeapon
{
    protected override void OnEnable()
    {
        EventHandler.RegisterEvent(transform.root.gameObject, GameEvents.OnAiFireUpdate, Shoot);
    }

    protected override void OnDisable()
    {
        EventHandler.UnregisterEvent(transform.root.gameObject, GameEvents.OnAiFireUpdate, Shoot);
    }

    protected override void Shoot()
    {
        //base.Shoot();
        if (transform.root.GetComponent<AITree>() != null)
        {
            Debug.Log("AI shooot");
            var MuzzleObject = PoolManager.Instance.GetObject<PoolableObject>("muzzle_flash");
            MuzzleObject.GetComponent<DestroyAsset>().mtag = "muzzle_flash";


            if (MuzzleObject != null)
            {
                timer += Time.deltaTime;
                if (timer > FireRate)
                {
                    FireSource.PlayOneShot(FireClip);

                    MuzzleObject.transform.position = FirePoint.position;
                    MuzzleObject.transform.rotation = FirePoint.rotation;
                    MuzzleObject.transform.SetParent(FirePoint.transform);

                    timer = 0f;
                }
            }
        }
    }
}
