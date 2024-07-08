using CodeAssistant.UI.Utils;
using LLama;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LLama.Common.ChatHistory;

namespace CodeAssistant.UI.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly RelayCommand _initModelCommand;
        private readonly RelayCommand _newSessionCommand;
        private readonly ObservableCollection<ChatSessionViewModel> _chatSessions;
        private int _selectedTabIndex;

        private LLamaWeights? _llmModel;
        
        private bool _isModelLoading = false;
        private bool _isModelLoaded = false;
        private int _modelLoadingProgress;
        
        public bool IsModelLoaded 
        {
            get { return _isModelLoaded; }
            set
            {
                if (_isModelLoaded != value)
                {
                    _isModelLoaded = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public bool IsModelLoading
        {
            get { return _isModelLoading; }
            set
            {
                if (_isModelLoading != value)
                {
                    _isModelLoading = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ModelLoadingProgress 
        {
            get { return _modelLoadingProgress; }
            set
            {
                _modelLoadingProgress = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ChatSessionViewModel> ChatSessions => _chatSessions;

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public RelayCommand InitModelCommand => _initModelCommand;
        public RelayCommand NewSessionCommand => _newSessionCommand;

        public MainWindowViewModel()
        {
            _chatSessions = new ObservableCollection<ChatSessionViewModel>();

            _initModelCommand = new RelayCommand(InitModelAsync, CanExecuteInitModel);
            _newSessionCommand = new RelayCommand(NewSessionAsync, CanExecuteNewSession);
        }

        private async Task InitModelAsync()
        {
            string modelPath = @"C:\ML\Models\Phi-3-mini-4k\GGUF\Phi-3-mini-4k-instruct-q4.gguf";

            var parameters = new ModelParams(modelPath)
            {
                Seed = 1337,
                GpuLayerCount = 10
            };

            //Load model
            IsModelLoading = true;
            _llmModel = await LLamaWeights.LoadFromFileAsync(parameters, progressReporter: new Progress<float>(x => ModelLoadingProgress = (int)(x * 100)));
            IsModelLoading = false;
            IsModelLoaded = true;
        }

        private bool CanExecuteInitModel()
        {
            return !(IsModelLoaded || IsModelLoading);
        }

        private async Task NewSessionAsync()
        {
            if (_llmModel == null)
                return;

            var contextParameters = new ModelParams(@"C:\ML\Models\Phi-3-mini-4k\GGUF\Phi-3-mini-4k-instruct-q4.gguf")
            {
            };

            _chatSessions.Add(new ChatSessionViewModel(_llmModel, contextParameters, new InferenceParams()));
            SelectedTabIndex = _chatSessions.Count - 1;
        }

        private bool CanExecuteNewSession()
        {
            return IsModelLoaded;
        }
    }
}
