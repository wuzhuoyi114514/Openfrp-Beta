using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf.Controls;
using ModernWpf.Toolkit.UI.Controls;
using OpenFrp.Service.Api;

namespace OpenFrp.Launcher.Pages
{
	public class Home : ModernWpf.Controls.Page, IComponentConnector
	{
		internal Grid _wdWda4adw;

		internal Grid _awdawdk_Main_Scroll_rot;

		internal ScrollViewerEx _awdawdk_Main_Scroll;

		internal Grid Main_News_1;

		internal Grid Main_UserInfo;

		internal ModernWpf.Controls.ListView _035addwiuahdui_UserInfo_List;

		internal Grid Main_UserInfo_Loading;

		internal Grid Main_Markdown;

		internal MarkdownTextBlock Main_Markdown_content;

		internal Grid Main_Markdown_Loading;

		internal ModernWpf.Controls.ListView _app_serverStats_listview;

		internal ProgressRing _app_serverStats_Progress;

		private bool _contentLoaded;

		public Home()
		{
			InitializeComponent();
			base.Loaded += Home_Loaded;
		}

		private async void Home_Loaded(object sender, RoutedEventArgs e)
		{
			base.Background = new SolidColorBrush
			{
				Color = Colors.Transparent
			};
			Main_Markdown.Visibility = Visibility.Hidden;
			Main_Markdown_Loading.Visibility = Visibility.Visible;
			Class.DefualtClass broadCast = await User.GetBroadcast();
			await LoadUserInfo();
			await LoadServerStats();
			if (broadCast.Flag)
			{
				Main_Markdown_content.Text = broadCast.Data;
			}
			else
			{
				Main_Markdown.IsEnabled = false;
			}
			Main_Markdown_Loading.Visibility = Visibility.Hidden;
			Main_Markdown.Visibility = Visibility.Visible;
		}

		private async Task LoadUserInfo()
		{
			Main_UserInfo.Visibility = Visibility.Hidden;
			Main_UserInfo_Loading.Visibility = Visibility.Visible;
			Class.UserinfoClass userinfo = await User.GetUserinfo();
			if (userinfo.Flag)
			{
				_035addwiuahdui_UserInfo_List.Items.Clear();
				Action<Symbol, string> addView = delegate(Symbol symbol, string str)
				{
					_035addwiuahdui_UserInfo_List.Items.Add(new Grid
					{
						Height = 50.0,
						Children = { (UIElement)new SimpleStackPanel
						{
							HorizontalAlignment = HorizontalAlignment.Left,
							VerticalAlignment = VerticalAlignment.Center,
							Orientation = Orientation.Horizontal,
							Spacing = 8.0,
							Children = 
							{
								(UIElement)new SymbolIcon
								{
									Symbol = symbol
								},
								(UIElement)new TextBlock
								{
									Padding = new Thickness(0.0, 1.0, 0.0, 0.0),
									Text = str,
									FontSize = 14.0,
									TextTrimming = TextTrimming.WordEllipsis
								}
							}
						} }
					});
				};
				addView(Symbol.Contact, "昵称: " + userinfo.Data.username);
				addView(Symbol.Mail, "邮箱: " + userinfo.Data.email);
				addView(Symbol.AllApps, $"最大隧道数: {userinfo.Data.proxies}");
				addView(Symbol.Sync, $"可用流量: {Math.Round((double)userinfo.Data.traffic / 1024.0, 2)} GB");
				addView(Symbol.ContactInfo, "实名认证: " + (userinfo.Data.realname ? "已实名" : "未实名"));
			}
			else
			{
				Main_UserInfo.IsEnabled = false;
			}
			Main_UserInfo_Loading.Visibility = Visibility.Hidden;
			Main_UserInfo.Visibility = Visibility.Visible;
		}

