<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<%
	string url = Model != null && Model.Contains("site=true")
		? Request.Url.Scheme + "://" + Request.Url.Authority + Html.Content().Traverse.StartPage.Url
		: Request.Url.ToString();
	url = HttpUtility.HtmlAttributeEncode(url);
%>

<div style="min-height:30px;">
	<div class="g-plusone" data-size="medium" data-annotation="inline" data-href="<%= url %>" style="margin:10px 0"></div>

	<script type="text/javascript">
		(function () {
			var po = document.createElement('script');
			po.type = 'text/javascript';
			po.async = true;
			po.src = 'https://apis.google.com/js/plusone.js';
			var s = document.getElementsByTagName('script')[0];
			s.parentNode.insertBefore(po, s);
		})();
	</script>
</div>