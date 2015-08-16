using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ViData;

namespace ViCommV2.Interfaces
{
    public interface IChatWindow
    {
        TextBox GetInputTextBox();
        ScrollViewer GetEmoticonsContainer();
        object FindResource(object resourceKey);
    }
}
