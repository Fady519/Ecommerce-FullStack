namespace E_commerce.Presentation
{
    /// <summary>
    /// Names of the authorization policies (registered in AddJwtAuthentication) and the rate-limit policy.
    /// Controllers say WHAT is needed ([Authorize(Policy = AuthPolicies.AdminOnly)]); the rule behind the name
    /// lives in one place, so changing who counts as an admin is a one-line change.
    /// </summary>
    public static class AuthPolicies
    {
        public const string AdminOnly = "AdminOnly";

        /// <summary>Login, register and emailExists: a few requests per minute per IP, against password guessing.</summary>
        public const string AuthRateLimit = "auth";
    }
}
