using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using OpenFrp.Launcher.Model;

namespace OpenFrp.Launcher.Controls
{
	public class Expander : System.Windows.Controls.Expander
	{
		private static bool isClosing;

		public Expander()
		{
			base.MinHeight = 65.0;
		}

		protected override void OnExpanded()
		{
			base.OnExpanded();
			if (!isClosing)
			{
				FrameworkElement elementContent = (FrameworkElement)base.Content;
				BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation
				{
					Duration = AnimationHelper.FastDuration(350),
					EasingFunction = new QuarticEase
					{
						EasingMode = EasingMode.EaseOut
					},
					From = base.MinHeight,
					To = elementContent.Height + base.MinHeight + 32.0
				});
			}
		}

		protected override async void OnCollapsed()
		{
			base.OnCollapsed();
			if (!isClosing)
			{
				isClosing = true;
				base.IsExpanded = true;
				BeginAnimation(FrameworkElement.HeightProperty, new DoubleAnimation
				{
					Duration = AnimationHelper.FastDuration(250),
					EasingFunction = new CubicEase
					{
						EasingMode = EasingMode.EaseOut
					},
					To = base.MinHeight
				});
				await Task.Delay(200);
				base.IsExpanded = false;
				isClosing = false;
			}
		}
	}
}
