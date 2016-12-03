(window.n2 || (window.n2 = {})).FileUpload = function (maxFileSize, ticket, selected, refreshFrames, container) {
	"use strict";
	
	if (typeof FileReader == "undefined") {
		$("#fileupload em").hide();
	}

	var uploading = 0;
	// Initialize the jQuery File Upload widget:
	$('#fileupload').fileupload({
		url: "UploadFile.ashx",
		maxFileSize: maxFileSize,
		previewMaxWidth: 48,
		previewMaxHeight: 48,
		autoUpload: true,
		sequentialUploads: true,
		formData: { ticket: ticket, selected: selected },
		add: function (e, data) {
			$("#uploadcontrols").hide();
			$("#fileupload").addClass("uploading");

			uploading++;
			data.filename = data.files && data.files[0] && data.files[0].name;
			data.context = $('<p/>').html("<b class='fa fa-upload'></b> " + data.filename).appendTo(container);
			data.submit();
		},
		always: function (e, data) {
			uploading--;

			data.context.slideUp("fast", function () {
				data.context.remove();
			})

			if (!uploading) {
				$("#fileupload").removeClass("uploading");
				$("#uploadcontrols").slideDown("fast");
				refreshFrames();
			}
		},
		fail: function (e, data) {
			$("#fileupload").removeClass("uploading");
			$("#uploadcontrols").slideDown("fast");
			$("#uploadcontrols").prepend("<div class='alert alert-error'><button type='button' class='close' data-dismiss='alert'>×</button>Failed uploading " + data.filename + "</div>");
		}
	});
};