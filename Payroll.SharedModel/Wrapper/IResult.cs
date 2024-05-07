namespace Payroll.SharedModel.Wrapper
{
    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }

    public interface IResult
    {
        string Message { get; set; }
        int Code { get; set; }
        bool Succeeded { get; set; }
    }
}
