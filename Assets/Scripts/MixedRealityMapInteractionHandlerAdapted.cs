// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;



/*
 * This script was adapted by Jorge Wagner based on the original MixedRealityMapInteractionHandler
 * script from the Bing Maps SDK repository (https://github.com/microsoft/MapsSDK-Unity/blob/master/SampleProject/Assets/Microsoft.Maps.Unity.Examples/Common/Scripts/MixedRealityMapInteractionHandler.cs)
 * and the ObjectManipulator standard script from MRTK.
 * 
 * The goal was to implement zooming in and out based on holding the map with the two pointers
 * and moving them closer or farther apart (as opposed to by double clicking as in the original).
 */




/// <summary>
/// Handles panning and dragging the <see cref="MapRenderer"/> via pointer rays, and zooming in and out of a selected location.
/// </summary>
[RequireComponent(typeof(MapInteractionController))]
public class MixedRealityMapInteractionHandlerAdapted : MapInteractionHandler, IMixedRealityPointerHandler, IMixedRealityInputHandler<Vector2>, IMixedRealityFocusHandler
{
    private bool _isFocused = false;
    bool hasValidPriorPosition = false, hasValidPriorPositionsForTwoPointers = false;
    Vector3 oldCenterBetweenTargets, oldTargetPointInLocalSpace, oldTargetPointInLocalSpaceLeft, oldTargetPointInLocalSpaceRight;

    private void OnEnable()
    {
    }

    private void Update()
    {
    }

    private void OnDisable()
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

    public virtual void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (!TryGetPointerDataWithId(eventData.Pointer.PointerId, out _) && CoreServices.InputSystem.FocusProvider.TryGetFocusDetails(eventData.Pointer, out var focusDets) &&
    focusDets.Object == gameObject)
        {
            pointerDataList.Add(new PointerData(eventData.Pointer, eventData.Pointer.Result.Details.Point));

            eventData.Pointer.IsTargetPositionLockedOnFocusLock = false;
        }

