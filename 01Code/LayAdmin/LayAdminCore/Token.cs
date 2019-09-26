using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayAdminCore
{
    public class Token
    {
        private string TokenID { get; set; }
        private string TokenIP { get; set; }
        private string TokenUserID { get; set; }
        private DateTime TokenBeginTime { get; set; }

        private static Dictionary<string,Token> Tokens =new Dictionary<string,Token>();

        public static string GetToken()
        {
            Token t = new Token();
            t.TokenID =  Guid.NewGuid().ToString();
            t.TokenBeginTime = DateTime.Now;
            Tokens.Add(t.TokenID,t);
            return t.TokenID;
        }
        public static bool VerifyToken(string TokenID)
        {
            if (Tokens.ContainsKey(TokenID))
            {
                if (DateTime.Now.Subtract(Tokens[TokenID].TokenBeginTime).TotalMinutes <= 5)
                {
                    Tokens[TokenID].TokenBeginTime = DateTime.Now;
                    return true;
                }
            }
            return false;
        }
    }
}
