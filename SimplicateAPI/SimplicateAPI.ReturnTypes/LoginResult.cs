using SimplicateAPI.Enitities;

namespace SimplicateAPI.ReturnTypes;

public sealed class LoginResult
{
    /// <summary>
    /// The status of the login request. See <see cref="LoginStatus"/> for possible values.
    /// </summary>
    public LoginStatus Status { get; init; }
    /// <summary>
    /// Null if <see cref="Status"/> is not <see cref="LoginStatus.Success"/>.
    /// </summary>
    public User? User { get; init; }

    public enum LoginStatus
    {
        BadCredentials,
        ServerError,
        Failed,
        Success
    }
    
    /// <summary>
    /// Returns true if the status is <see cref="LoginStatus.Success"/> and the user is not null.
    /// </summary>
    public bool IsSuccess => Status == LoginStatus.Success && User != null;

    /// <summary>
    /// Returns a reusable instance of <see cref="LoginResult"/> with <see cref="Status"/> set to <see cref="LoginStatus.BadCredentials"/>.
    /// </summary>
    public static readonly LoginResult BadCredentials = new LoginResult { Status = LoginStatus.BadCredentials };
    
    /// <summary>
    /// Returns a reusable instance of <see cref="LoginResult"/> with <see cref="Status"/> set to <see cref="LoginStatus.Failed"/>.
    /// </summary>
    public static readonly LoginResult Failed = new LoginResult { Status = LoginStatus.Failed };
    
    /// <summary>
    /// Returns a reusable instance of <see cref="LoginResult"/> with <see cref="Status"/> set to <see cref="LoginStatus.ServerError"/>.
    /// </summary>
    public static readonly LoginResult ServerError = new LoginResult { Status = LoginStatus.ServerError };
}
