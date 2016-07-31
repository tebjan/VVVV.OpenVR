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
    [PluginInfo(Name = "Camera", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class OpenVRCameraNode : OpenVRProducerNode, IPluginEvaluate
    {
        [Output("View")]
        ISpread<Matrix> FViewOut;

        [Output("Projection")]
        ISpread<Matrix> FProjectionOut;

        [Output("Eye to Head")]
        ISpread<Matrix> FEyeToHeadOut;

        [Output("Vertices Left")]
        ISpread<float> FVerticesLeftOut;

        [Output("Vertices Right")]
        ISpread<float> FVerticesRightOut;

        [Output("Recommended Texture Size")]
        ISpread<Vector2D> FTexSizeOut;

        public override void Evaluate(int SpreadMax, CVRSystem system)
        {
            FTexSizeOut[0] = OpenVRManager.RecommendedRenderTargetSize;

            //camera properties
            var projL = system.GetProjectionMatrix(EVREye.Eye_Left, 0.05f, 100, EGraphicsAPIConvention.API_DirectX);
            var projR = system.GetProjectionMatrix(EVREye.Eye_Right, 0.05f, 100, EGraphicsAPIConvention.API_DirectX);
            FProjectionOut.SliceCount = 2;
            FProjectionOut[0] = projL.ToProjectionMatrix();
            FProjectionOut[1] = projR.ToProjectionMatrix();

            var eyeL = system.GetEyeToHeadTransform(EVREye.Eye_Left).ToEyeMatrix();
            var eyeR = system.GetEyeToHeadTransform(EVREye.Eye_Right).ToEyeMatrix();
            FEyeToHeadOut.SliceCount = 2;
            FEyeToHeadOut[0] = eyeL;
            FEyeToHeadOut[1] = eyeR;

            //view
            FViewOut.SliceCount = 2;
            FViewOut[0] = Matrix.Invert(eyeL * OpenVRManager.RenderPoses[0].mDeviceToAbsoluteTracking.ToMatrix());
            FViewOut[1] = Matrix.Invert(eyeR * OpenVRManager.RenderPoses[0].mDeviceToAbsoluteTracking.ToMatrix());

            //hidden pixels mesh
            var meshLeft = system.GetHiddenAreaMesh(EVREye.Eye_Left);
            var meshRight = system.GetHiddenAreaMesh(EVREye.Eye_Right);
            if (meshLeft.unTriangleCount > 0 && meshRight.unTriangleCount > 0)
            {
                GetMeshData(meshLeft, FVerticesLeftOut);
                GetMeshData(meshRight, FVerticesRightOut);
            }
            else
            {
                FVerticesLeftOut.SliceCount = 0;
                FVerticesRightOut.SliceCount = 0;
            }
        }

        void GetMeshData(HiddenAreaMesh_t meshData, ISpread<float> ret)
        {
            var floatCount = (int)meshData.unTriangleCount * 3 * 2;
            ret.SliceCount = floatCount;

            Marshal.Copy(meshData.pVertexData, ret.Stream.Buffer, 0, floatCount);
        }
    }
}
