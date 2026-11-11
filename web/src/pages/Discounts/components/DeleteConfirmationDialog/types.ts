import { DiscountSegmentTypes } from '@/types/discounts';

export interface DeleteConfirmationDialogProps {
	handleCloseDialog: VoidFunction;
	showLoader: boolean;
	segmentType: DiscountSegmentTypes;
	handleDeleteSegment: VoidFunction;
	title: string;
}
