﻿using System;
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
    /// A class to encapsulate a excellon R Code, this is an XY repeat
    /// </summary>
    /// <history>
    ///    01 Sep 10  Cynic - Started
    /// </history>
    public class ExcellonLine_RCode : ExcellonLine
    {

        // NOTE: In general, if a coordinate is an int it has been scaled and it represents
        //       a value in plot coordinates. If it is a float it represents an unscaled
        //       value from the gerber file or gCode file

        /// These values are the decimal compensated values from the RCode itself. They
        /// are not yet scaled to plot coordinates.
        private float xOffset = 0;
        private float yOffset = 0;
        private int repeatCount = 0;

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rawLineStrIn">The raw line string</param>
        /// <param name="processedLineStrIn">The processed line string</param>
        /// <history>
        ///    01 Sep 10  Cynic - Started
        /// </history>
        public ExcellonLine_RCode(string rawLineStrIn, string processedLineStrIn, int lineNumberIn)
            : base(rawLineStrIn, processedLineStrIn, lineNumberIn)
        {
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current repeat count value
        /// </summary>
        /// <history>
        ///    05 Sep 10  Cynic - Started
        /// </history>
        public int RepeatCount
        {
            get
            {
                return repeatCount;
            }
            set
            {
                repeatCount = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current X Offset value
        /// </summary>
        /// <history>
        ///    01 Sep 10  Cynic - Started
        /// </history>
        public float XOffset
        {
            get
            {
                return xOffset;
            }
            set
            {
                xOffset = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the current Y Offset value
        /// </summary>
        /// <history>
        ///    01 Sep 10  Cynic - Started
        /// </history>
        public float YOffset
        {
            get
            {
                return yOffset;
            }
            set
            {
                yOffset = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Inserts decimal places into the XY coordinate values as appropriate
        /// </summary>
        /// <param name="numToScale">the number we need to scale</param>
        /// <param name="integerPlaces">the number of integer places</param>
        /// <param name="decimalPlaces">the number of decimal places</param>
        /// <param name="leadingZeroMode">a flag to indicate if leading or traling zeros are discarded</param>
        /// <returns>z success, nz fail</returns>
        /// <history>
        ///    01 Sep 10  Cynic - Started
        /// </history>
        private float DecimalScaleNumber(float numToScale, int decimalPlaces, FileManager.ExcellonDrillingCoordinateZerosModeEnum leadingZeroMode)
        {
            switch (leadingZeroMode)
            {
                case FileManager.ExcellonDrillingCoordinateZerosModeEnum.FixedDecimalPoint:
                case FileManager.ExcellonDrillingCoordinateZerosModeEnum.OmitLeadingZeros:
                {
                    // all we have to do is divide the number by the 10^decimalPlaces
                    // for example if decimalPlaces is three and numToScale is 1503
                    // then the real number should be 01.503
                    if (decimalPlaces == 0) return numToScale;
                    float tmpFloat = numToScale / (float)Math.Pow(10, decimalPlaces);
                    return tmpFloat;
                }
                default: // probably omit trailing zeros mode
                {
                    // this is a lot more tricky since we have to have the original text
                    // value in order to figure this out. This blows chunks, and I have
                    // a hard time believing anybody uses it. I will leave it as not
                    // implemented at the moment because I have better things to do.
                    throw new NotImplementedException("Excellon Files in Omit Trailing Zeros Mode are not Supported");
                }
            }
        }
        
        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Performs the action the plot requires based on the current context
        /// </summary>
        /// <param name="graphicsObj">a graphics object on which to plot</param>
        /// <param name="stateMachine">the excellon plot state machine</param>
        /// <param name="errorString">the error string we return on fail</param>
        /// <param name="errorValue">the error value we return on fail, z success, nz fail </param>
        /// <returns>an enum value indicating what next action to take</returns>
        /// <history>
        ///    01 Sep 10  Cynic - Started
        /// </history>
        public override ExcellonLine.PlotActionEnum PerformPlotExcellonAction(Graphics graphicsObj, ExcellonFileStateMachine stateMachine, ref int errorValue, ref string errorString)
        {
            errorValue = 0;
            errorString = "Successful End";
            return ExcellonLine.PlotActionEnum.PlotAction_End;
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Parses out the line and gets the required information from it
        /// </summary>
        /// <param name="processedLineStr">a line string without block terminator or format parameters</param>
        /// <param name="stateMachine">The state machine containing the implied modal values</param>
        /// <returns>z success, nz fail</returns>
        /// <history>
        ///    01 Sep 10  Cynic - Started
        /// </history>
        public override int ParseLine(string processedLineStr, ExcellonFileStateMachine stateMachine)
        {
            float outFloat = 0;
            int outInt = 0;
            int nextStartPos = 0;
            bool retBool;

            //LogMessage("ParseLine(XY) started");

            if (processedLineStr == null) return 100;
            if (processedLineStr.StartsWith("R") == false)
            {
                return 200;
            }

            // now the line will have some combination of R, X and Y tags in some order
            // LOOK FOR THE X TAG
            nextStartPos = 0;
            nextStartPos = GerberParseUtils.FindCharacterReturnNextPos(processedLineStr, 'R', nextStartPos);
            if ((nextStartPos < 0) || (nextStartPos > processedLineStr.Length))
            {
                // we have to have this
                LogMessage("ParseLine(R) lineNumber=" + LineNumber.ToString() + " failed. No R repeat quantity found.");
                return 3331;
            }
            else
            {
                // this will have a integer number
                retBool = GerberParseUtils.ParseNumberFromString_TillNonDigit_RetInteger(processedLineStr, nextStartPos, ref outInt, ref nextStartPos);
                if (retBool != true)
                {
                    LogMessage("ParseLine(R) failed on call to ParseNumberFromString_TillNonDigit_RetInteger");
                    return 3332;
                }
                else
                {
                    // set the value now
                    repeatCount = outInt;
                }
            }


            // LOOK FOR THE X TAG
            nextStartPos = 0;
            nextStartPos = GerberParseUtils.FindCharacterReturnNextPos(processedLineStr, 'X', nextStartPos);
            if ((nextStartPos < 0) || (nextStartPos > processedLineStr.Length))
            {
                // we have to have this
                LogMessage("ParseLine(R) lineNumber="+LineNumber.ToString()+" failed. No X coordinate found.");
                return 1331;
            }
            else
            {
                // this will have a float number
                retBool = GerberParseUtils.ParseNumberFromString_TillNonDigit_RetFloat(processedLineStr, nextStartPos, ref outFloat, ref nextStartPos);
                if (retBool != true)
                {
                    // just means not found
                }
                else
                {
                    // set the value now
                    xOffset = DecimalScaleNumber(outFloat, stateMachine.ExcellonFileManager.DrillingNumberOfDecimalPlaces, stateMachine.ExcellonFileManager.DrillingCoordinateZerosMode);
                }
            }

            // LOOK FOR THE Y TAG
            nextStartPos = 0;
            nextStartPos = GerberParseUtils.FindCharacterReturnNextPos(processedLineStr, 'Y', nextStartPos);
            if ((nextStartPos < 0) || (nextStartPos > processedLineStr.Length))
            {
                // just means not found
            }
            else
            {
                // this will have a float number
                retBool = GerberParseUtils.ParseNumberFromString_TillNonDigit_RetFloat(processedLineStr, nextStartPos, ref outFloat, ref nextStartPos);
                if (retBool != true)
                {
                    LogMessage("ParseLine(R) failed on call to ParseNumberFromString_TillNonDigit_RetFloat");
                    return 2332;
                }
                else
                {
                    // set the value now
                    yOffset = DecimalScaleNumber(outFloat, stateMachine.ExcellonFileManager.DrillingNumberOfDecimalPlaces, stateMachine.ExcellonFileManager.DrillingCoordinateZerosMode);
                }
            }
            return 0;
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Converts the excellon line into a GCode line and returns it
        /// </summary>
        /// <param name="stateMachine">the state machine with the configuration</param>
        /// <param name="gcLineList">a list of the equivalent gcode line object. This can be 
        /// empty if there is no direct conversion</param>
        /// <returns>z success, nz fail</returns>
        /// <history>
        ///    05 Sep 10  Cynic - Started
        /// </history>
        public override int GetGCodeLine(ExcellonFileStateMachine stateMachine, out List<GCodeLine> gcLineList)
        {
            GCodeLine_ZMove zLine = null;
            GCodeLine_RapidMove rmLine = null;
            gcLineList = new List<GCodeLine>();
            float workingXCoord;
            float workingYCoord;

            if (RepeatCount < 0)
            {
                LogMessage("GetGCodeLine (R) invalid repeat count of " + RepeatCount.ToString() + " on line " + LineNumber.ToString());
                return 101;
            }

            // now put out our loop
            for (int i = 0; i < RepeatCount; i++)
            {
                workingXCoord = stateMachine.LastDCodeXCoord;
                workingYCoord = stateMachine.LastDCodeYCoord;

                // calculate the new coordinate now
                workingXCoord += xOffset;
                workingYCoord += yOffset;
               // float x0 = workingXCoord - this.PlotXCoordOriginAdjust;
               // float y0 = workingYCoord - this.PlotYCoordOriginAdjust;

                // G00 rapid move tool head to the xOffset, yCoord
                rmLine = new GCodeLine_RapidMove(workingXCoord, workingYCoord);
                gcLineList.Add(rmLine);
                stateMachine.LastDCodeXCoord = workingXCoord;
                stateMachine.LastDCodeYCoord = workingYCoord;

                // G00 - put the bit into the work piece
                zLine = new GCodeLine_ZMove(GCodeLine_ZMove.GCodeZMoveHeightEnum.GCodeZMoveHeight_ZCoordForCut);
                zLine.SetGCodePlotDrillValues(workingXCoord, workingYCoord, stateMachine.LastDrillWidth);
                gcLineList.Add(zLine);

                // G00 - pull the bit out of the work piece
                zLine = new GCodeLine_ZMove(GCodeLine_ZMove.GCodeZMoveHeightEnum.GCodeZMoveHeight_ZCoordForClear);
                zLine.SetGCodePlotDrillValues(workingXCoord, workingYCoord, stateMachine.LastDrillWidth);
                gcLineList.Add(zLine);
            }

            return 0;
        }

    }
}
