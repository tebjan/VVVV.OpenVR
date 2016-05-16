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

namespace VVVV.Nodes.ValveOpenVR
{
    [PluginInfo(Name = "Controller", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class ValveOpenVRControllerNode : IPluginEvaluate
    {
        [Input("System")]
        IDiffSpread<CVRSystem> FSystemIn;

        [Output("Events")]
        ISpread<String> FEventsOut;

        [Output("Error", IsSingle = true)]
        ISpread<String> FErrorOut;

        //the vr system
        CVRSystem FOpenVRSystem;

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
                while (FOpenVRSystem.PollNextEvent(ref evt, 0))
                {
                    FEventsOut.Add(evt.eventType.ToString());
                }
            }
        }
    }

}
