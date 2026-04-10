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
VALUES
    (1, 'NRLAIS_url', 'http://localhost:8888'),
    (2, 'GoeServer_url', 'http://localhost:8080'),
    (3, 'UTM_adindan', '37'),
    (4, 'region_name', 'SNNP'),
    (5, 'region_code', '07');

END IF;

END $$;

INSERT INTO sys.action_type(id, name)
SELECT 1004, 'Update System Configuration Value'
WHERE NOT EXISTS (SELECT 1 FROM sys.action_type WHERE id = 1004);

ALTER TABLE
    doc.document
ALTER COLUMN
    file DROP NOT NULL;

DO $$ 

BEGIN 
    CREATE SCHEMA IF NOT EXISTS env_mon AUTHORIZATION postgres;

    IF NOT EXISTS (SELECT 1 FROM information_schema.tables
                    WHERE table_schema = 'env_mon'
                    AND table_name = 'parcel_environmental_monitoring') THEN

            CREATE TABLE env_mon.parcel_environmental_monitoring (
                id serial,
                parcel_upid character varying(50) NOT NULL,
                monitoring_date date NOT NULL,
                ndvi numeric(5, 4),
                ndwi numeric(5, 4),
                ndbi numeric(5, 4),
                evi numeric(5, 4),
                mndwi numeric(5, 4),
                change_type character varying(50),
                change_magnitude numeric(6, 4),
                confidence numeric(4, 3) DEFAULT 0.7,
                severity character varying(50) DEFAULT 'MODERATE',
                soil_moisture numeric(5, 3),
                vegetation_health numeric(4, 3),
                water_presence numeric(4, 3),
                geometry geometry,
                area_sqkm numeric(10, 4),
                satellite_source character varying(50),
                cloud_cover numeric(4, 3),
                created_at timestamp without time zone DEFAULT NOW(),
                PRIMARY KEY (id)
            ) WITH (OIDS = FALSE);

            ALTER TABLE
                env_mon.parcel_environmental_monitoring OWNER to postgres;

            COMMENT ON COLUMN env_mon.parcel_environmental_monitoring.ndvi IS 'Vegetation index';

            COMMENT ON COLUMN env_mon.parcel_environmental_monitoring.ndwi IS 'Water index ';

            COMMENT ON COLUMN env_mon.parcel_environmental_monitoring.ndbi IS 'Built-up index';

            COMMENT ON COLUMN env_mon.parcel_environmental_monitoring.evi IS 'Enhanced vegetation index';

            COMMENT ON COLUMN env_mon.parcel_environmental_monitoring.mndwi IS 'Modified water index';

            COMMENT ON COLUMN env_mon.parcel_environmental_monitoring.change_type IS 'VEGETATION_LOSS, VEGETATION_GROWTH, WATER_CHANGE, URBANIZATION, FLOOD, DROUGHT';

    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.tables
                    WHERE table_schema = 'env_mon'
                    AND table_name = 'environmental_change_events') THEN

                    CREATE TABLE env_mon.environmental_change_events (
                        id serial,
                        parcel_upid character varying(50) NOT NULL,
                        event_date date NOT NULL,
                        event_type character varying(50) NOT NULL,
                        event_subtype character varying(50),
                        before_value numeric(8, 4),
                        after_value numeric(8, 4),
                        change_amount numeric(8, 4),
                        change_percentage numeric(6, 4),
                        affected_area_sqkm numeric(10, 4),
                        severity character(20),
                        confidence numeric(4, 3),
                        geometry geometry,
                        centroid geometry,
                        description text,
                        satellite_evidence boolean DEFAULT true,
                        verified boolean DEFAULT false,
                        detected_at timestamp without time zone DEFAULT NOW(),
                        PRIMARY KEY (id)
                    ) WITH (OIDS = FALSE);

                    ALTER TABLE
                        env_mon.environmental_change_events OWNER to postgres;

                    CREATE INDEX idx_parcel_monitoring_upid_date ON env_mon.parcel_environmental_monitoring(parcel_upid, monitoring_date);

                    CREATE INDEX idx_parcel_monitoring_type ON env_mon.parcel_environmental_monitoring(change_type, monitoring_date);

                    CREATE INDEX idx_change_events_upid_date ON env_mon.environmental_change_events(parcel_upid, event_date);

                    CREATE INDEX idx_change_events_type ON env_mon.environmental_change_events(event_type, event_date);

                    CREATE INDEX idx_parcel_monitoring_ndvi ON env_mon.parcel_environmental_monitoring(ndvi, monitoring_date);

                    CREATE INDEX idx_parcel_monitoring_ndwi ON env_mon.parcel_environmental_monitoring(ndwi, monitoring_date);

    END IF;    
END $$;



