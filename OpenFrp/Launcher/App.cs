using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using OpenFrp.Launcher.Model;
using OpenFrp.Service;
using OpenFrp.Service.App;

namespace OpenFrp.Launcher
{
	public class App : Application
	{
		private bool _contentLoaded;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			base.OnStartup(e);
			MessageBox.Show("在使用的你，应该是本软件的修改者，如果你不是，则立即退出.\n 按照Openfrp协议，分发此软件为违反使用条款\n 2022 Mr.Wu");
			try
			{
				Application.Current.DispatcherUnhandledException += delegate(object o, DispatcherUnhandledExceptionEventArgs w)
				{
					try
					{
						Dictionary<int, Process> allProcess = ConsoleHelper.AllProcess;
						if (allProcess.Count != 0)
						{
							foreach (KeyValuePair<int, Process> item in allProcess)
							{
								ConsoleHelper.Kill(item.Key);
							}
						}
					}
					catch
					{
					}
					UIHelper.ConfigHelper.WriteConfig();
					if (!Debugger.IsAttached)
					{
						Clipboard.SetText($"{w.Exception}");
						MessageBox.Show("\n操你妈的到底怎么用的。\n反馈链接:修改版反什么馈 \n 我们他们的已将错误复制到剪切板中,请使用Ctrl+V / 粘贴显示错误\n他妈的建议您放到txt文件中，然后他妈的发到OpenFrp 用户交流群中。");
					}
				};
				if (Process.GetProcessesByName("OpenFrp.Launcher").ToList().Count > 1)
				{
					MessageBox.Show("你他妈的已启动OpenFrp Launcher.");
					Shutdown();
				}
				if (e != null && e.Args.Length != 0 && e.Args[0] == "-nogui")
				{
					Shared.Visibility = Visibility.Hidden;
				}
			}
			catch
			{
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
				Uri resourceLocator = new Uri("/OpenFrp.Launcher;component/app.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
