using CodeAssistant.UI.Utils;
using LLama;
using LLama.Abstractions;
using LLama.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace CodeAssistant.UI.ViewModels
{
    internal class ChatSessionViewModel : ViewModel, IDisposable
    {
        private readonly ObservableCollection<ChatMessageViewModel> _messages = [];
        private readonly RelayCommand _sendCommand;
        private readonly IInferenceParams _inferenceParams;

        private LLamaContext _context;
        private string _userRequest = string.Empty;

        private bool _isActive = true;
        private bool _isGeneratingResponse = false;

        public bool IsActive
        {
            get { return _isActive; }
            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsGeneratingResponse
        {
            get { return _isGeneratingResponse; }
            private set
            {
                if (_isGeneratingResponse != value)
                {
                    _isGeneratingResponse = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Header => "Session";

        public RelayCommand SendCommand => _sendCommand;

        public string UserRequest 
        {
            get => _userRequest; 
            set
            {
                if (_userRequest != value)
                {
                    _userRequest = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public ObservableCollection<ChatMessageViewModel> Messages => _messages;

        public ChatSessionViewModel(LLamaWeights model, IContextParams contextParams, IInferenceParams inferenceParams)
        {
            _inferenceParams = inferenceParams;

            //Init commands
            _sendCommand = new RelayCommand(SendAsync, CanExecuteSend);

            //Init LLM Session
            _context = model.CreateContext(contextParams);
        }

        private async Task SendAsync()
        {
            Messages.Add(new ChatMessageViewModel(_userRequest, true));
            var prompt = _userRequest;
            UserRequest = string.Empty;

            //Generate response
            IsGeneratingResponse = true;
            var currentResult = new ChatMessageViewModel("", false, null);
            Messages.Add(currentResult);
                        
            var executor = new InteractiveExecutor(_context);
            await foreach (var text in executor.InferAsync(prompt, _inferenceParams))
            {
                currentResult.Content += text;
            }
            
            /*
            await foreach (var text in AsyncTextProvider.FetchData())
            {
                currentResult.Content += text;
            }
            */

            currentResult.Timestamp = DateTime.Now;
            IsGeneratingResponse = false;
        }

        private bool CanExecuteSend()
        {
            return !string.IsNullOrWhiteSpace(_userRequest) && IsActive && !IsGeneratingResponse;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
