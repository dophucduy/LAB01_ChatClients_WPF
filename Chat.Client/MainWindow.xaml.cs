using Chat.Client.Services;
using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chat.Client
{
    public partial class MainWindow : Window
    {
        private ChatService chatService = new ChatService();
        private int _messageCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            chatService.OnMessageReceived += DisplayMessage;
            chatService.OnUserListUpdated += UpdateUserList;
            chatService.OnError += ShowError;
            chatService.OnReactionsUpdated += UpdateMessageReactions;
            chatService.OnLoadAllReactions += LoadAllReactions;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username");
                return;
            }

            bool success = await chatService.ConnectAsync(username);
            if (success)
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ChatPanel.Visibility = Visibility.Visible;
                AddSystemMessage($"Connected as {username}. Welcome to Internal Chat!");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageInput.Text))
            {
                await chatService.SendMessage(MessageInput.Text);
                MessageInput.Clear();
            }
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendButton_Click(sender, e);
        }

        private void EmojiButton_Click(object sender, RoutedEventArgs e)
        {
            MessageInput.Text += "😊";
            MessageInput.CaretIndex = MessageInput.Text.Length;
        }

        private void DisplayMessage(JsonElement messageElement)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {

                    // Parse JsonElement
                    int id = 0;
                    if (messageElement.TryGetProperty("id", out var idElement))
                    {
                        id = idElement.GetInt32();
                    }

                    string user = "Unknown";
                    if (messageElement.TryGetProperty("user", out var userElement))
                    {
                        user = userElement.GetString();
                    }

                    string content = "";
                    if (messageElement.TryGetProperty("content", out var contentElement))
                    {
                        content = contentElement.GetString();
                    }

                    DateTime timestamp = DateTime.Now;

                    if (messageElement.TryGetProperty("timestamp", out var timestampElement))
                    {
                        string timestampStr = timestampElement.GetString();
                        if (!string.IsNullOrEmpty(timestampStr) && DateTime.TryParse(timestampStr, out var parsedTime))
                        {
                            timestamp = parsedTime.ToLocalTime();
                        }
                    }

                    bool isSystem = false;
                    if (messageElement.TryGetProperty("is_System", out var isSystemElement))
                    {
                        isSystem = isSystemElement.GetBoolean();
                    }


                    AddChatMessage(id, user, content, timestamp, isSystem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error parsing message: {ex.Message}");
                }
            });
        }


        private void AddChatMessage(int messageId, string user, string content, DateTime timestamp, bool isSystem = false)
        {
            var messageBorder = new Border
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(5),
                Margin = new Thickness(0, 2, 0, 2),
                Tag = messageId // Store message ID for reactions
            };

            var messageStack = new StackPanel();

            // Message header
            var headerText = new TextBlock
            {
                Text = $"[{timestamp:HH:mm:ss}] {user}:",
                FontWeight = isSystem ? FontWeights.Bold : FontWeights.Normal,
                Foreground = isSystem ? Brushes.Blue : Brushes.Black
            };

            // Message content
            var contentText = new TextBlock
            {
                Text = content,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 2, 0, 5)
            };

            messageStack.Children.Add(headerText);
            messageStack.Children.Add(contentText);

            // Reactions panel
            var reactionsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 0),
                Tag = $"ReactionsPanel_{messageId}"
            };

            // Reaction buttons
            var likeButton = CreateReactionButton("👍", messageId);
            var heartButton = CreateReactionButton("❤️", messageId);
            var laughButton = CreateReactionButton("😂", messageId);

            reactionsPanel.Children.Add(likeButton);
            reactionsPanel.Children.Add(heartButton);
            reactionsPanel.Children.Add(laughButton);

            // Reaction counts display
            var reactionCounts = new TextBlock
            {
                Name = $"ReactionCounts_{messageId}",
                Margin = new Thickness(10, 0, 0, 0),
                Text = "👍 0 ❤️ 0 😂 0"
            };

            reactionsPanel.Children.Add(reactionCounts);
            messageStack.Children.Add(reactionsPanel);
            messageBorder.Child = messageStack;

            ChatDisplayPanel.Children.Add(messageBorder);
            ChatScroll.ScrollToEnd();
        }

        private Button CreateReactionButton(string emoji, int messageId)
        {
            var button = new Button
            {
                Content = emoji,
                Width = 30,
                Height = 30,
                Margin = new Thickness(2, 0, 2, 0),
                Tag = new { MessageId = messageId, Emoji = emoji }
            };

            // Store message ID in button's Tag property properly
            button.Tag = new Tuple<int, string>(messageId, emoji);
            button.Click += ReactionButton_Click;
            return button;
        }

        private async void ReactionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Tuple<int, string> tag)
            {
                int messageId = tag.Item1;
                string reactionType = tag.Item2;
                await chatService.ReactToMessage(messageId, reactionType);
            }
        }

        private void UpdateMessageReactions(int messageId, System.Collections.Generic.Dictionary<string, int> reactions)
        {
            Dispatcher.Invoke(() =>
            {
                var reactionCountsText = $"👍 {reactions.GetValueOrDefault("👍", 0)} " +
                                       $"❤️ {reactions.GetValueOrDefault("❤️", 0)} " +
                                       $"😂 {reactions.GetValueOrDefault("😂", 0)}";

                // Find the corresponding TextBlock and update it
                foreach (UIElement element in ChatDisplayPanel.Children)
                {
                    if (element is Border border && border.Tag is int msgId && msgId == messageId)
                    {
                        // Find reactions panel within this message
                        if (border.Child is StackPanel mainStack)
                        {
                            foreach (var child in mainStack.Children)
                            {
                                if (child is StackPanel reactionPanel)
                                {
                                    foreach (var subChild in reactionPanel.Children)
                                    {
                                        if (subChild is TextBlock textBlock && textBlock.Name == $"ReactionCounts_{messageId}")
                                        {
                                            textBlock.Text = reactionCountsText;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        private void AddSystemMessage(string message)
        {
            var systemBorder = new Border
            {
                BorderBrush = Brushes.LightBlue,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(5),
                Background = Brushes.LightBlue
            };

            var systemText = new TextBlock
            {
                Text = $"[SYSTEM] {message}",
                FontStyle = FontStyles.Italic
            };

            systemBorder.Child = systemText;
            ChatDisplayPanel.Children.Add(systemBorder);
            ChatScroll.ScrollToEnd();
        }

        private void UpdateUserList(System.Collections.Generic.List<string> users)
        {
            Dispatcher.Invoke(() =>
            {
                UserList.ItemsSource = users;
            });
        }

        private void ShowError(string error)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(error, "Chat Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await chatService.DisconnectAsync();
            LoginPanel.Visibility = Visibility.Visible;
            ChatPanel.Visibility = Visibility.Collapsed;
            ChatDisplayPanel.Children.Clear();
            UserList.ItemsSource = null;
        }

        private void LoadAllReactions(Dictionary<int, Dictionary<string, int>> allReactions)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var messageReactions in allReactions)
                {
                    UpdateMessageReactions(messageReactions.Key, messageReactions.Value);
                }
            });
        }

        protected override async void OnClosed(EventArgs e)
        {
            await chatService.DisconnectAsync();
            base.OnClosed(e);
        }
    }
}
