using System;
using EasyStorage;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using Lapins.Engine.Core;

namespace Lapins.Engine.Storage
{
    /// <summary>
    /// Multi-platform savegame handler (with EasyStorage Lib)
    /// </summary>
    public class Saver<TSave>
    {
        /// <summary>
        /// Save file name
        /// </summary>
        private string _filename;

        /// <summary>
        /// Directory name
        /// </summary>
        private string _containerName;

        /// <summary>
        /// EasyStorage manager
        /// </summary>
        private IAsyncSaveDevice _saveDevice;

        /// <summary>
        /// Serializer
        /// </summary>
        private XmlSerializer _serializer;

        /// <summary>
        /// Current savegame
        /// </summary>
        private TSave _data;

        /// <summary>
        /// Raised when the save is completed
        /// </summary>
        public event Action<TSave> SaveCompleted;

        /// <summary>
        /// Raised when the load is completed
        /// </summary>
        public event Action<TSave> LoadCompleted;

        public Saver(Application instance, string filename, string contrainerName)
        {
            _filename = filename;
            _containerName = contrainerName;
            _serializer = new XmlSerializer(typeof(TSave));

            #region EasyStorage Init

            EasyStorageSettings.SetSupportedLanguages(Language.French, Language.Spanish, Language.English, Language.German, Language.Japanese, Language.Italian);

            //Windows Phone7: IsolatedStorage
#if WINDOWS_PHONE
            saveDevice = new IsolatedStorageSaveDevice();
#else
            //Xbox & PC: SharedDevice
            //-- Create
            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            instance.Components.Add(sharedSaveDevice);

            _saveDevice = sharedSaveDevice;

            //-- Events on Xbox
            // Force a player to choose a storage location on current storage disconnection
            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            // Ask the player to choose
            sharedSaveDevice.PromptForDevice();
#endif

            // End of savegame events
            _saveDevice.SaveCompleted += new SaveCompletedEventHandler(saveDevice_SaveCompleted);
            _saveDevice.LoadCompleted += new LoadCompletedEventHandler(saveDevice_LoadCompleted);

            #endregion
        }

        /// <summary>
        /// Register an empty save data (or reset the current one)
        /// </summary>
        /// <param name="emptySave"></param>
        public void Initialize(TSave emptySave)
        {
            _data = emptySave;
            ForceSaveAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void saveDevice_SaveCompleted(object sender, FileActionCompletedEventArgs args)
        {
            if (SaveCompleted != null) SaveCompleted(_data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void saveDevice_LoadCompleted(object sender, FileActionCompletedEventArgs args)
        {
            if (LoadCompleted != null) LoadCompleted(_data);
        }

        /// <summary>
        /// Save asynchronously the data
        /// </summary>
        /// <returns></returns>
        public bool SaveAsync()
        {
            if (_saveDevice.IsReady)
            {
                _saveDevice.SaveAsync(
                     _containerName,
                     _filename,
                    stream =>
                    {
                        XmlWriterSettings settings = new XmlWriterSettings()
                        {
                            Indent = true,
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            SerializeSave(writer);
                        }

                    });

                return true;
            }

            return false;
        }

        /// <summary>
        /// Load asynchronously the data
        /// </summary>
        /// <returns></returns>
        public bool LoadAsync()
        {
            if (_saveDevice.IsReady)
            {
                _saveDevice.LoadAsync(
                    _containerName,
                    _filename,
                   stream =>
                   {
                       using (XmlReader reader = XmlReader.Create(stream))
                       {
                           DeserializeSave(reader);
                       }
                   });

                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to save until it works
        /// </summary>
        /// <returns></returns>
        public void ForceSaveAsync()
        {
            new Thread(new ThreadStart(
              delegate()
              {
                  while (this.SaveAsync() == false) ;
              }
                )
             ).Start();
        }

        /// <summary>
        /// Try to load until it works
        /// </summary>
        /// <returns></returns>
        public void ForceLoad()
        {
            while (this.LoadAsync() == false) ;
        }

        /// <summary>
        /// Try to load until it works
        /// </summary>
        /// <returns></returns>
        public void ForceLoadAsync()
        {
            new Thread(new ThreadStart(
              delegate()
              {
                  while (this.LoadAsync() == false) ;
              }
                )
             ).Start();
        }

        /// <summary>
        /// Savegame serialization
        /// </summary>
        /// <param name="writer"></param>
        private void SerializeSave(XmlWriter writer)
        {
            _serializer.Serialize(writer, _data);
        }

        /// <summary>
        /// Savegame deserialization
        /// </summary>
        /// <param name="reader"></param>
        private void DeserializeSave(XmlReader reader)
        {
            var data = _serializer.Deserialize(reader);
            if (data != null)
            {
                _data = (TSave)data;
            }
        }

        /// <summary>
        /// Current Savegame
        /// </summary>
        public TSave Data
        {
            get { return _data; }
        }
    }
}
