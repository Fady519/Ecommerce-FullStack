namespace E_Commerce.Web.CommonResult
{
    public class Result
    {

        protected readonly List<Errors> _errors = [];

        public bool IsSuccess => _errors.Count == 0;

        public bool IsFailure => !IsSuccess;

        public IReadOnlyList<Errors> Errors => _errors;

        protected Result()
        {

        }

        protected Result(Errors error)
        {
            _errors.Add(error);
        }

        protected Result(List<Errors> errors )
        {
            _errors.AddRange(errors);
        }

        public static Result Ok() => new Result();

        public static Result Fail(Errors error) => new Result(error);

        public static Result Fail(List<Errors> errors) => new Result(errors);


    }


    public class Result<TValue> : Result
    {

        private readonly TValue _value;

        public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Can Not Access The Value Of Failure");

        private Result(TValue value) : base()
        {
            _value = value;
        }

        private Result(Errors error) : base(error) 
        {
            _value = default;
        }

        private Result(List<Errors> errors) : base(errors)
        {
            _value = default;
        }


        public static Result<TValue> Ok(TValue value) => new (value);

        public static new Result<TValue> Fail(Errors error) => new(error);

        public static new Result<TValue> Fail(List<Errors> errors) => new(errors);

        public static implicit operator Result<TValue>(TValue value) => Ok (value);

        public static implicit operator Result<TValue>(Errors error) => Fail(error);

        public static implicit operator Result<TValue>(List<Errors> errors) => Fail(errors);



    }

}
