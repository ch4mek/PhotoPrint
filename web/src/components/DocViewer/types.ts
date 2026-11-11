import { DocumentFileTypes } from '@/types/file';

export type DocViewerProps = {
	fileId: string;
	fileName: string;
	onClose: VoidFunction;
	fileType?: DocumentFileTypes;
	uri?: string;
};

export type DocViewerHeaderPosition = Partial<{
	top: number;
	left: number;
	right: number;
}>;
