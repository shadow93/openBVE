using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using SDL2;
namespace OpenBve {
	internal partial class formMain : Form {
		
		
		// ========
		// controls
		// ========

		// controls
		private void listviewControls_SelectedIndexChanged(object sender, EventArgs e) {
			if (listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				{
					this.Tag = new object();
					{ // command
						int j; for (j = 0; j < OpenBve.Controls.CommandInfos.Length; j++) {
							if (OpenBve.Controls.CommandInfos[j].Command == OpenBve.Controls.CurrentControls[i].Command) {
								comboboxCommand.SelectedIndex = j;
								break;
							}
						} if (j == OpenBve.Controls.CommandInfos.Length) {
							comboboxCommand.SelectedIndex = -1;
						}
					}
					// data
					if (OpenBve.Controls.CurrentControls[i].Method == OpenBve.Controls.ControlMethod.Keyboard) {
						radiobuttonKeyboard.Checked = true;
					} else if (OpenBve.Controls.CurrentControls[i].Method == OpenBve.Controls.ControlMethod.Joystick) {
						radiobuttonJoystick.Checked = true;
					} else {
						radiobuttonKeyboard.Checked = false;
						radiobuttonJoystick.Checked = false;
					}
					panelKeyboard.Enabled = radiobuttonKeyboard.Checked;
					if (radiobuttonKeyboard.Checked) {
						int j; for (j = 0; j < OpenBve.Controls.Keys.Length; j++) {
							if (OpenBve.Controls.Keys[j].Scancode != SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN && 
								OpenBve.Controls.Keys[j].Scancode == (SDL.SDL_Scancode)OpenBve.Controls.CurrentControls[i].Element) {
								comboboxKeyboardKey.SelectedIndex = j;
								break;
							}
						} if (j == OpenBve.Controls.Keys.Length) {
							comboboxKeyboardKey.SelectedIndex = -1;
						}
						checkboxKeyboardShift.Checked = (OpenBve.Controls.CurrentControls[i].Modifier & OpenBve.Controls.KeyboardModifier.Shift) != 0;
						checkboxKeyboardCtrl.Checked = (OpenBve.Controls.CurrentControls[i].Modifier & OpenBve.Controls.KeyboardModifier.Ctrl) != 0;
						checkboxKeyboardAlt.Checked = (OpenBve.Controls.CurrentControls[i].Modifier & OpenBve.Controls.KeyboardModifier.Alt) != 0;
					} else if (radiobuttonJoystick.Checked) {
						labelJoystickAssignmentValue.Text = GetControlDetails(i);
					} else {
						comboboxKeyboardKey.SelectedIndex = -1;
						checkboxKeyboardShift.Checked = false;
						checkboxKeyboardCtrl.Checked = false;
						checkboxKeyboardAlt.Checked = false;
					}
					panelJoystick.Enabled = radiobuttonJoystick.Checked;
					// finalize
					this.Tag = null;
				}
				buttonControlRemove.Enabled = true;
				buttonControlUp.Enabled = i > 0;
				buttonControlDown.Enabled = i < OpenBve.Controls.CurrentControls.Length - 1;
				groupboxControl.Enabled = true;
			} else {
				this.Tag = new object();
				comboboxCommand.SelectedIndex = -1;
				radiobuttonKeyboard.Checked = false;
				radiobuttonJoystick.Checked = false;
				groupboxControl.Enabled = false;
				comboboxKeyboardKey.SelectedIndex = -1;
				checkboxKeyboardShift.Checked = false;
				checkboxKeyboardCtrl.Checked = false;
				checkboxKeyboardAlt.Checked = false;
				labelJoystickAssignmentValue.Text = "";
				this.Tag = null;
				buttonControlRemove.Enabled = false;
				buttonControlUp.Enabled = false;
				buttonControlDown.Enabled = false;
			}
		}
		private void UpdateControlListElement(ListViewItem Item, int Index, bool ResizeColumns) {
			OpenBve.Controls.CommandInfo Info;
			OpenBve.Controls.TryGetCommandInfo(OpenBve.Controls.CurrentControls[Index].Command, out Info);
			Item.SubItems[0].Text = Info.Name;
			switch (Info.Type) {
					case OpenBve.Controls.CommandType.Digital: Item.SubItems[1].Text = Strings.GetInterfaceString("controls_list_type_digital"); break;
					case OpenBve.Controls.CommandType.AnalogHalf: Item.SubItems[1].Text = Strings.GetInterfaceString("controls_list_type_analoghalf"); break;
					case OpenBve.Controls.CommandType.AnalogFull: Item.SubItems[1].Text = Strings.GetInterfaceString("controls_list_type_analogfull"); break;
					default: Item.SubItems[1].Text = Info.Type.ToString(); break;
			}
			Item.SubItems[2].Text = Info.Description;
			if (OpenBve.Controls.CurrentControls[Index].Method == OpenBve.Controls.ControlMethod.Keyboard) {
				Item.ImageKey = "keyboard";
			} else if (OpenBve.Controls.CurrentControls[Index].Method == OpenBve.Controls.ControlMethod.Joystick) {
				if (Info.Type == OpenBve.Controls.CommandType.AnalogHalf | Info.Type == OpenBve.Controls.CommandType.AnalogFull) {
					Item.ImageKey = "joystick";
				} else {
					Item.ImageKey = "gamepad";
				}
			} else {
				Item.ImageKey = null;
			}
			Item.SubItems[3].Text = GetControlDetails(Index);
			if (ResizeColumns) {
				listviewControls.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		// get control details
		private string GetControlDetails(int Index) {
			OpenBve.Controls.CommandInfo Info;
			OpenBve.Controls.TryGetCommandInfo(OpenBve.Controls.CurrentControls[Index].Command, out Info);
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			string Separator = Strings.GetInterfaceString("controls_assignment_separator");
			if (OpenBve.Controls.CurrentControls[Index].Method == OpenBve.Controls.ControlMethod.Keyboard) {
				string t = Strings.GetInterfaceString("controls_assignment_keyboard") + Separator;
				if ((OpenBve.Controls.CurrentControls[Index].Modifier & OpenBve.Controls.KeyboardModifier.Shift) != 0) t += Strings.GetInterfaceString("controls_assignment_keyboard_shift");
				if ((OpenBve.Controls.CurrentControls[Index].Modifier & OpenBve.Controls.KeyboardModifier.Ctrl) != 0) t += Strings.GetInterfaceString("controls_assignment_keyboard_ctrl");
				if ((OpenBve.Controls.CurrentControls[Index].Modifier & OpenBve.Controls.KeyboardModifier.Alt) != 0) t += Strings.GetInterfaceString("controls_assignment_keyboard_alt");
				int j; string bvename="";
				for (j = 0; j < OpenBve.Controls.Keys.Length; j++) {
					if (OpenBve.Controls.Keys[j].Scancode != SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN && 
						OpenBve.Controls.Keys[j].Scancode == (SDL.SDL_Scancode)OpenBve.Controls.CurrentControls[Index].Element) {
						t += (bvename = OpenBve.Controls.Keys[j].Description);
						break;
					}
				} if (j == OpenBve.Controls.Keys.Length || OpenBve.Controls.Keys[j].Scancode == SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN) {
					t += "{" + ((SDL.SDL_Scancode)OpenBve.Controls.CurrentControls[Index].Element).ToString().Substring(13) + "}";
				} else {
					string sdlname = SDL.SDL_GetKeyName(SDL.SDL_GetKeyFromScancode(OpenBve.Controls.Keys[j].Scancode));
					if (Conversions.TrimInside(sdlname.ToLower()) != Conversions.TrimInside(bvename.ToLower()))
						t += " (" + sdlname + ")";
				}
					return t;
			} else if (OpenBve.Controls.CurrentControls[Index].Method == OpenBve.Controls.ControlMethod.Joystick) {
				string t = Strings.GetInterfaceString("controls_assignment_joystick").Replace("[index]", (OpenBve.Controls.CurrentControls[Index].Device + 1).ToString(Culture));
				switch (OpenBve.Controls.CurrentControls[Index].Component) {
					case OpenBve.Controls.JoystickComponent.Axis:
						t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_axis").Replace("[index]", (OpenBve.Controls.CurrentControls[Index].Element + 1).ToString(Culture));
						float pos = (float)OpenBve.Controls.CurrentControls[Index].Direction;
						if (pos == -1) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_axis_negative");
						} else if (pos == 1) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_axis_positive");
						} else {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_axis_invalid");
						} break;
					case OpenBve.Controls.JoystickComponent.Button:
						t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_button").Replace("[index]", (OpenBve.Controls.CurrentControls[Index].Element + 1).ToString(Culture));
						break;
					case OpenBve.Controls.JoystickComponent.Hat:
						t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat").Replace("[index]", (OpenBve.Controls.CurrentControls[Index].Element + 1).ToString(Culture));
						int state = (int)OpenBve.Controls.CurrentControls[Index].Direction;
						if (state == SDL.SDL_HAT_LEFT) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_left");
						} else if (state == SDL.SDL_HAT_LEFTUP) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_upleft");
						} else if (state == SDL.SDL_HAT_UP) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_up");
						} else if (state == SDL.SDL_HAT_RIGHTUP) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_upright");
						} else if (state == SDL.SDL_HAT_RIGHT) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_right");
						} else if (state == SDL.SDL_HAT_RIGHTDOWN) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_downright");
						} else if (state == SDL.SDL_HAT_DOWN) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_down");
						} else if (state == SDL.SDL_HAT_LEFTDOWN) {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_downleft");
						} else {
							t += Separator + Strings.GetInterfaceString("controls_assignment_joystick_hat_invalid");
						} break;
					default:
						break;
				}
				return t;
			} else {
				return Strings.GetInterfaceString("controls_assignment_invalid");
			}
		}

		// control add
		private void buttonControlAdd_Click(object sender, EventArgs e) {
			for (int i = 0; i < OpenBve.Controls.CurrentControls.Length; i++) {
				listviewControls.Items[i].Selected = false;
			}
			int n = OpenBve.Controls.CurrentControls.Length;
			Array.Resize<OpenBve.Controls.Control>(ref OpenBve.Controls.CurrentControls, n + 1);
			OpenBve.Controls.CurrentControls[n].Command = OpenBve.Controls.Command.None;
			ListViewItem Item = new ListViewItem(new string[] { "", "", "", "" });
			UpdateControlListElement(Item, n, true);
			listviewControls.Items.Add(Item);
			Item.Selected = true;
		}

		// control remove
		private void buttonControlRemove_Click(object sender, EventArgs e) {
			if (listviewControls.SelectedIndices.Count == 1) {
				int j = listviewControls.SelectedIndices[0];
				for (int i = j; i < OpenBve.Controls.CurrentControls.Length - 1; i++) {
					OpenBve.Controls.CurrentControls[i] = OpenBve.Controls.CurrentControls[i + 1];
				}
				Array.Resize<OpenBve.Controls.Control>(ref OpenBve.Controls.CurrentControls, OpenBve.Controls.CurrentControls.Length - 1);
				listviewControls.Items[j].Remove();
			}
		}

		// control up
		private void buttonControlUp_Click(object sender, EventArgs e) {
			if (listviewControls.SelectedIndices.Count == 1) {
				int j = listviewControls.SelectedIndices[0];
				if (j > 0) {
					OpenBve.Controls.Control c = OpenBve.Controls.CurrentControls[j];
					OpenBve.Controls.CurrentControls[j] = OpenBve.Controls.CurrentControls[j - 1];
					OpenBve.Controls.CurrentControls[j - 1] = c;
					ListViewItem v = listviewControls.Items[j];
					listviewControls.Items.RemoveAt(j);
					listviewControls.Items.Insert(j - 1, v);
				}
			}
		}

		// control down
		private void buttonControlDown_Click(object sender, EventArgs e) {
			if (listviewControls.SelectedIndices.Count == 1) {
				int j = listviewControls.SelectedIndices[0];
				if (j < OpenBve.Controls.CurrentControls.Length - 1) {
					OpenBve.Controls.Control c = OpenBve.Controls.CurrentControls[j];
					OpenBve.Controls.CurrentControls[j] = OpenBve.Controls.CurrentControls[j + 1];
					OpenBve.Controls.CurrentControls[j + 1] = c;
					ListViewItem v = listviewControls.Items[j];
					listviewControls.Items.RemoveAt(j);
					listviewControls.Items.Insert(j + 1, v);
				}
			}
		}

		// command
		private void comboboxCommand_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				int j = comboboxCommand.SelectedIndex;
				if (j >= 0) {
					OpenBve.Controls.CurrentControls[i].Command = OpenBve.Controls.CommandInfos[j].Command;
					OpenBve.Controls.CommandInfo Info;
					OpenBve.Controls.TryGetCommandInfo(OpenBve.Controls.CommandInfos[j].Command, out Info);
					OpenBve.Controls.CurrentControls[i].InheritedType = Info.Type;
					UpdateControlListElement(listviewControls.Items[i], i, true);
				}
			}
		}

		// ========
		// keyboard
		// ========

		// keyboard
		private void radiobuttonKeyboard_CheckedChanged(object sender, EventArgs e) {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				OpenBve.Controls.CurrentControls[i].Method = OpenBve.Controls.ControlMethod.Keyboard;
				UpdateControlListElement(listviewControls.Items[i], i, true);
			}
			panelKeyboard.Enabled = radiobuttonKeyboard.Checked;
		}

		// key
		private void comboboxKeyboardKey_SelectedIndexChanged(object sender, EventArgs e) {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				int j = comboboxKeyboardKey.SelectedIndex;
				if (j >= 0) {
					OpenBve.Controls.CurrentControls[i].Element = (int)OpenBve.Controls.Keys[j].Scancode;
				}
				UpdateControlListElement(listviewControls.Items[i], i, true);
			}
		}

		// modifiers
		private void checkboxKeyboardShift_CheckedChanged(object sender, EventArgs e) {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				OpenBve.Controls.CurrentControls[i].Modifier = (checkboxKeyboardShift.Checked ? OpenBve.Controls.KeyboardModifier.Shift : OpenBve.Controls.KeyboardModifier.None) |
					(checkboxKeyboardCtrl.Checked ? OpenBve.Controls.KeyboardModifier.Ctrl : OpenBve.Controls.KeyboardModifier.None) |
					(checkboxKeyboardAlt.Checked ? OpenBve.Controls.KeyboardModifier.Alt : OpenBve.Controls.KeyboardModifier.None);
				UpdateControlListElement(listviewControls.Items[i], i, true);
			}
		}
		private void checkboxKeyboardCtrl_CheckedChanged(object sender, EventArgs e) {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				OpenBve.Controls.CurrentControls[i].Modifier = (checkboxKeyboardShift.Checked ? OpenBve.Controls.KeyboardModifier.Shift : OpenBve.Controls.KeyboardModifier.None) |
					(checkboxKeyboardCtrl.Checked ? OpenBve.Controls.KeyboardModifier.Ctrl : OpenBve.Controls.KeyboardModifier.None) |
					(checkboxKeyboardAlt.Checked ? OpenBve.Controls.KeyboardModifier.Alt : OpenBve.Controls.KeyboardModifier.None);
				UpdateControlListElement(listviewControls.Items[i], i, true);
			}
		}
		private void checkboxKeyboardAlt_CheckedChanged(object sender, EventArgs e) {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				OpenBve.Controls.CurrentControls[i].Modifier = (checkboxKeyboardShift.Checked ? OpenBve.Controls.KeyboardModifier.Shift : OpenBve.Controls.KeyboardModifier.None) |
					(checkboxKeyboardCtrl.Checked ? OpenBve.Controls.KeyboardModifier.Ctrl : OpenBve.Controls.KeyboardModifier.None) |
					(checkboxKeyboardAlt.Checked ? OpenBve.Controls.KeyboardModifier.Alt : OpenBve.Controls.KeyboardModifier.None);
				UpdateControlListElement(listviewControls.Items[i], i, true);
			}
		}

		
		
		// ========
		// joystick
		// ========

		// joystick
		private void radiobuttonJoystick_CheckedChanged(object sender, EventArgs e) {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int i = listviewControls.SelectedIndices[0];
				OpenBve.Controls.CurrentControls[i].Method = OpenBve.Controls.ControlMethod.Joystick;
				UpdateControlListElement(listviewControls.Items[i], i, true);
			}
			panelJoystick.Enabled = radiobuttonJoystick.Checked;
		}

		// details
		private void UpdateJoystickDetails() {
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int j = listviewControls.SelectedIndices[0];
				labelJoystickAssignmentValue.Text = GetControlDetails(j);

			}
		}

		// import
		private void buttonControlsImport_Click(object sender, EventArgs e) {
			OpenFileDialog Dialog = new OpenFileDialog();
			Dialog.CheckFileExists = true;
			//Dialog.InitialDirectory = Interface.GetControlsFolder();
			Dialog.Filter = Strings.GetInterfaceString("dialog_controlsfiles") + "|*.controls|" + Strings.GetInterfaceString("dialog_allfiles") + "|*";
			if (Dialog.ShowDialog() == DialogResult.OK) {
				try {
					OpenBve.Controls.LoadControls(Dialog.FileName, out OpenBve.Controls.CurrentControls);
					for (int i = 0; i < listviewControls.SelectedItems.Count; i++) {
						listviewControls.SelectedItems[i].Selected = false;
					}
					listviewControls.Items.Clear();
					ListViewItem[] Items = new ListViewItem[OpenBve.Controls.CurrentControls.Length];
					for (int i = 0; i < OpenBve.Controls.CurrentControls.Length; i++) {
						Items[i] = new ListViewItem(new string[] { "", "", "", "" });
						UpdateControlListElement(Items[i], i, false);
					}
					listviewControls.Items.AddRange(Items);
					listviewControls.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				} catch (Exception ex) {
					MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}

		// export
		private void buttonControlsExport_Click(object sender, EventArgs e) {
			SaveFileDialog Dialog = new SaveFileDialog();
			Dialog.OverwritePrompt = true;
			//Dialog.InitialDirectory = Interface.GetControlsFolder();
			Dialog.Filter = Strings.GetInterfaceString("dialog_controlsfiles") + "|*.controls|" + Strings.GetInterfaceString("dialog_allfiles") + "|*";
			if (Dialog.ShowDialog() == DialogResult.OK) {
				try {
					OpenBve.Controls.SaveControls(Dialog.FileName);
				} catch (Exception ex) {
					MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}

		// joystick grab
		private void textboxJoystickGrab_Enter(object sender, EventArgs e) {
			bool FullAxis = false;
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int j = listviewControls.SelectedIndices[0];
				if (OpenBve.Controls.CurrentControls[j].InheritedType == OpenBve.Controls.CommandType.AnalogFull) {
					FullAxis = true;
				}
			}
			if (FullAxis) {
				textboxJoystickGrab.Text = Strings.GetInterfaceString("controls_selection_joystick_assignment_grab_fullaxis");
			} else {
				textboxJoystickGrab.Text = Strings.GetInterfaceString("controls_selection_joystick_assignment_grab_normal");
			}
			textboxJoystickGrab.BackColor = Color.Crimson;
			textboxJoystickGrab.ForeColor = Color.White;
		}
		private void textboxJoystickGrab_Leave(object sender, EventArgs e) {
			textboxJoystickGrab.Text = Strings.GetInterfaceString("controls_selection_joystick_assignment_grab");
			textboxJoystickGrab.BackColor = panelControls.BackColor;
			textboxJoystickGrab.ForeColor = Color.Black;
		}

		// attached joysticks
		private void pictureboxJoysticks_Paint(object sender, PaintEventArgs e) {
			int device = -1;
			OpenBve.Controls.JoystickComponent component = OpenBve.Controls.JoystickComponent.Invalid;
			int element = -1;
			int direction = SDL.SDL_HAT_CENTERED;
			OpenBve.Controls.CommandType type = OpenBve.Controls.CommandType.Digital;
			if (this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int j = listviewControls.SelectedIndices[0];
				if (OpenBve.Controls.CurrentControls[j].Method == OpenBve.Controls.ControlMethod.Joystick) {
					device = OpenBve.Controls.CurrentControls[j].Device;
					component = OpenBve.Controls.CurrentControls[j].Component;
					element = OpenBve.Controls.CurrentControls[j].Element;
					direction = (int)OpenBve.Controls.CurrentControls[j].Direction;
					type = OpenBve.Controls.CurrentControls[j].InheritedType;
				}
			}
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			Font f = new Font(this.Font.Name, 0.875f * this.Font.Size);
			float x = 2.0f, y = 2.0f;
			float threshold = ((float)trackbarJoystickAxisThreshold.Value - (float)trackbarJoystickAxisThreshold.Minimum) / (float)(trackbarJoystickAxisThreshold.Maximum - trackbarJoystickAxisThreshold.Minimum);
			int i = 0;
			foreach(var joystick in Joysticks.AttachedJoysticks) {
				float w, h;
				if (JoystickImage != null) {
					e.Graphics.DrawImage(JoystickImage, x, y);
					w = (float)JoystickImage.Width;
					h = (float)JoystickImage.Height;
					if (h < 64.0f) h = 64.0f;
				} else {
					w = 64.0f; h = 64.0f;
					e.Graphics.DrawRectangle(new Pen(labelControlsTitle.BackColor), x, y, w, h);
				}
				{ // joystick number
					e.Graphics.FillEllipse(Brushes.Gold, x + w - 16.0f, y, 16.0f, 16.0f);
					e.Graphics.DrawEllipse(Pens.Black, x + w - 16.0f, y, 16.0f, 16.0f);
					string t = (i + 1).ToString(Culture);
					SizeF s = e.Graphics.MeasureString(t, f);
					e.Graphics.DrawString(t, f, Brushes.Black, x + w - 8.0f - 0.5f * s.Width, y + 8.0f - 0.5f * s.Height);
				}
				{ // joystick name
					e.Graphics.DrawString(joystick.Name, this.Font, Brushes.Black, x + w + 8.0f, y);
				}
				float m;
				if (groupboxJoysticks.Enabled) {
					m = x;
					Pen p = new Pen(Color.DarkGoldenrod, 2.0f);
					Pen ps = new Pen(Color.Firebrick, 2.0f);
					{ // first row
						float u = x + w + 8.0f;
						float v = y + 24.0f;
						float g = h - 24.0f;
						{ // trackballs
							int n = SDL.SDL_JoystickNumBalls(joystick.Handle);
							for (int j = 0; j < n; j++) {
								e.Graphics.DrawEllipse(Pens.Gray, u, v, g, g);
								string t = "L" + (j + 1).ToString(Culture);
								SizeF s = e.Graphics.MeasureString(t, f);
								e.Graphics.DrawString(t, f, Brushes.Gray, u + 0.5f * (g - s.Width), v + 0.5f * (g - s.Height));
								int dx, dy;
								SDL.SDL_JoystickGetBall(joystick.Handle, j, out dx, out dy);
								u += g + 8.0f;
							}
						}
						{ // hats
							int n = SDL.SDL_JoystickNumHats(joystick.Handle);
							for (int j = 0; j < n; j++) {
								if (device == joystick.Index & component == OpenBve.Controls.JoystickComponent.Hat & element == j) {
									e.Graphics.DrawEllipse(ps, u, v, g, g);
								} else {
									e.Graphics.DrawEllipse(p, u, v, g, g);
								}
								string t = "H" + (j + 1).ToString(Culture);
								SizeF s = e.Graphics.MeasureString(t, f);
								e.Graphics.DrawString(t, f, Brushes.Black, u + 0.5f * (g - s.Width), v + 0.5f * (g - s.Height));
								int pos = SDL.SDL_JoystickGetHat(joystick.Handle, j);
								if (pos != SDL.SDL_HAT_CENTERED) {
									double rx = (pos & SDL.SDL_HAT_LEFT)!=0 ? -1.0 : (pos & SDL.SDL_HAT_RIGHT)!=0 ? 1.0 : 0.0;
									double ry = (pos & SDL.SDL_HAT_UP)!=0   ? -1.0 : (pos & SDL.SDL_HAT_DOWN)!=0  ? 1.0 : 0.0;
									double rt = rx * rx + ry * ry;
									rt = 1.0 / Math.Sqrt(rt);
									rx *= rt; ry *= rt;
									float dx = (float)(0.5 * rx * (g - 8.0));
									float dy = (float)(0.5 * ry * (g - 8.0));
									e.Graphics.FillEllipse(Brushes.White, u + 0.5f * g + dx - 4.0f, v + 0.5f * g + dy - 4.0f, 8.0f, 8.0f);
									e.Graphics.DrawEllipse(new Pen(Color.Firebrick, 2.0f), u + 0.5f * g + dx - 4.0f, v + 0.5f * g + dy - 4.0f, 8.0f, 8.0f);
								}
								int hatState = direction;
								if (device == joystick.Index & component == OpenBve.Controls.JoystickComponent.Hat & element == j) {
									double rx = (hatState & SDL.SDL_HAT_LEFT)!=0 ? -1.0 : (hatState & SDL.SDL_HAT_RIGHT)!=0 ? 1.0 : 0.0;
									double ry = (hatState & SDL.SDL_HAT_UP)!=0 ? -1.0 : (hatState & SDL.SDL_HAT_DOWN)!=0 ? 1.0 : 0.0;
									double rt = rx * rx + ry * ry;
									rt = 1.0 / Math.Sqrt(rt);
									rx *= rt; ry *= rt;
									float dx = (float)(0.5 * rx * (g - 8.0));
									float dy = (float)(0.5 * ry * (g - 8.0));
									e.Graphics.FillEllipse(Brushes.Firebrick, u + 0.5f * g + dx - 2.0f, v + 0.5f * g + dy - 2.0f, 4.0f, 4.0f);
								}
								u += g + 8.0f;
							}
						}
						if (u > m) m = u;
					}
					{ // second row
						float u = x;
						float v = y + h + 8.0f;
						{ // axes
							int n = SDL.SDL_JoystickNumAxes(joystick.Handle);
							float g = (float)pictureboxJoysticks.ClientRectangle.Height - v - 2.0f;
							for (int j = 0; j < n; j++) {
								float r = SDL.SDL_JoystickGetAxis(joystick.Handle, j);
								float r0 = r < 0.0f ? r : 0.0f;
								float r1 = r > 0.0f ? r : 0.0f;
								if ((float)Math.Abs((double)r) < threshold) {
									e.Graphics.FillRectangle(Brushes.RosyBrown, u, v + 0.5f * g - 0.5f * r1 * g, 16.0f, 0.5f * g * (r1 - r0));
								} else {
									e.Graphics.FillRectangle(Brushes.Firebrick, u, v + 0.5f * g - 0.5f * r1 * g, 16.0f, 0.5f * g * (r1 - r0));
								}
							float pos = (float)direction;
								if (device == joystick.Index & component == OpenBve.Controls.JoystickComponent.Axis & element == j) {
									if (pos == -1 & type != OpenBve.Controls.CommandType.AnalogFull) {
										e.Graphics.DrawRectangle(p, u, v, 16.0f, g);
										e.Graphics.DrawRectangle(ps, u, v + 0.5f * g, 16.0f, 0.5f * g);
									} else if (pos == 1 & type != OpenBve.Controls.CommandType.AnalogFull) {
										e.Graphics.DrawRectangle(p, u, v, 16.0f, g);
										e.Graphics.DrawRectangle(ps, u, v, 16.0f, 0.5f * g);
									} else {
										e.Graphics.DrawRectangle(ps, u, v, 16.0f, g);
									}
								} else {
									e.Graphics.DrawRectangle(p, u, v, 16.0f, g);
								}
								e.Graphics.DrawLine(p, u, v + (0.5f - 0.5f * threshold) * g, u + 16.0f, v + (0.5f - 0.5f * threshold) * g);
								e.Graphics.DrawLine(p, u, v + (0.5f + 0.5f * threshold) * g, u + 16.0f, v + (0.5f + 0.5f * threshold) * g);
								string t = "A" + (j + 1).ToString(Culture);
								SizeF s = e.Graphics.MeasureString(t, f);
								e.Graphics.DrawString(t, f, Brushes.Black, u + 0.5f * (16.0f - s.Width), v + g - s.Height - 2.0f);
								u += 24.0f;
							}
						}
						{ // buttons
							int n = SDL.SDL_JoystickNumButtons(joystick.Handle);
							float g = (float)0.5f * (pictureboxJoysticks.ClientRectangle.Height - v - 10.0f);
							for (int j = 0; j < n; j++) {
								bool q = SDL.SDL_JoystickGetButton(joystick.Handle, j)==1;
								float dv = (float)(j & 1) * (g + 8.0f);
								if (q) e.Graphics.FillRectangle(Brushes.Firebrick, u, v + dv, g, g);
								if (device == joystick.Index & component == OpenBve.Controls.JoystickComponent.Button & element == j) {
									e.Graphics.DrawRectangle(ps, u, v + dv, g, g);
								} else {
									e.Graphics.DrawRectangle(p, u, v + dv, g, g);
								}
								string t = "B" + (j + 1).ToString(Culture);
								SizeF s = e.Graphics.MeasureString(t, f);
								e.Graphics.DrawString(t, f, Brushes.Black, u + 0.5f * (g - s.Width), v + dv + 0.5f * (g - s.Height));
								if ((j & 1) != 0 | j == n - 1) u += g + 8.0f;
							}
						}
						if (u > m) m = u;
					}
				} else {
					m = x + w + 64.0f;
				}
				x = m + 8.0f;
				i++;
			}
		}		
	}
}