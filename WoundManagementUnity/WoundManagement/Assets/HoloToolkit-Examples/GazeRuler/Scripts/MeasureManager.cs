// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Examples.GazeRuler
{
    /// <summary>
    /// manager all measure tools here
    /// </summary>
    public class MeasureManager : Singleton<MeasureManager>, IHoldHandler, IInputClickHandler
    {
        private IGeometry manager;
        public GeometryMode Mode;

        // set up prefabs
        public GameObject LinePrefab;
        public GameObject PointPrefab;
        public GameObject ModeTipObject;
        public GameObject TextPrefab;

        private void Start()
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);

            // inti measure mode
            switch (Mode)
            {
                case GeometryMode.Polygon:
                    manager = PolygonManager.Instance;
                    break;
                default:
                    manager = LineManager.Instance;
                    break;
            }
        }

        // place spatial point
        public void OnSelect()
        {
            manager.AddPoint(LinePrefab, PointPrefab, TextPrefab);
        }

        // delete latest line or geometry
        public void DeleteLine()
        {
            manager.Delete();
        }

        // delete all lines or geometry
        public void ClearAll()
        {
            manager.Clear();
        }

        
        public void OnHoldStarted(HoldEventData eventData)
        {
			//Debug.Log ("started:"+eventData.selectedObject);
			// nothing to do
		}

        public void OnHoldCompleted(HoldEventData eventData)
        {
            // Nothing to do
			//Debug.Log ("completed:"+eventData.selectedObject);
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
            // Nothing to do
			//Debug.Log ("canceled:"+eventData.selectedObject);
        }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        OnSelect();
    }
}

    public class Point
    {
        public Vector3 Position { get; set; }

        public GameObject Root { get; set; }
        public bool IsStart { get; set; }
    }


    public enum GeometryMode
    {
        Line,
        Triangle,
        Rectangle,
        Cube,
        Polygon
    }
}
