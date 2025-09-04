namespace IATec.Shared.HttpClient.Dto
{
    public class ResponseDto<T, TError> : BaseResponseDto
    {
        public T? Data { get; private set; }

        public BaseResponseDto? BaseSucessDto { get; private set; }

        public TError? Error { get; private set; }

        public ErrorResponseDto? BaseErrorResponse { get; private set; }

        internal void SetData(T data)
        {
            Data = data;
        }

        internal void SetBaseData(BaseResponseDto data)
        {
            BaseSucessDto = data;
        }

        internal void SetError(TError error)
        {
            Error = error;
        }

        internal void SetBaseError(ErrorResponseDto error)
        {
            BaseErrorResponse = error;
        }

    }
}