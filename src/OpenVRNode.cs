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
    [PluginInfo(Name = "OpenVR")]
    public class ValveOpenVRNode : IPluginEvaluate, IDisposable
    {
        [Input("Handle")]
        IDiffSpread<int> FHandleIn;

        [Output("Projection")]
        ISpread<Matrix> FProjectionOut;

        [Output("Eye to Head")]
        ISpread<Matrix> FEyeToHeadOut;

        [Output("Render Poses")]
        ISpread<Matrix> FRenderPosesOut;

        [Output("Game Poses")]
        ISpread<Matrix> FGamePosesOut;

        [Output("Device Class")]
        ISpread<string> FDeviceClassOut;

        [Output("Recommended Texture Size")]
        ISpread<Vector2D> FTexSizeOut;

        [Output("Error", IsSingle = true)]
        ISpread<String> FErrorOut;

        //the vr system
        CVRSystem FOpenVRSystem;

        void SetStatus(object toString)
        {
            FErrorOut[0] = toString.ToString();
        }

        void InitOpenVR()
        {
            if (OpenVR.IsHmdPresent())
            {
                var initError = EVRInitError.Unknown;
                FOpenVRSystem = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Scene);
                uint sizeX = 0;
                uint sizeY = 0;
                FOpenVRSystem.GetRecommendedRenderTargetSize(ref sizeX, ref sizeY);
                FTexSizeOut[0] = new Vector2D(sizeX, sizeY);
                FDeviceClassOut.SliceCount = (int)OpenVR.k_unMaxTrackedDeviceCount;
                for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
                {
                    FDeviceClassOut[(int)i] = FOpenVRSystem.GetTrackedDeviceClass(i).ToString();
                }
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

        Texture_t FTex;

        VRTextureBounds_t FTexBoundsL = new VRTextureBounds_t() { uMin = 0, uMax = 0.5f, vMin = 0, vMax = 1 };
        VRTextureBounds_t FTexBoundsR = new VRTextureBounds_t() { uMin = 0.5f, uMax = 1, vMin = 0, vMax = 1 };
        public void Evaluate(int SpreadMax)
        {
            
            if (FOpenVRSystem == null)
                InitOpenVR();

            if(FHandleIn.IsChanged && FHandleIn[0] > 0)
            {
                FTex = new Texture_t() {
                    handle = new IntPtr(FHandleIn[0]),
                    eType = EGraphicsAPIConvention.API_DirectX,
                    eColorSpace = EColorSpace.Linear };
            }

            if(FOpenVRSystem != null)
            {
                //get campera properties
                var projL = FOpenVRSystem.GetProjectionMatrix(EVREye.Eye_Left, 0.05f, 100, EGraphicsAPIConvention.API_DirectX);
                var projR = FOpenVRSystem.GetProjectionMatrix(EVREye.Eye_Right, 0.05f, 100, EGraphicsAPIConvention.API_DirectX);
                FProjectionOut.SliceCount = 2;
                FProjectionOut[0] = projL.ToMatrix();
                FProjectionOut[1] = projR.ToMatrix();

                var eyeL = FOpenVRSystem.GetEyeToHeadTransform(EVREye.Eye_Left);
                var eyeR = FOpenVRSystem.GetEyeToHeadTransform(EVREye.Eye_Right);
                FEyeToHeadOut.SliceCount = 2;
                FEyeToHeadOut[0] = eyeL.ToMatrix();
                FEyeToHeadOut[1] = eyeR.ToMatrix();

                //get poses
                var renderPoses = new TrackedDevicePose_t[16];
                var gamePoses = new TrackedDevicePose_t[16];
                var error = OpenVR.Compositor.WaitGetPoses(renderPoses, gamePoses);
                SetStatus(error);
                if (error != EVRCompositorError.None) return;
                FRenderPosesOut.SliceCount = 16;
                FGamePosesOut.SliceCount = 16;

                for (int i = 0; i < 16; i++)
                {
                    FRenderPosesOut[i] = renderPoses[i].mDeviceToAbsoluteTracking.ToMatrix();
                    FGamePosesOut[i] = gamePoses[i].mDeviceToAbsoluteTracking.ToMatrix();
                }
                
                //set tex
                error = OpenVR.Compositor.Submit(EVREye.Eye_Left, ref FTex, ref FTexBoundsL, EVRSubmitFlags.Submit_Default);
                SetStatus(error);
                if (error != EVRCompositorError.None) return;
                error = OpenVR.Compositor.Submit(EVREye.Eye_Right, ref FTex, ref FTexBoundsR, EVRSubmitFlags.Submit_Default);
                SetStatus(error);
                if (error != EVRCompositorError.None) return;
            }
        }

        public void Dispose()
        {
            ShutDownOpenVR();
        }
    }

    public static class OpenVRExtensions
    {
        public static Matrix ToMatrix(this HmdMatrix44_t m)
        {
            return new Matrix()
            {
                M11 = m.m0,
                M12 = m.m1,
                M13 = m.m2,
                M14 = m.m3,

                M21 = m.m4,
                M22 = m.m5,
                M23 = m.m6,
                M24 = m.m7,

                M31 = m.m8,
                M32 = m.m9,
                M33 = m.m10,
                M34 = m.m11,

                M41 = m.m12,
                M42 = m.m13,
                M43 = m.m14,
                M44 = m.m15,
            };
        }

        public static Matrix ToMatrix(this HmdMatrix34_t m)
        {
            return new Matrix()
            {
                M11 = m.m0,
                M12 = m.m1,
                M13 = m.m2,
                M14 = m.m3,

                M21 = m.m4,
                M22 = m.m5,
                M23 = m.m6,
                M24 = m.m7,

                M31 = m.m8,
                M32 = m.m9,
                M33 = m.m10,
                M34 = m.m11,

                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1,
            };
        }
    }
}
