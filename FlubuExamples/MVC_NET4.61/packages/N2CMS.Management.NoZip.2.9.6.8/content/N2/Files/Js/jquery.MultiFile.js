/*
 ### jQuery Multiple File Upload Plugin ###
 By Diego A., http://www.fyneworks.com, diego@fyneworks.com
 
 Website:
  http://www.fyneworks.com/jquery/multiple-file-upload/
 Project Page:
  http://jquery.com/plugins/project/MultiFile/
 Forums:
  http://www.nabble.com/jQuery-Multiple-File-Upload-f20931.html
 Blog:
	 http://fyneworks.blogspot.com/2007/04/jquery-multiple-file-upload-plugin-v11.html
  (old) http://fyneworks.blogspot.com/2007/04/multiple-file-upload-plugin-for-jquery.html
 
 12-April-2007: v1.1
                Added events and file extension validation
                See website for details.
 
 06-June-2007: v1.2
                Now works in Opera.
 
 12-June-2007: v1.21
                Preserves name of file input so all current server-side
																functions don't need to be changed on new installations.
 
 24-June-2007: v1.22
                Now works perfectly in Opera, thanks to Adrian Wróbel <adrian [dot] wrobel [at] gmail.com>
*/

/*# AVOID COLLISIONS #*/
if(jQuery) (function($){
/*# AVOID COLLISIONS #*/

	// Fix for Opera: 6-June-2007
	// Stop confusion between null, 'null' and 'undefined'
	function IsNull(i){
		return (i==null || i=='null' || i=='' || i=='undefined');
	};
	
	// extend jQuery - $.MultiFile hook
	$.extend($, {
		MultiFile: function( o /* Object */ ){
			return $("INPUT[type='file'].multi").MultiFile(o);
		}
	});
	
	// extend jQuery function library
	$.extend($.fn, {
			
			// MultiFile function
			MultiFile: function( o /* Object */ ){
				if(this._MultiFile){ return $(this); }
				this._MultiFile = true;
				
				// DEBUGGING: disable plugin
				//return false;
				
				// Bind to each element in current jQuery object
				return $(this).each(function(i){
							// Remember our ancestors...
							var d = this;
							var x = $(d);
       
       // Copy parent attributes - Thanks to Jonas Wagner
							// we will use this one to create new input elements
       var xclone = x.clone();
							
							//#########################################
							// Find basic configuration in class string
							// debug???
							d.debug = (d.className.indexOf('debug')>0);
							// limit number of files that can be selected?
							if(IsNull(d.max)){
								d.max = x.attr('maxlength');
								if(IsNull(d.max)){
									d.max = ((d.className.match(/\b((max|limit)\-[0-9]+)\b/gi) || [''])[0]);
									if(IsNull(d.max)){
										d.max = -1;
									}else{
										d.max = d.max.match(/[0-9]+/gi)[0];
									}
								}
							}
							d.max = new Number(d.max);
							// limit extensions?
							if(!d.accept){
								d.accept = (d.className.match(/\b(accept\-[\w\|]+)\b/gi)) || '';
								d.accept = new String(d.accept).replace(/^(accept|ext)\-/i,'');
							}
							//#########################################
							
							
							// Attach a bunch of events, jQuery style ;-)
							$.each("on,after".split(","), function(i,o){
								$.each("FileSelect,FileRemove,FileAppend".split(","), function(j,event){
									d[o+event] = function(e, v, m){ // default functions do absolutelly nothing...
										// if(d.debug) alert(''+o+event+'' +'\nElement:' +e.name+ '\nValue: ' +v+ '\nMaster: ' +m.name+ '');
									};
								});
							});
							// Setup a global event handler
							d.trigger = function(event, e){
									var f = d[event];
									if(f){
										var v = $(this).attr('value');
										var r = f(e, v, d);
										if(r!=null) return r;
									}
									return true;
							};
							
							
							// Initialize options
							if( typeof o == 'number' ){ o = {max:o}; };
							$.extend(d, d.data || {}, o);
							
							// Default properties - INTERNAL USE ONLY
							$.extend(d, {
								STRING: d.STRING || {}, // used to hold string constants
								n: 0, // How many elements are currently selected?
								k: 'multi', // Instance Key?
								f: function(z){ return d.k+'_'+String(i)+'_'+String(z); }
							});
							
							// Visible text strings...
							// $file = file name (with path), $ext = file extension
							d.STRING = $.extend({
							    remove: '<img src="../Resources/img/ico/png/bullet_delete.png"/>',
								denied:'You cannot select a $ext file.\nTry again...',
								selected:'File selected: $file'
							}, d.STRING);
							
							
							// Setup dynamic regular expression for extension validation
							// - thanks to John-Paul Bader: http://smyck.de/2006/08/11/javascript-dynamic-regular-expresions/
							if(String(d.accept).length>1){
								d.rxAccept = new RegExp('\\.('+(d.accept?d.accept:'')+')$','gi');
							};
							
							// Create wrapper to hold our file list
							d.w = d.k+'multi'+'_'+i; // Wrapper ID?
							x.wrap('<div id="'+d.w+'"></div>');
							
							// Bind a new element
							d.add = function( e, ii ){
								
								// Keep track of how many elements have been displayed
								d.n++;
								
								// Add reference to master element
								e.d = d;
								
								// Define element's ID and name (upload components need this!)
								e.i = ii;//d.I;
								e.id = d.f(e.i);
								e.name = (e.name || x.attr('name') || 'file') + (e.i>0?e.i:''); // same name as master element
								
								// If we've reached maximum number, disable input e
								if( (d.max != -1) && ((d.n-1) > (d.max)) ){ // d.n Starts at 1, so subtract 1 to find true count
									e.disabled = true;
								};
								
								// Remember most recent e
								d.current = e;
								
								/// now let's use jQuery
								e = $(e);
								
								// Triggered when a file is selected
								e.change(function(){
										
										//# Trigger Event! onFileSelect
										if(!d.trigger('onFileSelect', this, d)) return false;
										//# End Event!
										
										// check extension
										if(d.accept){
											var v = String(e.attr('value'));
											if(!v.match(d.rxAccept)){
												// Clear element value
												e.val('').attr('value', '');
												e.get(0).value = '';
												
												// OPERA BUG FIX - 2007-06-24
												// Thanks to Adrian Wróbel <adrian [dot] wrobel [at] gmail.com>
 											// we add new input element and remove present one for browsers that can't clear value of input element
												//var f = $('<input name="'+(x.attr('name') || '')+'" type="file"/>');
												var f = xclone.clone();// Copy parent attributes - Thanks to Jonas Wagner
												
												d.n--;
												d.add(f.get(0), this.i);
												e.parent().prepend(f);
												e.remove();
												
												// Show error message
												// TO-DO: Some people have suggested alternative methods for displaying this message
												// such as inline HTML, lightbox, etc... maybe integrate with blockUI plugin?
												alert(d.STRING.denied.replace('$ext', String(v.match(/\.\w{1,4}$/gi))));
												
												return false;
											}
										};
										
										// Hide this element: display:none is evil!
										//this.style.display = 'block';
										this.style.position = 'absolute';
										this.style.left = '-1000px';
          
										// Create a new file input element
										//var f = $('<input name="'+(x.attr('name') || '')+'" type="file"/>');
          var f = xclone.clone();// Copy parent attributes - Thanks to Jonas Wagner
										
										// Add it to the form
										$(this).parent().prepend(f);
										
										// Update list
										d.list( this );
										
										// Bind functionality
										d.add( f.get(0), this.i+1 );
										
										//# Trigger Event! afterFileSelect
										if(!d.trigger('afterFileSelect', this, d)) return false;
										//# End Event!
										
								});
							
							};
							// Bind a new element
						
							// Add a new file to the list
							d.list = function( y ){
								
								//# Trigger Event! onFileAppend
								if(!d.trigger('onFileAppend', y, d)) return false;
								//# End Event!
								
								// Insert HTML
								var
									t = $('#'+d.w),
									r = $('<div></div>'),
									v = $(y).attr('value')+'',
									a = $('<span class="file" title="'+d.STRING.selected.replace('$file', v)+'">'+v.match(/[^\/\\]+$/gi)[0]+'</span>'),
									b = $('<a href="#'+d.w+'">'+d.STRING.remove+'</a>');
								t.append(r);
								r.append(b,'&nbsp;',a);
								b.click(function(){
									
										//# Trigger Event! onFileRemove
										if(!d.trigger('onFileRemove', y, d)) return false;
										//# End Event!
										
										d.n--;
										d.current.disabled = false;
										$('#'+d.f(y.i)).remove();
										$(this).parent().remove();
										
										//# Trigger Event! afterFileRemove
										if(!d.trigger('afterFileRemove', y, d)) return false;
										//# End Event!
										
										return false;
								});
								
								//# Trigger Event! afterFileAppend
								if(!d.trigger('afterFileAppend', y, d)) return false;
								//# End Event!
								
							};
							
							// Bind first file element
							if(!d.ft){ d.add(d, 0); d.ft = true; }
							d.I++;
							d.n++;
							
				});
				// each element
			
			}
			// MultiFile function
	
	});
	// extend jQuery function library



/*
 ### Default implementation ###
 The plugin will attach itself to file inputs
 with the class 'multi' when the page loads
	
	Use the jQuery start plugin to 
*/
if($.start){ $.start($.MultiFile) }
else $(function(){ $.MultiFile() });



/*# AVOID COLLISIONS #*/
})(jQuery);
/*# AVOID COLLISIONS #*/
