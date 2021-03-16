﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AutomaticGun : Gun
{
	// reference to player camera
	[SerializeField] Camera camera;

    [SerializeField] private AudioClip myClip;
    private AudioSource mySource;

	//ammo
	private int ammo_max = 50;
	private int ammo_current = 50;
	//for automatic fire
	private long time_fire=0;
	private long cooldown=100;
	//reloadtime
	private long time_reload=0;
	private long reload=5000;
	private bool isReloading;

    public override void Use()
	{
        mySource.Play();
		Shoot();
	}

	
	private void Awake()
	{
		mySource = gameObject.AddComponent<AudioSource>() as AudioSource;
		mySource.playOnAwake = false;
		mySource.clip = myClip;
	}
	
	public override void Release()
	{

	}
	void Reload()
    {
		DateTime dt = DateTime.Now;
		long cr = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		if (ammo_current <= 0)
		{
			isReloading = true;
			time_reload = cr + reload;
		}
	}
	void Shoot()
	{
			DateTime dt = DateTime.Now;
			long cr = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			//check if reloading
			if (isReloading)
			{
				if (cr < time_reload)
				{
					return;
				}
				ammo_current = ammo_max;
				isReloading = false;
			}
			//check if reloading end
			//cooldown of automatic
			if (cr < time_fire)
            {
				return;
            }
			time_fire = cr + cooldown;
		//cooldown of automatic end
		//check if there is ammo
		Reload();
			//check ammo end
		Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
			ray.origin = camera.transform.position;
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
			}
			ammo_current--;
            if (ammo_current <= 0)
            {
				isReloading = true;
				time_reload = cr + reload;
			}
		}
	
}
