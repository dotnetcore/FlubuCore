<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<%
	string url = Model != null && Model.Contains("site=true")
		? Request.Url.Scheme + "://" + Request.Url.Authority + Html.Content().Traverse.StartPage.Url
		: Request.Url.ToString();
	url = HttpUtility.HtmlAttributeEncode(url);
	bool track = Model != null && Model.Contains("track=true");
%>

<div style="min-height:30px;">
	<div id="fb-root"></div>
	<script>
		(function (d, s, id) {
			var js, fjs = d.getElementsByTagName(s)[0];
			if (d.getElementById(id)) return;
			js = d.createElement(s); js.id = id;
			js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
			fjs.parentNode.insertBefore(js, fjs);
		} (document, 'script', 'facebook-jssdk'));

<% if(track) { %>
		$(document).ready(function () {
			function push(type, targetUrl) {
				if (typeof _gaq !== "undefined" && _gaq.push)
					_gaq.push(['_trackSocial', 'facebook', type, targetUrl]);
			};
			function subscribe() {
				if (typeof FB !== 'undefined' && FB.Event && FB.Event.subscribe) {
					FB.Event.subscribe('edge.create', function (targetUrl) {
						push('like', targetUrl);
					});

					FB.Event.subscribe('edge.remove', function (targetUrl) {
						push('unlike', targetUrl);
					});

					FB.Event.subscribe('message.send', function (targetUrl) {
						push('send', targetUrl);
					});
				}
			};

			if (typeof _gaq === "undefined" || typeof FB == 'undefined')
				setTimeout(subscribe, 2500);
			else
				subscribe();
		});
<% } %>
	</script>
	<div class="fb-like" data-href="<%= url %>" data-send="true" data-layout="button_count" data-show-faces="false" data-font="arial" style="margin:10px 0"></div>
</div>