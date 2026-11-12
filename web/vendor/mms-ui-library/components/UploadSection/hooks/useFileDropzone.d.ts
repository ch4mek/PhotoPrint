import { UseFileDropzoneProps } from '../types';
export declare const useFileDropzone: ({ acceptedFormats, maxSizeBytes, maxFilesPerDrop, fileNamePattern, onFilesAccepted, onFileRejected, onChange, }: UseFileDropzoneProps) => (files: File[]) => void;
