using System.Text;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AJHTeamsBot;

public class TeamsBot(Kernel kernel) : ActivityHandler
{
    //protected override async Task OnMessageActivityAsync(
    //    ITurnContext<IMessageActivity> turnContext,
    //    CancellationToken cancellationToken
    //)
    //{
    //    // Show typing indicator
    //    await turnContext.SendActivityAsync(
    //        new Activity { Type = ActivityTypes.Typing },
    //        cancellationToken
    //    );

    //    // Remove @mention text (for channel messages)
    //    var text = turnContext.Activity.RemoveRecipientMention()?.Trim().ToLowerInvariant() ?? "";

    //    string response;

    //    if (text.StartsWith('!'))
    //    {
    //        var parts = text[1..].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    //        var command = parts.FirstOrDefault() ?? "";
    //        var arg = parts.Skip(1).FirstOrDefault() ?? "";

    //        response = command switch
    //        {
    //            "help" =>
    //                "ℹ️ Try commands like `!status`, `!hello`, or `!build`. I’m here to assist your team.",
    //            "status" => "✅ All systems are operational.",
    //            "build" => "🔨 Triggering a build... (not really — just a demo 😄)",
    //            "ask" => string.IsNullOrWhiteSpace(arg)
    //                ? "🤖 Please provide a prompt. Example: `!ask What's the weather?`"
    //                : "🤖 "
    //                    + (
    //                        (
    //                            await kernel.InvokePromptAsync(
    //                                arg,
    //                                cancellationToken: cancellationToken
    //                            )
    //                        ).GetValue<string>() ?? "No response."
    //                    ),
    //            _ => $"❓ Unknown command: `{command}`. Try `!help` to see available commands.",
    //        };
    //    }
    //    else
    //    {
    //        // If not a command, treat as general input
    //        response = $"🗣️ You said: {text}";
    //    }

    //    await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
    //}

    /// <summary>
    ///  activity
    /// </summary>
    /// <param name="turnContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    //protected override async Task OnMessageActivityAsync(
    //    ITurnContext<IMessageActivity> turnContext,
    //    CancellationToken cancellationToken
    //)
    //{
    //    var text = turnContext.Activity.RemoveRecipientMention()?.Trim().ToLowerInvariant() ?? "";
    //    var parts = text.StartsWith('!')
    //        ? text[1..].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)
    //        : [];
    //    var command = parts.FirstOrDefault() ?? "";
    //    var arg = parts.Skip(1).FirstOrDefault() ?? "";

    //    if (command == "ask" && !string.IsNullOrWhiteSpace(arg))
    //    {
    //        // Create initial card with "🤖 AI says: " in TextBlock
    //        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 4))
    //        {
    //            Body =
    //            {
    //                new AdaptiveTextBlock
    //                {
    //                    Id = "responseText",
    //                    Text = "🤖 AI says: ",
    //                    Wrap = true,
    //                    Weight = AdaptiveTextWeight.Default,
    //                    Size = AdaptiveTextSize.Default,
    //                },
    //            },
    //        };

    //        var reply = MessageFactory.Attachment(
    //            new Attachment { ContentType = AdaptiveCard.ContentType, Content = card }
    //        );

    //        var activity = await turnContext.SendActivityAsync(reply, cancellationToken);

    //        // Stream updates, appending only the AI output
    //        await foreach (
    //            var update in kernel.InvokePromptStreamingAsync(
    //                arg,
    //                cancellationToken: cancellationToken
    //            )
    //        )
    //        {
    //            // Append only the new AI output to the TextBlock
    //            ((AdaptiveTextBlock)card.Body[0]).Text += update.ToString();

    //            var updatedActivity = MessageFactory.Attachment(
    //                new Attachment { ContentType = AdaptiveCard.ContentType, Content = card }
    //            );
    //            updatedActivity.Id = activity.Id;

    //            await turnContext.UpdateActivityAsync(updatedActivity, cancellationToken);
    //        }
    //    }
    //    else
    //    {
    //        var response = command switch
    //        {
    //            "help" =>
    //                "ℹ️ Try commands like `!status`, `!hello`, or `!build`. I’m here to assist your team.",
    //            "status" => "✅ All systems are operational.",
    //            "build" => "🔨 Triggering a build... (not really — just a demo 😄)",
    //            "" => $"🗣️ You said: {text}",
    //            _ => $"❓ Unknown command: `{command}`. Try `!help` to see available commands.",
    //        };

    //        await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
    //    }
    //}

    //protected override async Task OnMessageActivityAsync(
    //    ITurnContext<IMessageActivity> turnContext,
    //    CancellationToken cancellationToken
    //)
    //{
    //    // Show typing indicator
    //    await turnContext.SendActivityAsync(
    //        new Activity { Type = ActivityTypes.Typing },
    //        cancellationToken
    //    );

    //    var text = turnContext.Activity.RemoveRecipientMention()?.Trim().ToLowerInvariant() ?? "";
    //    var parts = text.StartsWith('!')
    //        ? text[1..].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)
    //        : [];
    //    var command = parts.FirstOrDefault() ?? "";
    //    var arg = parts.Skip(1).FirstOrDefault() ?? "";

