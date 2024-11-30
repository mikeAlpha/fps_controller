using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private float FireRate = 0.5f;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject MuzzleObject;
    [SerializeField] private GameObject BulletImpact;

    public bool CurrentlySelected = false;

    private void OnEnable()
    {
        EventHandler.RegisterEvent(GameEvents.OnPlayerFire, Shoot);
    }

    private void OnDisable()
    {
        EventHandler.UnregisterEvent(GameEvents.OnPlayerFire, Shoot);
    }

    void Shoot()
    {
        if (MuzzleObject != null && CurrentlySelected)
        {
            float timer = 0f;
            while (timer < FireRate) { 
                timer += Time.deltaTime;
                Instantiate(MuzzleObject,FirePoint.position,FirePoint.rotation,FirePoint);

                Ray mRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                RaycastHit hit;
                if(Physics.Raycast(mRay,  out hit, 300))
                {
                    Vector3 pos = hit.point;
                    Vector3 norm = hit.normal;

                    Instantiate(BulletImpact, pos, Quaternion.LookRotation(norm));
                }
            }
        }
    }
}
