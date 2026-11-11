import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { DeleteConfirmationDialogProps } from '@/components/DeleteConfirmationDialog/types';
import { confirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { getDeletingPopUpTitle } from '@/constants/activityStatus';
import { ITEM_WILL_DELETED_POSTFIX } from '@/constants/notifications';
import { zIndexes } from '@/constants/zIndexes';

export function DeleteConfirmationDialog({
	title,
	customTitle,
	handleClose,
	handleSubmit,
	isLoading,
	content,
	item,
	showConfirmationMessage = true,
}: DeleteConfirmationDialogProps) {
	return (
		<PopUp
			title={customTitle ? title : getDeletingPopUpTitle(title)}
			onClose={handleClose}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: { onClick: handleSubmit },
				negativeControl: { onClick: handleClose },
			}}
			zIndex={zIndexes.formPopup}
			showLoader={isLoading}
		>
			{!content ? (
				<Typography variant="m-400">
					{title} <Typography variant="m-600">{item}</Typography>{' '}
					{ITEM_WILL_DELETED_POSTFIX}
				</Typography>
			) : (
				content
			)}
			{showConfirmationMessage && (
				<ConfirmationTypography variant="m-400">
					{confirmationMessage}
				</ConfirmationTypography>
			)}
		</PopUp>
	);
}
