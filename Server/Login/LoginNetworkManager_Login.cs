using Cysharp.Threading.Tasks;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System;

namespace MultiplayerARPG.MMO
{
    public partial class LoginNetworkManager
    {
        public bool RequestUserLogin(string username, string password, ResponseDelegate<ResponseUserLoginMessage> callback)
        {
            return ClientSendRequest(MMORequestTypes.UserLogin, new RequestUserLoginMessage()
            {
                username = username,
                password = password,
            }, responseDelegate: callback);
        }

        public bool RequestUserRegister(string username, string password, string email, ResponseDelegate<ResponseUserRegisterMessage> callback)
        {
            return ClientSendRequest(MMORequestTypes.UserRegister, new RequestUserRegisterMessage()
            {
                username = username,
                password = password,
                email = email,
            }, responseDelegate: callback);
        }

        public bool RequestUserLogout(ResponseDelegate<INetSerializable> callback)
        {
            return ClientSendRequest(MMORequestTypes.UserLogout, EmptyMessage.Value, responseDelegate: callback);
        }

        public bool RequestValidateAccessToken(string userId, string accessToken, ResponseDelegate<ResponseValidateAccessTokenMessage> callback)
        {
            return ClientSendRequest(MMORequestTypes.ValidateAccessToken, new RequestValidateAccessTokenMessage()
            {
                userId = userId,
                accessToken = accessToken,
            }, responseDelegate: callback);
        }

        public bool RequestChannels(ResponseDelegate<ResponseChannelsMessage> callback)
        {
            return ClientSendRequest(MMORequestTypes.Channels, EmptyMessage.Value, responseDelegate: callback);
        }

        protected async UniTaskVoid HandleRequestUserLogin(
            RequestHandlerData requestHandler,
            RequestUserLoginMessage request,
            RequestProceedResultDelegate<ResponseUserLoginMessage> result)
        {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
            if (disableDefaultLogin)
            {
                result.InvokeError(new ResponseUserLoginMessage()
                {
                    message = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE,
                });
                return;
            }

            if (activeLogins >= MaxConcurrentRequest)
            {
                loginQueue.Enqueue(new LoginQueueEntry
                {
                    handler = requestHandler,
                    request = request,
                    result = result
                });
                return;
            }

            await ProcessLogin(requestHandler, request, result);
#endif
        }

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
        private async UniTask ProcessLogin(
            RequestHandlerData handler,
            RequestUserLoginMessage request,
            RequestProceedResultDelegate<ResponseUserLoginMessage> result)
        {
            activeLogins++;

            try
            {
                long connectionId = handler.ConnectionId;
                DatabaseApiResult<ValidateUserLoginResp> validateUserLoginResp = await DatabaseClient.ValidateUserLoginAsync(new ValidateUserLoginReq()
                {
                    Username = request.username,
                    Password = request.password
                });
                if (!validateUserLoginResp.IsSuccess)
                {
                    result.InvokeError(new ResponseUserLoginMessage()
                    {
                        message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                    });
                    return;
                }
                string userId = validateUserLoginResp.Response.UserId;
                string accessToken = string.Empty;
                if (string.IsNullOrEmpty(userId))
                {
                    result.InvokeError(new ResponseUserLoginMessage()
                    {
                        message = UITextKeys.UI_ERROR_INVALID_USERNAME_OR_PASSWORD,
                    });
                    return;
                }
                // User already logged in
                if (_userPeersByUserId.ContainsKey(userId) || await MapContainsUser(userId))
                {
                    // Kick the user from game
                    if (_userPeersByUserId.ContainsKey(userId))
                        KickClient(_userPeersByUserId[userId].connectionId, UITextKeys.UI_ERROR_ACCOUNT_LOGGED_IN_BY_OTHER);
                    KickUser(userId, UITextKeys.UI_ERROR_ACCOUNT_LOGGED_IN_BY_OTHER);
                    RemoveUserPeerByUserId(userId, out _);
                    result.InvokeError(new ResponseUserLoginMessage()
                    {
                        message = UITextKeys.UI_ERROR_ALREADY_LOGGED_IN,
                    });
                    return;
                }
                // Email verification
                bool emailVerified = true;
                if (requireEmailVerification)
                {
                    DatabaseApiResult<ValidateEmailVerificationResp> validateEmailVerificationResp = await DatabaseClient.ValidateEmailVerificationAsync(new ValidateEmailVerificationReq()
                    {
                        UserId = userId
                    });
                    if (!validateEmailVerificationResp.IsSuccess)
                    {
                        result.InvokeError(new ResponseUserLoginMessage()
                        {
                            message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                        });
                        return;
                    }
                    emailVerified = validateEmailVerificationResp.Response.IsPass;
                }
                // Banning verification
                DatabaseApiResult<GetUserUnbanTimeResp> unbanTimeResp = await DatabaseClient.GetUserUnbanTimeAsync(new GetUserUnbanTimeReq()
                {
                    UserId = userId
                });
                if (!unbanTimeResp.IsSuccess)
                {
                    result.InvokeError(new ResponseUserLoginMessage()
                    {
                        message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                    });
                    return;
                }
                long unbanTime = unbanTimeResp.Response.UnbanTime;
                if (unbanTime > System.DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    result.InvokeError(new ResponseUserLoginMessage()
                    {
                        message = UITextKeys.UI_ERROR_USER_BANNED,
                    });
                    return;
                }
                if (!emailVerified)
                {
                    result.InvokeError(new ResponseUserLoginMessage()
                    {
                        message = UITextKeys.UI_ERROR_EMAIL_NOT_VERIFIED,
                    });
                    return;
                }
                // Generate new access token
                accessToken = DataManager.GenerateAccessToken(userId);
                DatabaseApiResult updateAccessTokenResp = await DatabaseClient.UpdateAccessTokenAsync(new UpdateAccessTokenReq()
                {
                    UserId = userId,
                    AccessToken = accessToken,
                });
                if (!updateAccessTokenResp.IsSuccess)
                {
                    result.InvokeError(new ResponseUserLoginMessage()
                    {
                        message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                    });
                    return;
                }
                // Update peer info
                CentralUserPeerInfo userPeerInfo = new CentralUserPeerInfo()
                {
                    connectionId = connectionId,
                    userId = userId,
                    accessToken = accessToken,
                };
                _userPeersByUserId[userId] = userPeerInfo;
                _userPeers[connectionId] = userPeerInfo;
                // Response
                result.InvokeSuccess(new ResponseUserLoginMessage()
                {
                    userId = userId,
                    accessToken = accessToken,
                    unbanTime = unbanTime,
                });
            }
            finally
            {
                activeLogins--;
                TryDequeue();
            }
        }

