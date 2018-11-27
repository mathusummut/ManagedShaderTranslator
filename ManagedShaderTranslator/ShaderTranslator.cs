using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using IIS.SLSharp.Bindings.OpenTK;
using IIS.SLSharp.Shaders;
using System.IO;

namespace ManagedShaderTranslator {
	public partial class ShaderTranslator : Form {
		private Assembly loadedAssembly;

		[STAThread]
		public static void Main() {
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ShaderTranslator());
		}

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
			AppDomain domain = (AppDomain) sender;
			foreach (Assembly assembly in domain.GetAssemblies()) {
				if (assembly.FullName == args.Name)
					return assembly;
			}
			return null;
		}

		public ShaderTranslator() {
			InitializeComponent();
			openFileDialog1.Title = "Select C# Shader Assembly:";
			openFileDialog1.FileName = "VertexShader.dll";
			openFileDialog1.Filter = "Assembly Files | *.dll;*.exe";
			saveFileDialog1.Title = "Choose where to save collective shader file:";
			saveFileDialog1.FileName = "Shaders.csf";
			saveFileDialog1.Filter = "Collective Shader File | *.csf";
			button4.Image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
			listBox1.MouseDown += ListBox1_MouseDown;
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			glControl1.MakeCurrent();
			SLSharp.Init();
		}

		private void ListBox1_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Right && !(listBox1.SelectedIndex == -1 || listBox1.SelectedIndex == listBox1.Items.Count - 1))
				listBox1.Items.RemoveAt(listBox1.SelectedIndex);
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
			if (loadedAssembly == null)
				MessageBox.Show("No assembly is loaded yet.", "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else {
				if (listBox1.SelectedIndex == listBox1.Items.Count - 1) {
					using (TextDialog dialog = new TextDialog()) {
						if (dialog.ShowDialog() == DialogResult.OK) {
							string input = dialog.Input.Trim();
							if (input != "")
								listBox1.Items.Insert(listBox1.Items.Count - 1, dialog.Input.Trim());
						}
					}
				}
			}
		}

		private void button2_Click(object sender, EventArgs e) {
			if (openFileDialog1.ShowDialog() == DialogResult.OK) {
				label2.Text = openFileDialog1.FileName;
				try {
					loadedAssembly = Assembly.LoadFrom(openFileDialog1.FileName);
					if (loadedAssembly == null)
						throw new NullReferenceException("An error occurred while trying to load the specified assembly.");
				} catch (Exception ex) {
					new ErrorInformer(ex).ShowDialog();
				}
			}
		}

		private void button1_Click(object sender, EventArgs e) {
			if (loadedAssembly == null)
				MessageBox.Show("No assembly is loaded yet.", "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else if (listBox1.Items.Count == 1)
				MessageBox.Show("No class name is added to the reference list yet.", "Hold On", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else {
				for (int i = 0; i < listBox1.Items.Count - 1; i++) {
					try {
						using (Shader shader = Shader.CreateSharedShader(loadedAssembly.GetType(listBox1.Items[i].ToString(), true, true))) {
							if (shader == null)
								throw new TypeInitializationException(listBox1.Items[i].ToString(), new NullReferenceException("An error occurred while loading the specified shader."));
						}
					} catch (Exception ex) {
						new ErrorInformer(ex, "Error on loading class " + i + ": " + listBox1.Items[i].ToString()).ShowDialog();
					}
				}
				if (SLSharp.Shaders.Count == 0)
					MessageBox.Show("No shader was loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				else {
					if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
						using (FileStream stream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate)) {
							using (StreamWriter writer = new StreamWriter(stream)) {
								for (int i = 0; i < SLSharp.Shaders.Count; i++)
									writer.WriteLine(SLSharp.Shaders[i]);
							}
						}
						MessageBox.Show("File successfully saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					SLSharp.Shaders.Clear();
				}
			}
		}

		private void button3_Click(object sender, EventArgs e) {
			int index = listBox1.SelectedIndex;
			if (index <= 0 || index == listBox1.Items.Count - 1)
				return;
			string temp = listBox1.Items[index].ToString();
			listBox1.Items.RemoveAt(index);
			index -= 1;
			listBox1.Items.Insert(index, temp);
			listBox1.SelectedIndex = index;
		}

		private void button4_Click(object sender, EventArgs e) {
			int index = listBox1.SelectedIndex;
			if (index == -1 || index >= listBox1.Items.Count - 2)
				return;
			string temp = listBox1.Items[index].ToString();
			listBox1.Items.RemoveAt(index);
			index += 1;
			listBox1.Items.Insert(index, temp);
			listBox1.SelectedIndex = index;
		}
	}
}