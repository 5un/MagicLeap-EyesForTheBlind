// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

﻿using UnityEngine;

namespace MagicKit
{
	/// <summary>
	/// Destroy GameObject when its attached ParticleSystem terminates.
	/// </summary>
	public class AutoDestroyPFX : MonoBehaviour
	{
		private ParticleSystem _particleSystem;

		private void OnEnable()
		{
			_particleSystem = GetComponent<ParticleSystem>();
			if (_particleSystem == null)
			{
				enabled = false;
			}
		}

		private void LateUpdate()
		{
			if (!_particleSystem.IsAlive(true))
			{
				Destroy(gameObject);
			}
		}
	}
}