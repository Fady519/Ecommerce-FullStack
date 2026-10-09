namespace E_commerce.Shared.DTOs.IdentityDTOs
{
    /// <summary>
    /// Returned by login and register. <c>Roles</c> lets the React app show the admin menu without decoding the token,
    /// and <c>ExpiresAt</c> lets it send the user back to the login page before calls start failing with 401.
    /// (The real permission check always happens on the server.)
    /// </summary>
    public record UserDTO(string Email, string DisplayName, string Token, IReadOnlyList<string> Roles, DateTime ExpiresAt);
}
