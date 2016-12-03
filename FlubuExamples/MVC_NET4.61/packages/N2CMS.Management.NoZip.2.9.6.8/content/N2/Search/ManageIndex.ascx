<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageIndex.ascx.cs" Inherits="N2.Management.Search.ManageIndex" %>

<fieldset><legend>Status</legend>
    <table>
        <tr>
            <th><asp:Label runat="server" Text="Indexer type" /></th><td id="IndexerType"><%= IndexerType %></td>
        </tr><tr>
            <th><asp:Label runat="server" Text="Total documents" /></th><td id="TotalDocuments"><%= Statistics.TotalDocuments %></td>
        </tr><tr>
            <th><asp:Label runat="server" Text="Worker count" /></th><td id="WorkerCount"><%= Status.WorkerCount %></td>
        </tr><tr>
            <th><asp:Label runat="server" Text="Current work" /></th><td id="CurrentWork"><%= Status.CurrentWork %></td>
        </tr><tr>
            <th><asp:Label runat="server" Text="Queue size" /></th><td id="ErrorQueueCount"><%= Status.QueueSize %></td>
        </tr>
    </table>
</fieldset>

<fieldset><legend>Maintenance</legend>
    <asp:Button ID="btnClear" OnCommand="OnClear" runat="server" Text="Purge index" />
    <asp:Button ID="btnIndex" OnCommand="OnIndex" runat="server" Text="Schedule index of all content" />
</fieldset>

<fieldset><legend>Search</legend>
<asp:Panel DefaultButton="btnSearch" runat="server">
    <asp:TextBox ID="txtSearch" runat="server" />
    <asp:Button ID="btnSearch" OnCommand="OnSearch" runat="server" Text="Search" />
    
    <asp:Repeater ID="rptSearch" runat="server">
        <HeaderTemplate><ul></HeaderTemplate>
        <ItemTemplate>
            <li>
                <a href="<%# Engine.EditManager.GetEditExistingItemUrl((N2.ContentItem)Eval("Content")) %>">
                    <img src="<%# Eval("Content.IconUrl") %>" />
                    <%# Eval("Content.Title") ?? "(empty)" %>
                </a>
            </li>
        </ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
</asp:Panel>
</fieldset>

<script type="text/javascript">
	$(document).ready(function () {
		var checkInterval = <%= Status.WorkerCount > 0 ? 2000 : 10000 %>;
		function recheckIndex() {
			setTimeout(function(){
				$.getJSON('<%= N2.Web.Url.ResolveTokens("{ManagementUrl}/Search/IndexInfo.ashx") %>', function (data) {
					$("#IndexerType").text(data.IndexerType);
					$("#TotalDocuments").text(data.Statistics.TotalDocuments);
					$("#WorkerCount").text(data.Status.WorkerCount);
					$("#CurrentWork").text(data.Status.CurrentWork);
					$("#ErrorQueueCount").text(data.Status.QueueSize);
					recheckIndex();
				});
			}, checkInterval);
		}
		recheckIndex();
    });
</script>