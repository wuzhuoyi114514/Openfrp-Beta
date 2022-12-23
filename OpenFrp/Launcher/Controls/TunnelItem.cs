using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using OpenFrp.Launcher.Pages;
using OpenFrp.Service;
using OpenFrp.Service.Api;
using OpenFrp.Service.App;

namespace OpenFrp.Launcher.Controls
{
	public class TunnelItem : UserControl, IComponentConnector
	{
		private string TunnelUrl = "";

		private Class.UserTunnelsClass._Data.info Tunnel_info;

		internal TextBlock Ctrl_Title;

		internal TextBlock Ctrl_Subtitle;

		internal ToggleSwitch Card_ToggleSwitch;

		private bool _contentLoaded;

		public TunnelItem(Class.UserTunnelsClass._Data.info tunnelInfo, Class.ProxiesClass proxies)
		{
			InitializeComponent();
			Tunnel_info = tunnelInfo;
			Ctrl_Title.Text = tunnelInfo.proxyName;
			Ctrl_Subtitle.Text = tunnelInfo.proxyType.ToUpper() + " 穿透 " + tunnelInfo.localIp + ":" + tunnelInfo.localPort + "\n" + tunnelInfo.friendlyNode + $"\n隧道 ID : {tunnelInfo.id}";
			foreach (Class.ProxiesClass._Data.info proxy in proxies.Data.List)
			{
				if (proxy.name == tunnelInfo.friendlyNode)
				{
					TunnelUrl = proxy.ip + ":" + tunnelInfo.remotePort;
					break;
				}
			}
			if (ConsoleHelper.AllProcess.ContainsKey(tunnelInfo.id))
			{
				Card_ToggleSwitch.IsOn = true;
			}
			if (UIHelper.ConfigHelper.Settings.Asid.Count == 0)
			{
				return;
			}
			foreach (int item in UIHelper.ConfigHelper.Settings.Asid)
			{
				if (item == tunnelInfo.id)
				{
					if (tunnelInfo.status)
					{
						Card_ToggleSwitch.IsOn = true;
					}
					else
					{
						ConsoleHelper.Kill(tunnelInfo.id);
					}
					break;
				}
			}
		}

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
			if (!toggleSwitch.IsOn)
			{
				ConsoleHelper.Kill(Tunnel_info.id);
				UIHelper.ConfigHelper.Settings.Asid.Remove(Tunnel_info.id);
				return;
			}
			ConsoleHelper.Launch(Tunnel_info.id, TunnelUrl);
			if (UIHelper.ConfigHelper.Settings.Asid.Contains(Tunnel_info.id))
			{
				UIHelper.ConfigHelper.Settings.Asid.Remove(Tunnel_info.id);
			}
			UIHelper.ConfigHelper.Settings.Asid.Add(Tunnel_info.id);
			ConsoleHelper.AllProcess.TryGetValue(Tunnel_info.id, out var process);
			process.Exited += delegate
			{
				ConsoleHelper.AllProcess.Remove(Tunnel_info.id);
				ConsoleHelper.ConsoleHandle.Remove(Tunnel_info.id);
				UIHelper.ConfigHelper.Settings.Asid.Remove(Tunnel_info.id);
				if (Application.Current != null)
				{
					Application.Current.Dispatcher.Invoke(() => toggleSwitch.IsOn = false);
				}
			};
		}

		private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(TunnelUrl);
		}

		private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
		{
			UIHelper.Frame.Navigate(typeof(OpenFrp.Launcher.Pages.Console));
		}

		private async void AppBarButton_Click_3(object sender, RoutedEventArgs e)
		{
			TextBox localhost = new TextBox
			{
				Text = Tunnel_info.localIp
			};
			NumberBox localPort = new NumberBox
			{
				Text = Tunnel_info.localPort,
				Minimum = 1.0,
				Maximum = 65535.0,
				SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline
			};
			ControlHelper.SetHeader(localhost, "本地链接");
			ControlHelper.SetHeader(localPort, "本地端口");
			if (await new ContentDialog
			{
				Title = "编辑隧道...",
				Content = new SimpleStackPanel
				{
					Spacing = 8.0,
					Children = 
					{
						(UIElement)localhost,
						(UIElement)localPort
					}
				},
				PrimaryButtonText = "更改",
				CloseButtonText = "取消",
				DefaultButton = ContentDialogButton.Primary
			}.ShowAsync() != ContentDialogResult.Primary)
			{
				return;
			}
			ContentDialog loading = new ContentDialog
			{
				Title = "编辑隧道...",
				Content = new ProgressRing
				{
					Width = 100.0,
					Height = 100.0
				},
				PrimaryButtonText = "取消",
				IsPrimaryButtonEnabled = false
			};
			loading.Loaded += async delegate
			{
				Class.DefualtClass result = await User.EditTunnel(new Class.EditTunnelContent(Tunnel_info.id, localhost.Text, Convert.ToInt32(localPort.Text)));
				loading.Hide();
				if (!result.Flag)
				{
					await new ContentDialog
					{
						Title = "编辑隧道...",
						Content = result.Msg,
						CloseButtonText = "确定",
						DefaultButton = ContentDialogButton.Primary
					}.ShowAsync();
				}
				else
				{
					await UIHelper.RefreshTunnel();
				}
			};
			await loading.ShowAsync();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocater = new Uri("/OpenFrp.Launcher;component/controls/tunnelitem.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocater);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				Ctrl_Title = (TextBlock)target;
				break;
			case 2:
				Ctrl_Subtitle = (TextBlock)target;
				break;
			case 3:
				((AppBarButton)target).Click += AppBarButton_Click_1;
				break;
			case 4:
				((AppBarButton)target).Click += AppBarButton_Click_2;
				break;
			case 5:
				((AppBarButton)target).Click += AppBarButton_Click_3;
				break;
			case 6:
				Card_ToggleSwitch = (ToggleSwitch)target;
				Card_ToggleSwitch.Toggled += ToggleSwitch_Toggled;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
