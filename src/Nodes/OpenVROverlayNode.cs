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
    //[PluginInfo(Name = "Overlay", Category = "OpenVR", Tags = "vr, htc, vive, oculus, rift", Author = "tonfilm")]
    public class ValveOpenVROverlayNode : IPluginEvaluate
    {
        [Input("System")]
        IDiffSpread<CVRSystem> FSystemIn;

        [Input("Texture Handle")]
        IDiffSpread<int> FHandleIn;

        [Input("Show")]
        IDiffSpread<bool> FShowIn;

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
        Spread<OpenVROverlay> FOverlayers = new Spread<OpenVROverlay>(0);
        public void Evaluate(int SpreadMax)
        {
            if (FSystemIn.IsChanged)
                FOpenVRSystem = FSystemIn[0];


            if(FOpenVRSystem != null)
            {
                
                //set tex
                var overlay = OpenVR.Overlay;
                FOverlayers.Resize(SpreadMax, i => new OpenVROverlay(overlay), ov => ov.Dispose());

                for (int i = 0; i < SpreadMax; i++)
                {
                    var overlayer = FOverlayers[i];
                    EVROverlayError error;
                    if (!overlayer.Created)
                    {
                        error = overlayer.Create("OV " + i, "Overlayer " + i);

                        SetStatus(error);
                        if (error != EVROverlayError.None) return;
                    }

                    if (FHandleIn.IsChanged && FHandleIn[i] > 0)
                    {
                        error = overlayer.SetTexture(FHandleIn[i]);
                        SetStatus(error);
                        if (error != EVROverlayError.None) return;
                    }

                    if (FShowIn.IsChanged)
                    {
                        if (FShowIn[i])
                            overlayer.Show();
                        else
                            overlayer.Hide();
                    } 
                }

            }
        }

        public class OpenVROverlay : IDisposable
        {
            ulong FHandle;
            ulong FThumbnailHandle;
            readonly CVROverlay FOverlay;
            public OpenVROverlay(CVROverlay overlay)
            {
                FOverlay = overlay;
            }

            public string Key;
            public string FriendlyName;

            public bool Created;
            public EVROverlayError Create(string key, string friendlyName)
            {
                if (!Created)
                {
                    Key = key;
                    FriendlyName = friendlyName;
                    var error = FOverlay.CreateOverlay(key, friendlyName, ref FHandle);
                    Created = error == EVROverlayError.None;
                    FOverlay.SetOverlayWidthInMeters(FHandle, 1.5f);
                    return error; 
                }
                else
                {
                    return EVROverlayError.None;
                }
            }

            Texture_t FTexture;
            public EVROverlayError SetTexture(int textureHandle)
            {
                FTexture = new Texture_t()
                {
                    handle = new IntPtr(textureHandle),
                    eType = EGraphicsAPIConvention.API_DirectX,
                    eColorSpace = EColorSpace.Linear
                };

                return FOverlay.SetOverlayTexture(FHandle, ref FTexture);
            }

            public EVROverlayError Show()
            {
                return FOverlay.ShowOverlay(FHandle);
            }

            public EVROverlayError Hide()
            {
                return FOverlay.HideOverlay(FHandle);
            }

            public void Dispose()
            {
                FOverlay.DestroyOverlay(FHandle);
            }
        }
    }

}
