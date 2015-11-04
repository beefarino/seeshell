using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Windows.Threading;
using Caliburn.Micro;
using CodeOwls.SeeShell.Common.Triggers;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.Common.DataSources
{
    public class CompositePowerShellDataSource : PropertyChangedBase, IPowerShellDataSource
    {
        private List<IPowerShellDataSource> _dataSources;
        private AggregatedObservableCollection<object> _data;
        private AggregatedObservableCollection<object> _allRecords;
        private AggregatedObservableCollection<ProgressRecord> _progressRecords;
        private string _name;

        public CompositePowerShellDataSource()
        {
            _data = new AggregatedObservableCollection<object>();
            _dataSources = new List<IPowerShellDataSource>();
            _allRecords = new AggregatedObservableCollection<object>();
            _progressRecords = new AggregatedObservableCollection<ProgressRecord>();
            _name = "_"+Guid.NewGuid().ToString("N");
        }

        static private object Adapt(object arg1, IList arg2)
        {
            return new KeyValuePair<IPowerShellDataSource, object>(arg2 as IPowerShellDataSource, arg1 );
        }

        public void Add( IPowerShellDataSource dataSource )
        {
            PropertyChangedEventHandler handler = null;
            IList oldDataSource = dataSource.Data;
            handler=(s, a) =>
                                               {
                                                   if (a.PropertyName == "Data")
                                                   {
                                                       
                                                       _data.ChildCollections.Remove(oldDataSource);
                                                       _data.ChildCollections.Add( dataSource.Data);
                                                       oldDataSource = dataSource.Data;
                                                       NotifyOfPropertyChange(()=>Data);
                                                   }
                                               };
            dataSource.PropertyChanged += handler;

            _data.ChildCollections.Add( dataSource.Data );
            _allRecords.ChildCollections.Add(dataSource.AllRecords);
            _progressRecords.ChildCollections.Add( dataSource.ProgressRecords);
            _dataSources.Add( dataSource);
        }

        public void Dispose()
        {
            _dataSources.ForEach(a => a.Dispose());
            _dataSources.Clear();
        }

        public ScaleDescriptorAssignmentCollection Scales
        {
            get
            {
                var scales = new ScaleDescriptorAssignmentCollection();
                (from ds in _dataSources
                 from scale in ds.Scales
                 select scale).ToList().ForEach(scales.Add);
                return scales;
            }
        }

        public ITrigger Trigger
        {
            get { return _dataSources.First().Trigger; }
            set { _dataSources.First().Trigger = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Dispatcher Dispatcher
        {
            get { return _dataSources.First().Dispatcher; }
            set { _dataSources.First().Dispatcher = value; }
        }

        public ObservableCollection<ProgressRecord> ProgressRecords
        {
            get { return _progressRecords; }
        }

        public ObservableCollection<object> AllRecords
        {
            get { return _allRecords; }
        }

        public ObservableCollection<object> Data
        {
            get { return _data; }
        }

        public int DataCollectionMaxSize
        {
            get { return _dataSources.First().DataCollectionMaxSize; }
            set { _dataSources.First().DataCollectionMaxSize = value; }
        }

        public void AddDynamicMember(PSMemberInfo plotMember)
        {
            throw new NotSupportedException();
        }

        public IScaleDescriptorAssignment AddDynamicScaleForProperty(string name)
        {
            throw new NotSupportedException();
        }

        public void AddDynamicMemberSpecification(DynamicMemberSpecification spec)
        {
            throw new NotSupportedException();
        }

        public void AddDataObject(PSObject inputObject)
        {
            throw new NotSupportedException();
        }
    }
    /// <summary>
    /// Aggregated observable collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the observable collections.
    /// </typeparam>
    internal class AggregatedObservableCollection<T> : ReadOnlyObservableCollection<T> 
    {
        /// <summary>
        /// Initializes a new instance of an aggregated observable collection.
        /// </summary>
        public AggregatedObservableCollection()
        {
            this.ChildCollections = new NoResetObservableCollection<IList>();
            this.ChildCollections.CollectionChanged += new NotifyCollectionChangedEventHandler(ChildCollectionsChanged);
        }

        /// <summary>
        /// Rebuilds the list if a collection changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void ChildCollectionsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.Assert(e.Action != NotifyCollectionChangedAction.Reset, "Reset is not supported.");

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                e.NewItems
                    .OfType<IList>()
                    .ForEachWithIndex((newCollection, index) =>
                    {
                        int startingIndex = GetStartingIndexOfCollectionAtIndex(e.NewStartingIndex + index);
                        foreach (T item in newCollection.OfType<T>().Reverse())
                        {
                            this.Mutate(items => items.Insert(startingIndex, item));
                        }

                        INotifyCollectionChanged notifyCollectionChanged = newCollection as INotifyCollectionChanged;
                        if (notifyCollectionChanged != null)
                        {
                            notifyCollectionChanged.CollectionChanged += ChildCollectionCollectionChanged;
                        }
                    });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (IList oldCollection in e.OldItems)
                {
                    INotifyCollectionChanged notifyCollectionChanged = oldCollection as INotifyCollectionChanged;
                    if (notifyCollectionChanged != null)
                    {
                        notifyCollectionChanged.CollectionChanged -= ChildCollectionCollectionChanged;
                    }

                    foreach (T item in oldCollection)
                    {
                        this.Mutate(items => items.Remove(item));
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (IList oldCollection in e.OldItems)
                {
                    INotifyCollectionChanged notifyCollectionChanged = oldCollection as INotifyCollectionChanged;
                    if (notifyCollectionChanged != null)
                    {
                        notifyCollectionChanged.CollectionChanged -= ChildCollectionCollectionChanged;
                    }
                }

                foreach (IList newCollection in e.NewItems)
                {
                    INotifyCollectionChanged notifyCollectionChanged = newCollection as INotifyCollectionChanged;
                    if (notifyCollectionChanged != null)
                    {
                        notifyCollectionChanged.CollectionChanged += ChildCollectionCollectionChanged;
                    }
                }

                Rebuild();
            }
        }

        protected virtual T Adapt(object value, IList source) 
        {
            return (T)value;
        }

        /// <summary>
        /// Synchronizes the collection with changes made in a child collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void ChildCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.Assert(e.Action != NotifyCollectionChangedAction.Reset, "Reset is not supported.");
            IList collectionSender = sender as IList;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startingIndex = GetStartingIndexOfCollectionAtIndex(ChildCollections.IndexOf(collectionSender));
                e.NewItems
                    .OfType<T>()
                    .ForEachWithIndex((item, index) =>
                    {
                        this.Mutate(that => that.Insert(startingIndex + e.NewStartingIndex + index, Adapt(item, collectionSender)));
                    });
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (T item in e.OldItems.OfType<T>())
                {
                    this.Mutate(that => that.Remove(item));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                for (int cnt = 0; cnt < e.NewItems.Count; cnt++)
                {
                    T oldItem = (T)e.OldItems[cnt];
                    T newItem = (T)e.NewItems[cnt];
                    int oldItemIndex = this.IndexOf(oldItem);
                    this.Mutate((that) =>
                    {
                        that[oldItemIndex] = newItem;
                    });
                }
            }
        }

        /// <summary>
        /// Returns the starting index of a collection in the aggregate
        /// collection.
        /// </summary>
        /// <param name="index">The starting index of a collection.</param>
        /// <returns>The starting index of the collection in the aggregate 
        /// collection.</returns>
        private int GetStartingIndexOfCollectionAtIndex(int index)
        {
            return ChildCollections.OfType<IEnumerable>().Select(collection => collection.CastWrapper<T>()).Take(index).SelectMany(collection => collection).Count();
        }

        /// <summary>
        /// Rebuild the list in the correct order when a child collection 
        /// changes.
        /// </summary>
        private void Rebuild()
        {
            this.Mutate(that => that.Clear());
            this.Mutate(that =>
            {
                IList<T> items = ChildCollections.OfType<IEnumerable>().Select(collection => collection.CastWrapper<T>()).SelectMany(collection => collection).ToList();
                foreach (T item in items)
                {
                    that.Add(item);
                }
            });
        }

        /// <summary>
        /// Gets child collections of the aggregated collection.
        /// </summary>
        public ObservableCollection<IList> ChildCollections { get; private set; }
    }

    internal class AdaptingAggregatedObservableCollection : AggregatedObservableCollection<object>
    {
        private readonly Func<object, IList, object> _adapter;

        public AdaptingAggregatedObservableCollection( Func< object, IList, object> adapter )
        {
            _adapter = adapter;
        }

        protected override object Adapt(object value, IList source)
        {
            return _adapter(value, source);
        }
        
    }

    /// <summary>
    /// An observable collection that can only be written to by internal 
    /// classes.
    /// </summary>
    /// <typeparam name="T">The type of object in the observable collection.
    /// </typeparam>
    internal class ReadOnlyObservableCollection<T> : NoResetObservableCollection<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the owner is writing to the 
        /// collection.
        /// </summary>
        private bool IsMutating { get; set; }

        /// <summary>
        /// A method that mutates the collection.
        /// </summary>
        /// <param name="action">The action to mutate the collection.</param>
        public void Mutate(Action<ReadOnlyObservableCollection<T>> action)
        {
            IsMutating = true;
            try
            {
                action(this);
            }
            finally
            {
                IsMutating = false;
            }
        }

        /// <summary>
        /// Removes an item from the collection at an index.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        protected override void RemoveItem(int index)
        {
            if (!IsMutating)
            {
                throw new NotSupportedException();
            }
            else
            {
                base.RemoveItem(index);
            }
        }

        /// <summary>
        /// Sets an item at a particular location in the collection.
        /// </summary>
        /// <param name="index">The location to set an item.</param>
        /// <param name="item">The item to set.</param>
        protected override void SetItem(int index, T item)
        {
            if (!IsMutating)
            {
                throw new NotSupportedException();
            }
            else
            {
                base.SetItem(index, item);
            }
        }

        /// <summary>
        /// Inserts an item in the collection.
        /// </summary>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            if (!IsMutating)
            {
                throw new NotSupportedException();
            }
            else
            {
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Clears the items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            if (!IsMutating)
            {
                throw new NotSupportedException();
            }
            else
            {
                base.ClearItems();
            }
        }
    }

    internal class NoResetObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Instantiates a new instance of the NoResetObservableCollection 
        /// class.
        /// </summary>
        public NoResetObservableCollection()
        {
        }

        /// <summary>
        /// Clears all items in the collection by removing them individually.
        /// </summary>
        protected override void ClearItems()
        {
            IList<T> items = new List<T>(this);
            foreach (T item in items)
            {
                Remove(item);
            }
        }
    }
}
