using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using ModernWpf.Controls;
using OpenFrp.Launcher.Model;
using OpenFrp.Launcher.Pages;
using OpenFrp.Service;
using OpenFrp.Service.Api;
using OpenFrp.Service.App;

namespace OpenFrp.Launcher
{
	public class MainWindow : Window, IComponentConnector
	{
		private static bool isClosing;

		internal Border OfApp_TitleBar;

		internal NavigationView OfApp_NavgationView;

		private bool _contentLoaded;

		private ConfigHelper._Settings Settings
		{
			get
			{
				return UIHelper.ConfigHelper.Settings;
			}
			set
			{
				UIHelper.ConfigHelper.Settings = value;
			}
		}

		public MainWindow()
		{
			base.Initialized += MainWindow_Initialized;
			InitializeComponent();
			UIHelper.NavigationView = OfApp_NavgationView;
			UIHelper.Frame = OfApp_NavgationView.Content as ModernWpf.Controls.Frame;
			System.Windows.Application.Current.Exit += delegate
			{
				isClosing = true;
				Settings.AppElementTheme = UIHelper.AppElementTheme;
				Settings.FontFamily = UIHelper.FontFamily;
				Settings.FontSize = UIHelper.FontSize;
				UIHelper.ConfigHelper.WriteConfig();
				ToastNotificationManagerCompat.History.Clear();
				foreach (Process current in ConsoleHelper.AllProcess.Values)
				{
					current.Kill();
				}
			};
		}

