import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useCallback } from 'react';

import { confirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { getDeletingPopUpTitle } from '@/constants/activityStatus';
import { DEVELOPMENT_PLAN_TITLE } from '@/constants/developmentPlan';
import { ITEM_WILL_DELETED_POSTFIX } from '@/constants/notifications';
import { DevelopmentPlanDeleteConfirmationProps } from '@/forms/TeamboardForm/DevelopmentPlanDeleteConfirmation/types';
import { getDevelopmentPlanConformationMessage } from '@/forms/TeamboardForm/DevelopmentPlanDeleteConfirmation/utils/getDevelopmentPlanConformationMessage';
import { formatDateString } from '@/helpers/dateToISOString';
import dayjs from '@/lib/dayjs';

export function DevelopmentPlanDeleteConfirmation({
	id,
	handleClose,
	selectedDate,
	onSubmit,
	isLoading,
}: DevelopmentPlanDeleteConfirmationProps) {
	const handleSubmit = useCallback(async () => {
		if (id) {
			await onSubmit(id);
		}
	}, [id]);

	const convertedSelectedDate = formatDateString(dayjs(selectedDate), '');

	return (
		<PopUp
			title={getDeletingPopUpTitle(DEVELOPMENT_PLAN_TITLE)}
			type="approve"
			showCloseButton={false}
			onClose={handleClose}
			controls={{
				positiveControl: {
					onClick: handleSubmit,
				},
				negativeControl: {
					onClick: handleClose,
				},
			}}
			showLoader={isLoading}
		>
			<Typography variant="m-400">
				{getDevelopmentPlanConformationMessage(convertedSelectedDate)}{' '}
				{ITEM_WILL_DELETED_POSTFIX}
			</Typography>
			<ConfirmationTypography variant="m-400">
				{confirmationMessage}
			</ConfirmationTypography>
		</PopUp>
	);
}
