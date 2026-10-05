
using AuthApi.Application.Common;
using AuthApi.Application.DTOs;

namespace AuthApi.Application.Interfaces;

public interface IAuthenticationService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}