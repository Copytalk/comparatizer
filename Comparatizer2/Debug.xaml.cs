﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Comparatizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow(Logger logger)
        {
            InitializeComponent();

            logger.SetWriteToOutput(true);
            logger.SetOutput(tbxDebugOutput);
        }

        private void Debug_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
