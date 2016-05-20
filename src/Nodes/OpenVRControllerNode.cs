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
    [PluginInfo(Name = "Controller", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class ValveOpenVRControllerNode : IPluginEvaluate
    {
        [Input("System")]
        IDiffSpread<CVRSystem> FSystemIn;

        [Output("Events")]
        ISpread<String> FEventsOut;

        [Output("Device Index")]
        ISpread<int> FDeviceIndexOut;

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

        [Output("Error", IsSingle = true)]
        ISpread<String> FErrorOut;

        //the vr system
        CVRSystem FOpenVRSystem;
        uint FEvtSize = (uint)Marshal.SizeOf(typeof(VREvent_t));

        void SetStatus(object toString)
        {
            if(toString is EVRInitError)
                FErrorOut[0] = OpenVR.GetStringForHmdError((EVRInitError)toString);
            else
                FErrorOut[0] = toString.ToString();
        }

        public void Evaluate(int SpreadMax)
        {
            if (FSystemIn.IsChanged)
                FOpenVRSystem = FSystemIn[0];

            if(FOpenVRSystem != null)
            {
                VREvent_t evt = default(VREvent_t);
                FEventsOut.SliceCount = 0;
                FDeviceIndexOut.SliceCount = 0;

                while (FOpenVRSystem.PollNextEvent(ref evt, FEvtSize))
                {
                    var evtType = (EVREventType)evt.eventType;
                    FEventsOut.Add(evtType.ToString());
                    ProcessEvent(evtType, evt);
                }

                //controller states
                OpenVRController.Update(FFrame++);

                FDeviceIndexOut.SliceCount = 0;

                FTriggerTouchOut.SliceCount = 0;
                FTriggerPressOut.SliceCount = 0;
                FTriggerAxisOut.SliceCount = 0;

                FTouchpadTouchOut.SliceCount = 0;
                FTouchpadPressOut.SliceCount = 0;
                FTouchpadAxisOut.SliceCount = 0;

                var indexLeft = (int)FOpenVRSystem.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
                var indexRight = (int)FOpenVRSystem.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
                if (indexLeft > 0)
                {
                    OutputController(indexLeft);
                }

                if (indexRight > 0)
                {
                    OutputController(indexRight); 
                }
            }
        }

        void OutputController(int index)
        {
            var controller = OpenVRController.Input(index);
            FDeviceIndexOut.Add(index);

            FTriggerTouchOut.Add(controller.GetTouch(OpenVRController.ButtonMask.Trigger));
            FTriggerPressOut.Add(controller.GetPress(OpenVRController.ButtonMask.Trigger));
            FTriggerAxisOut.Add(controller.hairTriggerValue);


            FTouchpadTouchOut.Add(controller.GetTouch(OpenVRController.ButtonMask.Touchpad));
            FTouchpadPressOut.Add(controller.GetPress(OpenVRController.ButtonMask.Touchpad));
            FTouchpadAxisOut.Add(controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad));

            var contState = default(VRControllerState_t);
            var modelState = default(RenderModel_ControllerMode_State_t);
            var compState = default(RenderModel_ComponentState_t);

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
