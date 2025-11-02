using System;
using Avalonia.Controls;

namespace VitaClinic.WebAPI.Controls
{
    public partial class LoadingSpinner : UserControl
    {
        public static readonly StyledProperty<double> SpinnerSizeProperty =
            AvaloniaProperty.Register<LoadingSpinner, double>(nameof(SpinnerSize), defaultValue: 40.0);

        public static readonly StyledProperty<string> SpinnerColorProperty =
            AvaloniaProperty.Register<LoadingSpinner, string>(nameof(SpinnerColor), defaultValue: "#6366F1");

        public static readonly StyledProperty<string> SpinnerColorLightProperty =
            AvaloniaProperty.Register<LoadingSpinner, string>(nameof(SpinnerColorLight), defaultValue: "#818CF8");

        public static readonly StyledProperty<int> BorderThicknessProperty =
            AvaloniaProperty.Register<LoadingSpinner, int>(nameof(BorderThickness), defaultValue: 4);

        public static readonly StyledProperty<string> LoadingTextProperty =
            AvaloniaProperty.Register<LoadingSpinner, string>(nameof(LoadingText), defaultValue: "Loading...");

        public static readonly StyledProperty<double> TextSizeProperty =
            AvaloniaProperty.Register<LoadingSpinner, double>(nameof(TextSize), defaultValue: 14.0);

        public static readonly StyledProperty<string> TextColorProperty =
            AvaloniaProperty.Register<LoadingSpinner, string>(nameof(TextColor), defaultValue: "#6B7280");

        public static readonly StyledProperty<bool> ShowTextProperty =
            AvaloniaProperty.Register<LoadingSpinner, bool>(nameof(ShowText), defaultValue: true);

        public double SpinnerSize
        {
            get => GetValue(SpinnerSizeProperty);
            set => SetValue(SpinnerSizeProperty, value);
        }

        public string SpinnerColor
        {
            get => GetValue(SpinnerColorProperty);
            set => SetValue(SpinnerColorProperty, value);
        }

        public string SpinnerColorLight
        {
            get => GetValue(SpinnerColorLightProperty);
            set => SetValue(SpinnerColorLightProperty, value);
        }

        public int BorderThickness
        {
            get => GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public string LoadingText
        {
            get => GetValue(LoadingTextProperty);
            set => SetValue(LoadingTextProperty, value);
        }

        public double TextSize
        {
            get => GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        public string TextColor
        {
            get => GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public bool ShowText
        {
            get => GetValue(ShowTextProperty);
            set => SetValue(ShowTextProperty, value);
        }

        public LoadingSpinner()
        {
            InitializeComponent();
        }
    }
}