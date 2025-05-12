using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.SemanticKernel;

namespace AJHTeamsBot;

public class TeamsBot(Kernel kernel) : ActivityHandler
{
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

        // Remove @mention text (for channel messages)
        var text = turnContext.Activity.RemoveRecipientMention()?.Trim().ToLowerInvariant() ?? "";

        string response;

        if (text.StartsWith('!'))
        {
            var parts = text[1..].Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var command = parts.FirstOrDefault() ?? "";
            var arg = parts.Skip(1).FirstOrDefault() ?? "";

            response = command switch
            {
                "help" =>
                    "ℹ️ Try commands like `!status`, `!hello`, or `!build`. I’m here to assist your team.",
                "status" => "✅ All systems are operational.",
                "build" => "🔨 Triggering a build... (not really — just a demo 😄)",
                "ask" => string.IsNullOrWhiteSpace(arg)
                    ? "🤖 Please provide a prompt. Example: `!ask What's the weather?`"
                    : "🤖 "
                        + (
                            (
                                await kernel.InvokePromptAsync(
                                    arg,
                                    cancellationToken: cancellationToken
                                )
                            ).GetValue<string>() ?? "No response."
                        ),
                _ => $"❓ Unknown command: `{command}`. Try `!help` to see available commands.",
            };
        }
        else
        {
            // If not a command, treat as general input
            response = $"🗣️ You said: {text}";
        }

        await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
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