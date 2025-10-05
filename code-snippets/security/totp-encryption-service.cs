// Context: Protecting MFA TOTP secrets at rest with authenticated encryption.
// Problem: Plain or weakly hashed secrets risk disclosure & tampering.
// Solution: AES-GCM envelope with version prefix (v1:) enabling future rotation.
// Value: Confidentiality + integrity + migration path.

public interface ITotpSecretProtector
{
    string Protect(string plaintext);
    string Unprotect(string protectedValue);
}

public sealed class AesGcmTotpSecretProtector : ITotpSecretProtector
{
    private readonly byte[] _key;
    public AesGcmTotpSecretProtector(IConfiguration cfg)
    {
        var raw = cfg["Security:TotpKey"] ?? throw new InvalidOperationException("Missing TotpKey");
        // Stretch if short
        if (raw.Length < 32) raw = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
        _key = Convert.FromBase64String(raw);
    }

    public string Protect(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext) || IsProtected(plaintext)) return plaintext;
        Span<byte> nonce = stackalloc byte[12];
        RandomNumberGenerator.Fill(nonce);
        var pt = Encoding.UTF8.GetBytes(plaintext);
        var cipher = new byte[pt.Length];
        var tag = new byte[16];
        using var gcm = new AesGcm(_key);
        gcm.Encrypt(nonce, pt, cipher, tag);
        return "v1:" + Convert.ToBase64String(Concat(nonce.ToArray(), cipher, tag));
    }

    public string Unprotect(string protectedValue)
    {
        if (!IsProtected(protectedValue)) return protectedValue;
        var b64 = protectedValue[3..];
        var blob = Convert.FromBase64String(b64);
        var nonce = blob.AsSpan(0, 12);
        var tag = blob.AsSpan(blob.Length - 16, 16);
        var cipher = blob.AsSpan(12, blob.Length - 28);
        var pt = new byte[cipher.Length];
        using var gcm = new AesGcm(_key);
        gcm.Decrypt(nonce, cipher, tag, pt);
        return Encoding.UTF8.GetString(pt);
    }

    private static bool IsProtected(string v) => v.StartsWith("v1:");

    private static byte[] Concat(params byte[][] arrays)
    {
        var len = arrays.Sum(a => a.Length);
        var buf = new byte[len];
        var off = 0;
        foreach (var a in arrays)
        {
            Buffer.BlockCopy(a, 0, buf, off, a.Length);
            off += a.Length;
        }
        return buf;
    }
}
