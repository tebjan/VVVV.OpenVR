using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;
using Valve.VR;
using VVVV.Utils.VMath;
using SlimDX;
using System.Threading;
using System.Runtime.InteropServices;

namespace VVVV.Nodes.ValveOpenVR
{
    [PluginInfo(Name = "Controller (Split)", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class ValveOpenVRControllerSplitNode : IPluginEvaluate
    {
        [Input("Controller")]
        IDiffSpread<OpenVRController.Device> FControllerIn;

        [Output("Events")]
        ISpread<String> FEventsOut;

        [Output("Device Index")]
        ISpread<int> FDeviceIndexOut;
        
        [Output("Device Role")]
        ISpread<ETrackedControllerRole> FDeviceRoleOut;

        [Output("Trigger Touch")]
        ISpread<bool> FTriggerTouchOut;
        [Output("Trigger Press")]
        ISpread<bool> FTriggerPressOut;
        [Output("Trigger Axis")]
        ISpread<double> FTriggerAxisOut;

        [Output("Touchpad Touch")]
        ISpread<bool> FTouchpadTouchOut;
        [Output("Touchpad Press")]
        ISpread<bool> FTouchpadPressOut;
        [Output("Touchpad Axis")]
        ISpread<Vector2D> FTouchpadAxisOut;

        [Output("System Press")]
        ISpread<bool> FSystemPressOut;
        [Output("ApplicationMenu Press")]
        ISpread<bool> FApplicationMenuPressOut;
        [Output("Grip Press")]
        ISpread<bool> FGripPressOut;

        [Output("Valid")]
        ISpread<bool> FValidOut;
        [Output("Connected", Visibility = PinVisibility.OnlyInspector)]
        ISpread<bool> FConnectedOut;
        [Output("Has Tracking", Visibility = PinVisibility.OnlyInspector)]
        ISpread<bool> FHasTrackingOut;
        [Output("Out Of Range", Visibility = PinVisibility.OnlyInspector)]
        ISpread<bool> FOutOfRangeOut;
        [Output("Calibrating", Visibility = PinVisibility.OnlyInspector)]
        ISpread<bool> FCalibratingOut;
        [Output("Uninitialized", Visibility = PinVisibility.OnlyInspector)]
        ISpread<bool> FUninitializedOut;

        public void Evaluate(int SpreadMax)
        {
            if (FControllerIn != null)
            {
                FDeviceIndexOut.SliceCount = 0;
                FDeviceRoleOut.SliceCount = 0;

                FTriggerTouchOut.SliceCount = 0;
                FTriggerPressOut.SliceCount = 0;
                FTriggerAxisOut.SliceCount = 0;

                FTouchpadTouchOut.SliceCount = 0;
                FTouchpadPressOut.SliceCount = 0;
                FTouchpadAxisOut.SliceCount = 0;

                FSystemPressOut.SliceCount = 0;
                FApplicationMenuPressOut.SliceCount = 0;
                FGripPressOut.SliceCount = 0;
                FValidOut.SliceCount = 0;
                FConnectedOut.SliceCount = 0;
                FHasTrackingOut.SliceCount = 0;
                FOutOfRangeOut.SliceCount = 0;
                FCalibratingOut.SliceCount = 0;
                FUninitializedOut.SliceCount = 0;

                var system = OpenVR.System;

                for (int i = 0; i < SpreadMax; i++)
                {
                    var controller = FControllerIn[i];
                    if (controller == null) continue;

                    FDeviceIndexOut.Add((int)controller.index);

                    FTriggerTouchOut.Add(controller.GetTouch(OpenVRController.ButtonMask.Trigger));
                    FTriggerPressOut.Add(controller.GetPress(OpenVRController.ButtonMask.Trigger));
                    FTriggerAxisOut.Add(controller.hairTriggerValue);

                    FDeviceRoleOut.Add(system.GetControllerRoleForTrackedDeviceIndex(controller.index));

                    FTouchpadTouchOut.Add(controller.GetTouch(OpenVRController.ButtonMask.Touchpad));
                    FTouchpadPressOut.Add(controller.GetPress(OpenVRController.ButtonMask.Touchpad));
                    FTouchpadAxisOut.Add(controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad));

                    FSystemPressOut.Add(controller.GetPress(OpenVRController.ButtonMask.System));
                    FApplicationMenuPressOut.Add(controller.GetPress(OpenVRController.ButtonMask.ApplicationMenu));
                    FGripPressOut.Add(controller.GetPress(OpenVRController.ButtonMask.Grip));

                    FValidOut.Add(controller.valid);
                    FConnectedOut.Add(controller.connected);
                    FHasTrackingOut.Add(controller.hasTracking);
                    FOutOfRangeOut.Add(controller.outOfRange);
                    FCalibratingOut.Add(controller.calibrating);
                    FUninitializedOut.Add(controller.uninitialized);
                }
            }
        }
    }
}