CREATE OR REPLACE FUNCTION env_mon.detect_environmental_changes(
    start_date DATE,
    end_date DATE,
    change_threshold NUMERIC DEFAULT 0.15
) RETURNS TABLE(
    parcel_upid VARCHAR(100),
    event_type VARCHAR(50),
    event_subtype VARCHAR(50),
    severity VARCHAR(20),
    confidence NUMERIC(4, 3),
    before_ndvi NUMERIC(5, 4),
    after_ndvi NUMERIC(5, 4),
    before_ndwi NUMERIC(5, 4),
    after_ndwi NUMERIC(5, 4),
    before_ndbi NUMERIC(5, 4),
    after_ndbi NUMERIC(5, 4),
    ndvi_change NUMERIC(6, 4),
    ndwi_change NUMERIC(6, 4),
    ndbi_change NUMERIC(6, 4),
    geometry TEXT,
    area_sqkm NUMERIC(10, 4)
) AS $$ BEGIN RETURN QUERY WITH parcel_data AS (
    SELECT
        pem.parcel_upid,
        pem.monitoring_date,
        pem.ndvi,
        pem.ndwi,
        pem.ndbi,
        pem.geometry,
        pem.area_sqkm
    FROM
        env_mon.parcel_environmental_monitoring pem
    WHERE
        pem.monitoring_date IN (start_date, end_date)
        AND pem.cloud_cover < 0.3
),
changes AS (
    SELECT
        pd_start.parcel_upid,
        pd_start.ndvi as before_ndvi,
        pd_end.ndvi as after_ndvi,
        pd_start.ndwi as before_ndwi,
        pd_end.ndwi as after_ndwi,
        pd_start.ndbi as before_ndbi,
        pd_end.ndbi as after_ndbi,
        (pd_end.ndvi - pd_start.ndvi) as ndvi_change,
        (pd_end.ndwi - pd_start.ndwi) as ndwi_change,
        (pd_end.ndbi - pd_start.ndbi) as ndbi_change,
        pd_start.geometry,
        pd_start.area_sqkm
    FROM
        parcel_data pd_start
        JOIN parcel_data pd_end ON pd_start.parcel_upid = pd_end.parcel_upid
    WHERE
        pd_start.monitoring_date = start_date
        AND pd_end.monitoring_date = end_date
),
classified_changes AS (
    SELECT
        c.*,
        CASE
            -- Vegetation changes
            WHEN c.ndvi_change < - change_threshold
            AND c.before_ndvi > 0.3 THEN 'VEGETATION_LOSS'
            WHEN c.ndvi_change > change_threshold
            AND c.after_ndvi > 0.3 THEN 'VEGETATION_GROWTH'
            WHEN c.before_ndvi > 0.5
            AND c.after_ndvi < 0.2 THEN 'DEFORESTATION' -- Water changes
            WHEN c.ndwi_change > change_threshold
            AND c.after_ndwi > 0.1 THEN 'WATER_INCREASE'
            WHEN c.ndwi_change < - change_threshold
            AND c.before_ndwi > 0.1 THEN 'WATER_DECREASE'
            WHEN c.after_ndwi > 0.3 THEN 'FLOOD'
            WHEN c.before_ndwi > 0.1
            AND c.after_ndwi < 0 THEN 'DROUGHT' -- Urbanization
            WHEN c.ndbi_change > change_threshold
            AND c.after_ndbi > 0 THEN 'URBANIZATION'
            ELSE 'NO_CHANGE'
        END as event_type,
        CASE
            WHEN ABS(c.ndvi_change) > 0.4
            OR ABS(c.ndwi_change) > 0.4 THEN 'SEVERE'
            WHEN ABS(c.ndvi_change) > 0.25
            OR ABS(c.ndwi_change) > 0.25 THEN 'HIGH'
            WHEN ABS(c.ndvi_change) > 0.15
            OR ABS(c.ndwi_change) > 0.15 THEN 'MODERATE'
            ELSE 'LOW'
        END as severity,
        GREATEST(0.6, 1 - ABS(c.ndvi_change)) as confidence
    FROM
        changes c
    WHERE
        ABS(c.ndvi_change) >= change_threshold
        OR ABS(c.ndwi_change) >= change_threshold
        OR ABS(c.ndbi_change) >= change_threshold
)
SELECT
    cc.parcel_upid,
    cc.event_type,
    CASE
        WHEN cc.event_type = 'VEGETATION_LOSS'
        AND cc.ndvi_change < -0.3 THEN 'SEVERE_DEGRADATION'
        WHEN cc.event_type = 'VEGETATION_GROWTH'
        AND cc.ndvi_change > 0.3 THEN 'REFORESTATION'
        WHEN cc.event_type = 'WATER_INCREASE'
        AND cc.ndwi_change > 0.3 THEN 'FLOODING'
        WHEN cc.event_type = 'WATER_DECREASE'
        AND cc.ndwi_change < -0.3 THEN 'DROUGHT'
        ELSE cc.event_type
    END as event_subtype,
    cc.severity,
    cc.confidence,
    cc.before_ndvi,
    cc.after_ndvi,
    cc.before_ndwi,
    cc.after_ndwi,
    cc.before_ndbi,
    cc.after_ndbi,
    cc.ndvi_change,
    cc.ndwi_change,
    cc.ndbi_change,
    ST_AsGeoJSON(cc.geometry) as geometry,
    cc.area_sqkm
FROM
    classified_changes cc
WHERE
    cc.event_type != 'NO_CHANGE';

END;

$$ LANGUAGE plpgsql;

-- Function to get parcel environmental history
CREATE OR REPLACE FUNCTION env_mon.get_parcel_environmental_history(
    parcel_id VARCHAR(100),
    months_back INTEGER DEFAULT 12
) RETURNS TABLE(
    monitoring_date DATE,
    ndvi NUMERIC(5, 4),
    ndwi NUMERIC(5, 4),
    ndbi NUMERIC(5, 4),
    vegetation_health NUMERIC(4, 3),
    water_presence NUMERIC(4, 3),
    change_type VARCHAR(50),
    severity VARCHAR(20)
) AS $$ BEGIN RETURN QUERY
SELECT
    pem.monitoring_date,
    pem.ndvi,
    pem.ndwi,
    pem.ndbi,
    pem.vegetation_health,
    pem.water_presence,
    pem.change_type,
    pem.severity
FROM
    env_mon.parcel_environmental_monitoring pem
WHERE
    pem.parcel_upid = parcel_id
    AND pem.monitoring_date >= CURRENT_DATE - (months_back || ' months') :: INTERVAL
ORDER BY
    pem.monitoring_date DESC;

END;

$$ LANGUAGE plpgsql;

