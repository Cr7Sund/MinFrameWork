namespace Cr7Sund.Framework.Api
{
    public enum BinderExceptionType
    {

        // there exist boxing operation
        EXIST_BOXING,
        // bind different binding with same key and same name, and is not weak too
        CONFLICT_IN_BINDER
    }
}
