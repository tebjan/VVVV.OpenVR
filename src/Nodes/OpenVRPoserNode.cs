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
using SlimDX.Direct3D11;
using System.Runtime.InteropServices;

namespace VVVV.Nodes.ValveOpenVR
{
    [PluginInfo(Name = "Poser", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class ValveOpenVRInputNode : OpenVRProducerNode, IPluginEvaluate, IDisposable
    {

        [Output("HMD Pose", IsSingle = true)]
        ISpread<Matrix> FHMDPoseOut;

        [Output("Lighthouse Poses")]
        ISpread<Matrix> FLighthousePosesOut;

        [Output("Controller Poses")]
        ISpread<Matrix> FControllerPosesOut;

        [Output("Render Poses")]
        ISpread<Matrix> FRenderPosesOut;

        [Output("Game Poses")]
        ISpread<Matrix> FGamePosesOut;

        [Output("Device Class")]
        ISpread<string> FDeviceClassOut;

        [Output("Remaining Frame Time Pre")]
        ISpread<float> FRemainingTimePre;

        [Output("Remaining Frame Time Post")]
        ISpread<float> FRemainingTimePost;

        public override void Evaluate(int SpreadMax, CVRSystem system)
        {
            //poses
            var poseCount = (int)OpenVR.k_unMaxTrackedDeviceCount;
            var renderPoses = new TrackedDevicePose_t[poseCount];
            var gamePoses = new TrackedDevicePose_t[poseCount];

            FRemainingTimePre[0] = OpenVR.Compositor.GetFrameTimeRemaining();
            var error = OpenVR.Compositor.WaitGetPoses(renderPoses, gamePoses);
            SetStatus(error);
            if (error != EVRCompositorError.None) return;
            FRemainingTimePost[0] = OpenVR.Compositor.GetFrameTimeRemaining();

            OpenVRManager.RenderPoses = renderPoses;
            OpenVRManager.GamePoses = gamePoses;

            FRenderPosesOut.SliceCount = poseCount;
            FGamePosesOut.SliceCount = poseCount;
            FDeviceClassOut.SliceCount = poseCount;
            FLighthousePosesOut.SliceCount = 0;
            FControllerPosesOut.SliceCount = 0;

            for (int i = 0; i < poseCount; i++)
            {
                FRenderPosesOut[i] = renderPoses[i].mDeviceToAbsoluteTracking.ToMatrix();
                FGamePosesOut[i] = gamePoses[i].mDeviceToAbsoluteTracking.ToMatrix();
                var deviceClass = system.GetTrackedDeviceClass((uint)i);
                FDeviceClassOut[i] = deviceClass.ToString();

                if (deviceClass == ETrackedDeviceClass.TrackingReference)
                {
                    FLighthousePosesOut.Add(FGamePosesOut[i]);
                }

                if (deviceClass == ETrackedDeviceClass.Controller)
                {
                    FControllerPosesOut.Add(FGamePosesOut[i]);
                }
            }

            FHMDPoseOut[0] = FRenderPosesOut[0];
        }

        public void Dispose()
        {
            OpenVRManager.ShutDownOpenVR();
        }
    }
}
