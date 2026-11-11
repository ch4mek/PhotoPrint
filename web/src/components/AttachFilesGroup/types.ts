import { ChangeEvent } from 'react';

interface AttachFile {
	fileId: string | number;
	fileName: string;
	fileType?: string;
	file?: File;
}

export interface AttachFilesGroupProps {
	files: AttachFile[];
	onAttachedFileRemove: (fileId: string | number) => () => void;
	maxNumberOfFiles?: number;
	validExtensions?: string[];
	title?: string;
	fileTypes?: string[];
	onAttachedFileChange: (
		fileType?: string
	) => (e: ChangeEvent<HTMLInputElement>) => undefined | File | Array<File>;
	isAttachMultiple?: boolean;
	shouldShowFileType?: boolean;
}
