namespace IATec.Shared.HttpClient.Dto
{
    public class ResponseDto<T> : BaseResponseDto
    {
        public T Data { get; private set; }

        internal void SetData(T data)
        {
            Data = data;
        }
    }
}
