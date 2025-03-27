namespace Ex1.Common.Responses
{
    public class DataResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
}
