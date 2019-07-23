(function ($) {

    $.fn.foundationSelect = function () {

        // Check to see if custom dropdowns have already been drawn
        if (!$('.custom-dropdown-area').length) {

            // If custom dropdowns haven't been drawn, build and insert them
            return this.each(function () {
                var selectPrompt = '';
                var selectPosition = '';
                var selected = '';
                var translateClasses = '';
                var select = $(this);
                var selectId = select.attr('id');
                var multiple = false;
                var multiple = select.prop('multiple') ? true : false;
                var options = '';
                if (select.data('prompt')) {
                    selectPrompt = '<span class="default-label">' + select.data('prompt') + '</span>';
                    options = '<li class="disabled">' + selectPrompt + '</li>';
                } else {
                    selectPrompt = 'Choose...';
                }
                if (select.data('position')) {
                    selectPosition = select.data('position');
                }
                select.find('option').each(function () {
                    if ($(this).attr('selected')) {
                        selected = 'selected';
                        selectPrompt = "<div class='" + $(this).attr('class') + "'>" + $(this).html() + "</div>";
                    }
                    if ($(this).attr('class')) {
                        translateClasses = $(this).attr('class') + ' ';
                    }
                    options += '<li data-value="' + this.value + '" class="' + translateClasses + selected + '"><span class="option-title">' + $(this).html() + '</span></li>';
                    selected = '';
                });
                var newButton = '<div class="custom-dropdown-area" data-orig-select="#' + selectId + '"' + (multiple ? ' data-multiple="true"' : '') + '><button type="button" data-toggle="select-' + selectId + '" class="custom-dropdown-button">' + selectPrompt + '</button> \
        <ul id="select-' + selectId + '" class="dropdown-pane custom-dropdown-options' + selectPosition + '" data-dropdown> \
          ' + options + ' \
        </ul></div>';
                select.hide();
                select.after(newButton);
            });
        };
    };

    // setup a listener to deal with custom dropdown clicks.
    $(document).on('click', '.custom-dropdown-area li', function () {
        if ($(this).hasClass('disabled')) {
            return false;
        }
        var dropdown = $(this).closest('.custom-dropdown-area');
        var multiple = dropdown.data('multiple') ? true : false;
        var text = "<div class='" + $(this).attr('class') + "'>" + $(this).find('.option-title').html() + "</div>";
        var value = $(this).data('value');
        var totalOptions = dropdown.find('li').not('.disabled').length;
        var origDropdown = $(dropdown.data('orig-select'));
        var prompt = origDropdown.data('prompt') ? origDropdown.data('prompt') : 'Choose...';
        if (multiple) {
            $(this).toggleClass('selected');
            var selectedOptions = [];
            var selectedTitles = [];
            dropdown.find('.selected').each(function () {
                selectedOptions.push($(this).data('value'));
                selectedTitles.push($(this).find('.option-title').html());
            });
            origDropdown.val(selectedOptions).change();
            if (selectedOptions.length) {
                if (selectedOptions.length > 2) {
                    dropdown.find('.custom-dropdown-button').html(selectedOptions.length + ' of ' + totalOptions + ' selected');
                } else {
                    dropdown.find('.custom-dropdown-button').html(selectedTitles.join(', '));
                }
            } else {
                dropdown.find('.custom-dropdown-button').html(prompt);
            }
        } else {
            dropdown.find('li').removeClass('selected');
            $('#' + dropdown.find('ul').attr('id')).foundation('close');
            origDropdown.val(value).change();
            $(this).toggleClass('selected');
            dropdown.find('.custom-dropdown-button').html(text);
        }
    });

    $(document).on('reset', 'form', function () {
        if ($(this).children('.custom-dropdown-area').length) {
            $(this).find('.custom-dropdown-area').each(function () {
                var origDropdown = $($(this).data('orig-select'));
                var dropdown = $(this);
                var multiple = dropdown.data('multiple') ? true : false;
                var promt = '';
                dropdown.find('li').removeClass('selected');
                if (origDropdown.data('prompt')) {
                    prompt = origDropdown.data('prompt');
                } else {
                    origDropdown.find('option').each(function () {
                        if ($(this).attr('selected')) {
                            prompt = $(this).html();
                            dropdown.find('li[data-value="' + this.value + '"]').addClass('selected');
                        }
                    });
                    if (prompt == '') {
                        prompt = 'Choose...';
                    }
                }
                dropdown.find('.custom-dropdown-button').html(prompt);
            });
        }
    });

}(jQuery));