        private void TryDequeue()
        {
            if (loginQueue.Count == 0)
                return;

            if (activeLogins >= MaxConcurrentRequest)
                return;

            var entry = loginQueue.Dequeue();
            ProcessLogin(entry.handler, entry.request, entry.result).Forget();
        }

#endif


        protected async UniTaskVoid HandleRequestUserRegister(
            RequestHandlerData requestHandler,
            RequestUserRegisterMessage request,
            RequestProceedResultDelegate<ResponseUserRegisterMessage> result)
        {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
            if (disableDefaultLogin)
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE,
                });
                return;
            }
            string username = request.username.Trim();
            string password = request.password.Trim();
            string email = request.email.Trim();
            if (!NameExtensions.IsValidUsername(username))
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_INVALID_USERNAME,
                });
                return;
            }
            if (requireEmail)
            {
                if (string.IsNullOrEmpty(email) || !email.IsValidEmail())
                {
                    result.InvokeError(new ResponseUserRegisterMessage()
                    {
                        message = UITextKeys.UI_ERROR_INVALID_EMAIL,
                    });
                    return;
                }
                DatabaseApiResult<FindEmailResp> findEmailResp = await DatabaseClient.FindEmailAsync(new FindEmailReq()
                {
                    Email = email
                });
                if (!findEmailResp.IsSuccess)
                {
                    result.InvokeError(new ResponseUserRegisterMessage()
                    {
                        message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                    });
                    return;
                }
                if (findEmailResp.Response.FoundAmount > 0)
                {
                    result.InvokeError(new ResponseUserRegisterMessage()
                    {
                        message = UITextKeys.UI_ERROR_EMAIL_ALREADY_IN_USE,
                    });
                    return;
                }
            }
            DatabaseApiResult<FindUsernameResp> findUsernameResp = await DatabaseClient.FindUsernameAsync(new FindUsernameReq()
            {
                Username = username
            });
            if (!findUsernameResp.IsSuccess)
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                });
                return;
            }
            if (findUsernameResp.Response.FoundAmount > 0)
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_USERNAME_EXISTED,
                });
                return;
            }
            if (string.IsNullOrEmpty(username) || username.Length < minUsernameLength)
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_USERNAME_TOO_SHORT,
                });
                return;
            }
            if (username.Length > maxUsernameLength)
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_USERNAME_TOO_LONG,
                });
                return;
            }
            if (string.IsNullOrEmpty(password) || password.Length < minPasswordLength)
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_PASSWORD_TOO_SHORT,
                });
                return;
            }
            DatabaseApiResult createResp = await DatabaseClient.CreateUserLoginAsync(new CreateUserLoginReq()
            {
                Username = username,
                Password = password,
                Email = email,
            });
            if (!createResp.IsSuccess)
            {
                result.InvokeError(new ResponseUserRegisterMessage()
                {
                    message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                });
                return;
            }
            // Response
            result.InvokeSuccess(new ResponseUserRegisterMessage());
