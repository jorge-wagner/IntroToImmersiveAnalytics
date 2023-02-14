// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using System.Collections.Generic;
using System.Linq;
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
    bool hasValidPriorPosition = false;
    Vector3 centerBetweenCurrentTargets, targetPointInLocalSpace;

    private void OnEnable()
    {
        if (CoreServices.InputSystem != null)
        {
            CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
            CoreServices.InputSystem.RegisterHandler<IMixedRealityPointerHandler>(this);
        }
    }

    private void Update()
    {

    }

    private void OnDisable()
    {
        if (CoreServices.InputSystem != null)
        {
            CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
            CoreServices.InputSystem.UnregisterHandler<IMixedRealityPointerHandler>(this);
        }
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

        CoreServices.InputSystem.FocusProvider.TryGetFocusDetails(pointer, out var focusDetails);

        // Raycast an imaginary plane orignating from the updated targetPointInLocalSpace.

        var rayPositionInMapLocalSpace = MapRenderer.transform.InverseTransformPoint(pointer.Position);
        var rayDirectionInMapLocalSpace = MapRenderer.transform.InverseTransformDirection(pointer.Rotation * Vector3.forward).normalized;
        var rayInMapLocalSpace = new Ray(rayPositionInMapLocalSpace, rayDirectionInMapLocalSpace.normalized);
        var hitPlaneInMapLocalSpace = new Plane(Vector3.up, targetPointInLocalSpace);
        if (hitPlaneInMapLocalSpace.Raycast(rayInMapLocalSpace, out float enter))
        {
            targetPointInLocalSpace = focusDetails.PointLocalSpace;

            var targetPointInMercator =
                  MapRenderer.TransformLocalPointToMercatorWithAltitude(
                      targetPointInLocalSpace,
                      out var targetAltitudeInMeters,
                      out _);

            var newTargetPointInLocalSpace = rayInMapLocalSpace.GetPoint(enter);

            // Reconstruct ray from pointer position to focus details.
            var rayTargetPoint = MapRenderer.transform.TransformPoint(newTargetPointInLocalSpace);
            var ray = new Ray(pointer.Position, (rayTargetPoint - pointer.Position).normalized);
            MapInteractionController.PanAndZoom(ray, targetPointInMercator, targetAltitudeInMeters, 0f);

            // Also override the FocusDetails so that the pointer ray tracks the target coordinate.
            focusDetails.Point = MapRenderer.transform.TransformPoint(newTargetPointInLocalSpace);
            focusDetails.PointLocalSpace = newTargetPointInLocalSpace;
            CoreServices.InputSystem.FocusProvider.TryOverrideFocusDetails(pointer, focusDetails);

            // UpdatePlots(); // Update the data visualizations linked to this map -- alternatively, use the event system for this 

        }
    }

    void HandleTwoHandManipulationUpdated()
    {
        Debug.Assert(pointerDataList.Count == 2);
        IMixedRealityPointer p1 = pointerDataList[0].Pointer;
        IMixedRealityPointer p2 = pointerDataList[1].Pointer;

        CoreServices.InputSystem.FocusProvider.TryGetFocusDetails(p1, out var focus1);
        CoreServices.InputSystem.FocusProvider.TryGetFocusDetails(p2, out var focus2);


        var rayPositionInMapLocalSpace1 = MapRenderer.transform.InverseTransformPoint(p1.Position);
        var rayDirectionInMapLocalSpace1 = MapRenderer.transform.InverseTransformDirection(p1.Rotation * Vector3.forward).normalized;
        var rayInMapLocalSpace1 = new Ray(rayPositionInMapLocalSpace1, rayDirectionInMapLocalSpace1.normalized);

        var rayPositionInMapLocalSpace2 = MapRenderer.transform.InverseTransformPoint(p2.Position);
        var rayDirectionInMapLocalSpace2 = MapRenderer.transform.InverseTransformDirection(p2.Rotation * Vector3.forward).normalized;
        var rayInMapLocalSpace2 = new Ray(rayPositionInMapLocalSpace2, rayDirectionInMapLocalSpace2.normalized);


        var hitPlaneInMapLocalSpace = new Plane(Vector3.up, centerBetweenCurrentTargets);
        if (hitPlaneInMapLocalSpace.Raycast(rayInMapLocalSpace1, out float enter1) && hitPlaneInMapLocalSpace.Raycast(rayInMapLocalSpace2, out float enter2))
        {
            var currentTargetPointInLocalSpace1 = focus1.PointLocalSpace;
            var currentTargetPointInLocalSpace2 = focus2.PointLocalSpace;
            centerBetweenCurrentTargets = (currentTargetPointInLocalSpace1 + currentTargetPointInLocalSpace2) / 2f;

            var targetPointInMercator =
                  MapRenderer.TransformLocalPointToMercatorWithAltitude(
                      centerBetweenCurrentTargets,
                      out var targetAltitudeInMeters,
                      out _);

            if (hasValidPriorPosition)
            {

                var newTargetPointInLocalSpace1 = rayInMapLocalSpace1.GetPoint(enter1);
                var newTargetPointInLocalSpace2 = rayInMapLocalSpace2.GetPoint(enter2);
                var centerBetweenNewTargets = (newTargetPointInLocalSpace1 + newTargetPointInLocalSpace2) / 2f;

                float prevTouchDeltaMag = (new Vector2(currentTargetPointInLocalSpace1.x, currentTargetPointInLocalSpace1.z) - new Vector2(currentTargetPointInLocalSpace2.x, currentTargetPointInLocalSpace2.z)).magnitude;
                float touchDeltaMag = (new Vector2(newTargetPointInLocalSpace1.x, newTargetPointInLocalSpace1.z) - new Vector2(newTargetPointInLocalSpace2.x, newTargetPointInLocalSpace2.z)).magnitude;
                float zoomFactor = 1.0f * (touchDeltaMag - prevTouchDeltaMag);
                var touchPointDeltaToInitialDeltaRatio = touchDeltaMag / prevTouchDeltaMag;
                var _initialMapDimensionInMercator = Mathf.Pow(2, MapRenderer.ZoomLevel - 1);
                var newMapDimensionInMercator = touchPointDeltaToInitialDeltaRatio * _initialMapDimensionInMercator;
                float newZoomLevel = Mathf.Log(newMapDimensionInMercator) / Mathf.Log(2) + 1f;
                float zoomspeed = (newZoomLevel - MapRenderer.ZoomLevel) / Time.deltaTime;

                // Reconstruct ray from pointer position to focus details.
                var rayTargetPoint = MapRenderer.transform.TransformPoint(centerBetweenNewTargets);
                var ray = new Ray((p1.Position + p2.Position) / 2f, (rayTargetPoint - (p1.Position + p2.Position) / 2f).normalized);
                MapInteractionController.PanAndZoom(ray, targetPointInMercator, targetAltitudeInMeters, zoomspeed);

                // Also override the FocusDetails so that the pointer ray tracks the target coordinate.
                focus1.Point = MapRenderer.transform.TransformPoint(newTargetPointInLocalSpace1);
                focus1.PointLocalSpace = newTargetPointInLocalSpace1;
                CoreServices.InputSystem.FocusProvider.TryOverrideFocusDetails(p1, focus1);

                focus2.Point = MapRenderer.transform.TransformPoint(newTargetPointInLocalSpace2);
                focus2.PointLocalSpace = newTargetPointInLocalSpace2;
                CoreServices.InputSystem.FocusProvider.TryOverrideFocusDetails(p2, focus2);


                // UpdatePlots(); // Update the data visualizations linked to this map -- alternatively, use the event system for this 
                


            }

            hasValidPriorPosition = true;

        }
    }


    public virtual void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (TryGetPointerDataWithId(eventData.Pointer.PointerId, out PointerData pointerDataToRemove))
        {
            pointerDataList.Remove(pointerDataToRemove);
        }

        //if(pointerDataList.Count == 0)
        //{
        //   
        //}

        hasValidPriorPosition = false;

        eventData.Use();
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
