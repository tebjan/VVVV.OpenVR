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
        }
    }

}
