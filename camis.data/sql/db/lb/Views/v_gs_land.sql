--DROP VIEW lb.v_gs_land;

CREATE OR REPLACE VIEW lb.v_gs_land AS
 SELECT l.id,
    l.land_type,
    u.geometry,
    u.upin,
    r.right_type,
	(u.profile->'rights'->0->'party'->>'partyType')::integer as holder_type
   FROM lb.land l
     JOIN lb.land_upin u ON l.id = u.land_id
     LEFT JOIN lb.land_right r ON l.id = r.land_id;

ALTER TABLE lb.v_gs_land
    OWNER TO docker;