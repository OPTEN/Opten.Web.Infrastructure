using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;

namespace Opten.Web.Infrastructure.Cache
{
	/// <summary>
	/// The Memory Cache Layer (for Runtime Caching).
	/// </summary>
	public class RuntimeCache
	{

		//TODO: Do it like Umbraco does it? https://github.com/umbraco/Umbraco-CMS/blob/7bf28a20e7ff63fb9ff63829caf28d006e549b9d/src/Umbraco.Core/Cache/ObjectCacheRuntimeCacheProvider.cs

		readonly MemoryCache Cache;

		/// <summary>
		/// Initializes a new instance of the <see cref="RuntimeCache"/> class.
		/// </summary>
		public RuntimeCache()
		{
			Cache = MemoryCache.Default;
		}

		/// <summary>
		/// Tries to get the cached entry.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public T TryGet<T>(string key)
		{
			bool found;
			return this.TryGet<T>(key, string.Empty, out found);
		}

		/// <summary>
		/// Tries to get the cached entry.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public T TryGet<T>(string key, string language)
		{
			bool found;
			return this.TryGet<T>(key, language, out found);
		}

		/// <summary>
		/// Tries to get the cached entry.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="found">if set to <c>true</c> [found].</param>
		/// <returns></returns>
		public T TryGet<T>(string key, out bool found)
		{
			return this.TryGet<T>(key, string.Empty, out found);
		}

		/// <summary>
		/// Tries to get the cached entry.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="language">The language.</param>
		/// <param name="found">if set to <c>true</c> [found].</param>
		/// <returns></returns>
		public T TryGet<T>(string key, string language, out bool found)
		{
			try
			{
				string cacheKey = MakeKey(key, language);

				object item = Cache.Get(key: cacheKey);

				if (item != null)
				{
					found = true;
					return (T)item;
				}
				else
				{
					found = false;
					return default(T);
				}
			}
			catch
			{
				found = false;
				return default(T);
			}
		}

		/// <summary>
		/// Tries to get the cached item, otherwise it will be added.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="cacheItem">The cache item.</param>
		/// <param name="seconds">The seconds.</param>
		/// <returns></returns>
		public T TryGet<T>(string key, Func<T> cacheItem, int seconds)
		{
			return this.TryGet<T>(key, string.Empty, cacheItem, seconds);
		}

		/// <summary>
		/// Tries to get the cached item, otherwise it will be added.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="language">The language.</param>
		/// <param name="cacheItem">The cache item.</param>
		/// <param name="seconds">The seconds.</param>
		/// <returns></returns>
		public T TryGet<T>(string key, string language, Func<T> cacheItem, int seconds)
		{
			try
			{
				bool found;

				T result = this.TryGet<T>(key, language, out found);

				if (found == false)
				{
					// Execute result and add it to the cache
					result = cacheItem();

					//TODO: What if null?
					// https://github.com/umbraco/Umbraco-CMS/blob/7bf28a20e7ff63fb9ff63829caf28d006e549b9d/src/Umbraco.Core/Cache/HttpRuntimeCacheProvider.cs

					this.Add(result, key, language, seconds);
				}

				return result;
			}
			catch
			{
				return default(T);
			}
		}

		/// <summary>
		/// Adds the object to the cache.
		/// </summary>
		/// <param name="objectToCache">The object to cache.</param>
		/// <param name="key">The key.</param>
		/// <param name="seconds">The seconds.</param>
		public void Add(object objectToCache, string key, int seconds)
		{
			this.Add(objectToCache, key, string.Empty, seconds);
		}

		/// <summary>
		/// Adds the object to the cache.
		/// </summary>
		/// <param name="objectToCache">The object to cache.</param>
		/// <param name="key">The key.</param>
		/// <param name="language">The language.</param>
		/// <param name="seconds">The seconds.</param>
		public void Add(object objectToCache, string key, string language, int seconds)
		{
			CacheItemPolicy cachePolicy = new CacheItemPolicy
			{
				AbsoluteExpiration = DateTime.Now.AddSeconds(seconds)
			};

			string cacheKey = MakeKey(key, language);

			lock (Cache)
			{
				Cache.Add(cacheKey, objectToCache, cachePolicy);
			}
		}

		/// <summary>
		/// Removes the specified key from the cache.
		/// </summary>
		/// <param name="key">The key.</param>
		public void Remove(string key)
		{
			if (string.IsNullOrWhiteSpace(key) == false)
			{
				lock (Cache)
				{
					Cache.Remove(key);
				}
			}
		}

		/// <summary>
		/// Determines if the Cache contains the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public bool Contains(string key)
		{
			return this.Contains(key, string.Empty);
		}

		/// <summary>
		/// Determines if the Cache contains the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public bool Contains(string key, string language)
		{
			string cacheKey = MakeKey(key, language);

			return Cache.Contains(cacheKey);
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetKeys()
		{
			return Cache.Select(keyValuePair => keyValuePair.Key);
		}

		/// <summary>
		/// Makes the key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="language">The language.</param>
		/// <returns></returns>
		public string MakeKey(string key, string language)
		{
			if (string.IsNullOrWhiteSpace(language)) return key;

			return string.Format(CultureInfo.InvariantCulture, "{0}___{1}", key, language);
		}

	}
}