// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Animations.Effects
{
    /// <summary>
    /// An animation effect that applies blur.
    /// </summary>
    /// <seealso cref="Microsoft.Toolkit.Uwp.UI.Animations.Effects.AnimationEffect" />
    public class Blur : AnimationEffect
    {
        CompositionSurfaceBrush m_noiseBrush;

        /// <summary>
        /// Gets a value indicating whether blur is supported.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is supported; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSupported
            => ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);

        /// <summary>
        /// Gets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public override string EffectName { get; } = "Blur";

        protected override async Task OnCreate()
        {
            m_noiseBrush = Compositor.CreateSurfaceBrush();
            m_noiseBrush.Surface = await SurfaceLoader.LoadFromUri(new Uri("ms-appx:///Assets/Noise.jpg"));
            m_noiseBrush.Stretch = CompositionStretch.None;
        }

        /// <summary>
        /// Applies the effect.
        /// </summary>
        /// <returns>
        /// An array of strings of the effect properties to change.
        /// </returns>
        public override string[] ApplyEffect()
        {
            var gaussianBlur = new GaussianBlurEffect
            {
                Name = EffectName,
                BlurAmount = 0f,
                Optimization = EffectOptimization.Balanced,
                BorderMode = EffectBorderMode.Hard,
                Source = new CompositionEffectSourceParameter("source")
            };

            BlendEffect finalEffect = new BlendEffect
            {
                Foreground = new CompositionEffectSourceParameter("NoiseImage"),
                Background = gaussianBlur,
                Mode = BlendEffectMode.Screen,
            };

            var propertyToChange = $"{EffectName}.BlurAmount";
            var propertiesToAnimate = new[] { propertyToChange };
            
            EffectBrush = Compositor.CreateEffectFactory(finalEffect, propertiesToAnimate).CreateBrush();
            EffectBrush.SetSourceParameter("NoiseImage", m_noiseBrush);
            EffectBrush.SetSourceParameter("source", Compositor.CreateHostBackdropBrush());

            return propertiesToAnimate;
        }
    }
}