using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace AJHTeamsBot;

public class TeamsBot : ActivityHandler
{
    protected override async Task OnMessageActivityAsync(
        ITurnContext<IMessageActivity> turnContext,
        CancellationToken cancellationToken
    )
    {
        // Remove @mention text (for channel messages)
        var text = turnContext.Activity.RemoveRecipientMention()?.Trim().ToLowerInvariant() ?? "";

        string response;

        // Check if the message starts with a command prefix
        if (text.StartsWith('!'))
        {
            // Extract the command (remove the '/')
            var command = text[1..];

            response = command switch
            {
                "hi" or "hello" => "👋 Hi there! I'm your Teams bot. How can I help you?",
                "help" =>
                    "ℹ️ Try commands like `!status`, `!hello`, or `!build`. I’m here to assist your team.",
                "status" => "✅ All systems are operational.",
                "build" => "🔨 Triggering a build... (not really — just a demo 😄)",
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