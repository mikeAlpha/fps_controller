using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform FirePoint;
    public Transform MagPoint;

    public Transform rightHandIK;
    public Transform leftHandIK;

    protected AudioSource FireSource;
    protected Transform FireRayCastSource;

    protected float timer = 0f;
    protected IPlayerController attachedTo;

    protected float FireRate;
    protected AudioClip FireClip;
    protected float FireSpreadAngle;

    protected float RecoilRotation = 2f;
    protected float RecoilAmount = 0.025f;
    protected float RecoilRecoveryRate = 10f;
    protected float RecoilRandomness = 0.02f;

    public bool CurrentlySelected = false;

    private bool IsFiring = false;
    private Vector3 rightIKOriginalPos, leftIKOriginalPos, camOriginalPos;
    private Quaternion rightIKOriginalRot, leftIKOriginalRot, camOriginalRot;
    private Transform cameraTransform;
    private Vector3 spreadAngle, fireForward, firePoint;

    //private void OnEnable()
    //{
    //    var ai_obj = transform.root;
    //    if(ai_obj.GetComponent<AITree>() != null)
    //    {
    //        EventHandler.RegisterEvent(ai_obj.gameObject, GameEvents.OnAiFireUpdate, FireWeapon);
    //    }
    //}

    private void OnDisable()
    {
        if (attachedTo as PlayerControllerV2 != null)
        {
            EventHandler.UnregisterEvent(attachedTo, GameEvents.OnPlayerDied, HideWeapon);
            EventHandler.UnregisterEvent(attachedTo, GameEvents.OnPlayerFire, FireWeapon);
        }
        else if (attachedTo as AITree != null)
        {
            var ai_obj = attachedTo as AITree;
            EventHandler.UnregisterEvent(ai_obj.gameObject, GameEvents.OnAiFireUpdate, FireWeapon);   
        }
    }

    public void Update()
    {
        if (attachedTo as AITree != null)
        {
            spreadAngle = ApplySpread(FirePoint.forward, (FireSpreadAngle));
            fireForward = FirePoint.forward;
            firePoint = FirePoint.position;
            //Debug.DrawRay(FirePoint.position, spreadAngle * 300, Color.red);
        }
    }

    public void InitWeapon(BaseWeapon weapon, IPlayerController player)
    {
        attachedTo = player;
        FireRate = weapon.FireRate;
        FireClip = weapon.FireClip;
        FireSpreadAngle = weapon.FireSpreadAngle;

        if (player as PlayerControllerV2 != null) {
            rightIKOriginalPos = rightHandIK.localPosition;
            rightIKOriginalRot = rightHandIK.localRotation;
            leftIKOriginalPos = leftHandIK.localPosition;
            leftIKOriginalRot = leftHandIK.localRotation;

            var player_obj = (attachedTo as PlayerControllerV2);
            FireSource = player_obj.audioSource;
            FireRayCastSource = player_obj.playerIK.LookAtPosition;
            cameraTransform = player_obj.GetCameraTransform();

            camOriginalPos = cameraTransform.localPosition;
            camOriginalRot = cameraTransform.localRotation;

            EventHandler.RegisterEvent(player_obj.gameObject, GameEvents.OnPlayerDied, HideWeapon);
            EventHandler.RegisterEvent(player_obj.gameObject, GameEvents.OnPlayerFire, FireWeapon);
        }
        else if (player as AITree != null) {
            var ai_obj = (attachedTo as AITree);
            FireRayCastSource = ai_obj.RayCastPoint;
            FireSource = ai_obj.GetComponent<AudioSource>();
            EventHandler.RegisterEvent(ai_obj.gameObject, GameEvents.OnAiFireUpdate, FireWeapon);
        }   
    }

    async void FireWeapon()
    {

        if ((attachedTo as PlayerControllerV2) != null)
        {
            Debug.Log("Player shooot");
            var MuzzleObject = PoolManager.Instance.GetObject<PoolableObject>("muzzle_flash");
            MuzzleObject.gameObject.SetActive(true);
            MuzzleObject.GetComponent<DestroyAsset>().mtag = "muzzle_flash";

            if (MuzzleObject != null)
            {
                //if (timer < FireRate)
                //{
                //    return;
                //}
                //else
                //{
                //    timer = 0f;
                //}

                FireSource.Stop();

                //while (timer < FireRate)
                //{
                //timer += Time.deltaTime;

                FireSource.PlayOneShot(FireClip);

                MuzzleObject.transform.position = FirePoint.position;
                MuzzleObject.transform.rotation = FirePoint.rotation;
                MuzzleObject.transform.SetParent(FirePoint.transform);
                //Instantiate(MuzzleObject, FirePoint.position, FirePoint.rotation, FirePoint);

                Vector3 spreadAngle = ApplySpread(FireRayCastSource.forward, (FireSpreadAngle));

                Ray mRay = /*Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2))*/new Ray(FireRayCastSource.position, spreadAngle);
                RaycastHit hit;

                if (Physics.Raycast(mRay, out hit, 300, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {

                    if (hit.collider.gameObject == (attachedTo as PlayerControllerV2).gameObject)
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

                        EventHandler.ExecuteEvent<float>(hitObj, GameEvents.OnAiHealthUpdate, -15);

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
                await StartRecoil();
                Debug.Log("Firing finished");
                //}
            }
        }
        if ((attachedTo as AITree) != null)
        {
            Debug.Log("AI shooot");

            var ai_obj = (attachedTo as AITree).gameObject;

            var MuzzleObject = PoolManager.Instance.GetObject<PoolableObject>("muzzle_flash");
            MuzzleObject.gameObject.SetActive(true);
            MuzzleObject.GetComponent<DestroyAsset>().mtag = "muzzle_flash";


            if (MuzzleObject != null)
            {
                timer += Time.deltaTime;
                if (timer > Random.Range(0.2f, FireRate))
                {
                    FireSource.PlayOneShot(FireClip);

                    MuzzleObject.transform.position = FirePoint.position;
                    MuzzleObject.transform.rotation = FirePoint.rotation;
                    MuzzleObject.transform.SetParent(FirePoint.transform);

                    Vector3 spreadAngle = ApplySpread(fireForward, (FireSpreadAngle));
                    RaycastHit hitInfo;

                    Debug.DrawRay(firePoint, spreadAngle * 300, Color.red);

                    if (Physics.Raycast(firePoint, spreadAngle , out hitInfo, 300, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (hitInfo.collider.gameObject == ai_obj.transform)
                            return;

                        Debug.Log("ShotBot====1" + hitInfo.collider.gameObject);

                        Vector3 pos = hitInfo.point;
                        Vector3 norm = hitInfo.normal;

                        var hitObj = hitInfo.collider.gameObject;

                        if (hitObj.GetComponent<PlayerControllerV2>())
                        {
                            var blood_fx = PoolManager.Instance.GetObject<PoolableObject>("blood_fx");
                            blood_fx.GetComponent<DestroyAsset>().mtag = "blood_fx";
                            blood_fx.transform.position = pos;
                            blood_fx.transform.rotation = Quaternion.LookRotation(norm);

                            Debug.Log("ShotPlayer====2" + hitInfo.collider.gameObject);

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

    async Task StartRecoil()
    {
        if(rightHandIK == null || leftHandIK == null)
        {
            return;
        }

        //var randX = Random.Range(-RecoilRandomness, RecoilRandomness);
        //var randY = Random.Range(-RecoilRandomness, RecoilRandomness);

        var recoilOffset = new Vector3(0, 0, -RecoilAmount);
        var recoidRotation = Quaternion.Euler(-RecoilRotation, 0, 0);

        rightHandIK.localPosition += recoilOffset;
        rightHandIK.localRotation *= recoidRotation;

        leftHandIK.localPosition += recoilOffset * 2f;

        //cameraTransform.localPosition += recoilOffset * 0.5f;
        //cameraTransform.localRotation *= recoidRotation;
        //yield return new WaitForSeconds(0.1f);
        await Task.Delay(50);

        //Debug.Log("Recoil dist1======" + Vector3.Distance(rightHandIK.localPosition, rightIKOriginalPos));

        while (Vector3.Distance(rightHandIK.localPosition, rightIKOriginalPos) > 0.0001f)
        {
            if (rightHandIK == null || leftHandIK == null)
            {
                break;
            }

            //Debug.Log("Recoil dist2======"+ Vector3.Distance(rightHandIK.localPosition, rightIKOriginalPos));

            rightHandIK.localPosition = Vector3.Lerp(rightHandIK.localPosition, rightIKOriginalPos, Time.smoothDeltaTime * RecoilRecoveryRate);
            rightHandIK.localRotation = Quaternion.Lerp(rightHandIK.localRotation, rightIKOriginalRot, Time.smoothDeltaTime * RecoilRecoveryRate);
            leftHandIK.localPosition = Vector3.Lerp(leftHandIK.localPosition, leftIKOriginalPos, Time.smoothDeltaTime * RecoilRecoveryRate);
            leftHandIK.localRotation = Quaternion.Lerp(leftHandIK.localRotation, leftIKOriginalRot, Time.smoothDeltaTime * RecoilRecoveryRate);
            //cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, camOriginalPos, Time.smoothDeltaTime * RecoilRecoveryRate);
            //cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, camOriginalRot, Time.smoothDeltaTime * RecoilRecoveryRate);

            await Task.Yield();
        }

        rightHandIK.localPosition = rightIKOriginalPos;
        rightHandIK.localRotation = rightIKOriginalRot;
        leftHandIK.localPosition = leftIKOriginalPos;
        leftHandIK.localRotation = leftIKOriginalRot;
        //cameraTransform.localPosition = camOriginalPos;
        //cameraTransform.localRotation = camOriginalRot;
    }

    void HideWeapon()
    {
        gameObject.SetActive(false);
    }
}
