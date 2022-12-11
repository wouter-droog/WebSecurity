using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using QRCoder;
using WebAppIdentity.Data.Account;

namespace WebAppIdentity.Pages.Account
{
    [Authorize]
    public class AuthenticatorSetup : PageModel
    {
        private readonly UserManager<User> _userManager;
        
        [BindProperty]
        public string? AuthenticatorKey { get; set; }
        
        // property for the QR code
        public byte[] QrCodeBytes { get; set; }

        public AuthenticatorSetup(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            // Get the user who is currently logged in.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Generate the key
            // Microsoft Authenticator app.
            await _userManager.ResetAuthenticatorKeyAsync(user);
            AuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user) ?? string.Empty;
            
            // Generate the QR code
            QrCodeBytes = GenerateQrCode("Droog app", AuthenticatorKey, user.Email!);
            
            // if AuthenticatorKey is null or empty, add error to model state
            if (string.IsNullOrEmpty(AuthenticatorKey))
            {
                ModelState.AddModelError(string.Empty, "Error generating authenticator key.");
            }
        }

        public async Task<IActionResult> OnPostAsync(string code)
        {
            // Get the user who is currently logged in.
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Verify the code entered by the user.
            var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
            if (!isCodeValid)
            {
                ModelState.AddModelError("code", "Invalid code.");
                return Page();
            }
            
            // Enable two-factor authentication for the user.
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            
            // Create success message to display on the page in temp data
            TempData["StatusMessage"] = "Two-factor authentication has been enabled. You can now use an authenticator app to generate verification codes when you log in.";
            

            // If the code is valid, the user's two-factor authentication setup
            // is complete. You can redirect the user to another page or show
            // a confirmation message.
            return RedirectToPage("Index");
        }
        
        private static byte[] GenerateQrCode(string provider, string authenticatorKey, string userEmail)
        {
            // Generate the QR code
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{userEmail}?secret={authenticatorKey}&issuer={provider}", QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeAsBytes = qrCode.GetGraphic(20);
            return qrCodeAsBytes;
            
            // using var stream = new MemoryStream();
            // qrCodeAsBitmap.Save(stream, ImageFormat.Png);
            // return stream.ToArray();
            
        }
    }
}