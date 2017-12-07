using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Satochat.Shared.Util;
using Satochat.Shared.ViewModels;

namespace Satochat.Client.Service.Api {
    public abstract class BaseSatoApi {
        private string _token;
        private string _endpoint;

        protected BaseSatoApi(string endpoint) {
            _endpoint = endpoint;
        }

        private async Task<HttpResponseMessage> requestAsync(HttpMethod method, string path, IDictionary<string, string> parameters = null, string content = null, bool authenticating = false, CancellationToken? cancellationToken = null, bool recursing = false) {
            string url = makeUrl(path, parameters);
            var req = new HttpRequestMessage(method, url);

            if (!authenticating && !String.IsNullOrEmpty(_token)) {
                req.Headers.Add("Authorization", String.Format("Bearer {0}", _token));
            }

            if (content != null) {
                req.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            var client = new HttpClient();
            try {
                var completionOption = HttpCompletionOption.ResponseHeadersRead;
                HttpResponseMessage res;
                if (cancellationToken.HasValue) {
                    res = await client.SendAsync(req, completionOption, cancellationToken.Value);
                } else {
                    res = await client.SendAsync(req, completionOption);
                }

                if (res.StatusCode != HttpStatusCode.OK) {
                    if (res.StatusCode == HttpStatusCode.Unauthorized && !recursing) {
                        if (authenticating) {
                            throw new SatoApiException("Authentication failed");
                        }

                        var tokenRes = await getTokenReqAsync(cancellationToken);
                        if (tokenRes.StatusCode != HttpStatusCode.OK) {
                            throw new SatoApiException("Failed to get access token");
                        }

                        var token = await parseGetTokenReqAsync(tokenRes);
                        _token = token.Token;
                        return await requestAsync(method, path, parameters, content, cancellationToken: cancellationToken, recursing: true);
                    }

                    if ((int)res.StatusCode >= 400 && (int)res.StatusCode <= 499) {
                        var str = await res.Content.ReadAsStringAsync();
                        throw new SatoApiException(String.Format("Client error ({0}): {1}", res.StatusCode, str), res.StatusCode);
                    }

                    if ((int)res.StatusCode >= 500 && (int)res.StatusCode <= 599) {
                        var str = await res.Content.ReadAsStringAsync();
                        throw new SatoApiException(String.Format("Server error ({0}): {1}", res.StatusCode, str), res.StatusCode);
                    }
                }

                return res;
            } catch (SatoApiException) {
                throw;
            } catch (HttpRequestException ex) {
                throw new SatoApiException("Network error during API call: " + ex.Message, ex);
            } catch (Exception ex) {
                throw new SatoApiException("Unknown (possibly network) error during API call", ex);
            }
        }

        protected async Task<HttpResponseMessage> getAsync(string path, IDictionary<string, string> parameters = null, CancellationToken? cancellationToken = null) {
            return await requestAsync(HttpMethod.Get, path, parameters, cancellationToken: cancellationToken);
        }

        protected async Task<HttpResponseMessage> postAsync(string path, string content = null, IDictionary<string, string> parameters = null, bool authenticating = false, CancellationToken? cancellationToken = null) {
            return await requestAsync(HttpMethod.Post, path, parameters, content, authenticating, cancellationToken);
        }

        private string makeUrl(string path, IDictionary<string, string> parameters) {
            string url = _endpoint + "/" + path;
            return UrlUtil.MakeUrl(url, parameters);
        }

        protected abstract Credential getCredential();

        private async Task<HttpResponseMessage> getTokenReqAsync(CancellationToken? cancellationToken) {
            var credential = getCredential();
            if (credential == null || String.IsNullOrEmpty(credential.Username) || String.IsNullOrEmpty(credential.Password)) {
                throw new SatoApiException("Credential must be set in order to authenticate");
            }

            var json = JsonConvert.SerializeObject(new AccessTokenViewModel.GetToken(credential.Username, credential.Password));
            return await postAsync("token", json, authenticating: true, cancellationToken: cancellationToken);
        }

        private async Task<AccessTokenViewModel.GetTokenResult> parseGetTokenReqAsync(HttpResponseMessage res) {
            string json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<AccessTokenViewModel.GetTokenResult>(json);
            return obj;
        }
    }
}
