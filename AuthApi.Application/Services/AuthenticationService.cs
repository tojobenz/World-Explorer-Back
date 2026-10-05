using AuthApi.Application.Common;
using AuthApi.Application.DTOs;
using AuthApi.Application.Interfaces;
using AuthApi.Domain.Entities;
using AuthApi.Domain.Interfaces;
using FluentValidation;

namespace AuthApi.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<ForgotPasswordDto> _forgotPasswordValidator;
    private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        IAuthService authService,
        IEmailService emailService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator,
        IValidator<ForgotPasswordDto> forgotPasswordValidator,
        IValidator<ResetPasswordDto> resetPasswordValidator)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _emailService = emailService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _forgotPasswordValidator = forgotPasswordValidator;
        _resetPasswordValidator = resetPasswordValidator;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerDto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse(
                "Validation failed",
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var existingUser = await _unitOfWork.Users.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            PasswordHash = _authService.HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _authService.GenerateJwtToken(user.Id, user.Email);
        var refreshToken = _authService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync();

        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(60)
        };

        return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Registration successful");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var validationResult = await _loginValidator.ValidateAsync(loginDto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse(
                "Validation failed",
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
        }

        if (!_authService.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
        }

        if (!user.IsActive)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("Account is deactivated");
        }

        var token = _authService.GenerateJwtToken(user.Id, user.Email);
        var refreshToken = _authService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(60)
        };

        return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful");
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid refresh token");
        }

        var token = _authService.GenerateJwtToken(user.Id, user.Email);
        var newRefreshToken = _authService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(60)
        };

        return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully");
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var validationResult = await _forgotPasswordValidator.ValidateAsync(forgotPasswordDto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Validation failed",
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            // Pour la sécurité, on ne révèle pas si l'email existe ou non
            return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a reset token has been sent");
        }

        var resetToken = _authService.GenerateRefreshToken();
        user.RefreshToken = resetToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1);
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

        return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a reset token has been sent");
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var validationResult = await _resetPasswordValidator.ValidateAsync(resetPasswordDto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Validation failed",
                validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(resetPasswordDto.Email);
        if (user == null || user.RefreshToken != resetPasswordDto.Token)
        {
            return ApiResponse<bool>.ErrorResponse("Invalid token or email");
        }

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return ApiResponse<bool>.ErrorResponse("Token has expired");
        }

        user.PasswordHash = _authService.HashPassword(resetPasswordDto.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
    }
}