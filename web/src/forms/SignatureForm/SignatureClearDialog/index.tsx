import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useQueryClient } from '@tanstack/react-query';

import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { HAS_BEEN_CLEARED, NotificationType } from '@/constants/notifications';
import { SignatureQueriesKeys } from '@/constants/queriesKeys';
import { useResetSignature } from '@/queries/signature';
import type { SignatureResponse } from '@/queries/types';

import {
	ATTENTION_MESSAGE,
	CLEAR_SIGNATURE_MESSAGE,
	POPUP_TITLE,
} from './constants';
import type { SignatureDialogProps } from './types';

export function SignatureClearDialog({
	isDialogOpen,
	setIsDialogOpen,
	resetForm,
	showToast,
	initValues,
	validateForm,
	setSubmittedValues,
}: SignatureDialogProps) {
	const queryClient = useQueryClient();

	const onSuccess = (values: SignatureResponse) => {
		resetForm({
			values: {
				...initValues,
			},
		});
		showToast(HAS_BEEN_CLEARED, NotificationType.SUCCESS);
		validateForm({ fullName: '' });
		setIsDialogOpen(false);
		setSubmittedValues({ ...initValues });
		queryClient.setQueryData([SignatureQueriesKeys.signature], () => ({
			...values,
		}));
	};

	const { mutate } = useResetSignature(showToast, { onSuccess });

	const handleCloseDialog = () => {
		setIsDialogOpen(false);
	};

	return isDialogOpen ? (
		<PopUp
			onClose={handleCloseDialog}
			title={POPUP_TITLE}
			controls={{
				positiveControl: { onClick: mutate },
				negativeControl: { onClick: handleCloseDialog },
			}}
			type="approve"
		>
			<Typography variant="m-400">{CLEAR_SIGNATURE_MESSAGE}</Typography>
			<ConfirmationTypography variant="m-600">
				{ATTENTION_MESSAGE}
			</ConfirmationTypography>
		</PopUp>
	) : null;
}