    //    if (command == "ask" && !string.IsNullOrWhiteSpace(arg))
    //    {
    //        // Create initial card with "🤖 AI says: " in TextBlock
    //        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 4))
    //        {
    //            Body =
    //            {
    //                new AdaptiveTextBlock
    //                {
    //                    Id = "responseText",
    //                    Text = "🤖 AI says: ",
    //                    Wrap = true,
    //                    Weight = AdaptiveTextWeight.Default,
    //                    Size = AdaptiveTextSize.Default,
    //                },
    //            },
    //        };

    //        var reply = MessageFactory.Attachment(
    //            new Attachment { ContentType = AdaptiveCard.ContentType, Content = card }
    //        );

    //        ResourceResponse activity = await turnContext.SendActivityAsync(
    //            reply,
    //            cancellationToken
    //        );

    //        // Stream updates, batching to reduce frequency
    //        StringBuilder accumulatedOutput = new();
    //        int updateCount = 0;
    //        const int batchSize = 5; // Update every 5 chunks

    //        try
    //        {
    //            await foreach (
    //                var update in kernel.InvokePromptStreamingAsync(
    //                    arg,
    //                    cancellationToken: cancellationToken
    //                )
    //            )
    //            {
    //                // Append only the new AI output
    //                accumulatedOutput.Append(update.ToString());
    //                updateCount++;

    //                // Update card every 'batchSize' chunks
    //                if (updateCount % batchSize == 0)
    //                {
    //                    ((AdaptiveTextBlock)card.Body[0]).Text = "🤖 AI says: " + accumulatedOutput;

    //                    var updatedActivity = MessageFactory.Attachment(
    //                        new Attachment
    //                        {
    //                            ContentType = AdaptiveCard.ContentType,
    //                            Content = card,
    //                        }
    //                    );
    //                    updatedActivity.Id = activity.Id;

    //                    try
    //                    {
    //                        await turnContext.UpdateActivityAsync(
    //                            updatedActivity,
    //                            cancellationToken
    //                        );
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        StringBuilder errorDetails = new StringBuilder();
    //                        errorDetails.AppendLine(
    //                            $"Error updating card, text length: {((AdaptiveTextBlock)card.Body[0]).Text.Length}, Activity ID: {activity.Id}"
    //                        );
    //                        errorDetails.AppendLine($"Error: {ex.Message}");
    //                        if (ex.InnerException != null)
    //                        {
    //                            errorDetails.AppendLine(
    //                                $"Inner Exception: {ex.InnerException.Message}"
    //                            );
    //                        }
    //                        errorDetails.AppendLine($"Stack Trace: {ex.StackTrace}");

    //                        await turnContext.SendActivityAsync(
    //                            MessageFactory.Text(errorDetails.ToString()),
    //                            cancellationToken
    //                        );
    //                        return;
    //                    }
    //                }
    //            }

    //            // Final update to ensure all content is sent
    //            if (accumulatedOutput.Length > 0)
    //            {
    //                ((AdaptiveTextBlock)card.Body[0]).Text = "🤖 AI says: " + accumulatedOutput;

    //                var updatedActivity = MessageFactory.Attachment(
    //                    new Attachment { ContentType = AdaptiveCard.ContentType, Content = card }
    //                );
    //                updatedActivity.Id = activity.Id;

    //                try
    //                {
    //                    await turnContext.UpdateActivityAsync(updatedActivity, cancellationToken);
    //                }
    //                catch (Exception ex)
    //                {
    //                    StringBuilder errorDetails = new StringBuilder();
    //                    errorDetails.AppendLine(
    //                        $"Error sending final card update, text length: {((AdaptiveTextBlock)card.Body[0]).Text.Length}, Activity ID: {activity.Id}"
    //                    );
    //                    errorDetails.AppendLine($"Error: {ex.Message}");
    //                    if (ex.InnerException != null)
    //                    {
    //                        errorDetails.AppendLine(
    //                            $"Inner Exception: {ex.InnerException.Message}"
    //                        );
    //                    }
    //                    errorDetails.AppendLine($"Stack Trace: {ex.StackTrace}");

    //                    await turnContext.SendActivityAsync(
    //                        MessageFactory.Text(errorDetails.ToString()),
    //                        cancellationToken
    //                    );
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            StringBuilder errorDetails = new();
    //            errorDetails.AppendLine(
    //                $"Error during AI streaming for input: {arg}, text length: {accumulatedOutput.Length}"
    //            );
    //            errorDetails.AppendLine($"Error: {ex.Message}");
    //            if (ex.InnerException != null)
    //            {
    //                errorDetails.AppendLine($"Inner Exception: {ex.InnerException.Message}");
    //            }
    //            errorDetails.AppendLine($"Stack Trace: {ex.StackTrace}");

