namespace ClearAcceptDemo.ClearAcceptApi.BaseClasses
{
    public interface IService
    {
        void Authenticate();
        TOut Get<TOut>(string url);
        TOut Post<TIn, TOut>(string url, TIn requestBody);
        TOut Put<TIn, TOut>(string url, TIn requestBody);
        System.Net.HttpStatusCode Delete(string url);
    }
}