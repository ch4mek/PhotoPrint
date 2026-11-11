import { SegmentTypes } from '@/types/wiki';

export interface DeleteConfirmationDialogProps {
	handleCloseDialog: VoidFunction;
	showLoader: boolean;
	segmentType: SegmentTypes;
	handleDeleteSegment: VoidFunction;
	title: string;
}