#endif
        }

        protected async UniTaskVoid HandleRequestUserLogout(
            RequestHandlerData requestHandler,
            EmptyMessage request,
            RequestProceedResultDelegate<EmptyMessage> result)
        {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
            if (RemoveUserPeerByConnectionId(requestHandler.ConnectionId, out CentralUserPeerInfo userPeerInfo))
            {
                // Clear access token
                DatabaseApiResult updateAccessTokenResp = await DatabaseClient.UpdateAccessTokenAsync(new UpdateAccessTokenReq()
                {
                    UserId = userPeerInfo.userId,
                    AccessToken = string.Empty
                });
                if (!updateAccessTokenResp.IsSuccess)
                {
                    result.InvokeError(EmptyMessage.Value);
                    return;
                }
            }
            // Response
            result.InvokeSuccess(EmptyMessage.Value);
#endif
        }

        protected async UniTaskVoid HandleRequestValidateAccessToken(
            RequestHandlerData requestHandler,
            RequestValidateAccessTokenMessage request,
            RequestProceedResultDelegate<ResponseValidateAccessTokenMessage> result)
        {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
            long connectionId = requestHandler.ConnectionId;
            string userId = request.userId;
            string accessToken = request.accessToken;
            DatabaseApiResult<ValidateAccessTokenResp> validateAccessTokenResp = await DatabaseClient.ValidateAccessTokenAsync(new ValidateAccessTokenReq()
            {
                UserId = userId,
                AccessToken = accessToken
            });
            if (!validateAccessTokenResp.IsSuccess)
            {
                result.InvokeError(new ResponseValidateAccessTokenMessage()
                {
                    message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                });
                return;
            }
            if (!validateAccessTokenResp.Response.IsPass)
            {
                result.InvokeError(new ResponseValidateAccessTokenMessage()
                {
                    message = UITextKeys.UI_ERROR_INVALID_USER_TOKEN,
                });
                return;
            }
            // Banning verification
            DatabaseApiResult<GetUserUnbanTimeResp> unbanTimeResp = await DatabaseClient.GetUserUnbanTimeAsync(new GetUserUnbanTimeReq()
            {
                UserId = userId
            });
            if (!unbanTimeResp.IsSuccess)
            {
                result.InvokeError(new ResponseValidateAccessTokenMessage()
                {
                    message = UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR,
                });
                return;
            }
            long unbanTime = unbanTimeResp.Response.UnbanTime;
            if (unbanTime > System.DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                result.InvokeError(new ResponseValidateAccessTokenMessage()
                {
                    message = UITextKeys.UI_ERROR_USER_BANNED,
                });
                return;
            }
            // Update peer info
            RemoveUserPeerByUserId(userId, out _);
            CentralUserPeerInfo userPeerInfo = new CentralUserPeerInfo()
            {
                connectionId = connectionId,
                userId = userId,
                accessToken = accessToken,
            };
            _userPeersByUserId[userId] = userPeerInfo;
            _userPeers[connectionId] = userPeerInfo;
            // Response
            result.InvokeSuccess(new ResponseValidateAccessTokenMessage()
            {
                userId = userId,
                accessToken = accessToken,
                unbanTime = unbanTime,
            });
#endif
        }

        protected async UniTaskVoid HandleRequestChannels(
            RequestHandlerData requestHandler,
            EmptyMessage request,
            RequestProceedResultDelegate<ResponseChannelsMessage> result)
        {
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
            // Response
            result.InvokeSuccess(new ResponseChannelsMessage()
            {
                channels =  await GetChannels(),
            });
#endif
        }
    }
}
