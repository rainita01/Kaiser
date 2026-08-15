namespace Busines_Layer;

public class ActionResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static ActionResult Completed()
    {
        return new ActionResult()
        {
            Success = true
        };
    }

    public static ActionResult Failed(string message)
    {
        return new ActionResult()
        {
            Success = false,
            Message = message
        };
    }
}