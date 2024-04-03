using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IdentityModel.Tokens.Jwt;

namespace CRM_SER_EWS.CRM.Helpers
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

        public static IEnumerable<String> ListadoErrores(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var modelErrors = new List<String>();
                foreach (var mS in modelState.Values)
                {
                    foreach (var modelError in mS.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }
                return modelErrors;
            }
            return null;
        }
    }
}
