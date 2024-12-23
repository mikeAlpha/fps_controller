//using System.Collections;
//using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] private float FireRate = 0.5f;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private AudioClip FireClip;

    private AudioSource FireSource;

    private Transform FireRayCastSource;

    protected virtual void Start()
    {
        FireRayCastSource = transform.root.GetComponent<PlayerController>().RaycastPoint;
        FireSource = GetComponent<AudioSource>();
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    protected virtual void Shoot()
    {
        var MuzzleObject = PoolManager.Instance.GetObject<PoolableObject>("muzzle_flash");
        MuzzleObject.GetComponent<DestroyAsset>().mtag = "muzzle_flash";

        if (MuzzleObject != null)
        {
            float timer = 0f;
            FireSource.Stop();
            while (timer < FireRate)
            {
                timer += Time.deltaTime;

                FireSource.PlayOneShot(FireClip);

                MuzzleObject.transform.position = FirePoint.position;
                MuzzleObject.transform.rotation = FirePoint.rotation;
                MuzzleObject.transform.SetParent(FirePoint.transform);
                //Instantiate(MuzzleObject, FirePoint.position, FirePoint.rotation, FirePoint);

                Ray mRay = /*Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2))*/new Ray(FireRayCastSource.position,FireRayCastSource.forward);
                RaycastHit hit;
                if (Physics.Raycast(mRay, out hit, 300, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {

                    if (hit.collider.gameObject == transform.root.gameObject)
                        return;

                    Vector3 pos = hit.point;
                    Vector3 norm = hit.normal;

                    //Instantiate(BulletImpact, pos, Quaternion.LookRotation(norm));
                    var BulletImpact = PoolManager.Instance.GetObject<PoolableObject>("bullet_impact");
                    BulletImpact.GetComponent<DestroyAsset>().mtag = "bullet_impact";
                    BulletImpact.transform.position = pos;
                    BulletImpact.transform.rotation = Quaternion.LookRotation(norm);
                }

                //RaycastHit[] hits = Physics.RaycastAll(mRay, 300);

                //Debug.DrawRay(mRay.origin, mRay.direction * 300);

                //foreach (var hit in hits)
                //{
                //    if (hit.collider.gameObject == transform.root.gameObject)
                //        continue;

                //    Debug.Log("Hit: " + hit.collider.name + " " + transform.root.gameObject);

                //    Vector3 pos = hit.point;
                //    Vector3 norm = hit.normal;

                //    //Instantiate(BulletImpact, pos, Quaternion.LookRotation(norm));
                //    var BulletImpact = PoolManager.Instance.GetObject<PoolableObject>("bullet_impact");
                //    BulletImpact.GetComponent<DestroyAsset>().mtag = "bullet_impact";
                //    BulletImpact.transform.position = pos;
                //    BulletImpact.transform.rotation = Quaternion.LookRotation(norm);

                //    break;
                //}

            }
        }
    }
}
