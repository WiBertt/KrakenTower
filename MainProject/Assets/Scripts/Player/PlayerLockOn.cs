using Com.LuisPedroFonseca.ProCamera2D;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    [SerializeField] private ProCamera2D proCamera;

    // Rewired
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    //Check variables
    [SerializeField] private float scanDuration;
    [SerializeField] private LayerMask objectsToScanLayerMask;
    [SerializeField] private float checkRadius;
    [SerializeField] private float maxDistanceForLockOn;


    private Scanable scannable;

    public GameObject objLockedOnTo { get; private set; }
    private bool isScanActive;

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
    }
    private void Update()
    {
        PlayerInput();

        if (scannable != null)
            CheckDistanceFromObject();
    }

    private void CheckDistanceFromObject()
    {
        float distance = Vector2.Distance(transform.position, scannable.transform.position);

        if (distance > maxDistanceForLockOn)
            TurnOffLock();
    }

    private void LookForObjects()
    {
      
        if (scannable != null)
            return;

        var hit = Physics2D.OverlapCircleAll(transform.position, checkRadius, objectsToScanLayerMask);



        if (hit == null)
            return;

        float minDist = Mathf.Infinity;
        GameObject closestObj = null;

        foreach (var obj in hit)
        {
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            if (dist < minDist)
            {
                closestObj = obj.gameObject;
                minDist = dist;
            }
        }

        objLockedOnTo = closestObj;
        scannable = closestObj.GetComponent<Scanable>();
        scannable.EnableLockUI();
        proCamera.AddCameraTarget(scannable.transform);
    }
    private void PlayerInput()
    {
        if (player.GetButtonDown("Lock On") && scannable == null)
            LookForObjects();
        else if (player.GetButtonDown("Lock On") && scannable != null)
            TurnOffLock();
    }

    private void TurnOffLock()
    {
        proCamera.RemoveCameraTarget(scannable.transform);
        scannable.DisableLockUI();
        scannable = null;
        objLockedOnTo = null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

}
