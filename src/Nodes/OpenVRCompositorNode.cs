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
    [PluginInfo(Name = "Compositor", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class ValveOpenVROutputNode : IPluginEvaluate
    {
        [Input("System")]
        IDiffSpread<CVRSystem> FSystemIn;

        [Input("Texture Handle")]
        IDiffSpread<int> FHandleIn;

        [Input("Is OU")]
        ISpread<bool> FIsOUIn;

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

        Texture_t FTexture;

        //side by side
        VRTextureBounds_t FSBSTexBoundsL = new VRTextureBounds_t() { uMin = 0, uMax = 0.5f, vMin = 0, vMax = 1 };
        VRTextureBounds_t FSBSTexBoundsR = new VRTextureBounds_t() { uMin = 0.5f, uMax = 1, vMin = 0, vMax = 1 };

        //over/under
        VRTextureBounds_t FOUTexBoundsL = new VRTextureBounds_t() { uMin = 0, uMax = 1, vMin = 0, vMax = 0.5f };
        VRTextureBounds_t FOUTexBoundsR = new VRTextureBounds_t() { uMin = 0, uMax = 1, vMin = 0.5f, vMax = 1 };
        public void Evaluate(int SpreadMax)
        {
            if (FSystemIn.IsChanged)
                FOpenVRSystem = FSystemIn[0];

            if(FHandleIn.IsChanged && FHandleIn[0] > 0)
            {
                FTexture = new Texture_t() {
                    handle = new IntPtr(FHandleIn[0]),
                    eType = EGraphicsAPIConvention.API_DirectX,
                    eColorSpace = EColorSpace.Linear };
            }

            if(FOpenVRSystem != null)
            {
                //set tex
                VRTextureBounds_t boundsL;
                VRTextureBounds_t boundsR;
                if (FIsOUIn[0])
                {
                    boundsL = FOUTexBoundsL;
                    boundsR = FOUTexBoundsR;
                }
                else
                {
                    boundsL = FSBSTexBoundsL;
                    boundsR = FSBSTexBoundsR;
                }

                var compositor = OpenVR.Compositor;
                var error = compositor.Submit(EVREye.Eye_Left, ref FTexture, ref boundsL, EVRSubmitFlags.Submit_Default);
                SetStatus(error);
                if (error != EVRCompositorError.None) return;
                error = compositor.Submit(EVREye.Eye_Right, ref FTexture, ref boundsR, EVRSubmitFlags.Submit_Default);
                SetStatus(error);
                if (error != EVRCompositorError.None) return;
            }
        }
    }

}
