-- Create sequence only if it doesn't exist
CREATE SEQUENCE IF NOT EXISTS lb.ground_data_id_seq INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 OWNED BY NONE;

-- Ensure the owner is set to postgres
ALTER SEQUENCE IF EXISTS lb.ground_data_id_seq OWNER TO postgres;

-- Check if the column exists before adding it
ALTER TABLE lb.ground_data
    ALTER COLUMN id SET DEFAULT nextval('lb.ground_data_id_seq'::regclass);

DO $$ BEGIN IF NOT EXISTS (
    SELECT
        1
    FROM
        information_schema.columns
    WHERE
        table_schema = 'frm'
        AND table_name = 'farm_operator'
        AND column_name = 'photo_id'
) THEN
ALTER TABLE
    frm.farm_operator
ADD
    COLUMN photo_id uuid;

END IF;

IF NOT EXISTS (
    SELECT
        1
    FROM
        information_schema.columns
    WHERE
        table_schema = 'sys'
        AND table_name = 'user'
        AND column_name = 'email'
) THEN
ALTER TABLE
    sys."user"
ADD
    COLUMN email character varying(250);

END IF;

END $$;

UPDATE
    wf.work_item
SET
    assigned_role = 6
WHERE
    assigned_role = 7;

DELETE FROM
    sys.user_role
WHERE
    role_id = 7
    or role_id = 11;

DELETE FROM
    sys.role
WHERE
    id = 7
    or id = 11;



UPDATE
    sys.role
SET
    name = 'System Admin'
WHERE
    id = 1;

UPDATE
    sys.role
SET
    name = 'Farm Data Registrar'
WHERE
    id = 2;

UPDATE
    sys.role
SET
    name = 'Farm Data Supervisor'
WHERE
    id = 3;

UPDATE
    sys.role
SET
    name = 'Land Bank Registrar'
WHERE
    id = 4;

UPDATE
    sys.role
SET
    name = 'Land Bank Supervisor'
WHERE
    id = 5;

UPDATE
    sys.role
SET
    name = 'Land Bank Admin'
WHERE
    id = 6;