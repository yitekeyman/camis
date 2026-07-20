
let split_state = 1;//0:initial, 1:waiting split 2: assignment 3: confirmation

let split_final_data = {};
let split_n;
let split_upid;
let split_id;
function splitInit(taskid,n, upid)
{
    split_id = taskid;
    split_n = n;
    split_upid = upid;
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
        if (split_state != 1){
            return;
        }
        if (count == split_n)
        {
            split_showReviewPage();
        }
        if(count>0 && count<split_n){
            renderSplitParcelsList();
        }
    };
  
}
function renderSplitParcelsList() {
    getGeomData(function(data) {
        var listContainer = document.getElementById('split_parcels_items');
        if (!listContainer) return;

        if (!data || data.length === 0) {
            listContainer.innerHTML = '<li><em>No split parcels found</em></li>';
            return;
        }

        var html = '';
        data.forEach(function(parcel) {
            // Format area to 2 decimal places (adjust as needed)
            if(data.length > 0 && data.length===split_n){
                split_showReviewPage()
            }
            let areaFormatted = parseFloat(parcel.area).toFixed(2);
            html += '<li><strong>Id:</strong> ' + split_upid+-+parcel.id +
                ' &nbsp;|&nbsp; <strong>Area:</strong> ' + parcel.area + '</li>';
        });
        listContainer.innerHTML = html;
    });
}

function split_showReviewPage()
{
    
    $('#split_buttons').show();
    $('#split_button_cancel').show();
    $('#split_button_commit').show();
    $('#split_wait_for_split').show();
    $('#split_title_first').hide();
    
    renderSplitParcelsList();
}
function split_cancel()
{
    split_state = 0;
    unloadTask();
    openHomePage();
}
function split_comit()
{
    if (!confirm('Are you sure you want to commit the change for approval?')) {
        return;
    }
    let reason = prompt('Please enter your note for Land Admin expert:(optional):');
    getGeomData(function (data) {
                let as = {
                    taskID: split_id,
                    geoms: [],
                    geomData:[]
                };
                //var labels = [];
                for (var i = 0; i < data.length; i++) {
                    //console.log(data[i].wkt);
                    as.geoms.push('SRID=20137;' + data[i].wkt);
                    let g={
                        id: data[i].id,
                        area: data[i].area,
                        geom:'SRID=20137;' + data[i].wkt
                    }
                    as.geomData.push(g);
                }
                split_final_data = as;
                split_state = 3;
               // console.log(split_final_data);
                $.ajax({
                    url: '/api/cmss/splitParcel?note=' + reason + "&sid=" + sessionid,
                    type: 'POST',
                    data: JSON.stringify(split_final_data),
                    contentType: 'application/json',
                    dataType: 'json',
                    success: function (res) {
                        if (res.error) {
                            alert('Error trying to save change.\n' + res.error);
                        } else {
                            alert('You have successfully commit split task');
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
function rejectTask() {

    if (!confirm('Are you sure you want to reject this task?')) {
        return;
    }

    let reason = prompt('Please enter your reason for rejection:');
    if (reason === null) {
        return;
    }
    if (reason.trim() === '') {
        alert('Reason cannot be empty. Rejection cancelled.');
        return;
    }

    //alert('Task ' + taskId + ' rejected with reason: ' + reason);
    let rejectData = {
        taskId: split_id,      // ← matches C# property name
        reason: reason,
        sid: sessionid
    };
    $.ajax({
        url: '/api/Cmss/CmssRejectTask',
        type: 'POST',
        data:JSON.stringify(rejectData),
        contentType: 'application/json',
        dataType: 'json',
        success: function(res) {
            if (res.error) {
                alert('Error trying to save change.\n' + res.error);
            } else {
                alert('Task rejected successfully');
                split_state = 0;
                unloadTask();
                openHomePage();
            }
        },
        error: function(err) {
            alert('Failed to reject task: ' + err.statusText + ' (' + err.status + ')');
        }
    });
}