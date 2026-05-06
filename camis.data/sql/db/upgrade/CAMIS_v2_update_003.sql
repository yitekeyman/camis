--modify parcel split without NRLAIS action
ALTER TABLE lb.land_split
    ADD COLUMN land_id uuid NOT NULL;

ALTER TABLE lb.land_split
    ADD COLUMN indexes integer NOT NULL;

ALTER TABLE lb.land_split
    ADD COLUMN status bigint NOT NULL;

ALTER TABLE lb.land_split
    ADD COLUMN wid uuid NOT NULL;
ALTER TABLE lb.land_split
    ADD COLUMN area double precision NOT NULL;

ALTER TABLE lb.land_split
    ADD CONSTRAINT fk_land_id_id FOREIGN KEY (land_id)
        REFERENCES lb.land (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
    NOT VALID;

--Add CMSS user at user role
INSERT INTO sys.role(id, name)
VALUES
    (12, 'CMSS User');
-- to insert land_status type at table
INSERT INTO lb.land_type(id, name)
VALUES
    (1, 'Identified'),
    (2, 'Prepared'),
    (3, 'Transferred'),
    (4, 'OnSplit'),
    (5, 'PreparedWithSplit'),
    (6, 'HalfTransferred');

-- Add split_index column as NOT NULL with a default value (0 for existing rows)
ALTER TABLE lb.certificate
    ADD COLUMN split_index integer NOT NULL DEFAULT 0;

-- Drop the existing primary key constraint (named certificate_pkey)
ALTER TABLE lb.certificate
DROP CONSTRAINT certificate_pkey;

-- Create a new composite primary key on land_id and split_index
ALTER TABLE lb.certificate
    ADD CONSTRAINT certificate_pkey PRIMARY KEY (land_id, split_index);

--Create a farm_status_type table

CREATE TABLE frm.farm_status_type
(
    id integer NOT NULL,
    name character varying(50) COLLATE pg_catalog."default",
    CONSTRAINT farm_status_type_pkey PRIMARY KEY (id)
)
    WITH (
        OIDS = FALSE
        )
    TABLESPACE pg_default;

ALTER TABLE frm.farm_status_type
    OWNER to postgres;

--insert status type records 
INSERT INTO frm.farm_status_type(id, name)
VALUES (1, 'On Editing'),
       (2, 'Ready'),
       (3, 'Transaction Locked'),
       (4, 'Active'),
       (5, 'Expired'),
       (6, 'Suspend'),
       (7, 'Deleted'),
       (8, 'Under Warning');

--add farm_status at farm table
ALTER TABLE frm.farm
    ADD COLUMN status integer;
ALTER TABLE frm.farm
    ADD CONSTRAINT fk_farms_farm_status FOREIGN KEY (status)
        REFERENCES frm.farm_status_type (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
    NOT VALID;

--modifiying farm_land by adding split_index

ALTER TABLE frm.farm_land
    ADD COLUMN split_index integer NOT NULL DEFAULT 0;

ALTER TABLE frm.farm_land
DROP CONSTRAINT farm_land_pk;

ALTER TABLE frm.farm_land
    ADD CONSTRAINT farm_land_pk PRIMARY KEY (land_id, farm_id, split_index);

--modifying land_right table by adding split index

ALTER TABLE lb.land_right
    ADD COLUMN split_index integer NOT NULL DEFAULT 0;

ALTER TABLE lb.land_right
    ADD COLUMN geom geometry;

ALTER TABLE  lb.land_right
    ADD COLUMN farm_id uuid;

ALTER TABLE lb.land_right
    ADD COLUMN status integer DEFAULT 4;

ALTER TABLE lb.land_right
    ADD COLUMN common_txt_uid uuid;

UPDATE lb.land_right lr
SET farm_id=(SELECT fl.farm_id FROM frm.farm_land fl WHERE fl.land_id=lr.land_id ),
    geom=(SELECT lu.geometry FROM lb.land_upin lu WHERE lu.land_id=lr.land_id);

ALTER TABLE lb.land_right
    ALTER COLUMN farm_id SET NOT NULL;

ALTER TABLE lb.land_right
    DROP CONSTRAINT land_right_pk;

ALTER TABLE lb.land_right
    ADD CONSTRAINT land_right_pk PRIMARY KEY (land_id, split_index, farm_id);
ALTER TABLE lb.land_right
    ADD CONSTRAINT land_right_farm_id_fk FOREIGN KEY (farm_id)
        REFERENCES frm.farm (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE NO ACTION
    NOT VALID;

ALTER TABLE lb.land
    ADD COLUMN locked boolean NOT NULL DEFAULT false;

ALTER TABLE frm.farm
    ADD COLUMN locked boolean NOT NULL DEFAULT false;

ALTER TABLE lb.land_split
    ADD COLUMN locked boolean NOT NULL DEFAULT false;
UPDATE frm.farm f
SET status = CASE
                 WHEN EXISTS (
                     SELECT 1
                     FROM frm.farm_land fl
                              JOIN lb.land_right lr ON lr.land_id = fl.land_id AND lr.split_index = fl.split_index
                     WHERE fl.farm_id = f.id
                       AND lr.right_to < EXTRACT(epoch FROM NOW())::bigint   -- assumes right_to is Unix timestamp (seconds)
                 ) THEN 5
                 WHEN EXISTS (
                     SELECT 1
                     FROM frm.farm_land fl
                     WHERE fl.farm_id = f.id
                 ) THEN 4
                 ELSE 2
    END;


ALTER TABLE lb.land_right DROP CONSTRAINT land_right_land_id_fk;

ALTER TABLE lb.land_right DROP CONSTRAINT land_right_farm_id_fk;



ALTER TABLE lb.land_right
    ADD CONSTRAINT land_right_land_land_id_fk FOREIGN KEY (land_id)
        REFERENCES lb.land (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
    NOT VALID;

ALTER TABLE frm.farm_land
    ADD CONSTRAINT farm_land_land_id_fk FOREIGN KEY (land_id)
        REFERENCES lb.land (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
    NOT VALID;

--20260506 end upgrade