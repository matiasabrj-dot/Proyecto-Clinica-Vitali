using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ClinicaVitaliApi.Security;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Falta el encabezado Authorization."));
        }

        try
        {
            var authorizationHeader = AuthenticationHeaderValue.Parse(Request.Headers.Authorization!);
            if (!"Basic".Equals(authorizationHeader.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Esquema de autenticacion no soportado."));
            }

            var credentialBytes = Convert.FromBase64String(authorizationHeader.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            if (credentials.Length != 2)
            {
                return Task.FromResult(AuthenticateResult.Fail("Credenciales invalidas."));
            }

            var username = _configuration["BasicAuth:Username"] ?? string.Empty;
            var password = _configuration["BasicAuth:Password"] ?? string.Empty;

            if (credentials[0] != username || credentials[1] != password)
            {
                return Task.FromResult(AuthenticateResult.Fail("Usuario o contrasena incorrectos."));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, credentials[0]),
                new Claim(ClaimTypes.Name, credentials[0])
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Encabezado Authorization invalido."));
        }
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers.Append("WWW-Authenticate", "Basic realm=\"ClinicaVitaliApi\"");
        return Task.CompletedTask;
    }
}
