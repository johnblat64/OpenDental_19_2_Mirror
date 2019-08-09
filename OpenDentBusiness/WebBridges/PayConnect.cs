using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CodeBase;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace OpenDentBusiness {
	public class PayConnect {

		private static PayConnectService.Credentials GetCredentials(Program prog,long clinicNum){
			PayConnectService.Credentials cred=new PayConnectService.Credentials();
			cred.Username=OpenDentBusiness.ProgramProperties.GetPropVal(prog.ProgramNum,"Username",clinicNum);
			cred.Password=OpenDentBusiness.ProgramProperties.GetPropVal(prog.ProgramNum,"Password",clinicNum);
			cred.Client="OpenDental2";
#if DEBUG
			cred.ServiceID="DCI Web Service ID: 002778";//Testing
#else
			cred.ServiceID="DCI Web Service ID: 006328";//Production
#endif
			cred.version="0310";
			return cred;
		}

		///<summary>Parameters starting at authCode are optional, because our eServices probably reference this function as well.</summary>
		public static PayConnectService.creditCardRequest BuildSaleRequest(decimal amount,string cardNumber,int expYear,int expMonth,string nameOnCard,string securityCode,string zip,string magData,PayConnectService.transType transtype,string refNumber,bool tokenRequested,string authCode="",bool isForced=false) {
			PayConnectService.creditCardRequest request=new PayConnectService.creditCardRequest();
			request.Amount=amount;
			request.AmountSpecified=true;
			request.CardNumber=cardNumber;
			request.Expiration=new PayConnectService.expiration();
			request.Expiration.year=expYear;
			request.Expiration.month=expMonth;
			if(magData!=null) { //MagData is the data returned from magnetic card readers. Will only be present if a card was swiped.
				request.MagData=magData;
			}
			request.NameOnCard=nameOnCard;
			request.RefNumber=refNumber;
			request.SecurityCode=securityCode;
			request.TransType=transtype;
			request.Zip=zip;
			request.PaymentTokenRequestedSpecified=true;
			request.PaymentTokenRequested=tokenRequested;
			//request.AuthCode=authCode;//This field does not exist in the WSDL yet.  Dentalxchange will let us know once they finish adding it.
			request.ForceDuplicateSpecified=true;
			request.ForceDuplicate=isForced;
			return request;
		}

		///<summary>Shows a message box on error.</summary>
		public static PayConnectService.transResponse ProcessCreditCard(PayConnectService.creditCardRequest request,long clinicNum,
			Action<string> showError) 
		{
			try {
				Program prog=Programs.GetCur(ProgramName.PayConnect);
				PayConnectService.Credentials cred=GetCredentials(prog,clinicNum);
				PayConnectService.MerchantService ms=new PayConnectService.MerchantService();
#if DEBUG
				ms.Url="https://prelive.dentalxchange.com/merchant/MerchantService?wsdl";
#else
				ms.Url="https://webservices.dentalxchange.com/merchant/MerchantService?wsdl";
#endif
				PayConnectService.transResponse response=ms.processCreditCard(cred,request);
				ms.Dispose();
				if(response.Status.code!=0 && response.Status.description.ToLower().Contains("duplicate")) {
					showError(Lans.g("PayConnect","Payment failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Pay Connect: \""
						+response.Status.description+"\"\r\n"
						+Lans.g("PayConnect","Try using the Force Duplicate checkbox if a duplicate is intended."));
				}
				if(response.Status.code!=0 && response.Status.description.ToLower().Contains("invalid user")) {
					showError(Lans.g("PayConnect","Payment failed")+".\r\n"
						+Lans.g("PayConnect","PayConnect username and password combination invalid.")+"\r\n"
						+Lans.g("PayConnect","Verify account settings by going to")+"\r\n"
						+Lans.g("PayConnect","Setup | Program Links | PayConnect. The PayConnect username and password are probably the same as the DentalXChange login ID and password."));
				}
				else if(response.Status.code!=0) {//Error
					showError(Lans.g("PayConnect","Payment failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Pay Connect: \""
						+response.Status.description+"\"");
				}
				return response;
			}
			catch(Exception ex) {
				showError(Lans.g("PayConnect","Payment failed")+". \r\n"+Lans.g("PayConnect","Error message")+": \""+ex.Message+"\"");
			}
			return null;
		}

		public static PayConnectService.signatureResponse ProcessSignature(PayConnectService.signatureRequest sigRequest,long clinicNum,
			Action<string> showError) 
		{
			try {
				Program prog=Programs.GetCur(ProgramName.PayConnect);
				PayConnectService.Credentials cred=GetCredentials(prog,clinicNum);
				PayConnectService.MerchantService ms=new PayConnectService.MerchantService();
#if DEBUG
				ms.Url="https://prelive.dentalxchange.com/merchant/MerchantService?wsdl";
#else
				ms.Url="https://webservices.dentalxchange.com/merchant/MerchantService?wsdl";
#endif
				PayConnectService.signatureResponse response=ms.processSignature(cred,sigRequest);
				ms.Dispose();
				if(response.Status.code!=0) {//Error
					showError(Lans.g("PayConnect","Signature capture failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Pay Connect: \""+response.Status.description+"\"");
				}
				return response;
			}
			catch(Exception ex) {
				showError(Lans.g("PayConnect","Signature capture failed")+". \r\n"+Lans.g("PayConnect","Error message from")+" Open Dental: \""+ex.Message+"\"");
			}
			return null;
		}

		public static bool IsValidCardAndExp(string cardNumber,int expYear,int expMonth,Action<string> showError) {
			bool isValid=false;
			try {
				PayConnectService.expiration pcExp=new PayConnectService.expiration();
				pcExp.year=expYear;
				pcExp.month=expMonth;
				PayConnectService.MerchantService ms=new PayConnectService.MerchantService();
#if DEBUG
				ms.Url="https://prelive.dentalxchange.com/merchant/MerchantService?wsdl";
#else
				ms.Url="https://webservices.dentalxchange.com/merchant/MerchantService?wsdl";
#endif
				isValid=(ms.isValidCard(cardNumber) && ms.isValidExpiration(pcExp));
				ms.Dispose();
			}
			catch(Exception ex) {
				showError(Lans.g("PayConnect","Credit Card validation failed")+". \r\n"+Lans.g("PayConnect","Error message from")
					+" Open Dental: \""+ex.Message+"\"");
			}
			return isValid;
		}


		///<summary>Return bool if value passed in is numeric only</summary>
		public static bool IsNumeric(string str) {
			if(str==null) {
				return false;
			}
			Regex objNotWholePattern=new Regex("[^0-9]");
			return !objNotWholePattern.IsMatch(str);
		}

		///<summary>Builds a receipt string for a web service transaction.</summary>
		public static string BuildReceiptString(PayConnectService.creditCardRequest request,PayConnectService.transResponse response,
			PayConnectService.signatureResponse sigResponse,long clinicNum) 
		{
			string result="";
			if(response==null) {
				return result;
			}
			int xmin=0;
			int xleft=xmin;
			int xright=15;
			int xmax=37;
			result+=Environment.NewLine;
			result+=CreditCardUtils.AddClinicToReceipt(clinicNum);
			//Print body
			result+="Date".PadRight(xright-xleft,'.')+DateTime.Now.ToString()+Environment.NewLine;
			result+=Environment.NewLine;
			result+="Trans Type".PadRight(xright-xleft,'.')+request.TransType.ToString()+Environment.NewLine;
			result+=Environment.NewLine;
			result+="Transaction #".PadRight(xright-xleft,'.')+response.RefNumber+Environment.NewLine;
			result+="Name".PadRight(xright-xleft,'.')+request.NameOnCard+Environment.NewLine;
			result+="Account".PadRight(xright-xleft,'.');
			for(int i = 0;i<request.CardNumber.Length-4;i++) {
				result+="*";
			}
			result+=request.CardNumber.Substring(request.CardNumber.Length-4)+Environment.NewLine;//last 4 digits of card number only.
			result+="Card Type".PadRight(xright-xleft,'.')+CreditCardUtils.GetCardType(request.CardNumber)+Environment.NewLine;
			result+="Entry".PadRight(xright-xleft,'.')+(String.IsNullOrEmpty(request.MagData) ? "Manual" : "Swiped")+Environment.NewLine;
			result+="Auth Code".PadRight(xright-xleft,'.')+response.AuthCode+Environment.NewLine;
			result+="Result".PadRight(xright-xleft,'.')+response.Status.description+Environment.NewLine;
			if(response.Messages!=null) {
				string label="Message";
				foreach(string m in response.Messages) {
					result+=label.PadRight(xright-xleft,'.')+m+Environment.NewLine;
					label="";
				}
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			if(request.TransType.In(PayConnectService.transType.RETURN,PayConnectService.transType.VOID)) {
				result+="Total Amt".PadRight(xright-xleft,'.')+(request.Amount*-1)+Environment.NewLine;
			}
			else {
				result+="Total Amt".PadRight(xright-xleft,'.')+request.Amount+Environment.NewLine;
			}
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine;
			result+="I agree to pay the above total amount according to my card issuer/bank agreement."+Environment.NewLine;
			result+=Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine+Environment.NewLine;
			if(sigResponse==null || sigResponse.Status==null || sigResponse.Status.code!=0) {
				result+="Signature X".PadRight(xmax-xleft,'_');
			}
			else {
				result+="Electronically signed";
			}
			return result;
		}

		#region Patient Portal Interface
		///<summary>New OTK is available. Send wakeup event to EConnector so it will start processing immediately.</summary>
		public static EventHandler WakeupWebPaymentsMonitor;

		///<summary>Creates and returns the HPF URL and validation OTK which can be used to make a payment for an unspecified credit card.  Throws exceptions.</summary>
		public static string GetHpfUrlForPayment(Patient pat,string payNote,bool isMobile,double amount,bool createAlias,CreditCardSource ccSource) {
			string response="";
			//TODO web call
			//TODO save response into table
			WakeupWebPaymentsMonitor?.Invoke(response,new EventArgs());
			return response;
		}

		///<summary>Creates and returns the HPF URL and validation OTK which can be used to create a credit card alias.  Throws exceptions.</summary>
		public static string GetHpfUrlForCreditCardAlias(Patient pat,bool isMobile) {
			string response="";
			//TODO web call
			//TODO save response into table
			WakeupWebPaymentsMonitor?.Invoke(response,new EventArgs());
			return response;
		}

		///<summary>Make a payment using HPF directly.  Throws exceptions.</summary>
		public static void MakePaymentWithAlias(Patient pat,string payNote,double amount,CreditCard cc) {
			if(string.IsNullOrEmpty(cc.PayConnectToken)) {
				throw new ODException("Invalid CC Alias",ODException.ErrorCodes.OtkArgsInvalid);
			}
			//todo:
		}

		///<summary>Make a payment using HPF directly.  Throws exceptions.</summary>
		public static void DeleteCreditCard(Patient pat,CreditCard cc) {
			if(string.IsNullOrEmpty(cc.PayConnectToken)) {
				throw new ODException("Invalid CC Alias",ODException.ErrorCodes.OtkArgsInvalid);
			}
			//todo:
		}

		///<summary>Putting this functionality in it's own class scope prevents it from being serialized for DTO.
		///Used by EConnector to monitor XWeb gateway.</summary>
		public class Monitor {
			#region Events
			public static EventHandler<Logger.LoggerEventArgs> LoggerEvent;
			///<summary>Any logging event. Sent as Verbose by default but can be sent as any log level.</summary>
			protected static void OnLoggerEvent(string s,LogLevel logLevel = LogLevel.Verbose) {
				if(LoggerEvent!=null) {
					LoggerEvent?.Invoke(null,new Logger.LoggerEventArgs(s,logLevel));
				}
			}
			///<summary>Logging event for a specific PayConnect. Uses json serializer so use sparingly if speed is an issue. 
			///Sent as Verbose by default but can be sent as any log level.</summary>
			protected static void OnLoggerEventForSingleResponse(XWebResponse response,string s,LogLevel logLevel = LogLevel.Verbose) {
				OnLoggerEvent(s+"\r\n\t"+JsonConvert.SerializeObject(response),logLevel);
			}
			#endregion

			///<summary>Look for outstanding OTKs and poll PayConnect for status. Update status and convert to paymentweb where necessary. 
			///Next run interval will be set to very short if there are still outstanding OTKs or long if not.
			///This thread will be awakened instantly if a new OTK becomes available (via PayConnect.WakeupWebPaymentsMonitor event).</summary>
			public static void OnThreadRun(ODThread odThread) {
				//Set next run interval to long by default;
				odThread.TimeIntervalMS=(int)TimeSpan.FromMinutes(1).TotalMilliseconds;
				try {
					//Returns number of OTKs still left in pending state.
					if(ProcessOutstandingTransactions()>0) { //We still have pending OTKs, set next run interval to short.
						odThread.TimeIntervalMS=(int)TimeSpan.FromSeconds(.5).TotalMilliseconds;
					}
				}
				catch(Exception e) {
					OnLoggerEvent(e.Message,LogLevel.Error);
					//Something unforeseen went wrong, throttle the next run interval a bit.
					odThread.TimeIntervalMS=(int)TimeSpan.FromSeconds(10).TotalMilliseconds;
				}
			}

			///<summary>Returns number of pending transactions remaining after completion.</summary>
			private static int ProcessOutstandingTransactions() {
				OnLoggerEvent("Checking for outstanding OTKs.");
				//TODO: implement method
				int remaining=-1;
				OnLoggerEvent(remaining.ToString()+" OTKs still pending after processing.",LogLevel.Information);
				return remaining;
			}
		}
		#endregion

		public class WebPaymentProperties {
			public bool IsPaymentsAllowed;
			public string Token;
		}
		
		public static class ProgramProperties {
			public const string PayConnectForceRecurringCharge="PayConnectForceRecurringCharge";
			public const string DefaultProcessingMethod="DefaultProcessingMethod";
			public const string PayConnectPreventSavingNewCC="PayConnectPreventSavingNewCC";
			public const string PatientPortalPaymentsEnabled="IsOnlinePaymentsEnabled";
			public const string PatientPortalPaymentsToken="Patient Portal Payments Token";
		}
	}

	public class PayConnectREST {

		///<summary>Returns the account token for PayConnect to use their rest service.
		///Throws exceptions if not successful at retrieving an account token.</summary>
		public static string GetAccountToken(string username,string password) {
			#region Response Object
			var resObj=new {
				AccountToken="",
				Status=new {
					code=-1,
					description="",
				},
				Messages=new {
					Message=new string[0]
				}
			};
			#endregion
			//var res=GetAccountTokenResponseMock(resObj);
			List<string> listHeaders=GetClientRequestHeaders();
			listHeaders.Add("Username: "+username);
			listHeaders.Add("Password: "+password);
			var res=Request(ApiRoute.AccountToken,HttpMethod.Get,listHeaders,"",resObj);
			if(res==null) {
				throw new ODException("Invalid response from PayConnect.");
			}
			int code=-1;
			string codeMsg="";
			if(res.Status!=null) {
				code=res.Status.code;
				codeMsg="Response code: "+res.Status.code+"\r\n";
			}			
			if(code==1000) {
				throw new ODException("Request to PayConnect resulted in an internal server error.\r\n"
					+"This may be due to invalid credentials.  Please check your username and password.");
			}
			if(code>0) {
				string err="Invalid response from PayConnect.\r\nResponse code: "+code;
				if(res.Messages!=null && res.Messages.Message!=null && res.Messages.Message.Length>0) {
					err+="\r\nError retrieving account token.\r\nResponse message(s):\r\n"+string.Join("\r\n",res.Messages.Message);
				}
				throw new ODException(err);
			}
			if(string.IsNullOrWhiteSpace(res.AccountToken)) {
				throw new ODException("Invalid account token was retrieved from PayConnect."+(string.IsNullOrEmpty(codeMsg) ? "" : "\r\n"+codeMsg));
			}
			return res.AccountToken;
		}

		///<summary>Returns the URL for making a payment through PayConnect.
		///Throws exceptions if the request/response is invalid.</summary>
		public static string PostPaymentRequest(string accountToken,double amt,long invoiceNumber,out string url) {
			#region Response Object
			var resObj=new {
				PaymentToken="",
				Url="",
				Status=new {
					code=-1,
					description="",
				},
				Messages=new {
					Message=new string[0]
				}
			};
			#endregion
			//var res=PostPaymentRequestResponseMock(resObj);
			List<string> listHeaders=GetClientRequestHeaders();
			listHeaders.Add("AccountToken: "+accountToken);
			string postBody=JsonConvert.SerializeObject(new {
				Amount=amt,
				InvoiceNumber=invoiceNumber,
			});
			var res=Request(ApiRoute.PaymentRequest,HttpMethod.Post,listHeaders,postBody,resObj);
			if(res==null) {
				throw new ODException("Invalid response from PayConnect.");
			}
			int code=-1;
			string codeMsg="";
			if(res.Status!=null) {
				code=res.Status.code;
				codeMsg="Response code: "+res.Status.code+"\r\n";
			}
			if(code>0) {
				string err="Invalid response from PayConnect.\r\nResponse code: "+code;
				if(res.Messages!=null && res.Messages.Message!=null && res.Messages.Message.Length>0) {
					err+="\r\nError retrieving account token.\r\nResponse message(s):\r\n"+string.Join("\r\n",res.Messages.Message);
				}
				throw new ODException(err);
			}
			if(string.IsNullOrWhiteSpace(res.PaymentToken) || string.IsNullOrWhiteSpace(res.Url)) {
				throw new ODException("Invalid payment token or URL was retrieved from PayConnect."+(res.Status==null ? "" : "\r\n"+"Response code: "+res.Status.code));
			}
			url=res.Url;
			return res.PaymentToken;
		}

		///<summary>Poll the existing PayConnectResponseWeb for status changes.
		///This method will update the ResponseJSON/ProcessingStatus with any changes</summary>
		public static void GetPaymentStatus(PayConnectResponseWeb responseWeb) {
			#region Response Object
			var resObj=new {
				Amount=-1.00,
				TransactionType="",
				TransactionStatus="",
				TransactionDate=DateTime.MinValue,
				StatusDescription="",
				PaymentToken="",
				CreditCardNumber="",
				CreditCardExpireDate="",
				TransactionID=-1,
				AuthCode="",
				RefNumber="",
				Pending=true,
				Status=new {
					code=-1,
					description="",
				},
				Messages=new {
					Message=new string[0]
				},
			};
			#endregion
			List<string> listHeaders=GetClientRequestHeaders();
			listHeaders.Add("AccountToken: "+responseWeb.AccountToken);
			try {
				var res=Request(ApiRoute.PaymentStatus,HttpMethod.Get,listHeaders,"",resObj,$"?paymentToken={responseWeb.PaymentToken}");
				if(res==null) {
					HandleResponseErrorAndThrow(responseWeb,JsonConvert.SerializeObject(res),new ODException("Invalid response from PayConnect."));
				}
				int code=-1;
				string codeMsg="";
				if(res.Status!=null) {
					code=res.Status.code;
					codeMsg="Response code: "+res.Status.code+"\r\n";
				}
				if(code>0) {
					string err="Invalid response from PayConnect.\r\nResponse code: "+code;
					if(res.Messages!=null && res.Messages.Message!=null && res.Messages.Message.Length>0) {
						err+="\r\nError retrieving account token.\r\nResponse message(s):\r\n"+string.Join("\r\n",res.Messages.Message);
					}
					HandleResponseErrorAndThrow(responseWeb,JsonConvert.SerializeObject(res),new ODException(err));
				}
				HandleResponseSuccess(responseWeb,JsonConvert.SerializeObject(res),res.Pending);
			}
			catch(Exception ex) {
				HandleResponseErrorAndThrow(responseWeb,ex.Message,ex);
			}
		}

		private static void HandleResponseSuccess(PayConnectResponseWeb responseWeb,string resStr,bool isPending) {
			if(!isPending) {
				responseWeb.ProcessingStatus=PayConnectWebStatus.Completed;
				responseWeb.DateTimeCompleted=MiscData.GetNowDateTime();
			}
			else {
				responseWeb.ProcessingStatus=PayConnectWebStatus.Pending;
				responseWeb.DateTimePending=MiscData.GetNowDateTime();
			}
			responseWeb.LastResponseStr=resStr;
		}

		private static void HandleResponseErrorAndThrow(PayConnectResponseWeb responseWeb,string resStr,Exception exception) {
			if(responseWeb.ProcessingStatus==PayConnectWebStatus.Created) {
				responseWeb.ProcessingStatus=PayConnectWebStatus.CreatedError;
				responseWeb.DateTimeLastError=MiscData.GetNowDateTime();
			}
			else if(responseWeb.ProcessingStatus==PayConnectWebStatus.Pending) {
				responseWeb.ProcessingStatus=PayConnectWebStatus.PendingError;
				responseWeb.DateTimeLastError=MiscData.GetNowDateTime();
			}
			responseWeb.LastResponseStr=resStr;
			throw exception;
		}

		private static List<string> GetClientRequestHeaders() {
			return new List<string>() {
				"Client: OpenDental2",
				$"ServiceID: DCI Web Service ID: {(ODBuild.IsDebug() ? "002778" : "006328")}",
				"Version: 0310",
			};
		}

		private static T GetAccountTokenResponseMock<T>(T responseType) {
			string res=@"
			{
				""AccountToken"":""QIGhT2C1chpt"",
				""Status"": {
					""code"":0,
					""description"":""Operation Successful""
				},
				""Messages"": {
					""Message"":[]
				}
			}";
			return (T)JsonConvert.DeserializeAnonymousType(res,responseType);
		}

		private static T PostPaymentRequestResponseMock<T>(T responseType) {
			string res=@"
			{
				""PaymentToken"": ""5v1JgWCU5hnA"",
				""Url"": ""https://.../pay/payment?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwYXltZW50VG9r..."",
				""Status"": {
					""code"": 0,
					""description"": ""Operation Successful""
				},
				""Messages"": {
					""Message"":[]
				}
			}";
			return (T)JsonConvert.DeserializeAnonymousType(res,responseType);
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private static T Request<T>(ApiRoute route,HttpMethod method,List<string> listHeaders,string body,T responseType,string queryStr="") {
			using(WebClient client=new WebClient()) {
				client.Headers[HttpRequestHeader.ContentType]="application/json";
				listHeaders.ForEach(x => client.Headers.Add(x));
				client.Encoding=UnicodeEncoding.UTF8;
				try {
					string res="";
					if(method==HttpMethod.Get) {
						res=client.DownloadString(GetApiUrl(route)+queryStr);
					}
					else if(method==HttpMethod.Post) {
						res=client.UploadString(GetApiUrl(route)+queryStr,HttpMethod.Post.Method,body);
					}
					else if(method==HttpMethod.Put) {
						res=client.UploadString(GetApiUrl(route)+queryStr,HttpMethod.Put.Method,body);
					}
					else {
						throw new Exception("Unsupported HttpMethod type: "+method.Method);
					}
#if DEBUG
					if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
						return (T)Convert.ChangeType(res,typeof(T));
					}
#endif
					return JsonConvert.DeserializeAnonymousType(res,responseType);
				}
				catch(WebException wex) {
					string res="";
					if(!(wex.Response is HttpWebResponse)) {
						throw new Exception("Could not connect to the PayConnect server:\r\n"+wex.Message,wex);
					}
					using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
					}
					if(string.IsNullOrWhiteSpace(res)) {
						//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
						if(wex.Response.GetType()==typeof(HttpWebResponse)) {
							HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
							if(statusCode==HttpStatusCode.Unauthorized) {
								throw new ODException(Lans.g("PayConnect","Invalid PayConnect credentials."));
							}
						}
					}
					string errorMsg=wex.Message+(string.IsNullOrWhiteSpace(res) ? "" : "\r\nRaw response:\r\n"+res);
					throw new Exception(errorMsg,wex);//If we got this far and haven't rethrown, simply throw the entire exception.
				}
				catch(Exception ex) {
					//WebClient returned an http status code >= 300
					ex.DoNothing();
					//For now, rethrow error and let whoever is expecting errors to handle them.
					//We may enhance this to care about codes at some point.
					throw;
				}
			}
		}

		///<summary>Returns the full URL according to the route/route id given.</summary>
		private static string GetApiUrl(ApiRoute route) {
			string apiUrl=Introspection.GetOverride(Introspection.IntrospectionEntity.PayConnectRestURL,"https://webservices.dentalxchange.com/pay/rest/PayService");
#if DEBUG
			apiUrl="https://prelive2.dentalxchange.com/pay/rest/PayService";
#endif
			switch(route) {
				case ApiRoute.Root:
					//Do nothing.  This is to allow someone to quickly grab the URL without having to make a copy+paste reference.
					break;
				case ApiRoute.AccountToken:
					apiUrl+="/accountToken";
					break;
				case ApiRoute.PaymentRequest:
					apiUrl+="/paymentRequest";
					break;
				case ApiRoute.PaymentStatus:
					apiUrl+="/paymentStatus";
					break;
				default:
					break;
			}
			return apiUrl;
		}

		private enum ApiRoute {
			Root,
			AccountToken,
			PaymentRequest,
			PaymentStatus,
		}
	}

	///<summary>Response class that can hold information for a web service response or a terminal response.</summary>
	public class PayConnectResponse {
		public string Description;
		public string StatusCode;
		public string AuthCode;
		public string RefNumber;
		public string PaymentToken;
		public DateTime TokenExpiration;
		public string CardType;

		public PayConnectResponse() {
		}
		
		///<summary>For web services.</summary>
		public PayConnectResponse(PayConnectService.transResponse response,PayConnectService.creditCardRequest request) {
			if(response != null) {
				AuthCode=response.AuthCode;
				RefNumber=response.RefNumber;
				if(response.Status != null) {
					Description=response.Status.description;
					StatusCode=response.Status.code.ToString();
				}
				if(response.PaymentToken != null) {
					PaymentToken=response.PaymentToken.TokenId;
					if(response.PaymentToken.Expiration != null) {
						TokenExpiration=new DateTime(response.PaymentToken.Expiration.year,response.PaymentToken.Expiration.month,1);
					}
				}
				CardType=CreditCardUtils.GetCardType(request.CardNumber);
			}
		}		
	}
	
	///<summary>The method of completing PayConnect credit card transactions.</summary>
	public enum PayConnectProcessingMethod {
		///<summary>Performs transactions by making a web call to PayConnect's service.</summary>
		[Description("Web Service")]
		WebService,
		///<summary>Performs the transactions on a credit card terminal hooked up to the computer.</summary>
		[Description("Terminal")]
		Terminal,
	}

}
