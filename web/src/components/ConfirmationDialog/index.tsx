import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { ConfirmationDialogWrapper } from './styles';
import { ConfirmationDialogProps } from './types';

export function ConfirmationDialog({
	handleClose,
	handleSubmit,
	title,
	contentText,
	confirmationMessage,
	isLoading,
}: ConfirmationDialogProps) {
	return (
		<PopUp
			title={title}
			showCloseButton={false}
			onClose={handleClose}
			controls={{
				positiveControl: { onClick: handleSubmit },
				negativeControl: { onClick: handleClose },
			}}
			showLoader={isLoading}
		>
			<ConfirmationDialogWrapper>
				{contentText && <Typography variant="m-400">{contentText}</Typography>}
				<Typography variant="m-400">{confirmationMessage}</Typography>
			</ConfirmationDialogWrapper>
		</PopUp>
	);
}
