import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';

import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { REQUEST_FOR_LABEL } from '@/constants/absence';
import {
	HAS_BEEN_SUCCESSFULLY_MESSAGE,
	NotificationType,
} from '@/constants/notifications';
import {
	absencesQueriesKeys,
	dashboardQueriesKeys,
	homeQueriesKeys,
} from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { getFullName } from '@/helpers/formatName';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useManageRequest } from '@/queries/absences/useManageRequest';

import { messageLabelByType } from './constants';
import { RequestManageFormProps } from './types';

export function RequestManageForm({
	type,
	item,
	handleClose,
}: RequestManageFormProps) {
	const { requestName, id, createdForUser } = item;
	const { firstName, lastName } = createdForUser;
	const toast = useToast();
	const queryClient = useQueryClient();
	const { t } = useTranslation('common');

	const { mutateAsync: manageRequest, isPending } = useManageRequest();

	const handleManageRequest = () => {
		manageRequest(
			{
				id,
				type,
			},
			{
				onSuccess: () => {
					toast.open(
						createToast(
							NotificationType.SUCCESS,
							`${requestName} ${REQUEST_FOR_LABEL} ${getFullName(
								firstName,
								lastName
							)} ${HAS_BEEN_SUCCESSFULLY_MESSAGE} ${
								messageLabelByType[type].toast
							}`
						)
					);

					queryClient.invalidateQueries({
						queryKey: [absencesQueriesKeys.getUserAbsencesRequest],
					});
					queryClient.invalidateQueries({
						queryKey: [absencesQueriesKeys.getUserTransactionsHistory],
					});
					queryClient.invalidateQueries({
						queryKey: [absencesQueriesKeys.getUserAbsences],
					});
					queryClient.invalidateQueries({
						queryKey: [absencesQueriesKeys.getRequestsAbsences],
					});
					queryClient.invalidateQueries({
						queryKey: [absencesQueriesKeys.getAbsencesRequestById],
					});
					queryClient.invalidateQueries({
						queryKey: [absencesQueriesKeys.getInquiriesCalendar],
					});
					queryClient.invalidateQueries({
						queryKey: [homeQueriesKeys.getUserUpcomingEvents],
					});
					queryClient.invalidateQueries({
						queryKey: [dashboardQueriesKeys.getRequestsWidget],
					});

					handleClose();
				},
				onError: createOnErrorHandler(toast),
			}
		);
	};

	return (
		<PopUp
			title={`${requestName} ${t(`absence.${messageLabelByType[type].title}`)}`}
			type="approve"
			showCloseButton={false}
			onClose={handleClose}
			controls={{
				positiveControl: {
					onClick: handleManageRequest,
					text: t('controlButtons.yes'),
				},
				negativeControl: {
					onClick: handleClose,
					text: t('controlButtons.no'),
				},
			}}
			showLoader={isPending}
		>
			<Typography variant="m-400">
				<Typography variant="m-600">{requestName}</Typography> {t('requestFor')}{' '}
				<Typography variant="m-600">
					{getFullName(firstName, lastName)}
				</Typography>{' '}
				{t('willBe')} {t(`absence.${messageLabelByType[type].toast}`)}.
			</Typography>
			<ConfirmationTypography variant="m-400">
				{t('messages.confirmationMessage')}
			</ConfirmationTypography>
		</PopUp>
	);
}
