using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using ModernWpf.Controls;
using OpenFrp.Service;
using OpenFrp.Service.App;

namespace OpenFrp.Launcher.Pages
{
	public class Console : ModernWpf.Controls.Page, IComponentConnector
	{
		internal System.Windows.Controls.ComboBox ConsoleList;

		internal System.Windows.Controls.TextBox AppConsoleContent;

		private bool _contentLoaded;

		public Console()
		{
			InitializeComponent();
			base.Loaded += Console_Loaded;
		}

		private void Console_Loaded(object sender, RoutedEventArgs e)
		{
			base.Background = new SolidColorBrush
			{
				Color = Colors.Transparent
			};
			if (ConsoleHelper.AllProcess.Keys.Count > 0)
			{
				foreach (int name in ConsoleHelper.AllProcess.Keys)
				{
					ConsoleList.Items.Add(new ComboBoxItem
					{
						Content = $"隧道 ID : {name}",
						Tag = name
					});
				}
			}
			if (UIHelper.FontSize != 0.0)
			{
				AppConsoleContent.FontSize = UIHelper.FontSize;
			}
			if (UIHelper.FontFamily != null)
			{
				AppConsoleContent.FontFamily = new FontFamily(UIHelper.FontFamily);
			}
		}

		private void AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			if (ConsoleList.SelectedValue == null)
			{
				return;
			}
			ComboBoxItem item = ConsoleList.SelectedValue as ComboBoxItem;
			if (item == null)
			{
				return;
			}
			ConsoleList.IsDropDownOpen = false;
			AppConsoleContent.Text = string.Empty;
			int id = (int)item.Tag;
			ConsoleHelper.ConsoleHandle.TryGetValue(id, out var value);
			string[] strings = value.Item2.ToArray();
			string[] array = strings;
			foreach (string s in array)
			{
				AppConsoleContent.Text += s;
				System.Windows.Forms.Application.DoEvents();
			}
			value.Item1 = null;
			ref DataReceivedEventHandler item2 = ref value.Item1;
			item2 = (DataReceivedEventHandler)Delegate.Combine(item2, (DataReceivedEventHandler)delegate(object o, DataReceivedEventArgs w)
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate
				{
					System.Windows.Controls.TextBox appConsoleContent = AppConsoleContent;
					appConsoleContent.Text = appConsoleContent.Text + w.Data + "\n";
				});
				System.Windows.Forms.Application.DoEvents();
			});
			ConsoleHelper.ConsoleHandle[id] = (value.Item1, value.Item2);
		}

		private void ConsoleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Windows.Controls.ComboBox comboBox = (System.Windows.Controls.ComboBox)sender;
			ComboBoxItem item = comboBox.SelectedValue as ComboBoxItem;
			if (item == null)
			{
				return;
			}
			AppConsoleContent.Text = string.Empty;
			int id = (int)item.Tag;
			ConsoleHelper.ConsoleHandle.TryGetValue(id, out var value);
			string[] strings = value.Item2.ToArray();
			string[] array = strings;
			foreach (string s in array)
			{
				AppConsoleContent.Text += s;
				System.Windows.Forms.Application.DoEvents();
			}
			ref DataReceivedEventHandler item2 = ref value.Item1;
			item2 = (DataReceivedEventHandler)Delegate.Combine(item2, (DataReceivedEventHandler)delegate(object o, DataReceivedEventArgs w)
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate
				{
					System.Windows.Controls.TextBox appConsoleContent = AppConsoleContent;
					appConsoleContent.Text = appConsoleContent.Text + w.Data + "\n";
				});
				System.Windows.Forms.Application.DoEvents();
			});
			ConsoleHelper.ConsoleHandle[id] = (value.Item1, value.Item2);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "6.0.9.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocater = new Uri("/OpenFrp.Launcher;component/pages/console.xaml", UriKind.Relative);
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
				((AppBarButton)target).Click += AppBarButton_Click;
				break;
			case 2:
				ConsoleList = (System.Windows.Controls.ComboBox)target;
				ConsoleList.SelectionChanged += ConsoleList_SelectionChanged;
				break;
			case 3:
				AppConsoleContent = (System.Windows.Controls.TextBox)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
