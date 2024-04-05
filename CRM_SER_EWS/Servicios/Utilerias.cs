using System.IdentityModel.Tokens.Jwt;

namespace CRM_EWS.Servicios
{
    public class Utilerias
    {
        public static string GetUserName(HttpRequest request)
        {
            var tokenAuth = request.Headers["auth"].ToString();
            if (String.IsNullOrWhiteSpace(tokenAuth))
            {
                return String.Empty;
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenAuth);

            var claimUsuario = token.Claims.Where(c => c.Type.Equals("USUARIO")).FirstOrDefault();
            var usuario = claimUsuario == null ? String.Empty : claimUsuario.Value;

            return usuario;
        }

        public static int GetUserId(HttpRequest request)
        {
            var tokenAuth = request.Headers["auth"].ToString();
            if (String.IsNullOrWhiteSpace(tokenAuth))
            {
                throw new Exception("");
            }
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenAuth);

            var claimUsuario = token.Claims.Where(c => c.Type.Equals("USUARIO_ID")).FirstOrDefault();
            var usuarioId = claimUsuario == null ? -1 : Int32.Parse(claimUsuario.Value);

            return usuarioId;
        }
    }
}
