using NUnit.Framework;
using Opten.Web.Infrastructure.Cache;
using System;
using System.Threading;

namespace Opten.Web.Infrastructure.Test.Cache
{
	[TestFixture]
	public class CacheTests
	{

		private readonly RuntimeCache _cache = new RuntimeCache();

		[SetUp]
		public void Initialize()
		{
			RemoveAll();
		}

		[TearDown]
		public void TearDown()
		{
			RemoveAll();
		}

		[Test]
		public void Get_And_Add_String()
		{
			string testString = "test";

			var returnString = _cache.TryGet("test", () => testString, 1);

			// Try to add it again with 2000
			var returnString2 = _cache.TryGet("test", () => testString, 2);
			
			Assert.AreEqual(testString, returnString);
			Assert.AreEqual(testString, returnString2);

			Thread.Sleep(1500); //sleep longer than the cache expiration 

			Assert.AreEqual(default(string), _cache.TryGet<string>("test"));
		}

		[Test]
		public void Can_Add_And_Expire_Struct_Strongly_Typed_With_Null()
		{
			DateTime now = DateTime.Now;
			
			_cache.TryGet("DateTimeTest", () => now, 2);

			bool found;

			Assert.AreEqual(now, _cache.TryGet<DateTime>("DateTimeTest", out found));
			Assert.AreEqual(now, _cache.TryGet<DateTime?>("DateTimeTest", out found));

			Thread.Sleep(3000); //sleep longer than the cache expiration 

			Assert.AreEqual(default(DateTime), _cache.TryGet<DateTime>("DateTimeTest", out found));
			Assert.AreEqual(null, _cache.TryGet<DateTime?>("DateTimeTest", out found));
		}

		[Test]
		public void Add_And_Get_String()
		{
			string testString = "test";

			_cache.Add(testString, "test", 1000);

			bool found;
			Assert.AreEqual(testString, _cache.TryGet<string>("test", out found));
			Assert.AreEqual(true, found);
		}

		[Test]
		public void Add_And_Get_Dynamic()
		{
			var test = new
			{
				ThisIsATest = "ThisIsATest"
			};

			_cache.Add(test, "test", 1000);

			bool found;
			Assert.AreEqual(test, _cache.TryGet<dynamic>("test", out found));
			Assert.AreEqual(true, found);
		}

		[Test]
		public void Contains()
		{
			string testString = "test";

			_cache.Add(testString, "test", 1000);

			Assert.AreEqual(true, _cache.Contains("test"));
			Assert.AreEqual(false, _cache.Contains("whatever"));
		}

		[Test]
		public void Remove()
		{
			string testString = "test";
			
			_cache.Add(testString, "test", 1000);

			_cache.Remove("test");

			Assert.AreEqual(false, _cache.Contains("test"));
		}

		[Test]
		public void Remove_Non_Existing_Key_But_Nearly_Equal()
		{
			string testString = "test";

			_cache.Add(testString, "test", 1000);

			_cache.Remove("te");

			Assert.AreEqual(true, _cache.Contains("test"));
		}

		[Test]
		public void Remove_Non_Existing_Key_But()
		{
			string testString = "test";

			_cache.Add(testString, "test", 1000);

			_cache.Remove("remove_me_pelase");

			Assert.AreEqual(true, _cache.Contains("test"));
		}

		private void RemoveAll()
		{
			foreach (string key in _cache.GetKeys())
			{
				_cache.Remove(key);
			}
		}

	}
}
