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
                case EVREventType.VREvent_WatchdogWakeUpRequested:
                    break;
                case EVREventType.VREvent_LensDistortionChanged:
                    break;
                case EVREventType.VREvent_PropertyChanged:
                    break;
                case EVREventType.VREvent_WirelessDisconnect:
                    break;
                case EVREventType.VREvent_WirelessReconnect:
                    break;
                case EVREventType.VREvent_ButtonPress:
                    break;
                case EVREventType.VREvent_ButtonUnpress:
                    break;
                case EVREventType.VREvent_ButtonTouch:
                    break;
                case EVREventType.VREvent_ButtonUntouch:
                    break;
                case EVREventType.VREvent_DualAnalog_Press:
                    break;
                case EVREventType.VREvent_DualAnalog_Unpress:
                    break;
                case EVREventType.VREvent_DualAnalog_Touch:
                    break;
                case EVREventType.VREvent_DualAnalog_Untouch:
                    break;
                case EVREventType.VREvent_DualAnalog_Move:
                    break;
                case EVREventType.VREvent_DualAnalog_ModeSwitch1:
                    break;
                case EVREventType.VREvent_DualAnalog_ModeSwitch2:
                    break;
                case EVREventType.VREvent_DualAnalog_Cancel:
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
                case EVREventType.VREvent_ScrollDiscrete:
                    break;
                case EVREventType.VREvent_TouchPadMove:
                    break;
                case EVREventType.VREvent_OverlayFocusChanged:
                    break;
                case EVREventType.VREvent_ReloadOverlays:
                    break;
                case EVREventType.VREvent_ScrollSmooth:
                    break;
                case EVREventType.VREvent_InputFocusCaptured:
                    break;
                case EVREventType.VREvent_InputFocusReleased:
                    break;
                case EVREventType.VREvent_SceneApplicationChanged:
                    break;
                case EVREventType.VREvent_SceneFocusChanged:
                    break;
                case EVREventType.VREvent_InputFocusChanged:
                    break;
                case EVREventType.VREvent_SceneApplicationUsingWrongGraphicsAdapter:
                    break;
                case EVREventType.VREvent_ActionBindingReloaded:
                    break;
                case EVREventType.VREvent_HideRenderModels:
                    break;
                case EVREventType.VREvent_ShowRenderModels:
                    break;
                case EVREventType.VREvent_SceneApplicationStateChanged:
                    break;
                case EVREventType.VREvent_ConsoleOpened:
                    break;
                case EVREventType.VREvent_ConsoleClosed:
                    break;
                case EVREventType.VREvent_OverlayShown:
                    break;
                case EVREventType.VREvent_OverlayHidden:
                    break;
                case EVREventType.VREvent_DashboardActivated:
                    break;
                case EVREventType.VREvent_DashboardDeactivated:
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
                case EVREventType.VREvent_ScreenshotTriggered:
                    break;
                case EVREventType.VREvent_ImageFailed:
                    break;
                case EVREventType.VREvent_DashboardOverlayCreated:
                    break;
                case EVREventType.VREvent_SwitchGamepadFocus:
                    break;
                case EVREventType.VREvent_RequestScreenshot:
                    break;
                case EVREventType.VREvent_ScreenshotTaken:
                    break;
                case EVREventType.VREvent_ScreenshotFailed:
                    break;
                case EVREventType.VREvent_SubmitScreenshotToDashboard:
                    break;
                case EVREventType.VREvent_ScreenshotProgressToDashboard:
                    break;
                case EVREventType.VREvent_PrimaryDashboardDeviceChanged:
                    break;
                case EVREventType.VREvent_RoomViewShown:
                    break;
                case EVREventType.VREvent_RoomViewHidden:
                    break;
                case EVREventType.VREvent_ShowUI:
                    break;
                case EVREventType.VREvent_ShowDevTools:
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
                case EVREventType.VREvent_QuitAcknowledged:
                    break;
                case EVREventType.VREvent_DriverRequestedQuit:
                    break;
                case EVREventType.VREvent_RestartRequested:
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
                case EVREventType.VREvent_ChaperoneFlushCache:
                    break;
                case EVREventType.VREvent_ChaperoneRoomSetupStarting:
                    break;
                case EVREventType.VREvent_ChaperoneRoomSetupFinished:
                    break;
                case EVREventType.VREvent_AudioSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_BackgroundSettingHasChanged:
                    break;
                case EVREventType.VREvent_CameraSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_ReprojectionSettingHasChanged:
                    break;
                case EVREventType.VREvent_ModelSkinSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_EnvironmentSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_PowerSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_EnableHomeAppSettingsHaveChanged:
                    break;
                case EVREventType.VREvent_SteamVRSectionSettingChanged:
                    break;
                case EVREventType.VREvent_LighthouseSectionSettingChanged:
                    break;
                case EVREventType.VREvent_NullSectionSettingChanged:
                    break;
                case EVREventType.VREvent_UserInterfaceSectionSettingChanged:
                    break;
                case EVREventType.VREvent_NotificationsSectionSettingChanged:
                    break;
                case EVREventType.VREvent_KeyboardSectionSettingChanged:
                    break;
                case EVREventType.VREvent_PerfSectionSettingChanged:
                    break;
                case EVREventType.VREvent_DashboardSectionSettingChanged:
                    break;
                case EVREventType.VREvent_WebInterfaceSectionSettingChanged:
                    break;
                case EVREventType.VREvent_TrackersSectionSettingChanged:
                    break;
                case EVREventType.VREvent_LastKnownSectionSettingChanged:
                    break;
                case EVREventType.VREvent_DismissedWarningsSectionSettingChanged:
                    break;
                case EVREventType.VREvent_StatusUpdate:
                    break;
                case EVREventType.VREvent_WebInterface_InstallDriverCompleted:
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
                case EVREventType.VREvent_ApplicationListUpdated:
                    break;
                case EVREventType.VREvent_ApplicationMimeTypeLoad:
                    break;
                case EVREventType.VREvent_ProcessConnected:
                    break;
                case EVREventType.VREvent_ProcessDisconnected:
                    break;
                case EVREventType.VREvent_Compositor_ChaperoneBoundsShown:
                    break;
                case EVREventType.VREvent_Compositor_ChaperoneBoundsHidden:
                    break;
                case EVREventType.VREvent_Compositor_DisplayDisconnected:
                    break;
                case EVREventType.VREvent_Compositor_DisplayReconnected:
                    break;
                case EVREventType.VREvent_Compositor_HDCPError:
                    break;
                case EVREventType.VREvent_Compositor_ApplicationNotResponding:
                    break;
                case EVREventType.VREvent_Compositor_ApplicationResumed:
                    break;
                case EVREventType.VREvent_Compositor_OutOfVideoMemory:
                    break;
                case EVREventType.VREvent_Compositor_DisplayModeNotSupported:
                    break;
                case EVREventType.VREvent_TrackedCamera_StartVideoStream:
                    break;
                case EVREventType.VREvent_TrackedCamera_StopVideoStream:
                    break;
                case EVREventType.VREvent_TrackedCamera_PauseVideoStream:
                    break;
                case EVREventType.VREvent_TrackedCamera_ResumeVideoStream:
                    break;
                case EVREventType.VREvent_TrackedCamera_EditingSurface:
                    break;
                case EVREventType.VREvent_PerformanceTest_EnableCapture:
                    break;
                case EVREventType.VREvent_PerformanceTest_DisableCapture:
                    break;
                case EVREventType.VREvent_PerformanceTest_FidelityLevel:
                    break;
                case EVREventType.VREvent_MessageOverlay_Closed:
                    break;
                case EVREventType.VREvent_MessageOverlayCloseRequested:
                    break;
                case EVREventType.VREvent_Input_HapticVibration:
                    break;
                case EVREventType.VREvent_Input_BindingLoadFailed:
                    break;
                case EVREventType.VREvent_Input_BindingLoadSuccessful:
                    break;
                case EVREventType.VREvent_Input_ActionManifestReloaded:
                    break;
                case EVREventType.VREvent_Input_ActionManifestLoadFailed:
                    break;
                case EVREventType.VREvent_Input_ProgressUpdate:
                    break;
                case EVREventType.VREvent_Input_TrackerActivated:
                    break;
                case EVREventType.VREvent_Input_BindingsUpdated:
                    break;
                case EVREventType.VREvent_SpatialAnchors_PoseUpdated:
                    break;
                case EVREventType.VREvent_SpatialAnchors_DescriptorUpdated:
                    break;
                case EVREventType.VREvent_SpatialAnchors_RequestPoseUpdate:
                    break;
                case EVREventType.VREvent_SpatialAnchors_RequestDescriptorUpdate:
                    break;
                case EVREventType.VREvent_SystemReport_Started:
                    break;
                case EVREventType.VREvent_Monitor_ShowHeadsetView:
                    break;
                case EVREventType.VREvent_Monitor_HideHeadsetView:
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
