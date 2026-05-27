/* LoginRequestData.cs
 * License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 */


namespace SPT.Launcher
{
    public struct LoginRequestData
    {
        public string username;
        public string password;
        /** 将启动器输入的用户名和密码打包成登录请求数据。 */
        public LoginRequestData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
