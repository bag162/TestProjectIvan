
namespace FileParserService.QueueManager
{
	public interface IQueueService
	{
		Task SendMessage(object obj);
		Task SendMessage(string message);
	}
}
