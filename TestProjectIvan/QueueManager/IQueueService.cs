
namespace FileParserService.QueueManager
{
	public interface IQueueService
	{
		Task SendMessageAsync(object obj);
		Task SendMessageAsync(string message);
	}
}
