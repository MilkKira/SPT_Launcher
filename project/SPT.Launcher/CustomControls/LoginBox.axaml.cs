using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace SPT.Launcher.CustomControls;

public partial class LoginBox : UserControl
{
    /** 初始化登录框控件并加载对应的 Avalonia XAML。 */
    public LoginBox()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string> UsernameProperty = AvaloniaProperty.Register<LoginBox, string>(
        "Username");

    public string Username
    {
        get => GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    public static readonly StyledProperty<ICommand> LoginCommandProperty = AvaloniaProperty.Register<LoginBox, ICommand>(
        "LoginCommand");
    
    
    public static readonly StyledProperty<string> PasswordProperty = AvaloniaProperty.Register<LoginBox, string>(
        "Password");

    public string Password
    {
        get => GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public ICommand LoginCommand
    {
        get => GetValue(LoginCommandProperty);
        set => SetValue(LoginCommandProperty, value);
    }

    public static readonly StyledProperty<bool> IsLoggedInProperty = AvaloniaProperty.Register<LoginBox, bool>(
        "IsLoggedIn");

    public bool IsLoggedIn
    {
        get => GetValue(IsLoggedInProperty);
        set => SetValue(IsLoggedInProperty, value);
    }
}