ALTER TABLE lb.land_split
    ADD COLUMN land_id uuid NOT NULL;

ALTER TABLE lb.land_split
    ADD COLUMN indexes integer NOT NULL;

ALTER TABLE lb.land_split
    ADD COLUMN status bigint NOT NULL;

ALTER TABLE lb.land_split
    ADD COLUMN wid uuid NOT NULL;


ALTER TABLE lb.land_split
    ADD CONSTRAINT fk_land_id_id FOREIGN KEY (land_id)
        REFERENCES lb.land (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
    NOT VALID;