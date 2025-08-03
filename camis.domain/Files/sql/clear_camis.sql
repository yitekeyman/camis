delete from frm.farm_land;
delete from frm.farm_registration;
delete from frm.farm;
delete from frm.farm_operator_registration;
delete from frm.farm_operator;

delete from pm.activity_progress;
delete from pm.activity_progress_status;
delete from pm.activity_progress_report_document;
delete from pm.activity_progress_report;
delete from pm.activity_plan_detail;
delete from pm.activity_plan_document;
delete from pm.activity_schedule;
delete from pm.activity_plan;
update pm.activity set parent_activity_id = NULL;
delete from pm.activity;

delete from lb.water_src_param;
delete from lb.surface_water;
delete from lb.ground_data;
delete from lb.irrigation;
delete from lb.land_upin;
delete from lb.land_right;
delete from lb.land_split;
delete from lb.soil_test;
delete from lb.land_climate;
delete from lb.land_accessibility;
delete from lb.land;

delete from wf.work_item;
delete from wf.workflow;

delete from doc.document;

delete from sys.audit_log;
delete from sys.user_action;
