import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import {
	confirmationMessage,
	confirmationPostfix,
} from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { getDeletingPopUpTitle } from '@/constants/activityStatus';
import { zIndexes } from '@/constants/zIndexes';
import { DeleteConfirmationDialogProps } from '@/pages/Discounts/components/DeleteConfirmationDialog/types';
import { DiscountSegmentTypes } from '@/types/discounts';

const { Category, Discount } = DiscountSegmentTypes;

export function DeleteConfirmationDialog({
	handleCloseDialog,
	showLoader,
	segmentType,
	handleDeleteSegment,
	title,
}: DeleteConfirmationDialogProps) {
	const popupTitle =
		segmentType === Category
			? `${Discount} ${Category.toLowerCase()}`
			: Discount;

	return (
		<PopUp
			title={getDeletingPopUpTitle(popupTitle)}
			onClose={handleCloseDialog}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: { onClick: handleDeleteSegment },
				negativeControl: { onClick: handleCloseDialog },
			}}
			zIndex={zIndexes.formPopup}
			showLoader={showLoader}
		>
			<Typography variant="m-400">
				<Typography variant="m-600">{title}</Typography>{' '}
				{segmentType.toLowerCase()} {confirmationPostfix.deletePostfix}
			</Typography>
			<ConfirmationTypography variant="m-400">
				{confirmationMessage}
			</ConfirmationTypography>
		</PopUp>
	);
}
