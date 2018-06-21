﻿using Assets.Scripts.Missions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MultipleBombsAssembly
{
    public class MissionDetailPageMonitor : MonoBehaviour
    {
        public MultipleBombs MultipleBombs { get; set; }
        private MissionDetailPage page;
        private TMPro.TextAlignmentOptions originalAlignment;
        private bool originalEnableAutoSizing;

        private void Awake()
        {
            page = GetComponent<MissionDetailPage>();
        }

        private void OnEnable()
        {
            StartCoroutine(changeTextNextFrame());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            page.TextStrikes.alignment = originalAlignment;
            page.TextStrikes.enableAutoSizing = originalEnableAutoSizing;
        }

        private IEnumerator changeTextNextFrame()
        {
            originalAlignment = page.TextStrikes.alignment;
            originalEnableAutoSizing = page.TextStrikes.enableAutoSizing;
            yield return null;
            Mission currentMission = (Mission)page.GetType().BaseType.GetField("currentMission", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(page);
            List<ComponentPool> bombPools;
            int bombCount = MultipleBombs.ProcessMultipleBombsMission(currentMission, out bombPools);
            if (bombCount > 1)
            {
                page.SetMission(currentMission, page.BombBinder.MissionTableOfContentsPageManager.GetMissionEntry(currentMission.ID).Selectable, page.BombBinder.MissionTableOfContentsPageManager.GetCurrentToCIndex(), page.BombBinder.MissionTableOfContentsPageManager.GetCurrentPage());
                currentMission.GeneratorSetting.ComponentPools.AddRange(bombPools); //Null checking left out as this code is temporary
                page.TextStrikes.alignment = TMPro.TextAlignmentOptions.Right;
                page.TextStrikes.enableAutoSizing = false;
                page.TextStrikes.text = bombCount + " Bombs\n" + page.TextStrikes.text + "\n ";
                if (bombCount > MultipleBombs.GetCurrentMaximumBombCount())
                {
                    page.GetType().GetField("canStartMission", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(page, false);
                    page.TextDescription.text = "A room that can support more bombs is required.\n\nCurrent rooms only support up to " + MultipleBombs.GetCurrentMaximumBombCount() + " bombs.";
                }
            }
        }
    }
}
