﻿using System;
namespace Cr7Sund
{
    public interface IInternalLog
    {
        void Init();


        void Info(string message);
        void Error(string message);
        void Error(string prefix, Exception e);
        void Error(Exception e);
        void Fatal(string message);
        void Fatal(Exception e);
        void Fatal(string prefix, Exception e);
    }
}
