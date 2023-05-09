using System.Collections.Generic;
using System.Instant;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Uniques;

namespace System.Series
{
    public class MassCache<V> : MassCatalogBase<V> where V : IUnique
    {
        private readonly Board<Timer> timers = new Board<Timer>();

        private TimeSpan duration;
        private IDeputy callback;

        private void setupExpiration(TimeSpan? lifetime, IDeputy callback)
        {
            duration = (lifetime != null) ? lifetime.Value : TimeSpan.FromMinutes(15);
            if (callback != null)
                this.callback = callback;
        }

        public MassCache(
            IEnumerable<IUnique<V>> collection,
            TimeSpan? lifeTime = null,
            IDeputy callback = null,
            int capacity = 17
        ) : base(collection, capacity)
        {
            setupExpiration(lifeTime, callback);
        }

        public MassCache(
            IEnumerable<V> collection,
            TimeSpan? lifeTime = null,
            IDeputy callback = null,
            int capacity = 17
        ) : base(collection, capacity)
        {
            setupExpiration(lifeTime, callback);
        }

        public MassCache(
            IList<IUnique<V>> collection,
            TimeSpan? lifeTime = null,
            IDeputy callback = null,
            int capacity = 17
        ) : base(collection, capacity)
        {
            setupExpiration(lifeTime, callback);
        }

        public MassCache(
            IList<V> collection,
            TimeSpan? lifeTime = null,
            IDeputy callback = null,
            int capacity = 17
        ) : base(collection, capacity)
        {
            setupExpiration(lifeTime, callback);
        }

        public MassCache(TimeSpan? lifeTime = null, IDeputy callback = null, int capacity = 17)
            : base(capacity)
        {
            setupExpiration(lifeTime, callback);
        }

        public override ICard<V> EmptyCard()
        {
            return new CacheCard<V>();
        }

        public override ICard<V>[] EmptyCardTable(int size)
        {
            return new CacheCard<V>[size];
        }

        public override ICard<V>[] EmptyDeck(int size)
        {
            return new CacheCard<V>[size];
        }

        public override ICard<V> NewCard(ICard<V> card)
        {
            return new CacheCard<V>(card, duration, callback);
        }

        public override ICard<V> NewCard(object key, V value)
        {
            return new CacheCard<V>(key, value, duration, callback);
        }

        public override ICard<V> NewCard(ulong key, V value)
        {
            return new CacheCard<V>(key, value, duration, callback);
        }

        public override ICard<V> NewCard(V value)
        {
            return new CacheCard<V>(value, duration, callback);
        }

        protected virtual IMassDeck<IUnique> cache { get; set; }

        protected virtual T InnerMemorize<T>(T item) where T : IUnique
        {
            uint group = GetValidTypeKey(typeof(T));
            if (!cache.TryGet(group, out IUnique deck))
            {
                Sleeve sleeve = SleeveFactory.Create(GetValidType(typeof(T)), group);
                sleeve.Combine();

                IRubrics keyrubrics = sleeve.Rubrics.KeyRubrics;

                ISleeve isleeve = item.ToSleeve();

                deck = new MassCatalog<IUnique>();

                foreach (MemberRubric keyRubric in keyrubrics)
                {
                    Catalog<IUnique> subdeck = new Catalog<IUnique>();

                    subdeck.Add(item);

                    ((IMassDeck<IUnique>)deck).Put(
                        isleeve[keyRubric.RubricId],
                        keyRubric.RubricName.UniqueKey32(),
                        subdeck);
                }

                cache.Add(group, deck);

                cache.Add(item);

                return item;
            }

            if (!cache.ContainsKey(item))
            {
                IMassDeck<IUnique> _deck = (IMassDeck<IUnique>)deck;

                ISleeve isleeve = item.ToSleeve();

                foreach (MemberRubric keyRubric in isleeve.Rubrics.KeyRubrics)
                {
                    if (!_deck.TryGet(
                        isleeve[keyRubric.RubricId],
                        keyRubric.RubricName.UniqueKey32(),
                        out IUnique outdeck))
                    {
                        outdeck = new Catalog<IUnique>();

                        ((IDeck<IUnique>)outdeck).Put(item);

                        _deck.Put(isleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outdeck);
                    }
                    else
                    {
                        ((IDeck<IUnique>)outdeck).Put(item);
                    }
                }
                cache.Add(item);
            }

            return item;
        }

        protected virtual T InnerMemorize<T>(T item, params string[] names) where T : IUnique
        {
            Memorize(item);

            ISleeve sleeve = item.ToSleeve();

            MemberRubric[] keyrubrics = sleeve.Rubrics.Where(p => names.Contains(p.RubricName)).ToArray();

            IMassDeck<IUnique> _deck = (IMassDeck<IUnique>)cache.Get(item.UniqueType);

            foreach (MemberRubric keyRubric in keyrubrics)
            {
                if (!_deck.TryGet(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), out IUnique outdeck))
                {
                    outdeck = new Catalog<IUnique>();

                    ((IDeck<IUnique>)outdeck).Put(item);

                    _deck.Put(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outdeck);
                }
                else
                {
                    ((IDeck<IUnique>)outdeck).Put(item);
                }
            }

            return item;
        }      

