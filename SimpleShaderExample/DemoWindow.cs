using System;
using IIS.SLSharp.Bindings.OpenTK;
using SimpleShaderExample.Shaders;
using IIS.SLSharp.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SimpleShaderExample {
	public sealed class DemoWindow : GameWindow {
		private MyShader _myShader;

		private static void Main() {
			using (var win = new DemoWindow { Title = "GeoClip Shader Demo" })
				win.Run(0.0d);
		}

		protected override void OnLoad(EventArgs e) {
			SLSharp.Init();
			_myShader = Shader.CreateSharedShader<MyShader>();
		}

		protected override void OnUnload(EventArgs e) {
			_myShader.Dispose();
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(ClientRectangle);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Disable(EnableCap.CullFace);
			_myShader.Begin();
			_myShader.Blue = 0.5f;
			_myShader.Invert.Channels = (new Vector4(0.0f, 1.0f, 0.0f, 0.0f)).ToVector4F();
			_myShader.RenderQuad();
			_myShader.End();

			SwapBuffers();
		}
	}
}