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
INSERT INTO lb.land_type(id, name)
VALUES
    (1, 'Identified'),
    (2, 'Prepared'),
    (3, 'Transferred'),
    (4, 'OnSplit'),
    (5, 'PreparedWithSplit'),
    (6, 'HalfTransferred');