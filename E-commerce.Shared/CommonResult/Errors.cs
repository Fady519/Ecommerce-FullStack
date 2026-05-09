namespace E_Commerce.Web.CommonResult
{
    public class Errors
    {
        public string Code { get; }

        public string Description { get; }

        public ErrorType Type {get; }


        private Errors(string code , string description , ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        #region Error Factory Methods

        public static Errors Failure(string Code = "General.Failure", string Description = "A General Has Occurred")
        {
            return new Errors(Code, Description, ErrorType.Failure);
        }

        public static Errors Validation(string Code = "General.Validation", string Description = "A Validation Error Has Occurred")
        {
            return new Errors(Code, Description, ErrorType.Validation);
        }

        public static Errors NotFound(string Code = "General.NotFound", string Description = "A Request Resourse Not Found")
        {
            return new Errors(Code, Description, ErrorType.NotFound);
        }

        public static Errors Unauthorized(string Code = "General.Unauthorized", string Description = "You Are Not Authorized")
        {
            return new Errors(Code, Description, ErrorType.Unauthorized);
        }

        public static Errors Forbidden(string Code = "General.forbidden", string Description = "You Do Not Have Permision To Access")
        {
            return new Errors(Code, Description, ErrorType.forbidden);
        }

        public static Errors InvalidCredentails(string Code = "General.InvalidCredentials", string Description = "A General Has Occurred")
        {
            return new Errors(Code, Description, ErrorType.InvalidCredentials);
        }

        #endregion

    }
}
