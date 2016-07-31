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
    //not in use
    //[PluginInfo(Name = "Models", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class ValveOpenVModelsNode : IPluginEvaluate
    {
        [Input("System")]
        IDiffSpread<CVRSystem> FSystemIn;

        [Output("Model Names")]
        ISpread<String> FModelNamesOut;

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
                var models = OpenVR.RenderModels;
                var modelCount = models.GetRenderModelCount();
                FModelNamesOut.SliceCount = (int)modelCount;
                for (int i = 0; i < modelCount; i++)
                {
                    var sb = new StringBuilder(64);
                    models.GetRenderModelName((uint)i, sb, 64);
                    FModelNamesOut[i] = sb.ToString().TrimEnd();
                }
            }
        }
    }

}
