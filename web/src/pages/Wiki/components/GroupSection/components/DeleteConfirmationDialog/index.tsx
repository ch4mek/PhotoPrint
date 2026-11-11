import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import {
	confirmationMessage,
	confirmationPostfix,
} from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { zIndexes } from '@/constants/zIndexes';
import { DeleteConfirmationDialogProps } from '@/pages/Wiki/components/GroupSection/components/DeleteConfirmationDialog/types';

export function DeleteConfirmationDialog({
	handleCloseDialog,
	showLoader,
	segmentType,
	handleDeleteSegment,
	title,
}: DeleteConfirmationDialogProps) {
	return (
		<PopUp
			title={`${segmentType} deleting`}
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
