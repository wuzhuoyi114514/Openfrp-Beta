using System;
using System.Globalization;
using System.Windows.Data;

namespace OpenFrp.Launcher.Converter
{
	public class LessConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = parameter as string;
			if (str != null)
			{
				double Than = System.Convert.ToInt32(str);
				double d = default(double);
				int num;
				if (value is double)
				{
					d = (double)value;
					num = 1;
				}
				else
				{
					num = 0;
				}
				if (num != 0)
				{
					return d < Than;
				}
				float f = default(float);
				int num2;
				if (value is float)
				{
					f = (float)value;
					num2 = 1;
				}
				else
				{
					num2 = 0;
				}
				if (num2 != 0)
				{
					return (double)f < Than;
				}
				ulong ul = default(ulong);
				int num3;
				if (value is ulong)
				{
					ul = (ulong)value;
					num3 = 1;
				}
				else
				{
					num3 = 0;
				}
				if (num3 != 0)
				{
					return (double)ul < Than;
				}
				long j = default(long);
				int num4;
				if (value is long)
				{
					j = (long)value;
					num4 = 1;
				}
				else
				{
					num4 = 0;
				}
				if (num4 != 0)
				{
					return (double)j < Than;
				}
				uint ui = default(uint);
				int num5;
				if (value is uint)
				{
					ui = (uint)value;
					num5 = 1;
				}
				else
				{
					num5 = 0;
				}
				if (num5 != 0)
				{
					return (double)ui < Than;
				}
				int i = default(int);
				int num6;
				if (value is int)
				{
					i = (int)value;
					num6 = 1;
				}
				else
				{
					num6 = 0;
				}
				if (num6 != 0)
				{
					return (double)i < Than;
				}
				ushort us = default(ushort);
				int num7;
				if (value is ushort)
				{
					us = (ushort)value;
					num7 = 1;
				}
				else
				{
					num7 = 0;
				}
				if (num7 != 0)
				{
					return (double)(int)us < Than;
				}
				short s = default(short);
				int num8;
				if (value is short)
				{
					s = (short)value;
					num8 = 1;
				}
				else
				{
					num8 = 0;
				}
				if (num8 != 0)
				{
					return (double)s < Than;
				}
				return false;
			}
			throw new ArgumentException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
