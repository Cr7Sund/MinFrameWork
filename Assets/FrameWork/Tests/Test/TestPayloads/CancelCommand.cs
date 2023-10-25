using System;
using Cr7Sund.Framework.Impl;

namespace Cr7Sund.Framework.Tests
{
	public class CancelCommand : Command
	{
		public override void Execute()
		{
			Reject();
		}
	}
}

