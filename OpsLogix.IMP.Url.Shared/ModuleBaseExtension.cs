using Microsoft.EnterpriseManagement.HealthService;
using OpsLogix.IMP.Url.Shared.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace OpsLogix.IMP.Url.Modules.ProbeActions
{
    /// <summary>
    /// Abstract ModuleBase extension class to handle common functionality
    /// </summary>
    /// <typeparam name="TOutputDataType"></typeparam>
    public abstract class ModuleBaseExtension<TOutputDataType> : ModuleBase<TOutputDataType> where TOutputDataType : DataItemBase
    {
        private readonly object _shutdownLock = new object();
        protected readonly ILogger _logger;
        private bool _shutdown;

        protected object ModuleState { get; set; }

        public ModuleBaseExtension(ModuleHost<TOutputDataType> moduleHost, XmlReader configuration, byte[] previousState) : base(moduleHost)
        {
            LoadConfiguration(configuration);

            //Move this 
            _logger = new TraceLogger(this.GetType().Name, "Operations Manager");

            //QUESTION: Not sure if we need to work with states...
            //LoadPreviousState(previousState);
        }

        protected abstract void LoadConfiguration(XmlReader configuration);
        protected abstract TOutputDataType[] GetOutputData(DataItemBase[] inputDataItems);

        protected void LoadConfigurationElement<T>(XmlDocument configuration, string paramName, out T paramValue, T defaultValue = default(T), bool compulsory = true)
        {
            paramValue = defaultValue;

            var nodes = configuration.GetElementsByTagName(paramName);
            if (nodes == null)
                return;

            if (nodes.Count == 1)
                paramValue = (T)Convert.ChangeType(nodes[0].InnerText, typeof(T));
            else if (nodes.Count >= 2)
                throw new InvalidOperationException($"Ambiguous configuration element name: {paramName} .Number of elements found: {nodes.Count}");
            else if (compulsory)
                throw new Exception($"Missing compulsory configuration element: {paramName}");
        }

        protected virtual DataItemAcknowledgementCallback GetAcknowledgementCallback()
        {
            return null;
        }

        [InputStream(0)]
        public void OnNewDataItems(DataItemBase[] dataItems, bool isLogicalllyGrouped,
            DataItemAcknowledgementCallback acknowledgeCallback, object acknowledgeState,
            DataItemProcessingCompleteCallback completionCallback, object completionState)
        {
            if ((acknowledgeCallback == null && completionCallback != null) || (acknowledgeCallback != null && completionCallback == null))
                throw new ArgumentOutOfRangeException(nameof(acknowledgeCallback), nameof(completionCallback));

            lock (_shutdownLock)
            {
                if (_shutdown)
                    return;

                _logger.WriteLog(TraceEventType.Information, 450, $"Executing probe OnNewDataItems action");

                TOutputDataType[] results = null;

                try
                {
                    results = GetOutputData(dataItems);
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(TraceEventType.Critical, 500, $"Critical exception while trying to retrieve output data \n\n {ex}");

                    acknowledgeCallback?.Invoke(acknowledgeState);
                    completionCallback?.Invoke(completionState);

                    //QUESTION: Not sure if we need to request next dataItem
                    ModuleHost.RequestNextDataItem();

                    throw new ModuleException(ex.Message, ex.InnerException);
                }

                acknowledgeCallback?.Invoke(acknowledgeState);
                completionCallback?.Invoke(completionState);

                //QUESTION: Should the state always be null? 
                if(results.Length > 0)
                    ModuleHost.PostOutputDataItems(results, isLogicalllyGrouped, GetAcknowledgementCallback(), null);

                _logger.WriteLog(TraceEventType.Information, 451, $"Requesting next data item");

                ModuleHost.RequestNextDataItem();
            }
        }

        public override void Shutdown()
        {
            lock (_shutdownLock)
                _shutdown = true;
        }

        public override void Start()
        {
            lock (_shutdownLock)
            {
                if (_shutdown)
                    return;

                ModuleHost.RequestNextDataItem();
            }
        }

        protected virtual void LoadPreviousState(byte[] previousState)
        {
            if (previousState == null)
                return;

            using (var memoryStream = new MemoryStream(previousState))
            {
                var binaryFormatter = new BinaryFormatter();

                try
                {
                    ModuleState = binaryFormatter.Deserialize(memoryStream);
                }
                catch (Exception ex)
                {
                    //_logger.TraceEvent()
                    //Global.logWriteException(e, this);
                    ModuleState = null;
                }
            }
        }

        /// <summary>
        /// Tries to serialize the state object and save it using SCOM Agent API.
        /// </summary>
        /// <param name="state">Module state object to save.</param>
        /// <returns>Returns true if success, false otherwise.</returns>
        protected bool SavePreviousState()
        {
            if (ModuleState == null)
                return false;

            lock (_shutdownLock)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    try
                    {
                        binaryFormatter.Serialize(memoryStream, ModuleState);
                        ModuleHost.SaveState(memoryStream.GetBuffer(), (int)memoryStream.Length);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Global.logWriteException(e, this);
                        return false;
                    }
                }
            }
        }
    }
}
