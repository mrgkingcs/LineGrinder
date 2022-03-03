using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OISCommon;

/// +------------------------------------------------------------------------------------------------------------------------------+
/// ¦                                                   TERMS OF USE: MIT License                                                  ¦
/// +------------------------------------------------------------------------------------------------------------------------------¦
/// ¦Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation    ¦
/// ¦files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,    ¦
/// ¦modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software¦
/// ¦is furnished to do so, subject to the following conditions:                                                                   ¦
/// ¦                                                                                                                              ¦
/// ¦The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.¦
/// ¦                                                                                                                              ¦
/// ¦THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE          ¦
/// ¦WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR         ¦
/// ¦COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,   ¦
/// ¦ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                         ¦
/// +------------------------------------------------------------------------------------------------------------------------------+

namespace LineGrinder
{
    /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
    /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
    /// <summary>
    /// A class for the arc objects the IsoPlotBuilder creates to 
    /// convert Gerber drawing primitives into GCodes
    /// </summary>
    public class IsoPlotObject_Arc : IsoPlotObject
    {

        // NOTE: These values should all be in plot coordinates.

        // these are the centerpoint of the circle
        int xCenter = -1;
        int yCenter = -1;
        int radius = -1;
        int width = -1;
        float startAngle = 0;
        float sweepAngle = 0;
        bool wantClockWise = false;
        bool isMultiQuadrantArc = false;

        int outerFirstX;
        int outerFirstY;
        int outerLastX;
        int outerLastY;
        int innerFirstX;
        int innerFirstY;
        int innerLastX;
        int innerLastY;

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isoPlotObjectIDIn">the isoPlotObjectID for this object</param>
        /// <param name="xCenter">X center</param>
        /// <param name="xCenter">Y center</param>
        /// <param name="radiusIn">circle radius</param>
        /// <param name="startAngleIn">the start angle</param>
        /// <param name="sweepAngleIn">the sweep angle</param>
        /// <param name="wantClockWiseIn">the wantClockWise value</param>
        /// <param name="isMultiQuadrantArcIn">is an arc specified in multiquadrant mode</param>
        public IsoPlotObject_Arc(int isoPlotObjectIDIn, int xCenterIn, int yCenterIn, int radiusIn, int widthIn, float startAngleIn, float sweepAngleIn, bool wantClockWiseIn, bool isMultiQuadrantArcIn) : base(isoPlotObjectIDIn)
        {
            xCenter = xCenterIn;
            yCenter = yCenterIn;
            radius = radiusIn;
            width = widthIn;
            startAngle = startAngleIn;
            sweepAngle = sweepAngleIn;
            wantClockWise = wantClockWiseIn;
            isMultiQuadrantArc = isMultiQuadrantArcIn;
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets two points which form a rectangle which completely encloses the arc
        /// </summary>
        public bool GetBoundingPoints(out Point lowerLeft, out Point upperRight)
        {
            Rectangle outRect = MiscGraphicsUtils.GetBoundingRectForCircle(xCenter, yCenter, GetOuterRadius());
            lowerLeft = new Point(outRect.X, outRect.Y);
            upperRight = new Point(outRect.X+outRect.Width, outRect.Y+outRect.Height);
            if (lowerLeft.X < 0) return false;
            if (lowerLeft.Y < 0) return false;
            if (upperRight.X < 0) return false;
            if (upperRight.Y < 0) return false;
            return true;
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets whether the segment needs to pay attention to multi quadrant mode
        /// </summary>
        public bool IsMultiQuadrantArc
        {
            get
            {
                return isMultiQuadrantArc;
            }
            set
            {
                isMultiQuadrantArc = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets the arc outer radius from the radius and width
        /// </summary>
        public int GetOuterRadius()
        {
            return (int)(radius + (width / 2));
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets the arc inner radius from the radius and width
        /// </summary>
        public int GetInnerRadius()
        {
            return (int)(radius - (width / 2));
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current radius value
        /// </summary>
        public int Radius
        {
            get
            {
                return radius;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current width value
        /// </summary>
        public int Width
        {
            get
            {
                return radius;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current startAngle value
        /// </summary>
        public float StartAngle
        {
            get
            {
                return startAngle;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current sweepAngle value
        /// </summary>
        public float SweepAngle
        {
            get
            {
                return sweepAngle;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current wantClockWise value
        /// </summary>
        public bool WantClockWise
        {
            get
            {
                return wantClockWise;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current xCenter value
        /// </summary>
        public int XCenter
        {
            get
            {
                return xCenter;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current yCenter value
        /// </summary>
        public int YCenter
        {
            get
            {
                return yCenter;
            }
        }

        public int OuterFirstX { get => outerFirstX; set => outerFirstX = value; }
        public int OuterFirstY { get => outerFirstY; set => outerFirstY = value; }
        public int OuterLastX { get => outerLastX; set => outerLastX = value; }
        public int OuterLastY { get => outerLastY; set => outerLastY = value; }
        public int InnerFirstX { get => innerFirstX; set => innerFirstX = value; }
        public int InnerFirstY { get => innerFirstY; set => innerFirstY = value; }
        public int InnerLastX { get => innerLastX; set => innerLastX = value; }
        public int InnerLastY { get => innerLastY; set => innerLastY = value; }
    }
}

