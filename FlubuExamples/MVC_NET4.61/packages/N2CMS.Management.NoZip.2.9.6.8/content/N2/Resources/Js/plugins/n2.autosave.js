n2autosave = {
	interval: 10000,
	dirty: false,
	destroy: function(){
		clearTimeout(this.intervalHandle);
	},
	init: function () {
		this.getChanges = function () {
			var dirtbags = [];
			jQuery.each(this.adapters, function () {
				if (this.dirty()) {
					dirtbags.push(this);
				}
			});
			return dirtbags;
		}

		this.extractChangedItems = function (dirtbags) {
			var items = {};
			jQuery.each(dirtbags, function () {
				var idAndVersion = $("#" + this.newItemReference).val() || "";
				var item = items[this.itemID] || (items[this.itemID] = { changes: { ID: parseInt(idAndVersion.split(".")[0]), VersionIndex: parseInt(idAndVersion.split(".")[1]) } });
				item.changes[this.name] = this.checkout();
				item.newItemReference = this.newItemReference;
			});
			return items;
		}

		this.saveItem = function (item) {
			return $.ajax({
				url: "/N2/Api/Content.ashx/autosave" + window.location.search,
				type: 'post',
				contentType: "application/json",
				data: JSON.stringify(item.changes)
			});
		}

		this.saveChanges = function (deferred) {
			var dirtbags = self.getChanges();
			if (!dirtbags.length)
			{
				self.dirty = false;
				return deferred;
			}
			var items = self.extractChangedItems(dirtbags);

			$(".publish.command").attr("disabled", "disabled");
			jQuery.each(items, function (id, item) {
				if (deferred)
					deferred = deferred.then(function () { return self.saveItem(item); });
				else
					deferred = self.saveItem(item);

				deferred = deferred.then(function (result) {
					$("#" + item.newItemReference).val(result.ID + "." + result.VersionIndex);
					$(".publish.command").attr("disabled", null);
					$(".discard.command").attr("href", "DiscardPreview.aspx?n2item=" + result.ID + "&n2versionIndex=" + result.VersionIndex).show();
					$(".cancel.command").hide();
					window.n2ctx && n2ctx.update && n2ctx.update({ id: result.ID, path: result.Path, permission: result.Permission, previewUrl: result.PreviewUrl, draft: result.Draft, node: result.Node, autosaved: true });
				}, function () {
					$(".publish.command").attr("disabled", null);
					console.warn("Error auto-saving", item, arguments);
				});
			})

			return deferred.then(function () {
				self.dirty = false;
			});
		}

		var self = this;
		this.intervalHandle = setInterval(function () {
			var deferred = null;
			if (self.dirty) {
				if (deferred)
					// defer until previous save operations copmletes
					deferred = deferred.then(function () { self.saveChanges(deferred) });
				else
					deferred = self.saveChanges();
			} else if (self.getChanges().length) {
				// ensure minimum interval by waiting until next setInterval call
				self.dirty = true;
			} else
				deferred = null;
		}, this.interval);
	},
	register: function (editorID, name, adapter) {
		function resolve(parent, key) {
			var dotIndex = key.indexOf(".");
			if (dotIndex < 0) {
				return parent[key];
			} else {
				return resolve(parent[key.substr(0, dotIndex)], key.substr(dotIndex + 1));
			}
		}
		var editor = $("#" + editorID).closest("[data-item]");
		if (editor.is("[data-disable-autosave]"))
			return;
		var itemID = parseInt(editor.attr("data-item"));
		var fn = resolve(window, adapter);
		if (fn) {
			fn.prototype = { editorID: editorID, newItemReference: editor.attr("data-item-reference"), itemID: itemID, name: name, dirty: function () { return undefined; }, checkout: function () { return undefined; } };
			var adapter = new fn();
			this.adapters.push(adapter);
		} else
			console.warn("Couldn't find client adapter: ", adapter);
	},
	adapters: [],
	input: function () {
		var self = this;
		var dirty = false;
		$("#" + this.editorID).on("change", function () {
			dirty = true;
		})
		this.dirty = function () {
			return dirty;
		}
		this.checkout = function () {
			dirty = false;
			return $("#" + this.editorID).val();
		}
	},
	select: function () {
		var self = this;
		var dirty = false;
		$("#" + this.editorID).on("change", function () {
			dirty = true;
		})
		this.dirty = function () {
			return dirty;
		}
		this.checkout = function () {
			dirty = false;
			return $("#" + this.editorID).val();
		}
	},
	checkbox: function () {
		var self = this;
		var dirty = false;
		$("#" + this.editorID).on("change", function () {
			dirty = true;
		})
		this.dirty = function () {
			return dirty;
		}
		this.checkout = function () {
			dirty = false;
			return $("#" + this.editorID).prop("checked");
		}
	},
	ckeditor: function () {
		this.dirty = function () {
			return CKEDITOR.instances[this.editorID].checkDirty();
		}
		this.checkout = function () {
			var editor = CKEDITOR.instances[this.editorID];
			editor.resetDirty();
			return editor.getData();
		}
	}
}
