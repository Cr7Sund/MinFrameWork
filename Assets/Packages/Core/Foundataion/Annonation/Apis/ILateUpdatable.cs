namespace Cr7Sund
{
    public interface ILateUpdatable 
    {
        /// <summary>
        /// LateUpdate
        /// </summary>
        /// <param name="millisecond">流逝时间, 单位：毫秒</param>
        void LateUpdate(int millisecond);
    }
}