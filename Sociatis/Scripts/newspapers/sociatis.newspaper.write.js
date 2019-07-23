$(document).ready(function () {
    createNormalText();
    createPayText();

    $("#EnablePayOnly").click(function () {
        var $this = $(this);
        if($this.is(":checked"))
        {
            $("#payOnly").show();
            $("#contentPriceRow").show();
        }
        else
        {
            $("#payOnly").hide();
            $("#contentPriceRow").hide();
        }
    })
});

function createNormalText()
{
    $('#text').markdownEditor({

        // width / height of the editor
        width: '100%',
        height: '400px',

        // font size
        fontSize: '14px',

        // Ace.js themes
        // refer to https://github.com/ajaxorg/ace/tree/master/lib/ace/theme
        theme: 'tomorrow',

        // Soft tabs means you're using spaces instead of the tab character ('\t')
        softTabs: true,

        // active fullscreen mode
        fullscreen: true,

        // enable image upload
        imageUpload: false,

        // The path of the server side script that receives the images. 
        // The script has to return an array of the public path of the successfully uploaded images in json format.
        uploadPath: '',

        // active preview mode
        preview: true,

        // called when the user clicks on the preview button
        onPreview: function (content, callback) {
            callback(marked(content));
        },

        // custom label text
        label: {
            btnHeader1: 'Header 1',
            btnHeader2: 'Header 2',
            btnHeader3: 'Header 3',
            btnBold: 'Bold',
            btnItalic: 'Italic',
            btnList: 'Unordered list',
            btnOrderedList: 'Ordered list',
            btnLink: 'Link',
            btnImage: 'Insert image',
            btnUpload: 'Upload image',
            btnEdit: 'Edit',
            btnPreview: 'Preview',
            btnFullscreen: 'Fullscreen',
            loading: 'Loading'
        }

    });

    $('#text').hide();

}

function createPayText() {
    $('#payText').markdownEditor({

        // width / height of the editor
        width: '100%',
        height: '400px',

        // font size
        fontSize: '14px',

        // Ace.js themes
        // refer to https://github.com/ajaxorg/ace/tree/master/lib/ace/theme
        theme: 'tomorrow',

        // Soft tabs means you're using spaces instead of the tab character ('\t')
        softTabs: true,

        // active fullscreen mode
        fullscreen: true,

        // enable image upload
        imageUpload: false,

        // The path of the server side script that receives the images. 
        // The script has to return an array of the public path of the successfully uploaded images in json format.
        uploadPath: '',

        // active preview mode
        preview: true,

        // called when the user clicks on the preview button
        onPreview: function (content, callback) {
            callback(marked(content));
        },

        // custom label text
        label: {
            btnHeader1: 'Header 1',
            btnHeader2: 'Header 2',
            btnHeader3: 'Header 3',
            btnBold: 'Bold',
            btnItalic: 'Italic',
            btnList: 'Unordered list',
            btnOrderedList: 'Ordered list',
            btnLink: 'Link',
            btnImage: 'Insert image',
            btnUpload: 'Upload image',
            btnEdit: 'Edit',
            btnPreview: 'Preview',
            btnFullscreen: 'Fullscreen',
            loading: 'Loading'
        }

    });

    $('#payText').hide();
}

$(".articleWriteComment textarea").click(function () {
    $(this).attr("placeholder", "");
    $(".articleWriteComment button").addClass("flash");
});

$('.articleWriteComment form').preventDoubleSubmission();