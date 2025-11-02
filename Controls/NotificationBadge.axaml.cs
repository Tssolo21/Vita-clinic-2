using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace VitaClinic.WebAPI.Controls
{
    public partial class NotificationBadge : UserControl
    {
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(Title), defaultValue: "Notification");

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(Message), defaultValue: "This is a notification message");

        public static readonly StyledProperty<string> IconProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(Icon), defaultValue: "🔔");

        public static readonly StyledProperty<string> BackgroundColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(BackgroundColor), defaultValue: "#FFFFFF");

        public static readonly StyledProperty<string> BorderColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(BorderColor), defaultValue: "#E5E7EB");

        public static readonly StyledProperty<string> TitleColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(TitleColor), defaultValue: "#111827");

        public static readonly StyledProperty<string> MessageColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(MessageColor), defaultValue: "#6B7280");

        public static readonly StyledProperty<string> DismissColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(DismissColor), defaultValue: "#9CA3AF");

        public static readonly StyledProperty<string> IconBackgroundColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(IconBackgroundColor), defaultValue: "#F3F4F6");

        public static readonly StyledProperty<string> ProgressColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(ProgressColor), defaultValue: "#E5E7EB");

        public static readonly StyledProperty<string> ProgressActiveColorProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(ProgressActiveColor), defaultValue: "#6366F1");

        public static readonly StyledProperty<bool> ShowProgressProperty =
            AvaloniaProperty.Register<NotificationBadge, bool>(nameof(ShowProgress), defaultValue: false);

        public static readonly StyledProperty<double> ProgressWidthProperty =
            AvaloniaProperty.Register<NotificationBadge, double>(nameof(ProgressWidth), defaultValue: 0.0);

        public static readonly StyledProperty<string> AnimationClassProperty =
            AvaloniaProperty.Register<NotificationBadge, string>(nameof(AnimationClass), defaultValue: "bounce-in");

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string BackgroundColor
        {
            get => GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public string BorderColor
        {
            get => GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public string TitleColor
        {
            get => GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value);
        }

        public string MessageColor
        {
            get => GetValue(MessageColorProperty);
            set => SetValue(MessageColorProperty, value);
        }

        public string DismissColor
        {
            get => GetValue(DismissColorProperty);
            set => SetValue(DismissColorProperty, value);
        }

        public string IconBackgroundColor
        {
            get => GetValue(IconBackgroundColorProperty);
            set => SetValue(IconBackgroundColorProperty, value);
        }

        public string ProgressColor
        {
            get => GetValue(ProgressColorProperty);
            set => SetValue(ProgressColorProperty, value);
        }

        public string ProgressActiveColor
        {
            get => GetValue(ProgressActiveColorProperty);
            set => SetValue(ProgressActiveColorProperty, value);
        }

        public bool ShowProgress
        {
            get => GetValue(ShowProgressProperty);
            set => SetValue(ShowProgressProperty, value);
        }

        public double ProgressWidth
        {
            get => GetValue(ProgressWidthProperty);
            set => SetValue(ProgressWidthProperty, value);
        }

        public string AnimationClass
        {
            get => GetValue(AnimationClassProperty);
            set => SetValue(AnimationClassProperty, value);
        }

        public event EventHandler<RoutedEventArgs>? Dismissed;

        public NotificationBadge()
        {
            InitializeComponent();
        }

        private void DismissNotification(object? sender, RoutedEventArgs e)
        {
            Dismissed?.Invoke(this, e);
        }

        public void ShowInfo(string title, string message)
        {
            Title = title;
            Message = message;
            Icon = "ℹ️";
            BackgroundColor = "#EBF8FF";
            BorderColor = "#3B82F6";
            AnimationClass = "bounce-in";
            IsVisible = true;
        }

        public void ShowSuccess(string title, string message)
        {
            Title = title;
            Message = message;
            Icon = "✅";
            BackgroundColor = "#F0FDF4";
            BorderColor = "#10B981";
            AnimationClass = "bounce-in";
            IsVisible = true;
        }

        public void ShowWarning(string title, string message)
        {
            Title = title;
            Message = message;
            Icon = "⚠️";
            BackgroundColor = "#FFFBEB";
            BorderColor = "#F59E0B";
            AnimationClass = "slide-in-right";
            IsVisible = true;
        }

        public void ShowError(string title, string message)
        {
            Title = title;
            Message = message;
            Icon = "❌";
            BackgroundColor = "#FEF2F2";
            BorderColor = "#EF4444";
            AnimationClass = "warning-shake";
            IsVisible = true;
        }

        public async Task AutoDismissAsync(TimeSpan delay)
        {
            await Task.Delay(delay);
            AnimationClass = "fade-out";
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            IsVisible = false;
        }
    }
}