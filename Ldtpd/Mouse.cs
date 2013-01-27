﻿/*
 * Cobra WinLDTP 3.0
 * 
 * Author: Nagappan Alagappan <nalagappan@vmware.com>
 * Copyright: Copyright (c) 2011-13 VMware, Inc. All Rights Reserved.
 * License: MIT license
 * 
 * http://ldtp.freedesktop.org
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/
using System;
using ATGTestInput;
using System.Windows;
using System.Collections;
using CookComputing.XmlRpc;
using System.Windows.Forms;
using System.Windows.Automation;

namespace Ldtpd
{
    class Mouse
    {
        Utils utils;
        public Mouse(Utils utils)
        {
            this.utils = utils;
        }
        private void LogMessage(Object o)
        {
            utils.LogMessage(o);
        }
        public int MouseLeftClick(String windowName, String objName)
        {
            Object pattern = null;
            AutomationElement childHandle;
            try
            {
                childHandle = utils.GetObjectHandle(windowName, objName);
                if (!utils.IsEnabled(childHandle))
                {
                    throw new XmlRpcFaultException(123,
                        "Object state is disabled");
                }
                try
                {
                    childHandle.SetFocus();
                }
                catch (Exception ex)
                {
                    // Have noticed exception with
                    // maximize / minimize button
                    LogMessage(ex);
                }
                if (childHandle.Current.ControlType == ControlType.Pane)
                {
                    // NOTE: Work around, as the pane doesn't seem to work
                    // with any actions. Noticed this window, when Windows
                    // Security Warning dialog pop's up
                    utils.InternalClick(childHandle);
                    return 1;
                }
                else if (childHandle.TryGetCurrentPattern(InvokePattern.Pattern,
                    out pattern))
                {
                    if (childHandle.Current.ControlType == ControlType.Menu ||
                        childHandle.Current.ControlType == ControlType.MenuBar ||
                        childHandle.Current.ControlType == ControlType.MenuItem ||
                        childHandle.Current.ControlType == ControlType.ListItem)
                    {
                        //((InvokePattern)invokePattern).Invoke();
                        // NOTE: Work around, as the above doesn't seem to work
                        // with UIAComWrapper and UIAComWrapper is required
                        // to Edit value in Spin control
                        utils.InternalClick(childHandle);
                    }
                    else
                    {
                        try
                        {
                            ((InvokePattern)pattern).Invoke();
                        }
                        catch (Exception ex)
                        {
                            LogMessage(ex);
                            // Have noticed exception with
                            // maximize / minimize button
                            utils.InternalClick(childHandle);
                        }
                    }
                    return 1;
                }
                else if (childHandle.TryGetCurrentPattern(SelectionItemPattern.Pattern,
                    out pattern))
                {
                    ((SelectionItemPattern)pattern).Select();
                    return 1;
                }
                else
                {
                    utils.InternalClick(childHandle);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex);
                if (ex is XmlRpcFaultException)
                    throw;
                else
                    throw new XmlRpcFaultException(123,
                        "Unhandled exception: " + ex.Message);
            }
            finally
            {
                pattern = null;
                childHandle = null;
            }
            throw new XmlRpcFaultException(123, "Unable to perform action");
        }
        public int MouseRightClick(String windowName, String objName)
        {
            AutomationElement childHandle;
            try
            {
                childHandle = utils.GetObjectHandle(windowName, objName);
                if (!utils.IsEnabled(childHandle))
                {
                    throw new XmlRpcFaultException(123,
                        "Object state is disabled");
                }
                try
                {
                    childHandle.SetFocus();
                }
                catch (Exception ex)
                {
                    // Have noticed exception with
                    // maximize / minimize button
                    LogMessage(ex);
                }
                Rect rect = childHandle.Current.BoundingRectangle;
                GenerateMouseEvent((int)(rect.X + rect.Width / 2),
                    (int)(rect.Y + rect.Height / 2), "b3c");
                return 1;
            }
            catch (Exception ex)
            {
                LogMessage(ex);
                if (ex is XmlRpcFaultException)
                    throw;
                else
                    throw new XmlRpcFaultException(123,
                        "Unhandled exception: " + ex.Message);
            }
            finally
            {
                childHandle = null;
            }
            throw new XmlRpcFaultException(123, "Unable to perform action");
        }
        public int DoubleClick(String windowName, String objName)
        {
            AutomationElement childHandle;
            try
            {
                childHandle = utils.GetObjectHandle(windowName, objName);
                if (!utils.IsEnabled(childHandle))
                {
                    throw new XmlRpcFaultException(123,
                        "Object state is disabled");
                }
                try
                {
                    childHandle.SetFocus();
                }
                catch (Exception ex)
                {
                    // Have noticed exception with
                    // maximize / minimize button
                    LogMessage(ex);
                }
                Rect rect = childHandle.Current.BoundingRectangle;
                GenerateMouseEvent((int)(rect.X + rect.Width / 2),
                    (int)(rect.Y + rect.Height / 2), "b1d");
                return 1;
            }
            catch (Exception ex)
            {
                LogMessage(ex);
                if (ex is XmlRpcFaultException)
                    throw;
                else
                    throw new XmlRpcFaultException(123,
                        "Unhandled exception: " + ex.Message);
            }
            finally
            {
                childHandle = null;
            }
            throw new XmlRpcFaultException(123, "Unable to perform action");
        }
        public int GenerateMouseEvent(int x, int y, String type = "b1c")
        {
            Point pt = new Point(x, y);
            switch (type)
            {
                case "b1p":
                    Input.SendMouseInput(x, y, 0, SendMouseInputFlags.LeftDown);
                    break;
                case "b1r":
                    Input.SendMouseInput(x, y, 0, SendMouseInputFlags.LeftUp);
                    break;
                case "b1c":
                    Input.MoveTo(pt);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
                    break;
                case "b1d":
                    Input.MoveTo(pt);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
                    break;
                case "b2p":
                    Input.SendMouseInput(x, y, 0, SendMouseInputFlags.MiddleDown);
                    break;
                case "b2r":
                    Input.SendMouseInput(x, y, 0, SendMouseInputFlags.MiddleUp);
                    break;
                case "b2c":
                    Input.MoveTo(pt);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.MiddleDown);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.MiddleUp);
                    break;
                case "b3p":
                    Input.SendMouseInput(x, y, 0, SendMouseInputFlags.RightDown);
                    break;
                case "b3r":
                    Input.SendMouseInput(x, y, 0, SendMouseInputFlags.RightUp);
                    break;
                case "b3c":
                    Input.MoveTo(pt);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.RightDown);
                    Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.RightUp);
                    break;
                case "abs":
                    Input.SendMouseInput(pt.X, pt.Y, 0,
                        SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);
                    break;
                case "rel":
                    Input.SendMouseInput(pt.X, pt.Y, 0, SendMouseInputFlags.Move);
                    break;
                default:
                    throw new XmlRpcFaultException(123,
                        "Unsupported mouse type: " + type);
            }
            return 1;
        }
        public int SimulateMouseMove(int source_x, int source_y, int dest_x, int dest_y, double delay = 0.0)
        {
            int[] size;
            Generic generic = new Generic(utils);
            try
            {
                size = generic.GetWindowSize("paneProgramManager");
            }
            finally
            {
                generic = null;
            }
            if (source_x < size[0] || source_y < size[1] ||
                dest_x > size[2] || dest_y > size[3] ||
                source_x > size[2] || source_y > size[3] ||
                dest_x < size[0] || dest_y < size[1])
                return 0;
            bool x_flag = true; // Iterated x ?
            bool y_flag = true; // Iterated y ?
            while (true)
            {
                if (x_flag)
                {
                    if (source_x > dest_x)
                    {
                        // If source X greather than dest X
                        // then move -1 pixel
                        source_x -= 1;
                    }
                    else if (source_x < dest_x)
                    {
                        // If source X less than dest X
                        // then move +1 pixel
                        source_x += 1;
                    }
                    else
                    {
                        // If source X equal to dest X
                        // then don't process X co-ordinate
                        x_flag = false;
                    }
                }
                if (y_flag)
                {
                    if (source_y > dest_y)
                    {
                        // If source Y greather than dest Y
                        // then move -1 pixel
                        source_y -= 1;
                    }
                    else if (source_y < dest_y)
                    {
                        // If source Y less than dest Y
                        // then move +1 pixel
                        source_y += 1;
                    }
                    else
                    {
                        // If source Y equal to dest Y
                        // then don't process Y co-ordinate
                        y_flag = false;
                    }
                }
                if (delay != 0.0)
					utils.InternalWait(delay);
                // Start mouse move from source_x, source_y to dest_x, dest_y
                GenerateMouseEvent(source_x, source_y, "abs");
                if (source_x == dest_x && source_y == dest_y)
                {
                    // If we have reached the dest_x and dest_y
                    // then break the loop
                    break;
                }
            }
            return 1;
        }
    }
}
