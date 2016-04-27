using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;
using Valve.VR;

namespace VVVV.Nodes.ValveOpenVR
{
    [PluginInfo(Name = "OpenVR")]
    public class ValveOpenVRNode : IPluginEvaluate, IDisposable
    {

        [Output("Status", IsSingle = true)]
        ISpread<String> FStatusOut;

        //the vr system
        CVRSystem FOpenVRSystem;

        void SetStatus(object toString)
        {
            FStatusOut[0] = toString.ToString();
        }

        void InitOpenVR()
        {
            if (OpenVR.IsHmdPresent())
            {
                var initError = EVRInitError.Unknown;
                FOpenVRSystem = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Scene);
                SetStatus(initError);
            }
            else
            {
                SetStatus(EVRInitError.Init_HmdNotFound);
            }
        }

        void ShutDownOpenVR()
        {
            OpenVR.Shutdown();
            FOpenVRSystem = null;
        }

        public void Evaluate(int SpreadMax)
        {
            if (FOpenVRSystem == null)
                InitOpenVR();

            if(FOpenVRSystem != null)
            {
                var dist = FOpenVRSystem.ComputeDistortion(EVREye.Eye_Left, 0, 0);
            }
        }

        public void Dispose()
        {
            ShutDownOpenVR();
        }
    }
}
