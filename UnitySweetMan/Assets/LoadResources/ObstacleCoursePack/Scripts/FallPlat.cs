using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlat : MonoBehaviour
{
	public float fallTime = 0.5f;
	public float showTime = 2f;

	private MeshRenderer _selfRenderer;
	private Collider _collider;


	private void Awake()
	{
		_selfRenderer = GetComponent<MeshRenderer>();
		_collider = GetComponent<BoxCollider>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (SM_SceneManager.Instance.CurLevelData.ELevelState != ELevelState.Playing)
		{
			return;
		}

		foreach (ContactPoint contact in collision.contacts)
		{
			//Debug.DrawRay(contact.point, contact.normal, Color.white);
			if (collision.gameObject.tag == "Player")
			{
				StartCoroutine(Fall(fallTime));
			}
		}
	}

	IEnumerator Fall(float time)
	{
		yield return new WaitForSeconds(time);
		EnableRender(false);
		yield return new WaitForSeconds(showTime);
		EnableRender(true);
	}

	void EnableRender(bool enable)
	{
		_selfRenderer.enabled = enable;
		_collider.enabled = enable;
	}

}
