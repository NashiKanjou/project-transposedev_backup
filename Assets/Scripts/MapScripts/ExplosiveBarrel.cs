﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviourPunCallbacks, IDamageable {
    //public GameObject Barrel;
    public GameObject explosionEffect;
    public AudioSource explosionSound;

    [SerializeField]
    private float explosionRange;
    private float targetDamage = 100;
    private float barrelHealth = 50;
    private bool played = false;

    PhotonView PV;

    private void Awake() {
        PV = GetComponent<PhotonView>();
    }

    public void Explode() {

        Collider[] targets = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (Collider target in targets) {
            if (target.gameObject.GetComponent<PlayerMovement>() != null || target.gameObject.GetComponent<PlayerMovement_Grappler>() != null) { // Check it's a player
                target.gameObject.GetComponent<IDamageable>()?.TakeDamage(100, this);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

    public void TakeDamage(float damage, Component source)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        Debug.Log("Barrel is taking damage");
        barrelHealth -= damage;

        if (barrelHealth <= 0)
		{
            Instantiate(explosionEffect, transform.position, transform.rotation);
            explosionSound.Play();

            if (PV.IsMine)
            {
                Explode();
                GameManager.Instance.DestroyHazard(gameObject);
            }
		}
    }
}