        if (pointerDataList.Count > 0)
        {
            // Always mark the pointer data as used to prevent any other behavior to handle pointer events
            // as long as the ObjectManipulator is active.
            // This is due to us reacting to both "Select" and "Grip" events.
            eventData.Use();
        }
    }

    public virtual void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        // Call manipulation updated handlers
        if (pointerDataList.Count == 1)
        {
            HandleOneHandMoveUpdated(eventData);
        }
        else if (pointerDataList.Count > 1)
        {
            HandleTwoHandManipulationUpdated();
        }

        eventData.Use();
    }

    void HandleOneHandMoveUpdated(MixedRealityPointerEventData eventData)
    {
        Debug.Assert(pointerDataList.Count == 1);
        IMixedRealityPointer pointer = pointerDataList[0].Pointer;

        if (hasValidPriorPosition)
        {

            var targetPointInMercator =
                MapRenderer.TransformLocalPointToMercatorWithAltitude(
                oldTargetPointInLocalSpace,
                out var targetAltitudeInMeters,
                out _);

            var newTargetPointInLocalSpace = pointer.Result.Details.PointLocalSpace;

            var rayTargetPoint = MapRenderer.transform.TransformPoint(newTargetPointInLocalSpace);
            var ray = new Ray(pointer.Position, (rayTargetPoint - pointer.Position).normalized);
            MapInteractionController.PanAndZoom(ray, targetPointInMercator, targetAltitudeInMeters, 0f);

            // UpdatePlots(); // Update the data visualizations linked to this map -- alternatively, use the event system for this 
        }

        oldTargetPointInLocalSpace = pointer.Result.Details.PointLocalSpace;

        hasValidPriorPosition = true;
    }

    void HandleTwoHandManipulationUpdated()
    {
        Debug.Assert(pointerDataList.Count == 2);
        IMixedRealityPointer p1 = pointerDataList[0].Pointer;
        IMixedRealityPointer p2 = pointerDataList[1].Pointer;

        if (hasValidPriorPositionsForTwoPointers)
        {
            var targetPointInMercator = MapRenderer.TransformLocalPointToMercatorWithAltitude(
                oldCenterBetweenTargets,
                out var targetAltitudeInMeters,
                out _);

            var newTargetPointInLocalSpaceLeft = p1.Result.Details.PointLocalSpace;
            var newTargetPointInLocalSpaceRight = p2.Result.Details.PointLocalSpace;
            var newCenterBetweenTargets = (newTargetPointInLocalSpaceLeft + newTargetPointInLocalSpaceRight) / 2f;

            float prevTouchDeltaMag = (new Vector2(oldTargetPointInLocalSpaceLeft.x, oldTargetPointInLocalSpaceLeft.z) - new Vector2(oldTargetPointInLocalSpaceRight.x, oldTargetPointInLocalSpaceRight.z)).magnitude;
            float touchDeltaMag = (new Vector2(newTargetPointInLocalSpaceLeft.x, newTargetPointInLocalSpaceLeft.z) - new Vector2(newTargetPointInLocalSpaceRight.x, newTargetPointInLocalSpaceRight.z)).magnitude;
            var touchPointDeltaToInitialDeltaRatio = touchDeltaMag / prevTouchDeltaMag;
            var _initialMapDimensionInMercator = Mathf.Pow(2, MapRenderer.ZoomLevel - 1);
            var newMapDimensionInMercator = touchPointDeltaToInitialDeltaRatio * _initialMapDimensionInMercator;
            float newZoomLevel = Mathf.Log(newMapDimensionInMercator) / Mathf.Log(2) + 1f;
            float zoomspeed = (newZoomLevel - MapRenderer.ZoomLevel) / Time.deltaTime;

            var rayTargetPoint = MapRenderer.transform.TransformPoint(newCenterBetweenTargets);
            var ray = new Ray((p1.Position + p2.Position) / 2f, (rayTargetPoint - (p1.Position + p2.Position) / 2f).normalized);
            MapInteractionController.PanAndZoom(ray, targetPointInMercator, targetAltitudeInMeters, zoomspeed);

            // UpdatePlots(); // Update the data visualizations linked to this map -- alternatively, use the event system for this 

        }

        oldTargetPointInLocalSpaceLeft = p1.Result.Details.PointLocalSpace;
        oldTargetPointInLocalSpaceRight = p2.Result.Details.PointLocalSpace;
        oldCenterBetweenTargets = (oldTargetPointInLocalSpaceLeft + oldTargetPointInLocalSpaceRight) / 2f;

        hasValidPriorPositionsForTwoPointers = true;
    }


    public virtual void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (TryGetPointerDataWithId(eventData.Pointer.PointerId, out PointerData pointerDataToRemove))
        {
            pointerDataList.Remove(pointerDataToRemove);
        }

        hasValidPriorPosition = false;
        hasValidPriorPositionsForTwoPointers = false;
    }

    public void OnInputChanged(InputEventData<Vector2> eventData)
    {

    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        _isFocused = eventData.NewFocusedObject == gameObject;
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        _isFocused = false;
    }




    #region Private methods

    private bool TryGetPointerDataWithId(uint id, out PointerData pointerData)
    {
        int pointerDataListCount = pointerDataList.Count;
        for (int i = 0; i < pointerDataListCount; i++)
        {
            PointerData data = pointerDataList[i];
            if (data.Pointer.PointerId == id)
            {
                pointerData = data;
                return true;
            }
        }

        pointerData = default(PointerData);
        return false;
    }
    #endregion Private methods

    #region Private Properties

    /// <summary>
    /// Holds the pointer and the initial intersection point of the pointer ray
    /// with the object on pointer down in pointer space
    /// </summary>
    private readonly struct PointerData
    {
        public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint) : this()
        {
            initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
            Pointer = pointer;
            IsNearPointer = pointer is IMixedRealityNearPointer;
        }

        private readonly Vector3 initialGrabPointInPointer;

        public IMixedRealityPointer Pointer { get; }

        public bool IsNearPointer { get; }

        /// <summary>
        /// Returns the grab point on the manipulated object in world space.
        /// </summary>
        public Vector3 GrabPoint => (Pointer.Rotation * initialGrabPointInPointer) + Pointer.Position;
    }

    private List<PointerData> pointerDataList = new List<PointerData>();

    #endregion Private Properties

}