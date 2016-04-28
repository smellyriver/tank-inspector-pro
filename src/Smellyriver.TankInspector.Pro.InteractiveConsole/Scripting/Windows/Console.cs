using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Smellyriver.TankInspector.Pro.InteractiveConsole.Scripting.Windows
{
    /// <summary>
    /// Defines a WPF Shell control which can be used to interact with an IScriptingContext. 
    /// </summary>
    /// <remarks>
    /// Any custom styles must provide a TextBox element named 'PART_TextBox', otherwise commands cannot be accepted.
    /// </remarks>
    [TemplatePart(Type = typeof(TextBox), Name = "PART_InputTextBox")]
    public class Console : Control
    {
        public static readonly DependencyProperty CurrentCommandProperty = DependencyProperty.Register("CurrentCommand", typeof(string), typeof(Console), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register("OutputText", typeof(string), typeof(Console), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty OutputDocumentProperty = DependencyProperty.Register("OutputDocument", typeof(FlowDocument), typeof(Console), new UIPropertyMetadata(null));
        public static readonly DependencyProperty OutputFormatterProperty = DependencyProperty.Register("OutputFormatter", typeof(IConsoleOutputFormatter), typeof(Console), new UIPropertyMetadata(null));
        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register("History", typeof(CommandHistory), typeof(Console), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ScriptingContextProperty = DependencyProperty.Register("ScriptingContext", typeof(IScriptingContext), typeof(Console), new UIPropertyMetadata(null, Console.ScriptingContextPropertySet));
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(ConsoleMode), typeof(Console), new UIPropertyMetadata(ConsoleMode.Immediate));
        public static readonly DependencyProperty PromptForegroundProperty = DependencyProperty.Register("PromptForeground", typeof(Brush), typeof(Console), new UIPropertyMetadata(null));
        public static readonly DependencyProperty OutputColorMappingsProperty = DependencyProperty.Register("OutputColorMappings", typeof(ConsoleColorMappings), typeof(Console), new UIPropertyMetadata(null));
        public static readonly DependencyProperty HasOutputProperty = DependencyProperty.Register("HasOutput", typeof(bool), typeof(Console), new UIPropertyMetadata(false));
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(Console), new UIPropertyMetadata(TextWrapping.NoWrap));
        private readonly CallbackScriptOutputWriter _writer;
        private readonly Paragraph _outputParagraph;
        private TextBox _inputTextBox;

        /// <summary>
        /// Initializes the <see cref="Console"/> class.
        /// </summary>
        static Console()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Console), new FrameworkPropertyMetadata(typeof(Console)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Console"/> class.
        /// </summary>
        public Console()
        {
            CommandBindings.Add(new CommandBinding(ConsoleCommands.Clear, HandleClearCommand));
            CommandBindings.Add(new CommandBinding(ConsoleCommands.Execute, HandleExecuteCommand));
            CommandBindings.Add(new CommandBinding(ConsoleCommands.CommandHistoryUp, HandleHistoryUpCommand));
            CommandBindings.Add(new CommandBinding(ConsoleCommands.CommandHistoryDown, HandleHistoryDownCommand));

            OutputDocument = new FlowDocument(_outputParagraph = new Paragraph());
            History = new CommandHistory();
            OutputFormatter = new ConsoleOutputFormatter { DelayNewlines = true };

            _writer = new CallbackScriptOutputWriter(
                OutputWriter_OutputWritten,
                OutputWriter_Clear
                );
        }

        /// <summary>
        /// Gets or sets the mode of the Shell.
        /// </summary>
        /// <value>The mode.</value>
        public ConsoleMode Mode
        {
            get { return (ConsoleMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current command. This property is intended to be bound to the input text box.
        /// </summary>
        /// <value>The current command.</value>
        public string CurrentCommand
        {
            get { return (string)GetValue(CurrentCommandProperty); }
            set { SetValue(CurrentCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the output text.
        /// </summary>
        /// <value>The output.</value>
        public string OutputText
        {
            get { return (string)GetValue(OutputTextProperty); }
            set { SetValue(OutputTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the output document.
        /// </summary>
        /// <value>The output document.</value>
        public FlowDocument OutputDocument
        {
            get { return (FlowDocument)GetValue(OutputDocumentProperty); }
            set { SetValue(OutputDocumentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command history.
        /// </summary>
        /// <value>The history.</value>
        public CommandHistory History
        {
            get { return (CommandHistory)GetValue(HistoryProperty); }
            set { SetValue(HistoryProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scripting context.
        /// </summary>
        /// <value>The scripting context.</value>
        public IScriptingContext ScriptingContext
        {
            get { return (IScriptingContext)GetValue(ScriptingContextProperty); }
            set { SetValue(ScriptingContextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the prompt foreground brush.
        /// </summary>
        /// <value>The prompt foreground.</value>
        public Brush PromptForeground
        {
            get { return (Brush)GetValue(PromptForegroundProperty); }
            set { SetValue(PromptForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the shell color mappings.
        /// </summary>
        /// <value>The shell color mappings.</value>
        public ConsoleColorMappings OutputColorMappings
        {
            get { return (ConsoleColorMappings)GetValue(OutputColorMappingsProperty); }
            set { SetValue(OutputColorMappingsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the shell output formatter.
        /// </summary>
        /// <value>The shell output formatter.</value>
        public IConsoleOutputFormatter OutputFormatter
        {
            get { return (IConsoleOutputFormatter)GetValue(OutputFormatterProperty); }
            set { SetValue(OutputFormatterProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has output.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has output; otherwise, <c>false</c>.
        /// </value>
        public bool HasOutput
        {
            get { return (bool)GetValue(HasOutputProperty); }
            set { SetValue(HasOutputProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text wrapping.
        /// </summary>
        /// <value>The text wrapping.</value>
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _inputTextBox = Template.FindName("PART_InputTextBox", this) as TextBox;
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Handles the Execute command.
        /// </summary>
        private void HandleExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ScriptingContext.VerifyCommand(CurrentCommand);
            var command = CurrentCommand;
            CurrentCommand = string.Empty;
            ScriptingContext.ExecuteCommand(command);
            History.Add(command);
        }

        /// <summary>
        /// Handles the HistoryUp command.
        /// </summary>
        private void HandleHistoryUpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentCommand = History.Up();
            if (_inputTextBox != null) _inputTextBox.SelectionStart = _inputTextBox.Text.Length;
        }

        /// <summary>
        /// Handles the HistoryDown command.
        /// </summary>
        private void HandleHistoryDownCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentCommand = History.Down();
            if (_inputTextBox != null) _inputTextBox.SelectionStart = _inputTextBox.Text.Length;
        }

        /// <summary>
        /// Handles the Clear command.
        /// </summary>
        private void HandleClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentCommand = string.Empty;
        }

        private void OutputWriter_OutputWritten(string text)
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    var formattedOutput = OutputFormatter.Format(text, _writer.ForegroundColor, OutputColorMappings);
                    OutputText += formattedOutput.Text;
                    _outputParagraph.Inlines.AddRange(formattedOutput.Inlines);
                    HasOutput = true;
                }));
        }

        private void OutputWriter_Clear()
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    OutputFormatter.Clear();
                    OutputText = string.Empty;
                    _outputParagraph.Inlines.Clear();
                    HasOutput = false;
                }));
        }

        /// <summary>
        /// Occurs when the ScriptingContext is set, as an opportunity to wire up the output writer to the scripting context.
        /// </summary>
        private static void ScriptingContextPropertySet(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var shell = (Console)target;
            if (e.NewValue != null) ((IScriptingContext)e.NewValue).AddOutputWriter(shell._writer);
            if (e.OldValue != null) ((IScriptingContext)e.OldValue).RemoveOutputWriter(shell._writer);
        }

        public void FocusOnInput()
        {
            _inputTextBox.Focus();
        }
    }
}
