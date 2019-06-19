using EasyNetQ;
using EasyNetQ.SystemMessages;
using EasyNetQ.Topology;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 本Demo参考：https://github.com/maleowy/ScrapTheWorld/tree/291a2468b6a418bf075175310a93b7d7d6c9b34a/Tests.EasyNetQ
/// </summary>
namespace EasyNetQDemo.ErrorHandler
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        private static IBus Bus;
        private static int WaitTime = 3000;
        private const string ErrorQueue = "EasyNetQ_Default_Error_Queue";

        static void Main(string[] args)
        {
            Bus = RabbitHutch.CreateBus("host=localhost");

            RpcCall(sleepToLong: false);

            SubscribeAsync(randomSleep: false, randomThrow: false, showEnd: false);

            HandleErrors();

            while (true)
            {
                string msg = Console.ReadLine();
                for (int i = 1; i <= 5; i++)
                {
                    Bus.Publish(new Question($"Question {i}"));
                }
            }
        }

        private static void RpcCall(bool sleepToLong = false)
        {
            Bus.Respond<Question, Answer>(q =>
            {
                if (sleepToLong)
                {
                    Thread.Sleep(6000);
                }
                return new Answer("Answer to " + q.Text);
            });

            var request = new Question("Question");
            var response = Bus.Request<Question, Answer>(request);

            Console.WriteLine(response.Text);
        }

        private static void SubscribeAsync(bool randomSleep, bool randomThrow, bool showEnd)
        {
            Bus.SubscribeAsync<Question>("subscriptionId", x => GetFunctionAsync(randomSleep, randomThrow, showEnd).Invoke(1, x));
            Bus.SubscribeAsync<Question>("subscriptionId", x => GetFunctionAsync(randomSleep, randomThrow, showEnd).Invoke(2, x));
        }
        private static Func<int, Question, Task> GetFunctionAsync(bool randomSleep, bool randomThrow, bool showEnd)
        {
            return async (id, q) =>
            {
                ShowStart(id, q);
                await WaitAsync(randomSleep);
                RandomThrow(id, q, randomThrow);
                ShowEnd(id, q, showEnd);
            };
        }
        private static void ShowStart(int id, Question q)
        {
            Console.WriteLine($"Start Worker {id} - {q.Text} {GetCurrentTime()}");
        }
        private static void ShowEnd(int id, Question q, bool show)
        {
            if (show)
            {
                Console.WriteLine($"End Worker {id} - {q.Text} {GetCurrentTime()}");
            }
        }
        private static void Wait(bool random)
        {
            Thread.Sleep(random ? new Random().Next(WaitTime) : WaitTime);
        }
        private static async Task WaitAsync(bool random)
        {
            await Task.Delay(random ? new Random().Next(WaitTime) : WaitTime);
        }
        private static void RandomThrow(int id, Question q, bool randomThrow)
        {
            //if (randomThrow && new Random().Next(0, 1) == 0)
            if (new Random().Next(0, 2) == 0)
            {
                var ex = $"throw: Worker {id} - {q.Text}";
                Console.WriteLine(ex);
                throw new Exception(ex);
            }
        }
        private static string GetCurrentTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        #region 异常处理
        private static void HandleErrors()
        {
            Action<IMessage<Error>, MessageReceivedInfo> handleErrorMessage = HandleErrorMessage;

            IQueue queue = new Queue(ErrorQueue, false);
            Bus.Advanced.Consume(queue, handleErrorMessage);
        }

        private static void HandleErrorMessage(IMessage<Error> msg, MessageReceivedInfo info)
        {
            Console.WriteLine("catch: " + msg.Body.Message);
        }
        #endregion
    }
}
