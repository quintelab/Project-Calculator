using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using ProjectCalculator.ViewModel;
using Xamarin.Forms;

namespace ProjectCalculator.Controls
{
	public class BindablePicker : Picker
	{
		public BindablePicker()
		{
			SelectedIndexChanged += OnSelectedIndexChanged;
		}

		public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(BindableProperty), null, BindingMode.TwoWay, null, OnSelectedItemChanged);

		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindablePicker), null, BindingMode.OneWay, null, OnItemsSourceChanged);

		public static readonly BindableProperty DisplayPropertyProperty = BindableProperty.Create("DisplayProperty", typeof(string), typeof(BindablePicker), null, BindingMode.OneWay, null, OnDisplayPropertyChanged);

		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set
			{
				SetValue(SelectedItemProperty, value);

				if (ItemsSource.Contains(SelectedItem))
					SelectedIndex = ItemsSource.IndexOf(SelectedItem);
				else
					SelectedIndex = -1;
			}
		}

		public string DisplayProperty
		{
			get { return (string)GetValue(DisplayPropertyProperty); }
			set { SetValue(DisplayPropertyProperty, value); }
		}

		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedItem = ItemsSource[SelectedIndex];
		}

		private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var picker = (BindablePicker)bindable;
			picker.SelectedItem = newValue;
			if (picker.ItemsSource != null && picker.SelectedItem != null)
			{
				var count = 0;
				foreach (var obj in picker.ItemsSource)
				{
					if (obj == picker.SelectedItem)
					{
						picker.SelectedIndex = count;
						break;
					}
					count++;
				}
			}
		}

		private static void OnDisplayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var picker = (BindablePicker)bindable;
			picker.DisplayProperty = (string)newValue;
			LoadItemsAndSetSelected(bindable);
		}

		private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var picker = (BindablePicker)bindable;
			picker.ItemsSource = (IList)newValue;

			var oc = newValue as INotifyCollectionChanged;

			if (oc != null)
			{
				oc.CollectionChanged += (a, b) =>
				{
					LoadItemsAndSetSelected(bindable);
				};
			}

			LoadItemsAndSetSelected(bindable);
		}

		private static void LoadItemsAndSetSelected(BindableObject bindable)
		{
			var picker = (BindablePicker)bindable;

			if (picker.ItemsSource == null)
				return;

			var count = 0;

			foreach (var obj in picker.ItemsSource)
			{
				var value = string.Empty;
				if (picker.DisplayProperty != null)
				{
					var prop = obj.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, picker.DisplayProperty, StringComparison.OrdinalIgnoreCase));

					if (prop != null)
						value = prop.GetValue(obj).ToString();
				}
				else
				{
					value = obj.ToString();
				}

				if (!picker.Items.Contains(value))
				{
					picker.Items.Add(value);
				}

				if (picker.SelectedItem != null && picker.SelectedItem == obj)
					picker.SelectedIndex = count;

				count++;
			}

			if (picker.ItemsSource.Count == picker.Items.Count - 1)
				picker.SelectedIndex++;
		}
	}
}