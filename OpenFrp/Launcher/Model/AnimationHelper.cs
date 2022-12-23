using System;
using System.Windows;

namespace OpenFrp.Launcher.Model
{
	public class AnimationHelper
	{
		public static Duration FastDuration(int milliesecond)
		{
			return new Duration(new TimeSpan(0, 0, 0, 0, milliesecond));
		}
	}
}
