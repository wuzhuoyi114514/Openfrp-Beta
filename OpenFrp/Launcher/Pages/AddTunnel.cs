using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf.Controls;
using OpenFrp.Service;
using OpenFrp.Service.Api;

namespace OpenFrp.Launcher.Pages
{
	public class AddTunnel : ModernWpf.Controls.Page, IComponentConnector
	{
		internal ModernWpf.Controls.ListView GridView_ChinaMain;

		internal ModernWpf.Controls.ListView GridView_ChinaTWHK;

		internal ModernWpf.Controls.ListView GridView_WWW;

		internal TextBox Main_Config_Name;

		internal ComboBox Config_ProxyType;

		internal TextBox Config_ProxyName;

		internal TextBox Config_LocalIp;

		internal NumberBox Config_LocalPort;

		internal NumberBox Config_RemotePort;

		internal TextBox Config_CNameBinding;

		internal ToggleSwitch Config_DataEncrypt;

		internal ToggleSwitch Config_GZIP;

		internal TextBox Config_URLRoted;

		internal TextBox Config_HOSTRed;

		internal TextBox Config_RequestForm;

		internal TextBox Config_ReqPwd;

		private bool _contentLoaded;

		public AddTunnel()
		{
			InitializeComponent();
			base.Loaded += AddTunnel_Loaded;
		}

		private async void AddTunnel_Loaded(object sender, RoutedEventArgs e)
		{
			base.Background = new SolidColorBrush
			{
				Color = Colors.Transparent
			};
			MouseButtonEventHandler action = delegate(object o, MouseButtonEventArgs w)
			{
				Main_Config_Name.Text = (o as ModernWpf.Controls.ListViewItem).Content.ToString();
				Main_Config_Name.Tag = (o as ModernWpf.Controls.ListViewItem).Tag.ToString();
			};
			foreach (Class.ProxiesClass._Data.info item in (await User.GetProxies()).Data.List)
			{
				ModernWpf.Controls.ListViewItem view1 = new ModernWpf.Controls.ListViewItem
				{
					Content = item.description,
					Tag = item.id
				};
				view1.MouseDoubleClick += action;
				if (item.status != 200)
				{
					view1.IsEnabled = false;
					view1.SetResourceReference(System.Windows.Controls.Page.ForegroundProperty, "SystemControlErrorTextForegroundBrush");
				}
				switch (item.classify)
				{
				case 1:
					GridView_ChinaMain.Items.Add(view1);
					break;
				case 2:
					GridView_ChinaTWHK.Items.Add(view1);
					break;
				default:
					GridView_WWW.Items.Add(view1);
					break;
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			UIHelper.Frame.GoBack();
		}

		private void ScrollViewerEx_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			(sender as ScrollViewerEx).ExcuteScroll(e);
		}

		private async void Button_Click_1(object sender, RoutedEventArgs e)
		{
			string TunnelName = "";
			int TunnelId = Convert.ToInt32(Main_Config_Name.Tag);
			if (string.IsNullOrEmpty(Config_ProxyName.Text))
			{
				TunnelName = "Of202" + new Random().Next(0, 1000000);
			}
			else
			{
				TunnelName = Config_ProxyName.Text;
			}
			if (string.IsNullOrEmpty(Config_LocalIp.Text))
			{
				Config_LocalIp.Text = "127.0.0.1";
			}
			if (string.IsNullOrEmpty(Config_LocalPort.Text))
			{
				Config_LocalPort.Text = "25565";
			}
			if (string.IsNullOrEmpty(Config_RemotePort.Text))
			{
				Config_RemotePort.Value = new Random().Next(10000, 65535);
			}
			ContentDialog loading = new ContentDialog
			{
				Title = "创建隧道...",
				Content = new ProgressRing
				{
					Width = 100.0,
					Height = 100.0
				}
			};
			loading.Loaded += async delegate
			{
				Class.DefualtClass result = await User.CreateTunnel(new Class.CreateTunnelContent
				{
					node_id = TunnelId,
					name = TunnelName,
					type = (Config_ProxyType.SelectedValue as ComboBoxItem).Content.ToString().ToLower(),
					local_addr = Config_LocalIp.Text,
					local_port = Config_LocalPort.Text,
					remote_port = Convert.ToInt32(Config_RemotePort.Text),
					domain_bind = Config_CNameBinding.Text,
					dataEncrypt = Config_DataEncrypt.IsOn.ToString(),
					dataGzip = Config_GZIP.IsOn.ToString(),
					url_route = Config_URLRoted.Text,
					host_rewrite = Config_HOSTRed.Text,
					request_from = Config_RequestForm.Text,
					request_pass = Config_ReqPwd.Text
				});
				loading.Hide();
				if (!result.Flag)
				{
					await new ContentDialog
					{
						Content = result.Msg,
						CloseButtonText = "确定",
						Title = "创建隧道",
						DefaultButton = ContentDialogButton.Close
					}.ShowAsync();
				}
				else
				{
					(UIHelper.NavigationView.MenuItems[1] as NavigationViewItem).IsSelected = true;
					UIHelper.Frame.GoBack();
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
				Uri resourceLocater = new Uri("/OpenFrp.Launcher;component/pages/addtunnel.xaml", UriKind.Relative);
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
				((ScrollViewerEx)target).PreviewMouseWheel += ScrollViewerEx_PreviewMouseWheel;
				break;
			case 2:
				GridView_ChinaMain = (ModernWpf.Controls.ListView)target;
				break;
			case 3:
				GridView_ChinaTWHK = (ModernWpf.Controls.ListView)target;
				break;
			case 4:
				GridView_WWW = (ModernWpf.Controls.ListView)target;
				break;
			case 5:
				Main_Config_Name = (TextBox)target;
				break;
			case 6:
				Config_ProxyType = (ComboBox)target;
				break;
			case 7:
				Config_ProxyName = (TextBox)target;
				break;
			case 8:
				Config_LocalIp = (TextBox)target;
				break;
			case 9:
				Config_LocalPort = (NumberBox)target;
				break;
			case 10:
				Config_RemotePort = (NumberBox)target;
				break;
			case 11:
				Config_CNameBinding = (TextBox)target;
				break;
			case 12:
				Config_DataEncrypt = (ToggleSwitch)target;
				break;
			case 13:
				Config_GZIP = (ToggleSwitch)target;
				break;
			case 14:
				Config_URLRoted = (TextBox)target;
				break;
			case 15:
				Config_HOSTRed = (TextBox)target;
				break;
			case 16:
				Config_RequestForm = (TextBox)target;
				break;
			case 17:
				Config_ReqPwd = (TextBox)target;
				break;
			case 18:
				((Button)target).Click += Button_Click;
				break;
			case 19:
				((Button)target).Click += Button_Click_1;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
