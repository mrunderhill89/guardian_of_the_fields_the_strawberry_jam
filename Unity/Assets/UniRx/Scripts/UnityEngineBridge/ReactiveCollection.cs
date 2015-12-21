using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UniRx
{
    public class CollectionAddEvent<T>
    {
        public int Index { get; private set; }
        public T Value { get; private set; }

        public CollectionAddEvent(int index, T value)
        {
            this.Index = index;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} Value:{1}", Index, Value);
        }
    }

    public class CollectionRemoveEvent<T>
    {
        public int Index { get; private set; }
        public T Value { get; private set; }

        public CollectionRemoveEvent(int index, T value)
        {
            this.Index = index;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} Value:{1}", Index, Value);
        }
    }

    public class CollectionMoveEvent<T>
    {
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }
        public T Value { get; private set; }

        public CollectionMoveEvent(int oldIndex, int newIndex, T value)
        {
            this.OldIndex = oldIndex;
            this.NewIndex = newIndex;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("OldIndex:{0} NewIndex:{1} Value:{2}", OldIndex,NewIndex, Value);
        }
    }

    public class CollectionReplaceEvent<T>
    {
        public int Index { get; private set; }
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }

        public CollectionReplaceEvent(int index, T oldValue, T newValue)
        {
            this.Index = index;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} OldValue:{1} NewValue:{2}", Index, OldValue, NewValue);
        }
    }
    
    public class CollectionContentsEvent<T>
    {
		public T[] Contents{ get; private set; }
		public int Count{ get; private set;}
		public CollectionContentsEvent(ICollection<T> contents)
		{
			this.Contents = contents.ToArray();
			this.Count = contents.Count;
		}
	}

    public interface IReactiveCollection<T> : IList<T>
    {
        void Move(int oldIndex, int newIndex);
        IObservable<CollectionAddEvent<T>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<CollectionMoveEvent<T>> ObserveMove();
        IObservable<CollectionRemoveEvent<T>> ObserveRemove();
        IObservable<CollectionReplaceEvent<T>> ObserveReplace();
        ReadOnlyReactiveProperty<CollectionContentsEvent<T>> ObserveContents();
        IObservable<Unit> ObserveReset();
    }

    [Serializable]
    public class ReactiveCollection<T> : Collection<T>, IReactiveCollection<T>
    {
        public ReactiveCollection()
        {

        }

        public ReactiveCollection(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public ReactiveCollection(List<T> list)
            : base(list != null ? new List<T>(list) : null)
        {
        }

        protected override void ClearItems()
        {
            var beforeCount = Count;
            base.ClearItems();

            if (collectionReset != null) collectionReset.OnNext(Unit.Default);
            if (collectionContents != null) collectionContents.OnNext(new CollectionContentsEvent<T>(this));
            if (beforeCount > 0)
            {
                if (countChanged != null) countChanged.OnNext(Count);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (collectionAdd != null) collectionAdd.OnNext(new CollectionAddEvent<T>(index, item));
            if (collectionContents != null) collectionContents.OnNext(new CollectionContentsEvent<T>(this));
            if (countChanged != null) countChanged.OnNext(Count);
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            T item = this[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);

            if (collectionMove != null) collectionMove.OnNext(new CollectionMoveEvent<T>(oldIndex, newIndex, item));
            if (collectionContents != null) collectionContents.OnNext(new CollectionContentsEvent<T>(this));
        }

        protected override void RemoveItem(int index)
        {
            T item = this[index];
            base.RemoveItem(index);

            if (collectionRemove != null) collectionRemove.OnNext(new CollectionRemoveEvent<T>(index, item));
            if (collectionContents != null) collectionContents.OnNext(new CollectionContentsEvent<T>(this));
            if (countChanged != null) countChanged.OnNext(Count);
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = this[index];
            base.SetItem(index, item);

            if (collectionReplace != null) collectionReplace.OnNext(new CollectionReplaceEvent<T>(index, oldItem, item));
            if (collectionContents != null) collectionContents.OnNext(new CollectionContentsEvent<T>(this));
        }


        [NonSerialized]
        Subject<int> countChanged = null;
        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            var subject = countChanged ?? (countChanged = new Subject<int>());
            if (notifyCurrentCount)
            {
                return subject.StartWith(() => this.Count);
            }
            else
            {
                return subject;
            }
        }

        [NonSerialized]
        Subject<Unit> collectionReset = null;
        public IObservable<Unit> ObserveReset()
        {
            return collectionReset ?? (collectionReset = new Subject<Unit>());
        }

        [NonSerialized]
        Subject<CollectionAddEvent<T>> collectionAdd = null;
        public IObservable<CollectionAddEvent<T>> ObserveAdd()
        {
            return collectionAdd ?? (collectionAdd = new Subject<CollectionAddEvent<T>>());
        }

        [NonSerialized]
        Subject<CollectionMoveEvent<T>> collectionMove = null;
        public IObservable<CollectionMoveEvent<T>> ObserveMove()
        {
            return collectionMove ?? (collectionMove = new Subject<CollectionMoveEvent<T>>());
        }

        [NonSerialized]
        Subject<CollectionRemoveEvent<T>> collectionRemove = null;
        public IObservable<CollectionRemoveEvent<T>> ObserveRemove()
        {
            return collectionRemove ?? (collectionRemove = new Subject<CollectionRemoveEvent<T>>());
        }

        [NonSerialized]
        Subject<CollectionReplaceEvent<T>> collectionReplace = null;
        public IObservable<CollectionReplaceEvent<T>> ObserveReplace()
        {
            return collectionReplace ?? (collectionReplace = new Subject<CollectionReplaceEvent<T>>());
        }
        
		[NonSerialized]
        Subject<CollectionContentsEvent<T>> collectionContents = null;
        ReadOnlyReactiveProperty<CollectionContentsEvent<T>> collectionContentsProperty = null;
		public ReadOnlyReactiveProperty<CollectionContentsEvent<T>> ObserveContents()
        {
			if (collectionContents == null)
				collectionContents = new Subject<CollectionContentsEvent<T>>();
			if (collectionContentsProperty == null)
				collectionContentsProperty = collectionContents.ToReadOnlyReactiveProperty<CollectionContentsEvent<T>>(new CollectionContentsEvent<T>(this));
            return collectionContentsProperty;
        }
		
		public ReactiveCollection<TResult> RxSelect<TResult>(System.Func<T, TResult> map){
			var that = new ReactiveCollection<TResult>(this.Select(map));
			ObserveAdd().Subscribe((evn)=>{
				that.InsertItem(evn.Index, map(evn.Value));
			});
			ObserveMove().Subscribe((evn)=>{
				that.MoveItem(evn.OldIndex, evn.NewIndex);
			});
			ObserveReplace().Subscribe((evn)=>{
				that.SetItem(evn.Index, map(evn.NewValue));
			});
			ObserveRemove().Subscribe((evn)=>{
				that.RemoveAt(evn.Index);
			});
			return that;
		}
		
		public ReactiveCollection<TResult> RxSelectMany<TResult>(System.Func<T[], IEnumerable<TResult>> map){
			var that = new ReactiveCollection<TResult>();
			ObserveContents().Subscribe((evn)=>{
				that.Clear();
				foreach (TResult result in map(evn.Contents)){
					that.Add(result);
				}
			});
			return that;
		}
		
		public ReactiveCollection<T> RxWhere(System.Func<T, bool> test){
			var that = new ReactiveCollection<T>(this.Where(test));
			ObserveAdd().Subscribe((evn)=>{
				if (test(evn.Value)){
					that.Add(evn.Value);
				}
			});
			ObserveReplace().Subscribe((evn)=>{
				if (that.Contains(evn.OldValue)){
					that.Remove(evn.OldValue);
				}
				if (test(evn.NewValue)){
					that.Add(evn.NewValue);
				}
			});
			//Since we aren't maintaining element order, anyway, we don't care about movement.
			ObserveRemove().Subscribe((evn)=>{
				if (that.Contains(evn.Value)){
					that.Remove(evn.Value);
				}
			});
			return that;
		}
    }

    public static partial class ReactiveCollectionExtensions
    {
        public static ReactiveCollection<T> ToReactiveCollection<T>(this IEnumerable<T> source)
        {
            return new ReactiveCollection<T>(source);
        }
    }
}
