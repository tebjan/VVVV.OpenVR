using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.ValveOpenVR
{
    public abstract class OpenVRBaseNode
    {

        [Output("Error")]
        ISpread<String> FErrorOut;

        protected CVRSystem InitOpenVR()
        {
            if (OpenVR.IsHmdPresent())
            {
                var initError = EVRInitError.Unknown;
                var system = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Scene);
                SetStatus(initError);

                if (initError != EVRInitError.None) return null;
                return system;
            }
            else
            {
                SetStatus(EVRInitError.Init_HmdNotFound);
                return null;
            }
        }

        protected void ShutDownOpenVR()
        {
            OpenVR.Shutdown();
        }

        protected void SetStatus(object toString)
        {
            if (toString is EVRInitError)
                FErrorOut[0] = OpenVR.GetStringForHmdError((EVRInitError)toString);
            else if (toString is EVRCompositorError)
            {
                var error = (EVRCompositorError)toString;

                if (error == EVRCompositorError.TextureIsOnWrongDevice)
                    FErrorOut[0] = "Texture on wrong device. Set your graphics driver to use the same video card for vvvv as the headset is plugged into.";
                else if (error == EVRCompositorError.TextureUsesUnsupportedFormat)
                    FErrorOut[0] = "Unsupported texture format. Make sure texture uses RGBA, is not compressed and has no mipmaps.";
                else
                    FErrorOut[0] = error.ToString();
            }
            else
                FErrorOut[0] = toString.ToString();
        }
    }
}
