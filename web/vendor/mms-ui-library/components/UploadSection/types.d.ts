import { HTMLAttributes } from 'react';
export type UploadSectionStyledProps = {
    borderRadius?: number;
    displayBorder?: boolean;
    padding?: number;
    disabled?: boolean;
    isDragOver?: boolean;
    withDropzone?: boolean;
};
export type UploadSectionProps = Omit<HTMLAttributes<HTMLDivElement>, 'onChange'> & UploadSectionStyledProps & {
    description?: string;
    onClick?: () => void;
    dataTestId?: string;
    className?: string;
    browseLinkClassName?: string;
    acceptedFormats?: FileFormat[];
    maxSizeBytes?: number;
    multiple?: boolean;
    fileNamePattern?: RegExp;
    maxFilesPerDrop?: number;
    onFilesAccepted?: (files: File[]) => void;
    onFileRejected?: (rejections: FileRejection[]) => void;
    onChange?: (files: File[]) => void;
};
export type FileFormat = 'pdf' | 'txt' | 'doc' | 'docx' | 'pptx' | 'jpg' | 'jpeg' | 'png' | 'xlsx' | 'xls' | 'csv' | 'json' | 'md';
export declare enum FileRejectionReason {
    InvalidFormat = "invalid-format",
    ExceedsSizeLimit = "exceeds-size-limit",
    TooManyFiles = "too-many-files",
    InvalidFileName = "invalid-file-name"
}
export interface FileRejection {
    file: File;
    reason: FileRejectionReason.InvalidFormat | FileRejectionReason.ExceedsSizeLimit | FileRejectionReason.TooManyFiles | FileRejectionReason.InvalidFileName;
}
export type UseFileDropzoneProps = Pick<UploadSectionProps, 'acceptedFormats' | 'maxSizeBytes' | 'fileNamePattern' | 'onFilesAccepted' | 'onFileRejected' | 'onChange' | 'maxFilesPerDrop'>;
export type ProcessedFiles = {
    acceptedFiles: File[];
    rejectedFiles: FileRejection[];
};
