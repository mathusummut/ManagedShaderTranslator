﻿using System;
using IIS.SLSharp.Bindings.OpenTK;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GeoClipmapExample {
	public sealed class DemoWindow : GameWindow {
		private Clipmap.Clipmap _clipmap;

		private int _rotation;

		private int _up;

		private float _z;

		private int _height;

		private bool _wireFrame;

		private Vector3 _debugPos;

		private static void Main() {
			using (var win = new DemoWindow { Title = "GeoClip Shader Demo" })
				win.Run(0.0d);
		}

		protected override void OnLoad(EventArgs e) {
			Mouse.Move += (x, args) => {
				if (!Mouse[MouseButton.Left])
					return;

				_rotation = (_rotation + 360 + args.XDelta) % 360;
				_up -= args.YDelta;
				if (_up < -100)
					_up = -100;
				if (_up > 100)
					_up = 100;
			};

			Keyboard.KeyDown += (x, args) => {
				if (args.Key == Key.T)
					_wireFrame = !_wireFrame;
			};

			SLSharp.Init();
			_clipmap = new Clipmap.Clipmap();
			RecalcHeight();
		}

		protected override void OnUnload(EventArgs e) {
			_clipmap.Dispose();
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(ClientRectangle);
		}

		private float Lerp(float a, float b, float w) {
			return w * b + (1 - w) * a;
		}

		private void RecalcHeight() {
			_z = _clipmap.GeneratePixelAt(-(_clipmap.Position.X.Integer * 2 - 1), -(_clipmap.Position.Y.Integer * 2 - 1));
			var zx = _clipmap.GeneratePixelAt(-(_clipmap.Position.X.Integer * 2 + 1), -(_clipmap.Position.Y.Integer * 2 - 1));
			var zy = _clipmap.GeneratePixelAt(-(_clipmap.Position.X.Integer * 2 - 1), -(_clipmap.Position.Y.Integer * 2 + 1));
			var zxy = _clipmap.GeneratePixelAt(-(_clipmap.Position.X.Integer * 2 + 1), -(_clipmap.Position.Y.Integer * 2 + 1));

			var v1 = Lerp(_z, zx, _clipmap.Position.X.Fraction);
			var v2 = Lerp(zy, zxy, _clipmap.Position.X.Fraction);
			_z = Lerp(v1, v2, _clipmap.Position.Y.Fraction);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			var phi = MathHelper.DegreesToRadians(_rotation);
			var cosPhi = (float) Math.Cos(phi);
			var sinPhi = (float) Math.Sin(phi);

			var dx = 0;
			var dy = 0;
			if (Keyboard[Key.W])
				dy--;
			if (Keyboard[Key.S])
				dy++;
			if (Keyboard[Key.A])
				dx++;
			if (Keyboard[Key.D])
				dx--;
			if (Keyboard[Key.R])
				_clipmap.Reset();
			if (Keyboard[Key.Q]) {
				if (_height < 100)
					_height++;
			}
			if (Keyboard[Key.E]) {
				if (_height > 0)
					_height--;
			}

			// rotate vec dx/dy by cam

			var v = new Vector2(dx, dy);
			v.Normalize();
			if (!(Keyboard[Key.ShiftLeft] || Keyboard[Key.ShiftRight]))
				v *= 0.02f;

			var drx = cosPhi * v.X + sinPhi * v.Y;
			var dry = cosPhi * v.Y - sinPhi * v.X;
			//var drx = dx * 0.1f;
			//var dry = dy * 0.1f;


			if (dx != 0 || dy != 0) {
				_clipmap.MoveBy(drx, dry);
				RecalcHeight();
			}


			GL.ClearColor(0.6f, 0.6f, 0.8f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			//

			var aspect = Width / (float) Height;



			//z = Lerp(z, zy, _clipmap.Position.Y.Fraction);

			var minHeight = _z * 0.3f + 0.01f;
			var heightTotal = _height * 0.03f;
			if (heightTotal < minHeight)
				heightTotal = minHeight;


			_clipmap.ActiveMin = _height / 30;
			_clipmap.ActiveMax = _clipmap.ActiveMin + 4;

			var mod = Matrix4.LookAt(_debugPos.X, _debugPos.Y, _debugPos.Z + heightTotal,
				_debugPos.X + sinPhi, _debugPos.Y + cosPhi, _debugPos.Z + heightTotal + _up * 0.01f, 0.0f, 0.0f, 1.0f);
			var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), aspect, 0.0001f, 100.0f);


			// debug camera
			dx = 0;
			dy = 0;
			if (Keyboard[Key.Up])
				dy--;
			if (Keyboard[Key.Down])
				dy++;
			if (Keyboard[Key.Left])
				dx--;
			if (Keyboard[Key.Right])
				dx++;
			if (dx != 0 || dy != 0) {
				v = new Vector2(dx, dy);
				v.Normalize();
				v *= 0.005f;
				if (!(Keyboard[Key.ShiftLeft] || Keyboard[Key.ShiftRight]))
					v *= 0.1f;

				_debugPos += v.X * mod.Column0.Xyz + v.Y * mod.Column2.Xyz;
			}

			if (Keyboard[Key.Z])
				_clipmap.Debug();
			else {
				if (_wireFrame)
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

				_clipmap.Render(mod * proj, mod, false);
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			}

			SwapBuffers();
		}
	}
}