		private async Task LoadServerStats()
		{
			_app_serverStats_listview.Visibility = Visibility.Collapsed;
			Class.ServerStats qresult = await User.GetServerStats();
			_app_serverStats_Progress.Visibility = Visibility.Hidden;
			if (!qresult.Flag)
			{
				return;
			}
			_app_serverStats_listview.Visibility = Visibility.Visible;
			foreach (Class.ServerStats._Data_index item in qresult.data)
			{
				_app_serverStats_listview.Items.Add(new Grid
				{
					Height = 75.0,
					VerticalAlignment = VerticalAlignment.Center,
					Children = { (UIElement)new SimpleStackPanel
					{
						Spacing = 6.0,
						Margin = new Thickness(4.0, 0.0, 0.0, 0.0),
						VerticalAlignment = VerticalAlignment.Center,
						Children = 
						{
							(UIElement)new TextBlock(new Run(item.name))
							{
								Style = (Application.Current.Resources["SubtitleTextBlockStyle"] as Style)
							},
							(UIElement)new TextBlock
							{
								FontSize = 12.0,
								Text = $"已连接用户数量: {item.client_counts}" + $" 入口流量: {Math.Round((double)item.total_traffic_in / 1024.0 / 1024.0, 2)}Mib" + $" 出口流量: {Math.Round((double)item.total_traffic_out / 1024.0 / 1024.0, 2)}Mib"
							}
						}
					} }
				});
			}
		}

		private void Main_Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Grid ctrl = (Grid)sender;
			ctrl.RowDefinitions[0].Height = new GridLength(Main_News_1.ActualWidth / 16.0 * 9.0);
			if (ctrl.Width < 730.0)
			{
				ctrl.Height = 700.0;
			}
			else
			{
				ctrl.Height = double.NaN;
			}
		}

		private void Main_ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewerEx parent = (sender as FrameworkElement).Parent as ScrollViewerEx;
			parent.ExcuteScroll(e);
		}

		private async void UserC_AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Content = new ProgressRing
				{
					Width = 100.0,
					Height = 100.0
				},
				Title = "登录",
				PrimaryButtonText = "我知道啦",
				IsPrimaryButtonEnabled = false
			};
			dialog.Loaded += async delegate
			{
				Class.DefualtClass result = await User.Signin();
				dialog.Hide();
				await new ContentDialog
				{
					CloseButtonText = "我知道啦",
					DefaultButton = ContentDialogButton.Close,
					Title = "登录",
					Content = new Run(result.Data)
				}.ShowAsync();
				await LoadUserInfo();
				dialog = null;
			};
			await dialog.ShowAsync();
		}

		private void UserC_AppBarButton_Click_1(object sender, RoutedEventArgs e)
		{
			Process.Start("https://www.openfrp.net");
		}

		private void Main_Markdown_content_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			Process.Start(e.Link);
		}

		private void Main_Markdown_content_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewerEx ctrl = (sender as FrameworkElement).Parent as ScrollViewerEx;
			if (ctrl.ExtentHeight > ctrl.ActualHeight && ((e.Delta > 0 && ctrl.ContentVerticalOffset == 0.0) || (e.Delta < 0 && ctrl.ContentVerticalOffset == ctrl.ScrollableHeight)))
			{
				e.Handled = true;
				_awdawdk_Main_Scroll.ExcuteScroll(e);
			}
			else
			{
				e.Handled = true;
				ctrl.ExcuteScroll(e);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocater = new Uri("/OpenFrp.Launcher;component/pages/home.xaml", UriKind.Relative);
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
				_wdWda4adw = (Grid)target;
				break;
			case 2:
				_awdawdk_Main_Scroll_rot = (Grid)target;
				break;
			case 3:
				_awdawdk_Main_Scroll = (ScrollViewerEx)target;
				break;
			case 4:
				((Grid)target).SizeChanged += Main_Grid_SizeChanged;
				break;
			case 5:
				Main_News_1 = (Grid)target;
				break;
			case 6:
				Main_UserInfo = (Grid)target;
				break;
			case 7:
				_035addwiuahdui_UserInfo_List = (ModernWpf.Controls.ListView)target;
				_035addwiuahdui_UserInfo_List.PreviewMouseWheel += Main_ListView_PreviewMouseWheel;
				break;
			case 8:
				((AppBarButton)target).Click += UserC_AppBarButton_Click;
				break;
			case 9:
				((AppBarButton)target).Click += UserC_AppBarButton_Click_1;
				break;
			case 10:
				Main_UserInfo_Loading = (Grid)target;
				break;
			case 11:
				Main_Markdown = (Grid)target;
				break;
			case 12:
				Main_Markdown_content = (MarkdownTextBlock)target;
				Main_Markdown_content.LinkClicked += Main_Markdown_content_LinkClicked;
				Main_Markdown_content.PreviewMouseWheel += Main_Markdown_content_PreviewMouseWheel;
				break;
			case 13:
				Main_Markdown_Loading = (Grid)target;
				break;
			case 14:
				_app_serverStats_listview = (ModernWpf.Controls.ListView)target;
				break;
			case 15:
				_app_serverStats_Progress = (ProgressRing)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
