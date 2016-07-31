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
    [PluginInfo(Name = "TrackedDevices", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift, controller, gamepad", Author = "tonfilm, herbst")]
    public class ValveOpenVRTrackedDeviceNode : OpenVRConsumerBaseNode, IPluginEvaluate
    {
        [Output("Events")]
        ISpread<String> FEventsOut;

        [Output("Event Device Index")]
        ISpread<int> FDeviceIndexOut;

        [Output("All Devices")]
        ISpread<OpenVRController.Device> FDevicesOut;

        [Output("Device Role")]
        ISpread<ETrackedControllerRole> FDeviceRoleOut;
        
        [Output("Left Controller")]
        ISpread<OpenVRController.Device> FControllerLeftOut;

        [Output("Right Controller")]
        ISpread<OpenVRController.Device> FControllerRightOut;

        [Output("Left and Right Controller")]
        ISpread<OpenVRController.Device> FControllerLeftRightOut;

        uint FEvtSize = (uint)Marshal.SizeOf(typeof(VREvent_t));

        public override void Evaluate(int SpreadMax, CVRSystem system)
        {

            VREvent_t evt = default(VREvent_t);
            FEventsOut.SliceCount = 0;
            FDeviceIndexOut.SliceCount = 0;

            while (system.PollNextEvent(ref evt, FEvtSize))
            {
                var evtType = (EVREventType)evt.eventType;
                FEventsOut.Add(evtType.ToString());
                ProcessEvent(evtType, evt);
            }

            //controller states
            OpenVRController.Update(FFrame++);

            FDeviceIndexOut.SliceCount = 0;
            FDeviceRoleOut.SliceCount = 0;

            FControllerLeftOut.SliceCount = 0;
            FControllerRightOut.SliceCount = 0;
            FControllerLeftRightOut.SliceCount = 0;
            FDevicesOut.SliceCount = 0;

            var indexLeft = (int)system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            var indexRight = (int)system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
            if (indexLeft > 0)
            {
                var c = OpenVRController.Input(indexLeft);
                FControllerLeftOut.Add(c);
                FControllerLeftRightOut.Add(c);
            }

            if (indexRight > 0)
            {
                var c = OpenVRController.Input(indexRight);
                FControllerRightOut.Add(c);
                FControllerLeftRightOut.Add(c);
            }

            //output all in one
            for (int i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                //if(FOpenVRSystem.GetTrackedDeviceClass((uint)i) != ETrackedDeviceClass.Controller) continue;
                var c = OpenVRController.Input(i);
                if (!c.connected || !c.valid) continue;
                FDevicesOut.Add(c);
                FDeviceRoleOut.Add(system.GetControllerRoleForTrackedDeviceIndex((uint)i));
            }
        }

        int FFrame = 0;

        private void ProcessEvent(EVREventType evtType, VREvent_t evt)
        {
            FDeviceIndexOut.Add((int)evt.trackedDeviceIndex);
            switch (evtType)
            {
                case EVREventType.VREvent_None:
                    break;
                case EVREventType.VREvent_TrackedDeviceActivated:
                    break;
                case EVREventType.VREvent_TrackedDeviceDeactivated:
                    break;
                case EVREventType.VREvent_TrackedDeviceUpdated:
                    break;
                case EVREventType.VREvent_TrackedDeviceUserInteractionStarted:
                    break;
                case EVREventType.VREvent_TrackedDeviceUserInteractionEnded:
                    break;
                case EVREventType.VREvent_IpdChanged:
                    break;
                case EVREventType.VREvent_EnterStandbyMode:
                    break;
                case EVREventType.VREvent_LeaveStandbyMode:
                    break;
                case EVREventType.VREvent_TrackedDeviceRoleChanged:
                    break;
                case EVREventType.VREvent_ButtonPress:    
                    break;
                case EVREventType.VREvent_ButtonUnpress:
                    break;
                case EVREventType.VREvent_ButtonTouch:
                    break;
                case EVREventType.VREvent_ButtonUntouch:
                    break;
                case EVREventType.VREvent_MouseMove:
                    break;
                case EVREventType.VREvent_MouseButtonDown:
                    break;
                case EVREventType.VREvent_MouseButtonUp:
                    break;
                case EVREventType.VREvent_FocusEnter:
                    break;
                case EVREventType.VREvent_FocusLeave:
                    break;
                case EVREventType.VREvent_Scroll:
                    break;
                case EVREventType.VREvent_TouchPadMove:
                    break;
                case EVREventType.VREvent_InputFocusCaptured:
                    break;
                case EVREventType.VREvent_InputFocusReleased:
                    break;
                case EVREventType.VREvent_SceneFocusLost:
                    break;
                case EVREventType.VREvent_SceneFocusGained:
                    break;
                case EVREventType.VREvent_SceneApplicationChanged:
                    break;
                case EVREventType.VREvent_SceneFocusChanged:
                    break;
                case EVREventType.VREvent_InputFocusChanged:
                    break;
                case EVREventType.VREvent_HideRenderModels:
                    break;
                case EVREventType.VREvent_ShowRenderModels:
                    break;
                case EVREventType.VREvent_OverlayShown:
                    break;
                case EVREventType.VREvent_OverlayHidden:
                    break;
                case EVREventType.VREvent_DashboardActivated:
                    break;
                case EVREventType.VREvent_DashboardDeactivated:
                    break;
                case EVREventType.VREvent_DashboardThumbSelected:
                    break;
                case EVREventType.VREvent_DashboardRequested:
                    break;
                case EVREventType.VREvent_ResetDashboard:
                    break;
                case EVREventType.VREvent_RenderToast:
                    break;
                case EVREventType.VREvent_ImageLoaded:
                    break;
                case EVREventType.VREvent_ShowKeyboard:
                    break;
                case EVREventType.VREvent_HideKeyboard:
                    break;
                case EVREventType.VREvent_OverlayGamepadFocusGained:
                    break;
                case EVREventType.VREvent_OverlayGamepadFocusLost:
                    break;
                case EVREventType.VREvent_OverlaySharedTextureChanged:
                    break;
                case EVREventType.VREvent_DashboardGuideButtonDown:
                    break;
                case EVREventType.VREvent_DashboardGuideButtonUp:
                    break;
                case EVREventType.VREvent_Notification_Shown:
                    break;
                case EVREventType.VREvent_Notification_Hidden:
                    break;
                case EVREventType.VREvent_Notification_BeginInteraction:
                    break;
                case EVREventType.VREvent_Notification_Destroyed:
                    break;
                case EVREventType.VREvent_Quit:
                    break;
                case EVREventType.VREvent_ProcessQuit:
                    break;
                case EVREventType.VREvent_QuitAborted_UserPrompt:
                    break;
                case EVREventType.VREvent_QuitAcknowledged:
                    break;
                case EVREventType.VREvent_DriverRequestedQuit:
                    break;
                case EVREventType.VREvent_ChaperoneDataHasChanged:
                    break;
                case EVREventType.VREvent_ChaperoneUniverseHasChanged:
                    break;
                case EVREventType.VREvent_ChaperoneTempDataHasChanged:
                    break;
                case EVREventType.VREvent_ChaperoneSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_SeatedZeroPoseReset:
                    break;
                case EVREventType.VREvent_AudioSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_BackgroundSettingHasChanged:
                    break;
                case EVREventType.VREvent_CameraSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_ReprojectionSettingHasChanged:
                    break;
                case EVREventType.VREvent_StatusUpdate:
                    break;
                case EVREventType.VREvent_MCImageUpdated:
                    break;
                case EVREventType.VREvent_FirmwareUpdateStarted:
                    break;
                case EVREventType.VREvent_FirmwareUpdateFinished:
                    break;
                case EVREventType.VREvent_KeyboardClosed:
                    break;
                case EVREventType.VREvent_KeyboardCharInput:
                    break;
                case EVREventType.VREvent_KeyboardDone:
                    break;
                case EVREventType.VREvent_ApplicationTransitionStarted:
                    break;
                case EVREventType.VREvent_ApplicationTransitionAborted:
                    break;
                case EVREventType.VREvent_ApplicationTransitionNewAppStarted:
                    break;
                case EVREventType.VREvent_Compositor_MirrorWindowShown:
                    break;
                case EVREventType.VREvent_Compositor_MirrorWindowHidden:
                    break;
                case EVREventType.VREvent_Compositor_ChaperoneBoundsShown:
                    break;
                case EVREventType.VREvent_Compositor_ChaperoneBoundsHidden:
                    break;
                case EVREventType.VREvent_TrackedCamera_StartVideoStream:
                    break;
                case EVREventType.VREvent_TrackedCamera_StopVideoStream:
                    break;
                case EVREventType.VREvent_TrackedCamera_PauseVideoStream:
                    break;
                case EVREventType.VREvent_TrackedCamera_ResumeVideoStream:
                    break;
                case EVREventType.VREvent_PerformanceTest_EnableCapture:
                    break;
                case EVREventType.VREvent_PerformanceTest_DisableCapture:
                    break;
                case EVREventType.VREvent_PerformanceTest_FidelityLevel:
                    break;
                case EVREventType.VREvent_VendorSpecific_Reserved_Start:
                    break;
                case EVREventType.VREvent_VendorSpecific_Reserved_End:
                    break;
                default:
                    break;
            }
        }
    }

}