    //            await turnContext.SendActivityAsync(
    //                MessageFactory.Text(errorDetails.ToString()),
    //                cancellationToken
    //            );
    //        }
    //    }
    //    else
    //    {
    //        var response = command switch
    //        {
    //            "help" =>
    //                "ℹ️ Try commands like `!status`, `!hello`, or `!build`. I’m here to assist your team.",
    //            "status" => "✅ All systems are operational.",
    //            "build" => "🔨 Triggering a build... (not really — just a demo 😄)",
    //            "" => $"🗣️ You said: {text}",
    //            _ => $"❓ Unknown command: `{command}`. Try `!help` to see available commands.",
    //        };

    //        await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
    //    }
    //}

    // In-memory storage for user chat history (use a database in production)
    private static readonly Dictionary<string, ChatHistory> UserChatHistory = [];

    private const int MaxHistoryLength = 10;

    protected override async Task OnMessageActivityAsync(
        ITurnContext<IMessageActivity> turnContext,
        CancellationToken cancellationToken
    )
    {
        // Show typing indicator
        await turnContext.SendActivityAsync(
            new Activity { Type = ActivityTypes.Typing },
            cancellationToken
        );

        var userId = turnContext.Activity.From.Id; // Unique user identifier
        var text = turnContext.Activity.RemoveRecipientMention()?.Trim().ToLowerInvariant() ?? "";
        var parts = text.StartsWith('!')
            ? text[1..].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)
            : [];
        var command = parts.FirstOrDefault() ?? "";
        var arg = parts.Skip(1).FirstOrDefault() ?? "";

        if (command == "ask" && !string.IsNullOrWhiteSpace(arg))
        {
            // Initialize user history if not exists
            if (!UserChatHistory.TryGetValue(userId, out ChatHistory chatHistory))
            {
                UserChatHistory[userId] = chatHistory = [];
            }

            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 4))
            {
                Body =
                {
                    new AdaptiveTextBlock
                    {
                        Id = "responseText",
                        Text = "🤖 ",
                        Wrap = true,
                        Weight = AdaptiveTextWeight.Default,
                        Size = AdaptiveTextSize.Default,
                    },
                },
            };

            var reply = MessageFactory.Attachment(
                new Attachment { ContentType = AdaptiveCard.ContentType, Content = card }
            );

            var activity = await turnContext.SendActivityAsync(reply, cancellationToken);

            StringBuilder accumulatedOutput = new();
            int updateCount = 0;
            const int batchSize = 5;

            chatHistory.AddUserMessage(arg);

            var chatCompletionService =
                kernel.Services.GetRequiredService<IChatCompletionService>();

            async Task UpdateCardAsync(string chunk)
            {
                ((AdaptiveTextBlock)card.Body[0]).Text += chunk;

                var updatedActivity = MessageFactory.Attachment(
                    new Attachment { ContentType = AdaptiveCard.ContentType, Content = card }
                );
                updatedActivity.Id = activity.Id;

                await turnContext.UpdateActivityAsync(updatedActivity, cancellationToken);
            }

            string lastPushedText = string.Empty;

            await foreach (
                var update in chatCompletionService.GetStreamingChatMessageContentsAsync(
                    chatHistory,
                    kernel: kernel,
                    cancellationToken: cancellationToken
                )
            )
            {
                accumulatedOutput.Append(update.Content ?? string.Empty);
                updateCount++;

                if (updateCount % batchSize == 0)
                {
                    var newText = accumulatedOutput.ToString()[lastPushedText.Length..];
                    await UpdateCardAsync(newText);
                    lastPushedText = accumulatedOutput.ToString();
                }
            }

            // Final update
            var finalText = accumulatedOutput.ToString()[lastPushedText.Length..];
            await UpdateCardAsync(finalText);

            chatHistory.AddAssistantMessage(accumulatedOutput.ToString());

            while (chatHistory.Count > MaxHistoryLength)
            {
                chatHistory.RemoveAt(0);
            }
        }
        else
        {
            var response = command switch
            {
                "help" =>
                    "ℹ️ Try commands like `!status`, `!ask {query}`, or `!build`. I’m here to assist your team.",
                "status" => "✅ All systems are operational.",
                "build" => "🔨 Triggering a build... (not really — just a demo 😄)",
                "" => $"🗣️ You said: {text}",
                _ => $"❓ Unknown command: `{command}`. Try `!help` to see available commands.",
            };

            await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
        }
    }

    protected override async Task OnMembersAddedAsync(
        IList<ChannelAccount> membersAdded,
        ITurnContext<IConversationUpdateActivity> turnContext,
        CancellationToken cancellationToken
    )
    {
        var conversationType = turnContext.Activity.Conversation.ConversationType;
        var isChannel = conversationType == "channel";
        var welcomeText = isChannel
            ? "👋 Hello everyone! I'm your helpful bot. Mention me or say 'hi' to get started."
            : "👋 Hi! Thanks for adding me — type 'hello' or ask me anything to begin.";

        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text(welcomeText),
                    cancellationToken
                );
                break; // Only send once
            }
        }
    }
}