using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;

namespace TeamSpirit
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        BotData userData;
        BotData botData;
        string botMessage;
        string userMessage;
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            //botData = await stateClient.BotState.GetConversationDataAsync(activity.ChannelId, activity.Conversation.Id);
            userMessage = activity.Text;
            
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                if(userMessage.Length > 0)
                {
                    if(userMessage.ToLower().Contains("genesis") || userMessage.ToLower().Contains("hi") || userMessage.ToLower().Contains("hello"))
                    {
                        userData.SetProperty<bool>("isSelectCustomerOption", true);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);

                        await connector.Conversations.SendToConversationAsync(CustomerOptions(activity));
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                    if (userData.GetProperty<bool>("isSelectCustomerOption"))
                    {
                        if (userMessage.ToLower().Equals("new customer") || userMessage.ToLower().Equals("1"))
                        {
                            // Adding new customer
                            userData.SetProperty<bool>("isRegister", true);
                            userData.SetProperty<bool>("isFirstName", true);
                            userData.SetProperty<bool>("isSelectCustomerOption", false);

                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);

                        }
                        else
                        {
                            userData.SetProperty<bool>("isSelectCustomerOption", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);

                            await connector.Conversations.SendToConversationAsync(CustomerOptions(activity));
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                    }
                   

                    // Adding new customer
                    if (userData.GetProperty<bool>("isRegister"))
                    {

                        //await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                        if (userData.GetProperty<bool>("isFirstName"))
                        {
                            if (!String.IsNullOrEmpty(userMessage) && !String.IsNullOrWhiteSpace(userMessage))
                            {
                                await QuestionRegister("Enter your first name:", connector, stateClient, activity);
                                userData.SetProperty<bool>("isFirstName", false);
                                userData.SetProperty<bool>("isMiddleName", true);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                                //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                           
                        }
                        if (userData.GetProperty<bool>("isMiddleName"))
                        {
                            if (!String.IsNullOrEmpty(userMessage) && !String.IsNullOrWhiteSpace(userMessage))
                            {
                                userData.SetProperty<string>("FirstName", userMessage);
                                await QuestionRegister("Enter your middle name:", connector, stateClient, activity);
                                userData.SetProperty<bool>("isMiddleName", false);
                                userData.SetProperty<bool>("isLastName", true);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                                //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                           
                        }
                        if (userData.GetProperty<bool>("isLastName"))
                        {
                            if (!String.IsNullOrEmpty(userMessage) && !String.IsNullOrWhiteSpace(userMessage))
                            {
                                userData.SetProperty<string>("MiddleName", userMessage);
                                await QuestionRegister("Enter your last name:", connector, stateClient, activity);
                                userData.SetProperty<bool>("isLastName", false);
                                userData.SetProperty<bool>("isDoB", true);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                                //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                        }
                            
                        if (userData.GetProperty<bool>("isDoB"))
                        {
                            if (!String.IsNullOrEmpty(userMessage) && !String.IsNullOrWhiteSpace(userMessage))
                            {
                                userData.SetProperty<string>("LastName", userMessage);
                                await QuestionRegister("Enter your Date of Birth with format as (mm/dd/yyyy):", connector, stateClient, activity);
                                userData.SetProperty<bool>("isDoB", false);
                                userData.SetProperty<bool>("isMobile", true);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                                //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                           
                        }
                        if (userData.GetProperty<bool>("isMobile"))
                        {
                            if(!String.IsNullOrEmpty(userMessage) && !String.IsNullOrWhiteSpace(userMessage))
                            {
                                userData.SetProperty<string>("DoB", userMessage);
                                await QuestionRegister("Enter your contact Mobile number:", connector, stateClient, activity);
                                userData.SetProperty<bool>("isMobile", false);
                                userData.SetProperty<bool>("isEmail", true);
                                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                                //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            
                        }
                        if (userData.GetProperty<bool>("isEmail"))
                        {
                            userData.SetProperty<string>("Mobile", userMessage);
                            await QuestionRegister("Enter your Password:", connector, stateClient, activity);
                            userData.SetProperty<bool>("isEmail", false);
                            userData.SetProperty<bool>("isPassword", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        if (userData.GetProperty<bool>("isPassword"))
                        {
                            userData.SetProperty<string>("Password", userMessage);
                            await QuestionRegister("Enter your contact Email:", connector, stateClient, activity);
                            userData.SetProperty<bool>("isPassword", false);
                            userData.SetProperty<bool>("isAddress", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }

                        if (userData.GetProperty<bool>("isAddress"))
                        {
                            userData.SetProperty<string>("Email", userMessage);
                            await QuestionRegister("Enter your House Address:", connector, stateClient, activity);
                            userData.SetProperty<bool>("isAddress", false);
                            userData.SetProperty<bool>("isEnd", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                            return Request.CreateResponse(HttpStatusCode.OK);

                        }
                        if (userData.GetProperty<bool>("isEnd"))
                        {
                            userData.SetProperty<string>("Address", userMessage);
                            Customer customer = new Customer
                            {
                                firstName = userData.GetProperty<string>("FirstName"),
                                middleName = userData.GetProperty<string>("MiddleName"),
                                lastName = userData.GetProperty<string>("LastName"),
                                dob = userData.GetProperty<string>("DoB"),
                                mobilePhone = userData.GetProperty<string>("Mobile"),
                                email = userData.GetProperty<string>("Email"),
                                postalAddress = userData.GetProperty<string>("Address"),
                                password = userData.GetProperty<string>("Password"),
                            };
                            HttpClient httpClient = new HttpClient();
                            // Customer Details
                            var uri = "http://52.237.219.181:8081/v1/expr/faceboobbot/customerddetails";
                            var req = JsonConvert.SerializeObject(customer);
                            byte[] data = Encoding.UTF8.GetBytes(req);
                            HttpResponseMessage res;
                            using (var content = new ByteArrayContent(data))
                            {
                                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                res = await httpClient.PostAsync(uri, content);
                            }
                            string stringJson = await res.Content.ReadAsStringAsync();
                            CustomerResponse customerResponse = JsonConvert.DeserializeObject<CustomerResponse>(stringJson);

                            // Property
                            Property property = new Property
                            {
                                address = userData.GetProperty<string>("Address"),
                                service = "Electricity",
                                moveInDate = DateTime.Now.ToShortDateString(),
                                primaryResidence = "Y",
                                customerId = customerResponse.customerId
                            };
                            uri = "http://52.237.219.181:8081/v1/expr/faceboobbot/propertydetails";
                            req = JsonConvert.SerializeObject(property);
                            data = Encoding.UTF8.GetBytes(req);
                            using (var content = new ByteArrayContent(data))
                            {
                                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                res = await httpClient.PostAsync(uri, content);
                            }
                            stringJson = await res.Content.ReadAsStringAsync();

                            await QuestionRegister("Thanks for your register!", connector, stateClient, activity);
                            userData.SetProperty<bool>("isEnd", false);
                            userData.SetProperty<bool>("isRegister", false);
                            userData.SetProperty<bool>("isSelectCustomerOption", true);
                            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                            //await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
                            await connector.Conversations.SendToConversationAsync(CustomerOptions(activity));
                            return Request.CreateResponse(HttpStatusCode.OK);

                        }
                    }
                }

                // return our reply to the user
                Activity reply = activity.CreateReply(botMessage);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task PostCustomerDetail(Customer customer)
        {

        } 

        private async Task QuestionRegister(string question, ConnectorClient connector, StateClient stateClient, Activity activity)
        {
            botMessage = question;

            Activity rep = activity.CreateReply(botMessage);
            await connector.Conversations.ReplyToActivityAsync(rep);
        }

        private Activity CustomerOptions(Activity activity)
        {
            Activity replyToConversation = activity.CreateReply("");
            replyToConversation.Recipient = activity.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();
            List<ReceiptItem> items = new List<ReceiptItem>();

            ReceiptItem item1 = new ReceiptItem()
            {
                Text = "1. New Customer"
            };
            items.Add(item1);

            ReceiptItem item2 = new ReceiptItem()
            {
                Text = "2. Existing Customer (Login)"
            };
            items.Add(item2);

           

            ReceiptCard Card = new ReceiptCard()
            {
                Title = "Welcome to Genesis chat bot! Please choose yours options",
                Items = items
            };

            Attachment plAttachment = Card.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            return replyToConversation;
        }

        private Activity ReceiveBillOptions(Activity activity)
        {
            Activity replyToConversation = activity.CreateReply("");
            replyToConversation.Recipient = activity.From;
            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();
            List<ReceiptItem> items = new List<ReceiptItem>();

            ReceiptItem item1 = new ReceiptItem()
            {
                Text = "1. Email"
            };
            items.Add(item1);

            ReceiptItem item2 = new ReceiptItem()
            {
                Text = "2. Post"
            };
            items.Add(item2);



            ReceiptCard Card = new ReceiptCard()
            {
                Title = "How would you like to receive your bill?",
                Items = items
            };

            Attachment plAttachment = Card.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            return replyToConversation;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}