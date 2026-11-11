import { DocumentFileTypes } from '@/types/file';

export type DocViewerHeaderProps = {
	fileName: string;
	onExport: VoidFunction;
	onClose: VoidFunction;
	fileType?: DocumentFileTypes;
};
