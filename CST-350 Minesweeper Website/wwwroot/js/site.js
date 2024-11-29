$(function () {
    // Attach event listener for click events
    $(document).on("mousedown", ".cell-class", function (event) {
        // Extract cell location from the button's id
        const cellId = $(this).attr("id");
        const cellLocation = cellId.split("-").slice(1).join(",");

        // Find out which button was clicked
        switch (event.which)
        {
            case 1: // Left click
                updateCell(cellLocation, "/Game/RevealCell");
                break;

            case 3: // Right click
                updateCell(cellLocation, "/Game/FlagCell");
                break;

            default:
                console.log("Unhandled mouse button.");
        }
    });

    // Prevent the context menu from appearing on right-click
    $(document).bind("contextmenu", function (event)
    {
        event.preventDefault();
        console.log("Right-click menu prevented.");
    });

    
    function updateCell(cellLocation, url)
    {
        // Get the cell coordinates
        let row = cellLocation.split(',')[0];
        let col = cellLocation.split(',')[1];
        // Ajax to handle the click action
        $.ajax({
            url: url,
            method: "POST",
            data: { cellLocation: cellLocation },
            success: function (responseHtml) {
                // Depending on the ID of the element...
                if (responseHtml.includes('id="game-board"')) {
                    // Replace the board
                    $('#game-board').replaceWith(responseHtml);
                }
                else if (responseHtml.includes('id="cell-' + row + '-' + col + '"'))
                {
                    // Update the specific cell
                    $('#cell-' + row + '-' + col).replaceWith(responseHtml);
                }
                else
                {
                    // Redirent to a different page
                    window.location.href = responseHtml;
                }
            }
        });
    }
});
