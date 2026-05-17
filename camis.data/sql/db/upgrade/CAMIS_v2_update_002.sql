--Address Removing Country
UPDATE
    sys.address
SET
    parent_id = null
WHERE
    parent_id =(
        SELECT
            id
        FROM
            sys.address
        WHERE
            unit_id = 1
    );

DELETE FROM
    sys.address_scheme_unit
WHERE
    unit_id = 1;

DELETE FROM
    sys.address
WHERE
    unit_id = 1;

UPDATE
    sys.address_scheme_unit
SET
    "order" = "order" -1,
    unit_id = unit_id -1;

UPDATE
    sys.address
SET
    unit_id = unit_id -1;

WITH next_names AS (
    SELECT
        id,
        name,
        LEAD(name) OVER (
            ORDER BY
                id
        ) as next_name
    FROM
        sys.address_unit
)
UPDATE
    sys.address_unit a
SET
    name = n.next_name
FROM
    next_names n
WHERE
    a.id = n.id
    AND NULLIF(n.next_name, '') IS NOT NULL;

WITH next_names AS (
    SELECT
        id,
        name,
        LEAD(name) OVER (
            ORDER BY
                id
        ) as next_name
    FROM
        sys.address_unit
)
DELETE FROM
    sys.address_unit a USING next_names n
WHERE
    a.id = n.id
    AND n.next_name IS NULL;

DO $$ 

BEGIN 
   

IF NOT EXISTS (
    SELECT
        1
    FROM
        information_schema.tables
    WHERE
        table_schema = 'sys'
        AND table_name = 'sys_config'
) THEN CREATE TABLE sys.sys_config (
    id integer NOT NULL,
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    value character varying(250) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT sys_config_pkey PRIMARY KEY (id),
    CONSTRAINT sys_config_name_unique UNIQUE (name)
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE
    sys.sys_config OWNER to postgres;

INSERT INTO
    sys.sys_config(id, name, value)
SELECT 
    1, 'NRLAIS_url', 'http://localhost:8540'
    WHERE NOT EXISTS (SELECT 1 FROM sys.sys_config WHERE id = 1);

INSERT INTO sys.sys_config(id, name, value)
SELECT 2, 'GoeServer_url', 'http://localhost:8080'
    WHERE NOT EXISTS (SELECT 1 FROM sys.sys_config WHERE id = 2);

INSERT INTO sys.sys_config(id, name, value)
SELECT 3, 'UTM_adindan', '37'
    WHERE NOT EXISTS (SELECT 1 FROM sys.sys_config WHERE id = 3);

INSERT INTO sys.sys_config(id, name, value)
SELECT 4, 'region_name', 'Amhara'
    WHERE NOT EXISTS (SELECT 1 FROM sys.sys_config WHERE id = 4);

INSERT INTO sys.sys_config(id, name, value)
SELECT 5, 'region_code', 'AM'
    WHERE NOT EXISTS (SELECT 1 FROM sys.sys_config WHERE id = 5);

INSERT INTO sys.sys_config(id, name, value)
SELECT 6, 'file_directory', '/usr/bin/CAMIS/data/docs'
    WHERE NOT EXISTS (SELECT 1 FROM sys.sys_config WHERE id = 6);


END IF;

END $$;

INSERT INTO sys.action_type(id, name)
SELECT 1004, 'Update System Configuration Value'
WHERE NOT EXISTS (SELECT 1 FROM sys.action_type WHERE id = 1004);

INSERT INTO sys.action_type(id, name)
SELECT 1005, 'Cancel Contract'
    WHERE NOT EXISTS (SELECT 1 FROM sys.action_type WHERE id = 1005);

INSERT INTO sys.action_type(id, name)
SELECT 1006, 'Update Contract'
    WHERE NOT EXISTS (SELECT 1 FROM sys.action_type WHERE id = 1006);

ALTER TABLE
    doc.document
ALTER COLUMN
    file DROP NOT NULL;

