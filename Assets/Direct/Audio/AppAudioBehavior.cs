// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using MagicLeap.Utilities;
using UnityEngine;

namespace MagicKit
{
    public class AppAudioBehavior : AudioBehavior
    {
        //----------- Private Members -----------

        [SerializeField] private AppController _appController;
        private const float CancelCooldown = 1f;
        private float _nextCancelTime = 0f;

        //----------- MonoBehaviour Methods -----------

        private void OnEnable()
        {
            _appController.OnSummonUi += HandleSummonUi;
            _appController.OnSelectStory += HandleSelectStory;
            _appController.OnGoodPlacement += HandleGoodPlacement;
            _appController.OnBadPlacement += HandleBadPlacement;
        }

        private void OnDisable()
        {
            _appController.OnSummonUi -= HandleSummonUi;
            _appController.OnSelectStory -= HandleSelectStory;
            _appController.OnGoodPlacement -= HandleGoodPlacement;
            _appController.OnBadPlacement -= HandleBadPlacement;
        }

        //----------- Public Methods -----------

        public void PlayIconHovered()
        {
            PlaySound("headpose_pan");
        }

        //----------- Event Handlers -----------

        private void HandleSummonUi()
        {
            PlaySound("summon_gallery");
        }

        private void HandleSelectStory()
        {
            PlaySound("select_story");
        }

        private void HandleGoodPlacement()
        {
            PlaySound("good_placement");
        }

        private void HandleBadPlacement()
        {
            // Only allow one cancel gesture per cancelCooldown duration
            if (Time.time > _nextCancelTime)
            {
                PlaySound("bad_placement");
                _nextCancelTime = Time.time + CancelCooldown;
            }
        }
    }
}