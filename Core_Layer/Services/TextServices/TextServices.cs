namespace Core_Layer.Services.TextServices;

public class TextServices
{
    public string GenerateSlug(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "";

        // تبدیل به حروف کوچک
        var slug = name.ToLower();

        // جایگزینی فاصله با خط تیره
        slug = slug.Replace(" ", "-");

        // حذف کاراکترهای خاص
        slug = slug
            .Replace("?", "")
            .Replace("!", "")
            .Replace("،", "")
            .Replace(".", "")
            .Replace(":", "")
            .Replace(";", "");

        return slug;
    }
}