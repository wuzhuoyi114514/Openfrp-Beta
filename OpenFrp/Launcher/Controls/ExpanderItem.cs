using System.Windows;
using System.Windows.Controls;

namespace OpenFrp.Launcher.Controls
{
	public class ExpanderItem : ContentControl
	{
		public static readonly DependencyProperty PressableProperty = DependencyProperty.RegisterAttached("Pressable", typeof(bool), typeof(ExpanderItem), new PropertyMetadata(false));

		public bool Pressable
		{
			get
			{
				return (bool)GetValue(PressableProperty);
			}
			set
			{
				SetValue(PressableProperty, value);
			}
		}
	}
}
