using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.ValveOpenVR
{
    public static class OpenVRManager
    {
        public static CVRSystem System
        {
            get;
            private set;
        }

        public static string ErrorMessage
        {
            get;
            private set;
        }

        public static Vector2D RecommendedRenderTargetSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the open vr system and sets the static System field.
        /// </summary>
        /// <returns>The created system class or null</returns>
        public static CVRSystem InitOpenVR()
        {
            if (System == null)
            {
                if (OpenVR.IsHmdPresent())
                {
                    var initError = EVRInitError.Unknown;
                    var system = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Scene);
                    SetStatus(initError);

                    if (initError != EVRInitError.None)
                        system = null;

                    System = system;
                }
                else
                {
                    SetStatus(EVRInitError.Init_HmdNotFound);
                    System = null;
                }

                if (System != null)
                {
                    //texture size
                    uint sizeX = 0;
                    uint sizeY = 0;
                    System.GetRecommendedRenderTargetSize(ref sizeX, ref sizeY);
                    RecommendedRenderTargetSize = new Vector2D(sizeX, sizeY);
                } 
            }

            return System;
        }

        public static TrackedDevicePose_t[] RenderPoses
        {
            get;
            set;
        }

        public static TrackedDevicePose_t[] GamePoses
        {
            get;
            set;
        }

        static void SetStatus(object toString)
        {
            if (toString is EVRInitError)
                ErrorMessage = OpenVR.GetStringForHmdError((EVRInitError)toString);
            else
                ErrorMessage = toString.ToString();
        }

        public static void ShutDownOpenVR()
        {
            OpenVR.Shutdown();
        }
    }
}
