using System;
using Avalonia.Controls;

namespace VitaClinic.WebAPI.Controls
{
    public partial class SkeletonLoader : UserControl
    {
        public static readonly StyledProperty<bool> ShowFooterProperty =
            AvaloniaProperty.Register<SkeletonLoader, bool>(nameof(ShowFooter), defaultValue: true);

        public static readonly StyledProperty<int> CardCountProperty =
            AvaloniaProperty.Register<SkeletonLoader, int>(nameof(CardCount), defaultValue: 3);

        public bool ShowFooter
        {
            get => GetValue(ShowFooterProperty);
            set => SetValue(ShowFooterProperty, value);
        }

        public int CardCount
        {
            get => GetValue(CardCountProperty);
            set => SetValue(CardCountProperty, value);
        }

        public SkeletonLoader()
        {
            InitializeComponent();
        }
    }
}