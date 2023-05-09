namespace System.Linq
{
    using Collections.Generic;
    using Collections.ObjectModel;
    using Series;
    using Threading.Tasks;
    using Transactions;
    using Uniques;

    public static class SeriesLinqExtensions
    {
        #region Methods

        public static IDeck<TResult> DoEach<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, TResult> action)
        {
            var set = new Album<TResult>(true);
            foreach (var item in items)
            {
                if (typeof(TResult).IsAssignableTo(typeof(IUnique)))
                    set.Add(action(item));
                else
                    set.Add(Unique.New, action(item));
            }
            return set;
        }
        public static void DoEach<TElement>(this IEnumerable<TElement> items, Action<TElement> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IEnumerable<TResult> ForOnly<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, bool> condition, Func<TElement, TResult> action)
        {
            if (items.Any(r => condition(r)))
            {
                foreach (var item in items)
                {
                    if (condition(item))
                        yield return action(item);
                }
            }
        }
        public static IEnumerable<TResult> ForOnly<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, bool> condition, Func<TElement, int, TResult> action)
        {
            if (items.Any(r => condition(r)))
            {
                int i = 0;
                foreach (var item in items)
                {
                    if (condition(item))
                        yield return action(item, i++);
                }
            }
        }

        public static async IAsyncEnumerable<TResult> ForOnlyAsync<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, bool> condition, Func<TElement, int, TResult> action)
        {
            if (items.Any(r => condition(r)))
            {
                int i = 0;
                foreach (var item in items)
                {
                    if (condition(item))
                        yield return await Task.FromResult<TResult>(action(item, i++));
                }
            }
        }
        public static async IAsyncEnumerable<TResult> ForOnlyAsync<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, bool> condition, Func<TElement, TResult> action)
        {
            if (items.Any(r => condition(r)))
            {
                foreach (var item in items)
                {
                    if (condition(item))
                        yield return await Task.FromResult<TResult>(action(item));
                }
            }
        }

        public static void ForOnly<TElement>(this IEnumerable<TElement> items, Func<TElement, bool> condition, Action<TElement> action)
        {
            if (items.Any(r => condition(r)))
            {
                foreach (var item in items)
                {
                    if (condition(item))
                        action(item);
                }
            }
        }
        public static void ForOnly<TElement>(this IEnumerable<TElement> items, Func<TElement, bool> condition, Action<TElement, int> action)
        {
            if (items.Any(r => condition(r)))
            {
                int i = 0;
                foreach (var item in items)
                {
                    if (condition(item))
                        action(item, i++);
                }
            }
        }

        public static Task ForOnlyAsync<TElement>(this IEnumerable<TElement> items, Func<TElement, bool> condition, Action<TElement> action)
        {
            return Task.Run(() => items.ForOnly(condition, action));
        }

        public static void ForEach<TElement>(this IEnumerable<TElement> items, Action<TElement> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
        public static void ForEach<TElement>(this IEnumerable<TElement> items, Action<TElement, int> action)
        {
            int i = 0;
            foreach (var item in items)
            {
                action(item, i++);
            }
        }

        public static IEnumerable<TResult> ForEach<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, int, TResult> action)
        {
            int i = 0;
            foreach (var item in items)
            {
                yield return action(item, i++);
            }
        }
        public static IEnumerable<TResult> ForEach<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, TResult> action)
        {
            foreach (var item in items)
            {
                yield return action(item);
            }
        }

        public static async IAsyncEnumerable<TResult> ForEachAsync<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, int, TResult> action)
        {   
            int i = 0;
            foreach (var item in items)
            {
                yield return await Task.Run(() => action(item, i++));
            }
        }
        public static async IAsyncEnumerable<TResult> ForEachAsync<TElement, TResult>(this IEnumerable<TElement> items, Func<TElement, TResult> action)
        {
            foreach (var item in items)
            {
                yield return await Task.Run(() => action(item));
            }
        }
        public static async IAsyncEnumerable<TResult> ForEachAsync<TElement, TResult>(this IAsyncEnumerable<TElement> items, Func<TElement, TResult> action)
        {
            await foreach (var item in items)
            {
                yield return await Task.Run(() => action(item));
            }
        }

        public static IQueryable<TResult> ForEach<TElement, TResult>(this IQueryable<TElement> items, Func<TElement, TResult> action)
        {
            return items.Select(i => action(i));
        }

        public static Task<IQueryable<TResult>> ForEachAsync<TElement, TResult>(this IQueryable<TElement> items, Func<TElement, TResult> action)
        {
            return Task.Run(() => items.Select(i => action(i)));
        }

        public static Task ForEachAsync<TElement>(this IEnumerable<TElement> items, Action<TElement> action)
        {
            return Task.Run(() => items.ForEach(action));
        }
        public static Task ForEachAsync<TElement>(this IEnumerable<TElement> items, Action<TElement, int> action)
        {
            return Task.Run(() => items.ForEach(action));
        }

        public static Task<List<TElement>> ToListAsync<TElement>(this IEnumerable<TElement> items, IDeputy callback = null)
        {
            return Task.Run(() =>
            {
                var list = items.ToList();
                if (callback != null)
                    callback.ExecuteAsync(list);
                return list;
            });
        }

        public static Task<TElement[]> ToArrayAsync<TElement>(this IEnumerable<TElement> items, IDeputy callback = null)
        {
            return Task.Run(() =>
            {
                var list = items.ToArray();
                if (callback != null)
                    callback.ExecuteAsync(list);
                return list;
            });
        }

        public static Task<Catalog<TElement>> ToCatalogAsync<TElement>(this IEnumerable<TElement> items, bool repeatable = false, IDeputy callback = null)
        {
            return Task.Run(() => items.ToCatalog(repeatable, callback));
        }

        public static Task<Deck<TElement>> ToDeckAsync<TElement>(this IEnumerable<TElement> items, IDeputy callback = null)
        {
            return Task.Run(() => items.ToDeck(callback));
        }

        public static Task<Album<TElement>> ToAlbumAsync<TElement>(this IEnumerable<TElement> items, bool repeatable = false, IDeputy callback = null)
        {
            return Task.Run(() => items.ToAlbum(repeatable, callback));
        }

        public static Catalog<TElement> ToCatalog<TElement>(this IEnumerable<TElement> items, bool repeatable = false, IDeputy callback = null)
        {
            var album = new Catalog<TElement>(items, 31, repeatable);

            if (callback == null) return album;

            callback.ExecuteAsync(album);
            return album;
        }

        public static Deck<TElement> ToDeck<TElement>(this IEnumerable<TElement> items, IDeputy callback = null)
        {
            var deck = new Deck<TElement>(items);

            if (callback == null) return deck;

            callback.ExecuteAsync(deck);
            return deck;
        }

        public static Album<TElement> ToAlbum<TElement>(this IEnumerable<TElement> items, bool repeatable = false, IDeputy callback = null)
        {
            var album = new Album<TElement>(items, 17, repeatable);

            if (callback == null) return album;

            callback.ExecuteAsync(album);
            return album;
        }

        public static Task<TElement[]> CommitAsync<TElement>(this IEnumerable<TElement> items)
        {
            return Task.Run(() => items.ToArray());
        }

        public static TElement[] Commit<TElement>(this IEnumerable<TElement> items)
        {
            return items.ToArray();
        }

        public static TElement[] Commit<TElement>(this IEnumerable<TElement> items, Action actionAfterCommit)
        {
            var array = items.ToArray();
            actionAfterCommit.Invoke();
            return array;
        }

        public static ObservableCollection<TElement> ToObservableCollection<TElement>(this IEnumerable<TElement> items)
        {
            return new ObservableCollection<TElement>(items);
        }

        public static TransactionScope CreateLockTransaction()
        {
            var options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };
            return new TransactionScope(TransactionScopeOption.Required, options);
        }

        public static T[] ToArray<T>(this IQueryable<T> query)
        {
            using (TransactionScope ts = CreateLockTransaction())
            {
                return query.ToArray();
            }
        }

        public static List<T> ToList<T>(this IQueryable<T> query)
        {
            using (TransactionScope ts = CreateLockTransaction())
            {
                return query.ToList();
            }
        }

        #endregion
    }
}
