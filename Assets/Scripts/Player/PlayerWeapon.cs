using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerWeapon : BaseWeapon
{

    protected override void OnEnable()
    {
        EventHandler.RegisterEvent(transform.root.gameObject, GameEvents.OnPlayerDied, HideObject);
        EventHandler.RegisterEvent(transform.root.gameObject,GameEvents.OnPlayerFire, Shoot);
    }

    protected override void OnDisable()
    {
        EventHandler.UnregisterEvent(transform.root.gameObject, GameEvents.OnPlayerDied, HideObject);
        EventHandler.UnregisterEvent(transform.root.gameObject,GameEvents.OnPlayerFire, Shoot);
    }

    protected override void Shoot()
    {
        if (CurrentlySelected)
        {
            //base.Shoot();
            Debug.Log("Player shooot");
            if (transform.root.GetComponent<PlayerController>() != null)
            {
                var MuzzleObject = PoolManager.Instance.GetObject<PoolableObject>("muzzle_flash");
                MuzzleObject.gameObject.SetActive(true);
                MuzzleObject.GetComponent<DestroyAsset>().mtag = "muzzle_flash";

                if (MuzzleObject != null)
                {
                    if (timer > 0)
                    {
                        timer = 0f;
                    }

                    FireSource.Stop();

                    while (timer < FireRate)
                    {
                        timer += Time.deltaTime;

                        FireSource.PlayOneShot(FireClip);

                        MuzzleObject.transform.position = FirePoint.position;
                        MuzzleObject.transform.rotation = FirePoint.rotation;
                        MuzzleObject.transform.SetParent(FirePoint.transform);
                        //Instantiate(MuzzleObject, FirePoint.position, FirePoint.rotation, FirePoint);

                        Ray mRay = /*Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2))*/new Ray(FireRayCastSource.position, FireRayCastSource.forward);
                        RaycastHit hit;

                        if (Physics.Raycast(mRay, out hit, 300, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                        {

                            if (hit.collider.gameObject == transform.root.gameObject)
                                return;

                            Vector3 pos = hit.point;
                            Vector3 norm = hit.normal;

                            Debug.Log("ShotPlayer====" + hit.collider.gameObject);

                            var hitObj = hit.collider.gameObject;

                            if (hitObj.GetComponent<AITree>())
                            {
                                var blood_fx = PoolManager.Instance.GetObject<PoolableObject>("blood_fx");
                                blood_fx.GetComponent<DestroyAsset>().mtag = "blood_fx";
                                blood_fx.transform.position = pos;
                                blood_fx.transform.rotation = Quaternion.LookRotation(norm);

                                EventHandler.ExecuteEvent<float>(hitObj, GameEvents.OnAiHealthUpdate, -1);

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
                    }
                }
            }
        }
    }
}