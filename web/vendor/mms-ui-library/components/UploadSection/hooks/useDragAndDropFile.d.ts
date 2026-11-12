import { ChangeEvent, DragEvent } from 'react';
export declare const useDragAndDropFile: (isBlocked: boolean, processFile: (files: File[]) => void) => {
    fileInputRef: import("react").RefObject<HTMLInputElement | null>;
    isDragOver: boolean;
    handleDrop: (e: DragEvent<HTMLDivElement>) => void;
    handleDragOver: (e: DragEvent<HTMLDivElement>) => void;
    handleDragEnter: () => void;
    handleDragLeave: () => void;
    handleFileDropzoneClick: () => void;
    handleFileInputChange: (e: ChangeEvent<HTMLInputElement>) => void;
};