        public virtual IMassDeck<IUnique> CacheSet<T>() where T : IUnique
        {
            if (cache.TryGet(GetValidTypeKey(typeof(T)), out IUnique deck))
                return (IMassDeck<IUnique>)deck;
            return null;
        }

        public virtual T Lookup<T>(object keys) where T : IUnique
        {
            if (cache.TryGet(keys, GetValidTypeKey(typeof(T)), out IUnique output))
                return (T)output;
            return default;
        }

        public virtual IDeck<IUnique> Lookup<T>(Tuple<string, object> valueNamePair) where T : IUnique
        { return Lookup<T>((m) => (IDeck<IUnique>)m.Get(valueNamePair.Item2, valueNamePair.Item2.UniqueKey32())); }

        public virtual IDeck<IUnique> Lookup<T>(Func<IMassDeck<IUnique>, IDeck<IUnique>> selector) where T : IUnique
        { return selector(CacheSet<T>()); }

        public virtual T Lookup<T>(T item) where T : IUnique
        {
            ISleeve shell = item.ToSleeve();
            IRubrics mrs = shell.Rubrics.KeyRubrics;
            T[] result = new T[mrs.Count];
            int i = 0;
            if (cache.TryGet(GetValidTypeKey(typeof(T)), out IUnique deck))
            {
                foreach (MemberRubric mr in mrs)
                {
                    if (((IMassDeck<IUnique>)deck).TryGet(
                        shell[mr.RubricId],
                        mr.RubricName.UniqueKey32(),
                        out IUnique outdeck))
                        if (((IDeck<IUnique>)outdeck).TryGet(item, out IUnique output))
                            result[i++] = (T)output;
                }
            }

            if (result.Any(r => r == null))
                return default;
            return result[0];
        }

        public virtual T[] Lookup<T>(object key, params Tuple<string, object>[] valueNamePairs) where T : IUnique
        {
            return Lookup<T>(
                (k) => k[key],
                valueNamePairs.ForEach(
                    (vnp) => new Func<IMassDeck<IUnique>, IDeck<IUnique>>(
                        (m) => (IDeck<IUnique>)m
                                                                        .Get(vnp.Item2, vnp.Item1.UniqueKey32())))
                    .ToArray());
        }

        public virtual T[] Lookup<T>(
            Func<IDeck<IUnique>, IUnique> key,
            params Func<IMassDeck<IUnique>, IDeck<IUnique>>[] selectors)
            where T : IUnique
        {
            if (cache.TryGet(GetValidTypeKey(typeof(T)), out IUnique deck))
            {
                T[] result = new T[selectors.Length];
                for (int i = 0; i < selectors.Length; i++)
                {
                    result[i] = (T)key(selectors[i]((IMassDeck<IUnique>)deck));
                }
                return result;
            }

            return default;
        }

        public virtual IDeck<IUnique> Lookup<T>(object key, string propertyNames) where T : IUnique
        {
            if (CacheSet<T>().TryGet(key, propertyNames.UniqueKey32(), out IUnique outdeck))
                return (IDeck<IUnique>)outdeck;
            return default;
        }

        public virtual T Lookup<T>(T item, params string[] propertyNames) where T : IUnique
        {
            ISleeve ilValuator = item.ToSleeve();
            MemberRubric[] mrs = ilValuator.Rubrics.Where(p => propertyNames.Contains(p.RubricName)).ToArray();
            T[] result = new T[mrs.Length];

            if (cache.TryGet(GetValidTypeKey(typeof(T)), out IUnique deck))
            {
                int i = 0;
                foreach (MemberRubric mr in mrs)
                {
                    if (((IMassDeck<IUnique>)deck).TryGet(
                        ilValuator[mr.RubricId],
                        mr.RubricName.UniqueKey32(),
                        out IUnique outdeck))
                        if (((IDeck<IUnique>)outdeck).TryGet(item, out IUnique output))
                            result[i++] = (T)output;
                }
            }

            if (result.Any(r => r == null))
                return default;
            return result[0];
        }

        public virtual IEnumerable<T> Memorize<T>(IEnumerable<T> items) where T : IUnique
        { return items.ForEach(p => Memorize(p)); }

        public virtual T Memorize<T>(T item) where T : IUnique
        {
            return InnerMemorize(item);
        }

        public virtual T Memorize<T>(T item, params string[] names) where T : IUnique
        {
            if (InnerMemorize(item) != null)
                return InnerMemorize(item, names);
            return default(T);
        }

        public virtual async Task<T> MemorizeAsync<T>(T item) where T : IUnique
        { return await Task.Run(() => Memorize(item)); }
        public virtual async Task<T> MemorizeAsync<T>(T item, params string[] names) where T : IUnique
        { return await Task.Run(() => Memorize(item, names)); }

        public virtual IMassDeck<IUnique> Catalog => cache;

        public virtual Type GetValidType(object obj)
        {
            return obj.GetType();
        }
        public virtual Type GetValidType(Type obj)
        {
            return obj;
        }
        public virtual uint GetValidTypeKey(object obj)
        {
            return obj.GetType().UniqueKey32();
        }

        public virtual uint GetValidTypeKey(Type obj)
        {
            return obj.UniqueKey32();
        }

    }
}
