using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Entities;
using TestManagement.Entities.Requests;
using TestManagement.Responses;
using TestManagement.Services;
using TestManagement.Requests;
using Org.BouncyCastle.Asn1.Ocsp;

namespace TestServer
{
    public class Server
    {
        const string SERVER_HOST = "127.0.0.1";
        const int PORT = 11111;

        Socket server;
        EndPoint clientEndpoint;

        bool running = false;

        DbContext dbContext;

        public Server()
        {
            IPAddress ipAddress = IPAddress.Parse(SERVER_HOST);
            EndPoint serverEndpoint = new IPEndPoint(ipAddress, PORT);

            server = new Socket(ipAddress.AddressFamily,
                SocketType.Dgram, ProtocolType.Udp);

            server.Bind(serverEndpoint);

            dbContext = new DbContext();
        }

        public void Close()
        {
            server.Close();
        }

        public void Start()
        {
            running = true;
            Thread runThread = new Thread(() =>
            {
                clientEndpoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

                int result = 0;
                const int BufferRecvSize = 1024 * 4;
                byte[] buffer = new byte[BufferRecvSize];
                string message = "";
                RequestData request;



                ClientRequest clientRequest;
                string[] items;

                while (running)
                {
                    result = server.ReceiveFrom(buffer, BufferRecvSize, 0, ref clientEndpoint);
                    message = Encoding.UTF8.GetString(buffer, 0, result);
                    if (result > 0) Console.WriteLine("Messgae RECV:" + message);
                    else Console.WriteLine("ERROR: recv message from client !!!");

                    clientRequest = new ClientRequest(message);

                    request = new RequestData();

                    switch (clientRequest.type)
                    {
                        case "AUTHENTICATION":
                            request.username = clientRequest.msg.Split(FormatService.PATTERN_ITEM)[0];
                            request.password = clientRequest.msg.Split(FormatService.PATTERN_ITEM)[1];

                            Thread authenicationThread = new Thread(() => ThreadAuthentication(request));
                            authenicationThread.Start();
                            break;

                        case "REGISTER":
                            request.username = clientRequest.msg.Split(FormatService.PATTERN_ITEM)[0];
                            request.password = clientRequest.msg.Split(FormatService.PATTERN_ITEM)[1];
                            request.accountName = clientRequest.msg.Split(FormatService.PATTERN_ITEM)[2];

                            Thread registerThread = new Thread(() => ThreadRegister(request));
                            registerThread.Start();
                            break;

                        case "ADD_QUEST":
                            items = clientRequest.msg.Split(FormatService.PATTERN_ITEM);

                            request.authorId = Int32.Parse(items[0]);
                            request.quest = items[1];
                            request.correct = items[2];
                            List<string> incorrects = new List<string>();
                            for (int i = 3; i < items.Length; i++)
                            {
                                incorrects.Add(items[i]);
                            }
                            request.incorrects = incorrects;

                            Thread addQuestThread = new Thread(() => ThreadAddQuest(request));
                            addQuestThread.Start();

                            break;

                        case "GENERATE_TEST":
                            request.totalQuestion = Int32.Parse(clientRequest.msg);

                            Thread generateTestThread = new Thread(() => ThreadGenerateTest(request));
                            generateTestThread.Start();

                            break;

                        case "LOAD_TEST":
                            Thread loadTestThread = new Thread(() => ThreadLoadTest());
                            loadTestThread.Start();

                            break;

                        case "LOAD_TEST_QUEST_ALL":
                            request.testId = Int32.Parse(clientRequest.msg);

                            Thread loadTestQuestAll = new Thread(() => ThreadLoadTestQuestAll(request));
                            loadTestQuestAll.Start();

                            break;

                        case "SUBMIT_ANWSER":
                            request.testId = Int32.Parse(message.Split(FormatService.PATTERN)[1]);
                            request.examineeId = Int32.Parse(message.Split(FormatService.PATTERN)[2]);
                            clientRequest.msg = message.Split(FormatService.PATTERN)[3];
                            Console.WriteLine("testID: " + request.testId);
                            Console.WriteLine("examineeID: " + request.examineeId);
                            Console.WriteLine("msg: " + clientRequest.msg);
                            List<string> answers = new List<string>();
                            string[] anss = clientRequest.msg.Split(FormatService.PATTERN_ITEM);
                            for(int i = 0; i < anss.Length; i++)
                            {
                                answers.Add(anss[i]);
                            }

                            Thread thread = new Thread(() => ThreadSubmitAnwser(request, answers));
                            thread.Start();

                            break;

                        default: break;
                    }
                }
            });

            runThread.Start();
            runThread.Join();

            Close();
        }

