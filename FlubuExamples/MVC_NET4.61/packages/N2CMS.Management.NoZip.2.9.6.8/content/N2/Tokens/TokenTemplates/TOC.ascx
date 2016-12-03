<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var id = "toc" + Guid.NewGuid().GetHashCode(); %>

<div id="<%= id %>" class="toc">
</div>

<%= Html.Resources().JQuery() %>

<script type="text/javascript">
	$(document).ready(function () {
		var indentation = 0;
		var opened = false;
		var html = "";
		$("h1,h2,h3,h4", "#main").each(function (i) {
			var expected = 0;
			switch (this.tagName) {
				case "H1":
					expected = 1;
					break;
				case "H2":
					expected = 2;
					break;
				case "H3":
					expected = 3;
					break;
				case "H4":
					expected = 4;
					break;
			}

			for (; indentation < expected; indentation++) {
				html += "<ul style='list-style-type:none'><li>";
			}
			for (; indentation > expected; indentation--) {
				html += "</li></ul>";
			}
			if (!this.id)
				this.id = "toc" + i;
			html += "<a style='display:block' href='#" + this.id + "'>" + this.innerHTML + "</a>";
		});
		$("#<%= id %>").html(html);
	});
</script>