using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;
using VVVV.Core.Logging;
using System.ComponentModel.Composition;
using Valve.VR;
using VVVV.Utils.VMath;
using SlimDX;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;

namespace VVVV.Nodes.ValveOpenVR
{
    [PluginInfo(Name = "HapticPulse", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift",
                Help = "Triggers the Controllers vibration motors", Author = "sebl", Credits = "tonfilm")]
    public class ValveOpenVRHapticPulseNode : IPluginEvaluate
    {
#pragma warning disable 0649
        [Input("Controller")]
        ISpread<OpenVRController.Device> FControllerIn;

        [Input("Enabled", IsSingle = true)]
        ISpread<bool> FInHapticEnabled;

        [Input("Power", IsSingle = true, DefaultValue = 0.5, MinValue = 0, MaxValue = 1)]
        IDiffSpread<double> FInHapticDuration;

        [Output("Status")]
        ISpread<string> FOutStatus;

        [Import()]
        public ILogger FLogger;
#pragma warning restore 0649


        public void Evaluate(int SpreadMax)
        {
            var system = OpenVRManager.System;
            if (system != null)
            {
                FOutStatus.SliceCount = 1;
                FOutStatus[0] = "Ok";

                var controller = FControllerIn[0];
                if (controller == null)
                {
                    FOutStatus[0] = "No Controller";
                }
                else if (FInHapticEnabled[0])
                {
                    // see: https://github.com/ValveSoftware/openvr/wiki/IVRSystem::TriggerHapticPulse

                    // see: https://steamcommunity.com/app/358720/discussions/0/517141624283630663/
                    // for now only axis with id 0 is working/implemented in OpenVR... and probably this will ner´ver change
                    var duration = VMath.Map(FInHapticDuration[0], 0, 1, 1, 3999, TMapMode.Clamp);
                    system.TriggerHapticPulse(controller.index, 0, (char)duration);
                }
            }
            else
            {
                FOutStatus.SliceCount = 1;
                FOutStatus[0] = "OpenVR is not initialized, at least one Poser (OpenVR) or Camera(OpenVR) must exist";
            }
        }


    }
}