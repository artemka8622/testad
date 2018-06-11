using System.Transactions;
using NUnit.Framework;

namespace Arm.CreateTask.Tests
{
	/// <summary>
	/// Откатываемые транзакцией тесты.
	/// </summary>
	public abstract class TransactionTests
	{
		/// <summary>
		/// Транзакция.
		/// </summary>
		protected TransactionScope transaction_scope;

		/// <summary>
		/// Открывает транзакцию.
		/// </summary>
		[SetUp]
		public void TransactionSetUp()
		{
			transaction_scope = new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
		}

		/// <summary>
		/// Откатывает транзакцию.
		/// </summary>
		[TearDown]
		public void TransactionTearDown()
		{
			transaction_scope.Dispose();
		}
	}
}
