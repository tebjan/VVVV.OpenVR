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

namespace VVVV.Nodes.ValveOpenVR
{
    [PluginInfo(Name = "TrackedCamera", Category = "OpenVR", AutoEvaluate = true, Tags = "vr, video, htc, vive, oculus, rift", Author = "tonfilm")]
    public class VOpenVRTrackedCameraNode : OpenVRConsumerBaseNode, IPluginEvaluate
    {
        [Input("Corrected Texture", IsSingle = true, DefaultBoolean = true)]
        ISpread<bool> FCorrectedTextureIn;

        OpenVRTrackedCamera.VideoStreamTexture FTexture;

        public override void Evaluate(int SpreadMax, CVRSystem system)
        {
            FTexture = OpenVRTrackedCamera.Source(FCorrectedTextureIn[0]);
        }
    }

}