        private void ThreadRegister(RequestData request)
        {
            string response = ServerResponse.REGISTER_RESPONSE + FormatService.PATTERN;

            AccountDTO account = null;
            if (!dbContext.accountService.ExistsByUsername(request.username))
            {
                dbContext.accountService.Add(request);
                account = dbContext.accountService.GetByUsername(request.username);

                response += ServerResponse.TRUE;

                response += FormatService.PATTERN + account.ToString();
            }
            else
            {
                response += ServerResponse.FALSE;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            int result = server.SendTo(buffer, buffer.Length, 0, clientEndpoint);
            if (result > 0)
            {
                Console.WriteLine($"Successfully send response.");
            }
            else
            {
                Console.WriteLine($"Failed send response !!!");
            }
        }

        private void ThreadSubmitAnwser(RequestData request, List<string> answers)
        {
            string response = ServerResponse.SUBMIT_ANWSER_RESPONSE + FormatService.PATTERN;
            if (dbContext.accountService.ExistsById(request.examineeId)) {
                response += ServerResponse.TRUE + FormatService.PATTERN;

                List<QuestionDTO> quests = dbContext.questionService.GetQuestListByTestId(request.testId);
                request.correctAmount = 0;
                request.totalQuestion = quests.Count;

                List<bool> isCorrects = new List<bool>();

                for (int i = 0; i < quests.Count; i++)
                {
                    if (quests[i].correctAnwser.Equals(answers[i]))
                    {
                        isCorrects.Add(true);
                        request.correctAmount++;
                    }
                    else
                    {
                        isCorrects.Add(false);
                    }
                }

                dbContext.resultService.Add(request);
                request.resultId = dbContext.resultService.GetMaxId();

                for (int i = 0; i < quests.Count; i++)
                {
                    request.questId = quests[i].id;
                    request.anwser = answers[i];
                    request.isCorrect = isCorrects[i];
                    dbContext.resultDetailService.Add(request);
                }

                response += $"{request.correctAmount} / {request.totalQuestion}: " +
                    $"{Math.Floor(request.correctAmount * 1.0 * 100 / request.totalQuestion)}%";
            } else
            {
                response += ServerResponse.FALSE;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            int result = server.SendTo(buffer, buffer.Length, 0, clientEndpoint);
            if (result > 0)
            {
                Console.WriteLine($"Successfully send response.");
            }
            else
            {
                Console.WriteLine($"Failed send response !!!");
            }
        }

        private void ThreadLoadTestQuestAll(RequestData request)
        {
            string response = ServerResponse.LOAD_TEST_QUEST_ALL_RESPONSE + FormatService.PATTERN;

            if (dbContext.testService.ExistsById(request.testId))
            {
                response += ServerResponse.TRUE;

                response += FormatService.PATTERN + 
                    dbContext.questionService.GetTestQuestAllStringByTestId(request.testId);

            }
            else
            {
                response += ServerResponse.FALSE;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            int result = server.SendTo(buffer, buffer.Length, 0, clientEndpoint);
            if (result > 0)
            {
                Console.WriteLine($"Successfully send response.");
            }
            else
            {
                Console.WriteLine($"Failed send response !!!");
            }
        }

        private void ThreadLoadTest()
        {
            string response = ServerResponse.LOAD_TEST_RESPONSE + FormatService.PATTERN;

            dbContext.testService.LoadData();

            if (dbContext.testService.data.Count <= 0) {
                response += ServerResponse.FALSE;
            } else
            {
                response += ServerResponse.TRUE + FormatService.PATTERN;
                int totalQuestion = 0;
                for (int i = 0; i < dbContext.testService.data.Count; i++) {
                    totalQuestion = dbContext.testQuestService
                        .GetSumByTestId(dbContext.testService.data[i].id);

                    response += dbContext.testService.data[i].id + FormatService.PATTERN_ITEM +
                        totalQuestion;
                    if (i < dbContext.testService.data.Count - 1) response += FormatService.PATTERN_END_LINE;
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            int result = server.SendTo(buffer, buffer.Length, 0, clientEndpoint);
            if (result > 0)
            {
                Console.WriteLine($"Successfully send response.");
            }
            else
            {
                Console.WriteLine($"Failed send response !!!");
            }
        }

        private void ThreadGenerateTest(RequestData request)
        {
            string response = ServerResponse.GENERATE_TEST_RESPONSE + FormatService.PATTERN;

            if (dbContext.testService.Add(request))
            {
                response += ServerResponse.TRUE;
            }
            else
            {
                response += ServerResponse.FALSE;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            int result = server.SendTo(buffer, buffer.Length, 0, clientEndpoint);
            if (result > 0)
            {
                Console.WriteLine($"Successfully send response.");
            }
            else
            {
                Console.WriteLine($"Failed send response !!!");
            }
        }

        private void ThreadAddQuest(RequestData request)
        {
            string response = ServerResponse.ADD_QUEST_RESPONSE + FormatService.PATTERN;

            if (dbContext.accountService.ExistsById(request.authorId) &&
                dbContext.questionService.Add(request))
            {
                response += ServerResponse.TRUE;
            }
            else
            {
                response += ServerResponse.FALSE;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            int result = server.SendTo(buffer, buffer.Length, 0, clientEndpoint);
            if (result > 0)
            {
                Console.WriteLine($"Successfully send response.");
            }
            else
            {
                Console.WriteLine($"Failed send response !!!");
            }
        }

        private void ThreadAuthentication(RequestData request)
        {
            string response = ServerResponse.AUTHENTICATION_RESPONSE + FormatService.PATTERN;

            int accountId = dbContext.accountService.Authentication(request);

            AccountDTO account = null;
            if(accountId != -1)
                account = dbContext.accountService.GetById(accountId);

            if (accountId != -1 && account.isActive)
            {
                response += ServerResponse.TRUE;

                response += FormatService.PATTERN + account.ToString();
            }
            else
            {
                response += ServerResponse.FALSE;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(response);
            int result = server.SendTo(buffer, buffer.Length, 0, clientEndpoint);
            if (result > 0)
            {
                Console.WriteLine($"Successfully send response.");
            }
            else
            {
                Console.WriteLine($"Failed send response !!!");
            }
        }
    }
}
