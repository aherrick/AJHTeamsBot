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