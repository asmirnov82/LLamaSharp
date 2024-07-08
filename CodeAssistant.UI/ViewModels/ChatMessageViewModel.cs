using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAssistant.UI.ViewModels
{
    internal class ChatMessageViewModel : ViewModel
    {
        private string _content = String.Empty;
        private DateTime? _timeStamp;
        private bool _isUserInput;

        public ChatMessageViewModel(string content, bool isUserInput)
        {
            _content = content;
            _isUserInput = isUserInput;
            _timeStamp = DateTime.Now;
        }

        public ChatMessageViewModel(string content, bool isUserInput, DateTime? dateTime)
        {
            _content = content;
            _isUserInput = isUserInput;
            _timeStamp = dateTime;
        }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsUserInput
        {
            get { return _isUserInput; }
            set
            { 
                _isUserInput = value;
                NotifyPropertyChanged(); 
            }
        }

        public DateTime? Timestamp
        {
            get => _timeStamp;
            set
            {
                _timeStamp = value;
                NotifyPropertyChanged();
            }
        }
        

        
    }
}
