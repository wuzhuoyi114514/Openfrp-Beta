using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf.Controls;
using OpenFrp.Launcher.Controls;
using OpenFrp.Service;
using OpenFrp.Service.Api;
using OpenFrp.Service.App;

namespace OpenFrp.Launcher.Pages
{
	public class Tunnels : ModernWpf.Controls.Page, IComponentConnector
	{
		internal ModernWpf.Controls.GridView User_Tunnels_ListView;

		internal Grid User_Tunnels_ListView_Loading;

		private bool _contentLoaded;

		public Tunnels()
		{
			base.Loaded += Tunnels_Loaded;
			InitializeComponent();
			UIHelper.RefreshTunnel = LoadTunnels;
		}

		private async void Tunnels_Loaded(object sender, RoutedEventArgs e)
		{
			base.Background = new SolidColorBrush
			{
				Color = Colors.Transparent
			};
			await LoadTunnels();
		}

		private async Task LoadTunnels()
		{
			User_Tunnels_ListView.Items.Clear();
			User_Tunnels_ListView.Visibility = Visibility.Hidden;
			User_Tunnels_ListView_Loading.Visibility = Visibility.Visible;
			User_Tunnels_ListView.IsEnabled = false;
			Class.UserTunnelsClass userTunnels = await User.GetUserTunnels();
			Class.ProxiesClass proxies = await User.GetProxies();
			await User.GetUserinfo();
			if (userTunnels.Flag)
			{
				foreach (Class.UserTunnelsClass._Data.info item in userTunnels.Data.List)
				{
					TunnelItem ctrl = new TunnelItem(item, proxies)
					{
						Tag = item.id
					};
					if (!item.status)
					{
						ctrl.IsEnabled = false;
					}
					User_Tunnels_ListView.Items.Add(ctrl);
				}
				User_Tunnels_ListView.IsEnabled = true;
			}
			else
			{
				User_Tunnels_ListView.IsEnabled = false;
			}
			User_Tunnels_ListView.Visibility = Visibility.Visible;
			User_Tunnels_ListView_Loading.Visibility = Visibility.Hidden;
		}

		private async void Remove_AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			if (await new ContentDialog
			{
				Title = "移除隧道",
				PrimaryButtonText = "确定",
				CloseButtonText = "取消",
				Content = "真的要移除此隧道吗？",
				DefaultButton = ContentDialogButton.Primary
			}.ShowAsync() != ContentDialogResult.Primary)
			{
				return;
			}
			int id2 = Convert.ToInt32((User_Tunnels_ListView.SelectedValue as TunnelItem).Tag);
			Class.DefualtClass result = await User.RemoveTunnel(id2);
			if (!result.Flag)
			{
				await new ContentDialog
				{
					Title = "移除隧道",
					Content = result.Msg,
					CloseButtonText = "确定",
					DefaultButton = ContentDialogButton.Close
				}.ShowAsync();
			}
			else
			{
				if (ConsoleHelper.AllProcess.ContainsKey(id2))
				{
					ConsoleHelper.Kill(id2);
				}
				User_Tunnels_ListView.Items.RemoveAt(User_Tunnels_ListView.SelectedIndex);
			}
		}

		private async void Refresh_AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			await LoadTunnels();
		}

		private void Add_AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			UIHelper.NavigationView.SelectedItem = null;
			UIHelper.Frame.Navigate(typeof(AddTunnel));
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocater = new Uri("/OpenFrp.Launcher;component/pages/tunnels.xaml", UriKind.Relative);
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
				((AppBarButton)target).Click += Refresh_AppBarButton_Click;
				break;
			case 2:
				((AppBarButton)target).Click += Add_AppBarButton_Click;
				break;
			case 3:
				((AppBarButton)target).Click += Remove_AppBarButton_Click;
				break;
			case 4:
				User_Tunnels_ListView = (ModernWpf.Controls.GridView)target;
				break;
			case 5:
				User_Tunnels_ListView_Loading = (Grid)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
