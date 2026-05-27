using SPT.Launcher.Helpers;
using SPT.Launcher.Models.Launcher;
using ReactiveUI;

namespace SPT.Launcher.ViewModels.Dialogs
{
    public class RegisterDialogViewModel : ViewModelBase
    {
        public string Question { get; set; }
        public string RegisterButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public string ComboBoxPlaceholderText { get; set; }
        public EditionCollection Editions { get; set; } = new EditionCollection();
        
        private string _password = "";
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        /// <summary>
        /// A registration dialog
        /// </summary>
        /// <param name="Host">Set to null when <see cref="ViewModelBase.ShowDialog(object)"/> is used, since the dialog host is handling routing</param>
        /// <param name="Username"></param>
        /** 初始化注册对话框，准备用户名提示、按钮文字和版本选择数据。 */
        public RegisterDialogViewModel(IScreen Host, string Username) : base(Host)
        {
            Question = string.Format(LocalizationProvider.Instance.registration_question_format_1, Username);

            RegisterButtonText = LocalizationProvider.Instance.register;

            CancelButtonText = LocalizationProvider.Instance.cancel;

            ComboBoxPlaceholderText = LocalizationProvider.Instance.select_edition;
        }
    }
}
