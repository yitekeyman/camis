-- Table: frm.contract_update_reason

-- DROP TABLE frm.contract_update_reason;

CREATE TABLE frm.contract_update_reason
(
    id integer NOT NULL,
    name character varying(250) COLLATE pg_catalog."default" NOT NULL,
    description character varying(500) COLLATE pg_catalog."default",
    CONSTRAINT contract_update_reason_pkey PRIMARY KEY (id),
    CONSTRAINT reason_name_uq UNIQUE (name)
)
    WITH (
        OIDS = FALSE
        )
    TABLESPACE pg_default;

ALTER TABLE frm.contract_update_reason
    OWNER to postgres;

INSERT INTO frm.contract_update_reason(
    id, name, description)
VALUES (1, 'Expired Contract', ''),
       (2, 'Land Area Change', ''),
       (3, 'Land Suitability Problem', ''),
       (4, 'Land Use Change', ''),
       (5, 'Contract Period Extension by Loan', ''),
       (6, 'Operator Name Change', ''),
       (7, 'Merging of Operator', '');

CREATE SCHEMA history
    AUTHORIZATION postgres;

CREATE TABLE history.c_h_archive
(
    id uuid NOT NULL,
    type_id integer NOT NULL,
    update_reason_id integer NOT NULL,
    description character varying(500),
    requested_on bigint NOT NULL,
    requested_by character varying(50),
    approved_on bigint,
    approved_by character varying(50),
    wfid uuid NOT NULL,
    aid integer,
    data json,
    CONSTRAINT c_h_archive_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_archive_workflow_type_id_fk FOREIGN KEY (type_id)
        REFERENCES wf.workflow_type (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT c_h_archive_update_reason_id_fk FOREIGN KEY (update_reason_id)
        REFERENCES frm.contract_update_reason (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID,
    CONSTRAINT c_h_archive_requested_by_fk FOREIGN KEY (requested_by)
        REFERENCES sys."user" (username) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT c_h_archive_approved_by_fk FOREIGN KEY (approved_by)
        REFERENCES sys."user" (username) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT c_h_archive_wf_id_fk FOREIGN KEY (wfid)
        REFERENCES wf.workflow (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_archive
    OWNER to postgres;

CREATE TABLE history.c_h_operator
(
    id uuid NOT NULL,
    seq integer NOT NULL,
    archive_id uuid NOT NULL,
    operator_id uuid,
    operator_attr json NOT NULL,
    CONSTRAINT c_h_operator_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_operator_archive_id_fk FOREIGN KEY (archive_id)
        REFERENCES history.c_h_archive (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_operator
    OWNER to postgres;

CREATE TABLE history.c_h_farm
(
    id uuid NOT NULL,
    seq integer NOT NULL,
    archive_id uuid NOT NULL,
    farm_id uuid NOT NULL,
    farm_attr json,
    CONSTRAINT c_h_farm_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_farm_archive_id_fk FOREIGN KEY (archive_id)
        REFERENCES history.c_h_archive (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_farm
    OWNER to postgres;

CREATE TABLE history.c_h_land
(
    id uuid NOT NULL,
    seq integer NOT NULL,
    archive_id uuid NOT NULL,
    land_id uuid NOT NULL,
    upid character varying(25),
    land_attr json NOT NULL,
    CONSTRAINT c_h_land_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_land_archive_id_fk FOREIGN KEY (archive_id)
        REFERENCES history.c_h_archive (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_land
    OWNER to postgres;

CREATE TABLE history.c_h_farm_land
(
    id uuid NOT NULL,
    seq integer NOT NULL,
    archive_id uuid NOT NULL,
    farm_id uuid NOT NULL,
    land_id uuid NOT NULL,
    land_split_id integer,
    farm_land_attr json NOT NULL,
    CONSTRAINT c_h_farm_land_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_farm_land_archive_id_fk FOREIGN KEY (archive_id)
        REFERENCES history.c_h_archive (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_farm_land
    OWNER to postgres;

CREATE TABLE history.c_h_land_split
(
    id uuid NOT NULL,
    seq integer NOT NULL,
    archive_id uuid NOT NULL,
    land_id uuid NOT NULL,
    split_id integer NOT NULL,
    land_split_attr json NOT NULL,
    CONSTRAINT c_h_land_split_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_land_split_archive_id_fk FOREIGN KEY (archive_id)
        REFERENCES history.c_h_archive (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_land_split
    OWNER to postgres;

CREATE TABLE history.c_h_land_right
(
    id uuid NOT NULL,
    seq integer NOT NULL,
    archive_id uuid NOT NULL,
    farm_id uuid NOT NULL,
    land_id uuid NOT NULL,
    land_split_id integer,
    common_txt_uid uuid,
    land_right_attr json NOT NULL,
    CONSTRAINT c_h_land_right_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_land_right_archive_id_fk FOREIGN KEY (archive_id)
        REFERENCES history.c_h_archive (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_land_right
    OWNER to postgres;

CREATE TABLE history.c_h_activity
(
    id uuid NOT NULL,
    seq integer NOT NULL,
    archive_id uuid NOT NULL,
    activity_id uuid NOT NULL,
    activity_attr json NOT NULL,
    CONSTRAINT c_h_activity_id_pk PRIMARY KEY (id),
    CONSTRAINT c_h_activity_archive_id_fk FOREIGN KEY (archive_id)
        REFERENCES history.c_h_archive (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE history.c_h_activity
    OWNER to postgres;

CREATE TABLE frm.farm_warning
(
    id uuid NOT NULL,
    farm_id uuid NOT NULL,
    land_id uuid NOT NULL,
    split_index integer NOT NULL,
    date bigint NOT NULL,
    stage integer NOT NULL,
    reason character varying(256),
    reason_details text,
    aid bigint,
    wfid uuid,
    PRIMARY KEY (id),
    CONSTRAINT farm_warning_fm_id_fk FOREIGN KEY (farm_id)
        REFERENCES frm.farm (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID,
    CONSTRAINT farm_warning_land_id_fk FOREIGN KEY (land_id)
        REFERENCES lb.land (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE frm.farm_warning
    OWNER to postgres;

-- Table: frm.warning_doc

-- DROP TABLE frm.warning_doc;

CREATE TABLE frm.warning_doc
(
    id uuid NOT NULL,
    warning_id uuid NOT NULL,
    doc_id uuid NOT NULL,
    CONSTRAINT warning_doc_pkey PRIMARY KEY (id),
    CONSTRAINT warning_doc_doc_id_fk FOREIGN KEY (doc_id)
        REFERENCES doc.document (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT warning_doc_farm_warning_id_fk FOREIGN KEY (warning_id)
        REFERENCES frm.farm_warning (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
    WITH (
        OIDS = FALSE
        )
    TABLESPACE pg_default;

ALTER TABLE frm.warning_doc
    OWNER to postgres;

INSERT INTO wf.workflow_type(
    id, name, description)
VALUES (14, 'Contract Warning', 'Register Contract Warning Workflow');

INSERT INTO sys.action_type(
    id, name)
VALUES (1007, 'Warning Contract');

CREATE TABLE frm.cancelled_contract
(
    id uuid NOT NULL,
    farm_id uuid NOT NULL,
    land_id uuid NOT NULL,
    split_index integer NOT NULL DEFAULT 0,
    date bigint NOT NULL,
    reason character varying(255) NOT NULL,
    reason_details text,
    source_txt_uid uuid,
    aid bigint,
    wfid uuid NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT cancelled_contract_farm_id_farm_id_fk FOREIGN KEY (farm_id)
        REFERENCES frm.farm (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT cancelled_contract_land_id_land_id_fk FOREIGN KEY (land_id)
        REFERENCES lb.land (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)
    WITH (
        OIDS = FALSE
        );

ALTER TABLE frm.cancelled_contract
    OWNER to postgres;

CREATE TABLE frm.cancelled_contract_doc
(
    id uuid NOT NULL,
    cancellation_id uuid NOT NULL,
    doc_id uuid NOT NULL,
    CONSTRAINT cancelled_contract_doc_pkey PRIMARY KEY (id),
    CONSTRAINT contract_cancellation_doc_doc_id_fk FOREIGN KEY (doc_id)
        REFERENCES doc.document (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT contract_cancellation_doc_id_fk FOREIGN KEY (cancellation_id)
        REFERENCES frm.cancelled_contract (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
    WITH (
        OIDS = FALSE
        )
    TABLESPACE pg_default;

ALTER TABLE frm.cancelled_contract_doc
    OWNER to postgres;