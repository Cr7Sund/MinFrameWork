namespace Cr7Sund
{
    public interface IUpdatable 
    {
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="millisecond">流逝时间, 单位：毫秒</param>
        void Update(int millisecond);
    }
}