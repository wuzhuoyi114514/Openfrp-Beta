using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.VisualBasic.Devices;
using ModernWpf;
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using OpenFrp.Service;
using OpenFrp.Service.Api;
using OpenFrp.Service.App;

namespace OpenFrp.Launcher.Pages
{
	public class Setting : ModernWpf.Controls.Page, IComponentConnector
	{
		internal ScrollViewerEx MainScroll;

		internal TextBlock Console_Preview;

		private bool _contentLoaded;

		public Setting()
		{
			InitializeComponent();
			base.Background = new SolidColorBrush
			{
				Color = Colors.Transparent
			};
		}

		private void Appearance_ComboBox_Loaded(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.ComboBox ctrl = sender as System.Windows.Controls.ComboBox;
			ctrl.SelectedIndex = (int)UIHelper.AppElementTheme;
			ctrl.SelectionChanged += delegate
			{
				UIHelper.AppElementTheme = (ElementTheme)ctrl.SelectedIndex;
			};
		}

		private void UserLogin_Button_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.TextBox username;
			PasswordBox password;
			TextBlock message;
			ContentDialog dialog;
			if (UIHelper.isUserLogin)
			{
				User.Logout();
				ToastNotificationManagerCompat.History.Clear();
				UIHelper.ConfigHelper.Settings.username = "";
				UIHelper.ConfigHelper.Settings.password = "";
				foreach (KeyValuePair<int, Process> item in ConsoleHelper.AllProcess)
				{
					item.Value.Kill();
				}
				ConsoleHelper.AllProcess.Clear();
				UIHelper.ConfigHelper.Settings.Asid.Clear();
				ConsoleHelper.Console.Clear();
				ConsoleHelper.ConsoleHandle.Clear();
				UIHelper.isUserLogin = false;
				(sender as System.Windows.Controls.Button).Content = (UIHelper.isUserLogin ? "登出" : "登录");
			}
			else
			{
				username = new System.Windows.Controls.TextBox();
				password = new PasswordBox
				{
					Margin = new Thickness(0.0, 8.0, 0.0, 0.0)
				};
				message = new TextBlock
				{
					Margin = new Thickness(0.0, 8.0, 0.0, 0.0)
				};
				message.SetResourceReference(System.Windows.Controls.Page.ForegroundProperty, "SystemControlErrorTextForegroundBrush");
				ControlHelper.SetHeader(username, "用户名");
				ControlHelper.SetHeader(password, "密码");
				dialog = new ContentDialog
				{
					CloseButtonText = "取消",
					PrimaryButtonText = "登录",
					DefaultButton = ContentDialogButton.Primary,
					Content = new SimpleStackPanel
					{
						Spacing = 8.0,
						Children = 
						{
							(UIElement)username,
							(UIElement)password,
							(UIElement)message
						}
					},
					Title = "登录"
				};
				ShowDialog();
			}
			async void ShowDialog(bool reset = false)
			{
				if (reset)
				{
					dialog.Hide();
					dialog = new ContentDialog
					{
						CloseButtonText = "取消",
						PrimaryButtonText = "登录",
						DefaultButton = ContentDialogButton.Primary,
						Content = new SimpleStackPanel
						{
							Spacing = 8.0,
							Children = 
							{
								(UIElement)username,
								(UIElement)password,
								(UIElement)message
							}
						},
						Title = "登录"
					};
				}
				if (await dialog.ShowAsync() == ContentDialogResult.Primary)
				{
					if (string.IsNullOrEmpty(username.Text) || string.IsNullOrEmpty(password.Password))
					{
						if (string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Password))
						{
							message.Text = "用户名不能为空。";
						}
						else if (!string.IsNullOrEmpty(username.Text) && string.IsNullOrEmpty(password.Password))
						{
							message.Text = "密码不能为空。";
						}
						else
						{
							message.Text = "请输入你的密码和用户名。";
						}
						ShowDialog();
					}
					else
					{
						(dialog.Content as SimpleStackPanel).Children.Clear();
						dialog.Content = new ProgressRing
						{
							Height = 80.0,
							Width = 80.0
						};
						dialog.DefaultButton = ContentDialogButton.Close;
						dialog.IsPrimaryButtonEnabled = false;
						dialog.PrimaryButtonText = "";
						dialog.Loaded += async delegate
						{
							Class.DefualtClass result = await User.Login(username.Text, password.Password);
							if (!result.Flag)
							{
								message.Text = result.Msg;
								ShowDialog(reset: true);
							}
							else
							{
								UIHelper.ConfigHelper.Settings.username = username.Text;
								UIHelper.ConfigHelper.Settings.password = password.Password;
								(sender as System.Windows.Controls.Button).Content = "登出";
								(sender as System.Windows.Controls.Button).ToolTip = "你已登录。";
								UIHelper.isUserLogin = true;
								dialog.Hide();
								dialog = null;
							}
						};
						await dialog.ShowAsync();
					}
				}
			}
		}

		private void UserLogin_Button_Loaded(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button ctrl = sender as System.Windows.Controls.Button;
			ctrl.Click += UserLogin_Button_Click;
			ctrl.Content = (UIHelper.isUserLogin ? "登出" : "登录");
		}

		private void Console_ComboBox_Initialized(object sender, EventArgs e)
		{
			System.Windows.Controls.ComboBox ctrl = sender as System.Windows.Controls.ComboBox;
			int selectIndex = -1;
			System.Drawing.FontFamily[] fonts = new InstalledFontCollection().Families;
			for (int i = 0; i < fonts.Length; i++)
			{
				if (!string.IsNullOrEmpty(UIHelper.FontFamily))
				{
					if (fonts[i].Name == UIHelper.FontFamily)
					{
						selectIndex = i;
					}
				}
				else if (fonts[i].Name == Console_Preview.FontFamily.ToString())
				{
					selectIndex = i;
				}
				ctrl.Items.Add(fonts[i].Name);
			}
			if (selectIndex != -1)
			{
				ctrl.SelectedIndex = selectIndex;
			}
			ctrl.SelectionChanged += delegate
			{
				UIHelper.FontFamily = ctrl.SelectedValue.ToString();
			};
		}

		private void Console_NumberBox_Initialized(object sender, EventArgs e)
		{
			NumberBox ctrl = sender as NumberBox;
			if (UIHelper.FontSize != double.NaN && UIHelper.FontSize > 0.0)
			{
				ctrl.Value = UIHelper.FontSize;
			}
			ctrl.ValueChanged += delegate
			{
				UIHelper.FontSize = Convert.ToInt32(ctrl.Text) + 1;
			};
		}

		private void Console_ScrollViewerEx_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewerEx ctrl = (ScrollViewerEx)sender;
			if (ctrl.ExtentHeight > ctrl.ActualHeight)
			{
				if (ctrl.ScrollableHeight == ctrl.ExtentHeight || ctrl.ScrollableHeight == 0.0)
				{
					MainScroll.ExcuteScroll(e);
					e.Handled = true;
				}
			}
			else
			{
				MainScroll.ExcuteScroll(e);
				e.Handled = true;
			}
		}

		private void Notifly_ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
		{
			ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
			string[] str = new ComputerInfo().OSVersion.Split(Encoding.UTF8.GetChars(Encoding.UTF8.GetBytes(".")));
			Version system = new Version(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]), Convert.ToInt32(str[2]), Convert.ToInt32(str[3]));
			if (system > new Version(10, 0, 10240, 0))
			{
				toggleSwitch.IsOn = UIHelper.ConfigHelper.Settings.DisplayToast;
				toggleSwitch.Toggled += delegate
				{
					UIHelper.ConfigHelper.Settings.DisplayToast = toggleSwitch.IsOn;
				};
			}
			else
			{
				UIHelper.ConfigHelper.Settings.DisplayToast = false;
				toggleSwitch.IsOn = false;
				toggleSwitch.IsEnabled = false;
			}
		}

		private void Mode_ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
		{
			ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
			if (UIHelper.ConfigHelper.Settings.AutoStartupMode == AutoStartupMode.Registry)
			{
				toggleSwitch.IsOn = true;
			}
			toggleSwitch.Toggled += delegate
			{
				if (toggleSwitch.IsOn)
				{
					UIHelper.ConfigHelper.Settings.AutoStartupMode = AutoStartupMode.Registry;
					Process.Start(new ProcessStartInfo
					{
						FileName = "\"" + System.Windows.Forms.Application.StartupPath + "\\OpenFrp.Service.exe\"",
						Arguments = "-install registry",
						CreateNoWindow = true
					});
				}
				else
				{
					UIHelper.ConfigHelper.Settings.AutoStartupMode = AutoStartupMode.Null;
					Process.Start(new ProcessStartInfo
					{
						FileName = "\"" + System.Windows.Forms.Application.StartupPath + "\\OpenFrp.Service.exe\"",
						Arguments = "-uninstall registry",
						CreateNoWindow = true
					});
				}
			};
		}

		private void Button_Loaded(object sender, RoutedEventArgs e)
		{
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocater = new Uri("/OpenFrp.Launcher;component/pages/setting.xaml", UriKind.Relative);
				System.Windows.Application.LoadComponent(this, resourceLocater);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				MainScroll = (ScrollViewerEx)target;
				break;
			case 2:
				((System.Windows.Controls.ComboBox)target).Loaded += Appearance_ComboBox_Loaded;
				break;
			case 3:
				((System.Windows.Controls.Button)target).Loaded += UserLogin_Button_Loaded;
				break;
			case 4:
				((ToggleSwitch)target).Loaded += Notifly_ToggleSwitch_Loaded;
				break;
			case 5:
				((ToggleSwitch)target).Loaded += Mode_ToggleSwitch_Loaded;
				break;
			case 6:
				((ScrollViewerEx)target).PreviewMouseWheel += Console_ScrollViewerEx_PreviewMouseWheel;
				break;
			case 7:
				Console_Preview = (TextBlock)target;
				break;
			case 8:
				((NumberBox)target).Initialized += Console_NumberBox_Initialized;
				break;
			case 9:
				((System.Windows.Controls.ComboBox)target).Initialized += Console_ComboBox_Initialized;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
