<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diagnose.aspx.cs" Inherits="N2.Edit.Install.Diagnose" %>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="System.Reflection"%>
<%@ Import Namespace="N2.Web"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Diagnose N2</title>
	<asp:PlaceHolder runat="server">
		<link href="<%=  N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapCssPath) %>" type="text/css" rel="stylesheet" />
		<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.BootstrapJsPath)  %>" type="text/javascript"></script>
	</asp:PlaceHolder>
    <link rel="stylesheet" href="../Resources/Css/All.css" type="text/css" />
    <style type="text/css">
        label{font-weight:bold;margin:5px 10px 0 0;}
        input{vertical-align:middle;margin-bottom:5px;}
        ul,li{margin-top:0;margin-bottom:0;}
        textarea{height:55px;width:70%;background-color:#FFA07A;border:none;font-size:11px}
		th {text-align:right; vertical-align:top; background-color:#eee; border:solid 1px #fff; }
        .t {font-size:.8em; width:100%;}
        .t thead td{ font-weight:bold; background-color:#eee;}
		.t th h2 { margin:0; padding:.1em; background-color:#ccc; width:auto;}
        .t td{vertical-align:top;text-align:left;border:solid 1px #eee; padding:1px; font-size:inherit; }
        .t input { font-size:inherit; }
        .EnabledFalse { color:#999; }
        .IsDefinedFalse { color:Red; }
        a { color:Blue; }
        .expandable { padding:1px; position:relative; }
			.expandable .opened { display:none; }
			.expandable .opener { display:block; }
			.expandable:hover .opened { display:block; position:absolute; background:#fff; z-index:9999; }
			.expandable:hover .opener { display:none; }
    </style>
	<script type="text/javascript">
		$(document).ready(function () {
			var $tb = $("table.openable").children("tbody").hide().end();
			$("<tbody><tr><td><a href='javascript:void(0);'>Show</a></td></tr></tbody>").appendTo($tb)
				.find("a").click(function (e) {
					$(this).closest("tbody").hide().siblings().show();
				});
		});
	</script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>N2 CMS diagnostic page</h1>

			<table class="t">
				<tbody>
					<tr><th colspan="2"><h2>Database</h2></th></tr>
					<tr><th>Connection string name</th><td><%
															string connectionStringName = Engine.Resolve<N2.Configuration.DatabaseSection>().ConnectionStringName;
															Response.Write(connectionStringName);
															%></td></tr>
					
					<tr><th>Connection provider</th><td><%
					try {
						if (string.IsNullOrEmpty(connectionStringName))
							Response.Write("No connection string name configured");
						else if (System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName] == null)
							Response.Write("No connection string with name '" + connectionStringName + "'");
						else
							Response.Write(System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);
					} catch (Exception ex) { Response.Write("<tr><td>" + ex + "</td></tr>"); }
					%></td></tr>
					<tr><th>Connection</th><td><asp:Label ID="lblDbConnection" runat="server" /></td></tr>
					<tr><th>Root item</th><td><asp:Label ID="lblRootNode" runat="server" /></td></tr>
					<tr><th>Start page</th><td><asp:Label ID="lblStartNode" runat="server" /></td></tr>
					<tr><th>N2 version (recorded)</th><td><%= typeof(N2.Content).Assembly.GetName().Version %> (<%= Status.RecordedAssemblyVersion %>)</td></tr>
					<tr><th rowspan="<%= 1 + Status.RecordedFeatures.Length %>">Installed features (recorded)</th></tr>
					<% foreach (string feature in Status.RecordedFeatures){ %>
					<tr><td><%= feature %></td></tr>
					<% } %>
					<tr><td></td></tr>
					
					<tr><th>Needs <a href="Upgrade.aspx">upgrade</a></th><td><%= Status.NeedsUpgrade %></td></tr>
					<tr><th>Needs <a href="Rebase.aspx">rebase</a></th><td><%= Status.NeedsRebase %></td></tr>
					<tr><th rowspan="<%= recentChanges.Length + 1 %>">Recent changes </th><td><asp:Label ID="lblChanges" runat="server" />
						<% foreach (string change in recentChanges){ %>
							<%= change %> </td></tr><tr><td>
						<% } %>
					</td></tr>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Key assemblies & types</h2></th></tr>
					<tr><th>N2 version</th><td><%= typeof(N2.Context).Assembly.GetName().Version %> (<%= N2.Management.Installation.InstallationUtility.GetFileVersion(typeof(N2.Context).Assembly)  %>)</td></tr>
					<tr><th>N2.Management version</th><td><%= typeof(N2.Edit.Install.Diagnose).Assembly.GetName().Version %> (<%= N2.Management.Installation.InstallationUtility.GetFileVersion(typeof(N2.Edit.Install.Diagnose).Assembly) %>)</td></tr>
<% try { %>
					<tr><th>Engine type</th><td><%= Engine.GetType() %></td></tr>
					<tr><th>IoC Container type</th><td><%= Engine.Container.GetType() %></td></tr>
					<tr><th>Url parser type</th><td><%= Engine.Resolve<N2.Web.IUrlParser>().GetType() %></td></tr>
					<tr><th>File System type</th><td><%= Engine.Resolve<N2.Edit.FileSystem.IFileSystem>().GetType() %></td></tr>
					<tr>
						<th>Sources</th>
						<td>
							<% foreach(object cs in Engine.Resolve<N2.Persistence.Sources.ContentSource>().Sources) { %>
							<%= cs %><br />
							<% } %>
						</td>
					</tr>
<% } catch (Exception ex) { Response.Write("<tr><th>Error</th><td>" + ex.ToString() + "</td>"); } %>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Server</h2></th></tr>
					<tr><th>Trust Level</th><td><%= GetTrustLevel() %></td></tr>
					<tr><th>Application Path</th><td><%= Request.ApplicationPath %></td></tr>
					<tr><th>CurrentDirectory</th><td><% try { Response.Write(Environment.CurrentDirectory); } catch(Exception ex) { Response.Write(ex.Message); } %></td></tr>
					<tr><th>BaseDirectory</th><td><% try { Response.Write(AppDomain.CurrentDomain.BaseDirectory); } catch(Exception ex) { Response.Write(ex.Message); } %></td></tr>
					<tr><th>ApplicationPath</th><td><%= Request.ApplicationPath %></td></tr>
					<tr><th>MapPath</th><td><%= Server.MapPath("~/") %></td></tr>
					<tr><th>PhysicalApplicationPath</th><td><%= Request.PhysicalApplicationPath %></td></tr>
					<tr><th>Path</th><td><%= Request.Path %></td></tr>
					<tr><th>PhysicalPath</th><td><%= Request.PhysicalPath %></td></tr>
					<tr><th>Culture</th><td><%= Culture %></td></tr>
					<tr><th>UI Culture</th><td><%= UICulture %></td></tr>
					<tr><th>Config culture</th><td><% try { Response.Write(((System.Web.Configuration.GlobalizationSection)System.Configuration.ConfigurationManager.GetSection("system.web/globalization")).Culture); } catch (Exception ex) { Response.Write(ex.Message); } %></td></tr>
					<tr><th>Config UI culture</th><td><% try { Response.Write(((System.Web.Configuration.GlobalizationSection)System.Configuration.ConfigurationManager.GetSection("system.web/globalization")).UICulture); } catch (Exception ex) { Response.Write(ex.Message); } %></td></tr>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Security</h2></th></tr>
					<tr><th>Membership</th><td><%= System.Web.Security.Membership.Provider %></td></tr>
					<tr><th>Roles</th><td><% try { Response.Write(System.Web.Security.Roles.Provider.ToString()); } catch(Exception ex) { Response.Write(ex.Message); } %></td></tr>
					<tr><th>Profile</th><td><%= System.Web.Profile.ProfileManager.Provider %></td></tr>
					<tr><th>User type</th><td><%= User %> <%= User.Identity %></td></tr>
					<tr><th>User</th><td><%= User.Identity.Name %></td></tr>
					<tr><th rowspan="3">Roles</th><td>Writers: <%= User.IsInRole("Writers") %></td></tr>
					<tr><td>Editors: <%= User.IsInRole("Editors") %></td></tr>
					<tr><td>Administrators: <%= User.IsInRole("Administrators") %></td></tr>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Sites</h2></th></tr>
<% try { %>
					<tr><th>Default (fallback) site</th><td><%= host.DefaultSite %></td></tr>
					<tr><th>Current site (<%= Request.Url.Authority %>)</th><td><%= host.CurrentSite %></td></tr>
					<tr>
						<th rowspan="<%= host.Sites.Count %>">All sites</th>
				<% foreach (Site site in host.Sites){ %>
						<td><%= site %></td></tr><tr>
				<% } %>
					</tr>
<% } catch (Exception ex) { Response.Write(ex.ToString()); } %>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Errors</h2></th></tr>
					<tr><th>Last error</th><td><asp:Label ID="lblError" runat="server" /></td></tr>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>MVC</h2></th></tr>
<% try {%>
					<tr><th rowspan="3">Versions</th><td><%= Assembly.LoadWithPartialName("System.Web.Mvc").FullName %></td></tr>
					<tr><td><%= Assembly.LoadWithPartialName("System.Web.Abstractions").FullName %></td></tr>
					<tr><td><%= Assembly.LoadWithPartialName("System.Web.Routing").FullName %></td></tr>
<% } catch (Exception ex) { Response.Write("<tr><td>" + ex + "</td></tr>"); } %>
				</tbody>
			</table>

            <asp:Repeater ID="rptDefinitions" runat="server">
                <HeaderTemplate>
					<table class="t openable">
						<thead>
							<tr><th colspan="9"><h2>Definitions</h2></th></tr>
							<tr>
								<td colspan="3">Definition</td>
								<td colspan="2">Zones</td>
								<td colspan="3">Details</td>
								<td rowspan="2">Templates</td>
							</tr>
							<tr>
								<td>Type</td>
								<td>Template</td>
								<td>Allowed children</td>
								<td>Available</td>
								<td>Allowed in</td>
								<td>Editables</td>
								<td>Containers</td>
								<td>Displayables</td>
							</tr>
						</thead>
						<tbody>
				</HeaderTemplate>
                <ItemTemplate>
                    <tr class="<%# Eval("Enabled", "Enabled{0}") %> <%# Eval("IsDefined", "IsDefined{0}") %>"><td>
                        <b title='#items: <%# Eval("NumberOfItems") %>'><%# Eval("Title") %></b><br />
						<span title='Discriminator: <%# Eval("Discriminator") %>, Type: <%# Eval("ItemType") %>' style="color:gray"><%# ((System.Type)Eval("ItemType")).Name %></span>						
                    </td><td>
						<%# Eval("TemplateKey") %>
                    </td><td>
                        <!-- Child definitions -->
                        <div class="expandable">
							<strong class="opener"><%# AllowedChildren(Container.DataItem).Count %></strong>
							<div class="opened">
							<asp:Repeater ID="Repeater1" runat="server" DataSource='<%# AllowedChildren(Container.DataItem) %>'>
								<ItemTemplate><%# Eval("Title")%></ItemTemplate>
								<SeparatorTemplate>, </SeparatorTemplate>
							</asp:Repeater></div>
						</div>
                    </td><td>
                        <!-- Available zones -->
                        <div class="expandable">
							<strong class="opener"><%# Eval("AvailableZones.Count") %></strong>
							<div class="opened">
                        <asp:Repeater ID="Repeater2" runat="server" DataSource='<%# Eval("AvailableZones") %>'>
                            <ItemTemplate><%# Eval("ZoneName") %>&nbsp;(<%# Eval("Title") %>)</ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;</div>
                    </td><td>
						<b><%# Eval("AllowedIn")%>: </b>
                        <span class="expandable">
							<strong class="opener"><%# Eval("AllowedZoneNames.Count") %></strong>
							<div class="opened">
                        <!-- Allowed in zone -->
                        <asp:Repeater ID="Repeater3" runat="server" DataSource='<%# Eval("AllowedZoneNames") %>'>
                            <ItemTemplate><%# Container.DataItem %></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;</span>
                    </td><td>
                        <!-- Editable attributes -->
                        <div class="expandable">
							<strong class="opener"><%# Eval("Editables.Count") %></strong>
							<div class="opened">
                        <asp:Repeater ID="Repeater4" runat="server" DataSource='<%# Eval("Editables") %>'>
                            <ItemTemplate><span title="Title: <%# Eval("Title")%>, Type: <%# Container.DataItem.GetType() %>, Container: <%# Eval("ContainerName") %>"><%# Eval("Name")%></span></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
							</div>
                    </td><td>
                        <!-- Container attributes -->
                        <asp:Repeater ID="Repeater7" runat="server" DataSource='<%# Eval("Containers") %>'>
                            <ItemTemplate><span title="<%# Container.DataItem.GetType() %>"><%# Eval("Name")%></span></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
                    </td><td>
                        <!-- Displayable attributes -->
                        <div class="expandable">
							<strong class="opener"><%# Eval("Displayables.Count") %></strong>
							<div class="opened">
                        <asp:Repeater ID="Repeater5" runat="server" DataSource='<%# Eval("Displayables") %>'>
                            <ItemTemplate><span title="<%# Container.DataItem.GetType() %>"><%# ((N2.Details.IDisplayable)Container.DataItem).Name %></span></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;</div>
                    </td><td>
                        <div class="expandable">
							<strong class="opener"><%# PathDictionary.GetFinders((Type)Eval("ItemType")).Length %></strong>
							<div class="opened">
                        <asp:Repeater ID="Repeater6" runat="server" DataSource='<%# PathDictionary.GetFinders((Type)Eval("ItemType")) %>'>
                            <ItemTemplate><%# Container.DataItem is TemplateAttribute ? ("/" + Eval("Action") + "&nbsp;->&nbsp;" + Eval("TemplateUrl")) : ("(" + Container.DataItem.GetType().Name + ")")%><br></ItemTemplate>
                        </asp:Repeater>&nbsp;</div>
                    </td></tr>
                </ItemTemplate>
                <FooterTemplate>
						</tbody>
					</table>
				</FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblDefinitions" runat="server" />

            
            <asp:Repeater ID="rptAssembly" runat="server">
                <HeaderTemplate><table class="t openable"><thead><tr><th colspan="7"><h2>Assemblies</h2></th></tr><tr><td>Assembly Name</td><td>Version</td><td>Culture</td><td>Public Key</td><td>File version</td><td>References N2</td><td>Definitions</td></tr></thead><tbody></HeaderTemplate>
                <ItemTemplate><tr>
                <asp:Repeater runat="server" DataSource="<%# ((Assembly)Container.DataItem).FullName.Split(',') %>">
					<ItemTemplate>
						<td><%# Container.DataItem %></td>
					</ItemTemplate>
                </asp:Repeater>
                <td><%# N2.Management.Installation.InstallationUtility.GetFileVersion((Assembly)Container.DataItem) %></td>
				<td><%# Array.Find(((Assembly)Container.DataItem).GetReferencedAssemblies(), delegate(AssemblyName an) { return an.Name.StartsWith("N2"); }) != null %></td>
				<td><%# GetDefinitions((Assembly)Container.DataItem) %></td>
                </tr></ItemTemplate>
                <FooterTemplate></tbody></table></FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblAssemblies" runat="server" />

			
			<% try { %>
			<table class="t openable"><thead><tr><th colspan="2"><h2>Services</h2></th></tr><tr><td>Service type</td><td>Implementation type</td></tr></thead>
			<tbody>
			<% foreach (N2.Engine.ServiceInfo info in Engine.Container.Diagnose()) { %>
				<tr><td><%= info.ServiceType %></td><td><%= info.ImplementationType %></td></tr>
			<% } %>
			</tbody></table>
			<% } catch (Exception ex) { %>
			<pre><%= ex %></pre>
			<% } %>

			
			<% try { %>
			<table class="t openable"><thead><tr><th colspan="2"><h2>Interface context</h2></th></tr></thead>
			<tbody>
				<% var ctx = Engine.Resolve<N2.Management.Api.InterfaceBuilder>().GetInterfaceDefinition(new System.Web.HttpContextWrapper(Context), new N2.Edit.SelectionUtility(Context, Engine)); %>
				<tr><th colspan="2">MainMenu</th></tr>
				<% foreach (var mi in ctx.MainMenu.Children) { %>
				<tr>
					<td><strong><%= mi.Current.Name %></strong></td>
					<td>
						<% foreach (var mi2 in mi.Children) { %>
						<div><%= mi2.Current.Name %></div>
						<% } %>
					</td>
			    </tr>
				<% } %>
				<tr><th colspan="2">ActionMenu</th></tr>
				<% foreach (var mi in ctx.ActionMenu.Children) { %>
				<tr>
					<td><strong><%= mi.Current.Name %></strong></td>
					<td>
						<% foreach (var mi2 in mi.Children) { %>
						<div><%= mi2.Current.Name %></div>
						<% } %>
					</td>
			    </tr>
				<% } %>
				<tr><th colspan="2">ContextMenu</th></tr>
				<% foreach (var mi in ctx.ContextMenu.Children) { %>
				<tr>
					<td><strong><%= mi.Current.Name %></strong></td>
					<td>
						<% foreach (var mi2 in mi.Children) { %>
						<div><%= mi2.Current.Name %></div>
						<% } %>
					</td>
			    </tr>
				<% } %>
			</tbody></table>
			<% } catch (Exception ex) { %>
			<pre><%= ex %></pre>
			<% } %>


			<% try { %>
			<table class="t openable"><thead><tr><th colspan="2"><h2>Cache</h2></th></tr><tr><td>NH Cache Region</td><td>Cache</td></tr></thead>
			<tbody>
			<% foreach (KeyValuePair<string, NHibernate.Cache.ICache> kvp in ((NHibernate.Impl.SessionFactoryImpl)Engine.Resolve<N2.Persistence.NH.IConfigurationBuilder>().BuildSessionFactory()).GetAllSecondLevelCacheRegions()) { %>
				<tr><td><%= kvp.Key%></td><td><%= kvp.Value%></td></tr>
			<% } %>
			</tbody></table>
			<% } catch (Exception ex) { %>
			<pre><%= ex %></pre>
			<% } %>

			<% try { %>

			<% if (Request["showcache"] != null) { %>
			<table id="runtimecache" class="t"><thead><tr><td>Runtime Cache Key</td><td>Value</td></tr></thead>
			<tbody>
			<% foreach (System.Collections.DictionaryEntry c in Context.Cache) { if(Request["cachefilter"] != null && !c.Key.ToString().Contains(Request["cachefilter"])) continue; %>
				<tr><td><%= c.Key %></td><td><%= c.Value is System.Collections.DictionaryEntry ? ("<b>" + ((System.Collections.DictionaryEntry)c.Value).Key + "</b>: " + ((System.Collections.DictionaryEntry)c.Value).Value) : c.Value%></td></tr>
			<% } %>
			</tbody></table>
			<% } else { %>
			<table class="t"><thead><td colspan="2">Runtime Cache</td></thead><tbody><tr><th>Count</th><td><%= Context.Cache.Count %> <a href="?showcache=true#runtimecache">(Show)</a></td></tr></tbody></table>
			<% } %>

			<% } catch (Exception ex) { %>
			<pre><%= ex %></pre>
			<% } %>


			<% try { %>
			<table class="t openable"><thead>
				<tr><th colspan="7"><h2>Scheduler</h2></th></tr>
				<tr><td>Name</td><td>Is executing</td><td>Enabled</td><td>Last executed</td><td>Interval</td><td>Error count</td><td>Last error</td></tr></thead>
			<tbody>
			<% foreach (N2.Plugin.Scheduling.ScheduledAction action in ScheduledActions) { %>
				<tr><td><%= action.GetType().Name %></td><td><%= action.IsExecuting %></td><td><%= action.Enabled %></td><td><%= action.LastExecuted %></td><td><%= action.Interval %></td><td><%= action.ErrorCount %></td><td><%= action.LastError %></td></tr>
			<% } %>
			</tbody></table>
			<% } catch (Exception ex) { %>
			<pre><%= ex %></pre>
			<% } %>

        </div>
    </form>
</body>
</html>
