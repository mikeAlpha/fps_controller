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
            MuzzleObject.gameObject.SetActive(true);
            MuzzleObject.GetComponent<DestroyAsset>().mtag = "muzzle_flash";


            if (MuzzleObject != null)
            {
                timer += Time.deltaTime;
                if (timer > Random.Range(0.2f , FireRate))
                {
                    FireSource.PlayOneShot(FireClip);

                    MuzzleObject.transform.position = FirePoint.position;
                    MuzzleObject.transform.rotation = FirePoint.rotation;
                    MuzzleObject.transform.SetParent(FirePoint.transform);

                    Vector3 spreadAngle = ApplySpread(transform.root.forward, FireSpreadAngle);
                    RaycastHit hitInfo;

                    Debug.DrawRay(transform.root.position, spreadAngle * 300);

                    if (Physics.Raycast(transform.root.position, spreadAngle, out hitInfo, 300, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (hitInfo.collider.gameObject == transform.root.gameObject)
                            return;

                        Debug.Log("ShotBot====" +  hitInfo.collider.gameObject);
                        Debug.DrawRay(transform.root.position, spreadAngle * 300, Color.red);

                        Vector3 pos = hitInfo.point;
                        Vector3 norm = hitInfo.normal;

                        var hitObj = hitInfo.collider.gameObject;

                        if (hitObj.GetComponent<PlayerController>())
                        {
                            var blood_fx = PoolManager.Instance.GetObject<PoolableObject>("blood_fx");
                            blood_fx.GetComponent<DestroyAsset>().mtag = "blood_fx";
                            blood_fx.transform.position = pos;
                            blood_fx.transform.rotation = Quaternion.LookRotation(norm);

                            EventHandler.ExecuteEvent<float>(hitObj, GameEvents.OnPlayerHealthUpdate, -10);

                        }
                        else
                        {

                            //Instantiate(BulletImpact, pos, Quaternion.LookRotation(norm));
                            var BulletImpact = PoolManager.Instance.GetObject<PoolableObject>("bullet_impact");
                            BulletImpact.GetComponent<DestroyAsset>().mtag = "bullet_impact";
                            BulletImpact.transform.position = pos;
                            BulletImpact.transform.rotation = Quaternion.LookRotation(norm);
                        }
                    }

                    timer = 0f;
                }
            }
        }
    }

    Vector3 ApplySpread(Vector3 direction, float spread)
    {
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            0);

        return randomRotation * direction;
    }
}