		private async void MainWindow_Initialized(object sender, EventArgs e)
		{
			OfApp_NavgationView.DisplayModeChanged += delegate(NavigationView o, NavigationViewDisplayModeChangedEventArgs k)
			{
				if ((o.DisplayMode == NavigationViewDisplayMode.Expanded && o.IsPaneOpen) || o.DisplayMode == NavigationViewDisplayMode.Compact)
				{
					ChangeTitleBar(o.CompactPaneLength * 1.5);
				}
				else if (o.DisplayMode == NavigationViewDisplayMode.Minimal)
				{
					ChangeTitleBar(o.CompactPaneLength * 2.0);
				}
			};
			OfApp_NavgationView.PaneOpening += delegate(NavigationView o, object k)
			{
				if (o.DisplayMode == NavigationViewDisplayMode.Expanded)
				{
					ChangeTitleBar(o.CompactPaneLength);
				}
			};
			OfApp_NavgationView.PaneClosing += delegate(NavigationView o, NavigationViewPaneClosingEventArgs k)
			{
				if (o.DisplayMode == NavigationViewDisplayMode.Expanded)
				{
					ChangeTitleBar(o.CompactPaneLength * 1.5);
				}
			};
			OfApp_NavgationView.ItemInvoked += delegate(NavigationView o, NavigationViewItemInvokedEventArgs k)
			{
				ModernWpf.Controls.Frame frame = (ModernWpf.Controls.Frame)o.Content;
				object tag = (o.SelectedItem as NavigationViewItem).Tag;
				if (tag != null)
				{
					string text = tag as string;
					if (text != null)
					{
						if (1 == 0)
						{
						}
						Type type = text switch
						{
							"Home" => typeof(Home), 
							"Setting" => typeof(Setting), 
							"Tunnels" => typeof(Tunnels), 
							"Console" => typeof(OpenFrp.Launcher.Pages.Console), 
							_ => typeof(Setting), 
						};
						if (1 == 0)
						{
						}
						Type type2 = type;
						if (frame.Content == null || !(frame.Content.GetType() == type2))
						{
							frame.Navigate(type2);
						}
					}
				}
			};
			ToastNotificationManagerCompat.OnActivated += delegate(ToastNotificationActivatedEventArgsCompat o)
			{
				ToastArguments _args = ToastArguments.Parse(o.Argument);
				ToastNotificationManagerCompat.History.Clear();
				if (_args["action"] == "copy-url")
				{
					System.Windows.Application.Current.Dispatcher.Invoke(delegate
					{
						System.Windows.Clipboard.SetText(_args["url"]);
					});
				}
			};
			UIHelper.MainWindow = this;
			try
			{
				UIHelper.ConfigHelper = await ConfigHelper.ReadConfig();
			}
			catch
			{
				UIHelper.ConfigHelper = new ConfigHelper();
			}
			UIHelper.AppElementTheme = Settings.AppElementTheme;
			UIHelper.FontFamily = Settings.FontFamily;
			UIHelper.FontSize = Settings.FontSize;
			TaskbarIcon taskbarIcon = new TaskbarIcon
			{
				ToolTipText = "OpenFrp Launcher正在运行,可在托盘区域找到我。",
				ContextMenu = new System.Windows.Controls.ContextMenu
				{
					Items = 
					{
						(object)new System.Windows.Controls.MenuItem
						{
							Header = "显示"
						},
						(object)new System.Windows.Controls.MenuItem
						{
							Header = "退出"
						}
					}
				},
				Icon = new Icon(System.Windows.Forms.Application.StartupPath + "\\OpenFrp.ico")
			};
			base.Closing += delegate(object o, CancelEventArgs w)
			{
				if (!isClosing)
				{
					taskbarIcon.ShowBalloonTip("OpenFrp Launcher", "我到任务栏了啦", BalloonIcon.None);
				}
				w.Cancel = true;
				base.Visibility = Visibility.Hidden;
			};
			((System.Windows.Controls.MenuItem)taskbarIcon.ContextMenu.Items[0]).Click += delegate
			{
				base.Visibility = Visibility.Visible;
				if (Shared.Visibility != Visibility.Hidden)
				{
					Activate();
				}
				else
				{
					Shared.Visibility = Visibility.Visible;
				}
			};
			((System.Windows.Controls.MenuItem)taskbarIcon.ContextMenu.Items[1]).Click += delegate
			{
				System.Windows.Application.Current.Shutdown();
			};
			taskbarIcon.TrayLeftMouseUp += delegate
			{
				base.Visibility = Visibility.Visible;
				if (Shared.Visibility != Visibility.Hidden)
				{
					Activate();
				}
				else
				{
					Shared.Visibility = Visibility.Visible;
				}
			};
			if ((await User.Login(UIHelper.ConfigHelper.Settings.username, UIHelper.ConfigHelper.Settings.password)).Flag)
			{
				UIHelper.isUserLogin = true;
				await User.GetUserinfo();
			}
			if (Settings.Asid.Count != 0)
			{
				Class.UserTunnelsClass tunnels = await User.GetUserTunnels();
				Class.ProxiesClass proxies = await User.GetProxies();
				foreach (Class.UserTunnelsClass._Data.info tunnel in tunnels.Data.List)
				{
					if (!tunnel.status)
					{
						continue;
					}
					foreach (Class.ProxiesClass._Data.info proxy in proxies.Data.List)
					{
						if (proxy.name == tunnel.friendlyNode)
						{
							ConsoleHelper.Launch(tunnel.id, proxy.ip);
							break;
						}
					}
				}
			}
			Class.UpdateClass wresult = await User.CheckUpdate();
			if (wresult != null && (wresult.version != UIHelper.LauncherVersion || wresult.type != UIHelper.LauncherType))
			{
				ContentDialog contentDialog = new ContentDialog();
				contentDialog.Title = "发现新版本";
				contentDialog.Content = "版本类型: " + wresult.type + " \n 版本号: " + wresult.version + " \n 请前往QQ群下载。";
				contentDialog.PrimaryButtonText = "下载";
				contentDialog.CloseButtonText = "确定";
				contentDialog.DefaultButton = ContentDialogButton.Primary;
				await contentDialog.ShowAsync();
			}
		}

		private void ChangeTitleBar(double left)
		{
			Thickness current = OfApp_TitleBar.Margin;
			OfApp_TitleBar.BeginAnimation(FrameworkElement.MarginProperty, new ThicknessAnimation
			{
				EasingFunction = new CircleEase
				{
					EasingMode = EasingMode.EaseOut
				},
				To = new Thickness(left, current.Top, current.Right, current.Bottom),
				Duration = AnimationHelper.FastDuration(200)
			});
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocater = new Uri("/OpenFrp.Launcher;component/mainwindow.xaml", UriKind.Relative);
				System.Windows.Application.LoadComponent(this, resourceLocater);
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
				OfApp_TitleBar = (Border)target;
				break;
			case 2:
				OfApp_NavgationView = (NavigationView)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
