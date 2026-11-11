import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { zIndexes } from '@/constants/zIndexes';

import {
	CLEAR_ALL_DIOLOG_BODY,
	CLEAR_ALL_DIOLOG_HEADER,
	CLEAR_ALL_DIOLOG_TITLE,
} from './constants';
import { ApproveClearAllDialogProps } from './types';

export function ApproveClearAllDialog({
	onClose,
	onApprove,
}: ApproveClearAllDialogProps) {
	return (
		<PopUp
			title={CLEAR_ALL_DIOLOG_TITLE}
			onClose={onClose}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: { onClick: onApprove },
				negativeControl: { onClick: onClose },
			}}
			zIndex={zIndexes.notification}
		>
			<Typography variant="m-400">{CLEAR_ALL_DIOLOG_HEADER}</Typography>
			<ConfirmationTypography variant="m-400">
				{CLEAR_ALL_DIOLOG_BODY}
			</ConfirmationTypography>
		</PopUp>
	);
}
