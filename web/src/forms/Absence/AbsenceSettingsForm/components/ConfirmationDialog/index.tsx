import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import {
	ADJUST_ABSENCE_POPUP_CONTENT,
	ADJUST_ABSENCE_POPUP_TITLE,
} from './constants';
import { ConfirmationDialogProps } from './types';

export function ConfirmationDialog({
	handleClose,
	handleSubmit,
}: ConfirmationDialogProps) {
	return (
		<PopUp
			title={ADJUST_ABSENCE_POPUP_TITLE}
			showCloseButton={false}
			onClose={handleClose}
			controls={{
				positiveControl: { onClick: handleSubmit },
				negativeControl: { onClick: handleClose },
			}}
		>
			<Typography variant="m-400">{ADJUST_ABSENCE_POPUP_CONTENT}</Typography>
		</PopUp>
	);
}
