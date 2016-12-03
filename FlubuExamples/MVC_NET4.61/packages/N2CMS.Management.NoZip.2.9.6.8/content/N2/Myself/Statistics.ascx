<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Statistics.ascx.cs" Inherits="N2.Management.Myself.Statistics" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<div class="uc">
	<h4 class="header"><%= CurrentItem.Title %></h4>
	<div class="box">
	<table class="data"><tbody>
		<tr><th>
			# of pages
		</th><td>
			<edit:InfoLabel Label="" id="lblPages" runat="server" />
		</td></tr><tr><th>
			Total # of items:
		</th><td>
			<edit:InfoLabel Label="" id="lblItems" runat="server" />
		</td></tr><tr><th>
			Pages served (since startup):
		</th><td>
			<edit:InfoLabel Label="" id="lblServed" runat="server" />
		</td></tr><tr><th>
			Versions per item:
		</th><td>
			<edit:InfoLabel Label="" id="lblVersionsRatio" runat="server" />
		</td></tr><tr><th>
			# of changes last week:
		</th><td>
				<edit:InfoLabel Label="" id="lblChangesLastWeek" runat="server" />
		</td></tr>		
	</tbody></table>

</div></div>