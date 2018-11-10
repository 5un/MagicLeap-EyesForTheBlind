// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEditor;

namespace MagicLeap.Utilities
{
    ///<summary>
    /// Editor for Placement.
    ///</summary>
    [CustomEditor(typeof(Placement), true)]
    public class PlacementEditor : Editor
    {
        //----------- Private Members -----------
        Placement _target;

        //----------- MonoBehaviour Methods -----------
        void OnEnable()
        {
            _target = target as Placement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxUneven"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowVertical"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("layerMask"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowHorizontal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("willFitVolume"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("willNotFitVolume"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("matchGravityOnVerticals"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tilt"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleVolumeWithDistance"));
            GUI.enabled = _target.scaleVolumeWithDistance;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minAutoScaleSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAutoScaleSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minAutoScaleDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAutoScaleDistance"));
            GUI.enabled = true;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPlacementEvent"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}