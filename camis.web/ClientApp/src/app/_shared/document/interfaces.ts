export interface IDocument {
    id?: string;
    date: number;
    ref?: string;
    note?: string;
    mimetype?: string;
    filename?: string;
    file?: string;

    overrideFilePath?: string;
}
