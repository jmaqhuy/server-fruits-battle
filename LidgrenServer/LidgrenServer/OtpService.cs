using LidgrenServer;
using Microsoft.Extensions.Caching.Memory;

public class OtpService
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _otpLifetime = TimeSpan.FromMinutes(5);
    private readonly int _maxFailedAttempts = 5;

    public OtpService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }


    public string GenerateOtp(string Username)
    {
        if (_memoryCache.TryGetValue(Username, out string existingOtp))
        {
            Logging.Info($"Existing OTP for '{Username}' found in cache: '{existingOtp}'");
            return existingOtp;
        }

        var otp = GenerateRandomOtp();
        Logging.Info($"Generated new OTP for '{Username}': '{otp}'");
        _memoryCache.Set(Username, otp, _otpLifetime);
        _memoryCache.Set($"{Username}_failedAttempts", 0);
        return otp;
    }


    public string ResendOtp(string Username)
    {
        _memoryCache.Remove(Username);
        _memoryCache.Remove($"{Username}_failedAttempts");
        return GenerateOtp(Username);
    }


    public bool VerifyOtp(string Username, string enteredOtp)
    {
        Logging.Info($"Enter OTP: '{enteredOtp}'");

        if (!_memoryCache.TryGetValue(Username, out string storedOtp))
        {
            Logging.Info("OTP expired or not found in cache.");
            return false;
        }

        if (!_memoryCache.TryGetValue($"{Username}_failedAttempts", out int failedAttempts))
        {
            failedAttempts = 0;
        }

        if (failedAttempts >= _maxFailedAttempts)
        {
            Logging.Info("User reached maximum failed attempts.");
            _memoryCache.Remove(Username);
            return false;
        }

        if (string.Equals(storedOtp, enteredOtp, StringComparison.OrdinalIgnoreCase))
        {
            _memoryCache.Remove(Username);
            _memoryCache.Remove($"{Username}_failedAttempts");
            Logging.Info("OTP verification successful.");
            return true;
        }
        else
        {
            failedAttempts++;
            _memoryCache.Set($"{Username}_failedAttempts", failedAttempts);
            Logging.Info($"Failed attempt #{failedAttempts} for username: {Username}");

            if (failedAttempts >= _maxFailedAttempts)
            {
                Logging.Info("Max failed attempts reached, OTP removed.");
                _memoryCache.Remove(Username);
            }
            return false;
        }
    }


    private string GenerateRandomOtp(int length = 6)
    {
        var random = new Random();
        string otp = "";

        for (int i = 0; i < length; i++)
        {
            otp += random.Next(0, 10);
        }

        return otp;
    }
}
