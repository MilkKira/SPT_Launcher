/* AccountManager.cs
 * License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 * waffle.lord
 */


using SPT.Launcher.Controllers;
using SPT.Launcher.Helpers;
using SPT.Launcher.MiniCommon;
using SPT.Launcher.Models.Launcher;
using SPT.Launcher.Models.SPT;
using System.Threading.Tasks;

namespace SPT.Launcher
{
    public enum AccountStatus
    {
        OK = 0,
        NoConnection = 1,
        LoginFailed = 2,
        RegisterFailed = 3,
        UpdateFailed = 4
    }

    public static class AccountManager
    {
        private const string STATUS_FAILED = "FAILED";
        private const string STATUS_OK = "OK";
        public static AccountInfo SelectedAccount { get; private set; } = null;
        public static ProfileInfo SelectedProfileInfo { get; private set; } = null;

        public static void Logout()
        {
            // Set currently selected account to null, as well as removing the old session token
            SelectedAccount = null;
            RequestHandler.ChangeSession(null);
        }

        public static async Task<AccountStatus> LoginAsync(LoginModel Creds)
        {
            return await LoginAsync(Creds.Username, Creds.Password);
        }
        
        
        /** 兼容只传用户名的旧调用；优先复用自动登录密码，当前会话已登录时直接通过。 */
        public static async Task<AccountStatus> LoginAsync(string username)
        {
            ServerSetting defaultServer = LauncherSettingsProvider.Instance.Server;
            if (defaultServer.AutoLoginCreds?.Username == username)
            {
                return await LoginAsync(username, defaultServer.AutoLoginCreds.Password);
            }

            if (SelectedAccount?.username == username)
            {
                return AccountStatus.OK;
            }

            return await LoginAsync(username, string.Empty);
        }
        /** 使用用户名和密码登录账号，通过补丁接口校验密码后再获取账号资料。 */
        public static async Task<AccountStatus> LoginAsync(string username, string password)
        {
            LoginRequestData data = new LoginRequestData(username, password);
            string id = STATUS_FAILED;
            string json = "";

            try
            {
                // id = await RequestHandler.RequestLogin(data);
                id = await RequestHandler.RequestLoginCheck(data);

                if (id == STATUS_FAILED)
                {
                    return AccountStatus.LoginFailed;
                }

                json = await RequestHandler.RequestAccount(data);
            }
            catch
            {
                return AccountStatus.NoConnection;
            }

            SelectedAccount = Json.Deserialize<AccountInfo>(json);
            RequestHandler.ChangeSession(SelectedAccount.id);

            await UpdateProfileInfoAsync();

            return AccountStatus.OK;
        }

        public static async Task UpdateProfileInfoAsync()
        {
            LoginRequestData data = new LoginRequestData(SelectedAccount.username, string.Empty);
            string profileInfoJson = await RequestHandler.RequestProfileInfo(data);

            if (profileInfoJson != null)
            {
                ServerProfileInfo serverProfileInfo = Json.Deserialize<ServerProfileInfo>(profileInfoJson);
                SelectedProfileInfo = new ProfileInfo(serverProfileInfo);
            }
        }

        public static async Task<ServerProfileInfo[]> GetExistingProfilesAsync()
        {
            string profileJsonArray = await RequestHandler.RequestExistingProfiles();

            if(profileJsonArray != null)
            {
                ServerProfileInfo[] miniProfiles = Json.Deserialize<ServerProfileInfo[]>(profileJsonArray);

                if (miniProfiles != null && miniProfiles.Length > 0)
                {
                    return miniProfiles;
                }
            }

            return [];
        }
        /** 使用用户名、密码和版本创建新账号，成功后自动按同一组凭据登录。 */
        public static async Task<AccountStatus> RegisterAsync(string username, string password, string edition)
        {
            string registerResult;
            try
            {
                registerResult = await RequestHandler.RequestRegister(new RegisterRequestData(username, password, edition));
            }
            catch
            {
                return AccountStatus.NoConnection;
            }

            if (registerResult == string.Empty)
            {
                return AccountStatus.RegisterFailed;
            }

            LogManager.Instance.Info($"Account Registered: {username} {registerResult}");

            return await LoginAsync(username, password);
        }

        public static async Task<AccountStatus> RemoveAsync()
        {
            LoginRequestData data = new LoginRequestData(SelectedAccount.username, string.Empty);

            try
            {
                string json = await RequestHandler.RequestRemove(data);

                if(Json.Deserialize<bool>(json))
                {
                    // Set currently selected account to null, as well as removing the old session token
                    SelectedAccount = null;
                    RequestHandler.ChangeSession(null);

                    LogManager.Instance.Info($"Account Removed: {data.username}");

                    return AccountStatus.OK;
                }
                else
                {
                    LogManager.Instance.Error($"Failed to remove account: {data.username}");
                    return AccountStatus.UpdateFailed;
                }
            }
            catch
            {
                LogManager.Instance.Error($"Failed to remove account: {data.username} - NO CONNECTION");
                return AccountStatus.NoConnection;
            }
        }

        public static async Task<AccountStatus> ChangeUsernameAsync(string username)
        {
            ChangeRequestData data = new ChangeRequestData(SelectedAccount.username, username);
            string json = STATUS_FAILED;

            try
            {
                json = await RequestHandler.RequestChangeUsername(data);

                if (json != STATUS_OK)
                {
                    return AccountStatus.UpdateFailed;
                }
            }
            catch
            {
                return AccountStatus.NoConnection;
            }

            ServerSetting DefaultServer = LauncherSettingsProvider.Instance.Server;

            if (DefaultServer.AutoLoginCreds != null)
            {
                DefaultServer.AutoLoginCreds.Username = username;
            }

            SelectedAccount.username = username;
            LauncherSettingsProvider.Instance.SaveSettings();

            return AccountStatus.OK;
        }

        public static async Task<AccountStatus> WipeAsync(string edition)
        {
            // RegisterRequestData data = new RegisterRequestData(SelectedAccount.username, edition);
            RegisterRequestData data = new RegisterRequestData(SelectedAccount.username, string.Empty, edition);
            string json = STATUS_FAILED;

            try
            {
                json = await RequestHandler.RequestWipe(data);

                if (json != STATUS_OK)
                {
                    LogManager.Instance.Error($"Failed to wipe account: {data.username}");
                    return AccountStatus.UpdateFailed;
                }
            }
            catch
            {
                LogManager.Instance.Error($"Failed to wipe account: {data.username} - NO CONNECTION");
                return AccountStatus.NoConnection;
            }

            SelectedAccount.edition = edition;
            SelectedAccount.wipe = true;
            LogManager.Instance.Info($"Account Wiped: {data.username} -> {edition}");
            return AccountStatus.OK;
        }
    }
}
