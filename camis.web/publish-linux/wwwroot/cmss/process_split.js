
var split_state = 1;//0:initial, 1:waiting split 2: assignment 3: confirmation

var split_final_data = {};
var split_n;
var split_id;
function splitInit(taskid,n)
{
    split_id = taskid;
    split_n = n;
    split_state = 1;
    /*$.ajax({url: '/api/cmss/GetSplitTask?id=' + taskid+ "&sid=" + sessionid,
        dataType: 'json',
        success: function (data)
        {
            split_task = data.task;
            split_new_parcels = data.newParcels;
            split_state = 1;
            split_selected = [];
        }
    });*/

    layerChanged = function (type, id, count)
    {
        console.log('split ' + count+' ,state '+split_state);
        if(split_state == 2)
        {
            alert('Unexpected split operation');
            split_cancel();
        }
        if (split_state != 1)
            return;
        if (count == split_n)
        {
            split_showReviewPage();
        }
    };
  
}


function split_showReviewPage()
{
    
    $('#split_buttons').show();
    $('#split_button_cancel').show();
    $('#split_button_commit').show();
    $('#split_wait_for_split').show();
    $('#split_title_first').hide();
}
function split_cancel()
{
    split_state = 0;
    unloadTask();
    openHomePage();
}
function split_comit()
{

    bootbox.confirm("Are you sure you want to commit the change?", function (result)
    {
        if (result) {

            getGeomData(function (data) {
                var as = {
                    taskID: split_id,
                    geoms: []
                };
                var labels = [];
                for (var i = 0; i < data.length; i++) {
                    console.log(data[i].wkt);
                    as.geoms.push('SRID=20137;' + data[i].wkt);
                }
                split_final_data = as;
                split_state = 3;
                console.log(split_final_data);
                $.ajax({
                    url: '/api/cmss/splitParcel?taskID=' + split_id + "&sid=" + sessionid,
                    method: 'POST',
                    data: JSON.stringify(split_final_data),
                    contentType: 'application/json',
                    dataType: 'json',
                    success: function (res) {
                        if (res.error) {
                            alert('Error trying to save change.\n' + res.error);
                        } else {
                            alert('succesfully saved');
                            split_state = 0;
                            unloadTask();
                            openHomePage();
                        }
                    },
                    error: function (err) {
                        alert('failed to save geometry to the database');
                    }
                });

            });

            
        }
    });
}