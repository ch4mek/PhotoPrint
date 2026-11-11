import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useQueryClient } from '@tanstack/react-query';

import { merchKitActionConfig } from '../../constants';

import { NotificationType } from '@/constants/notifications';
import { merchQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useUpdateMerchKitStatus } from '@/queries/merchKits/useUpdateMerchKitStatus';

import { MerchKitStatusPopUpProps } from './type';

export function MerchKitStatusPopUp({
	kitId,
	action,
	handleCloseForm,
	handleCloseModal,
}: MerchKitStatusPopUpProps) {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { popUpTitle, confirmMessage, successMessage, targetStatus } =
		merchKitActionConfig[action];

	const { mutate: updateMerchKitStatus, isPending } = useUpdateMerchKitStatus({
		onSuccess: () => {
			toast.open(createToast(NotificationType.SUCCESS, successMessage));

			handleCloseModal();
			handleCloseForm();
			queryClient.invalidateQueries({
				queryKey: [merchQueriesKeys.getMerchKitsData],
			});
		},
		onError: (error) => {
			createOnErrorHandler(toast)(error);
			handleCloseModal();
		},
	});

	const handleConfirm = () => {
		if (isPending) {
			return;
		}

		updateMerchKitStatus({ id: kitId, status: targetStatus });
	};

	return (
		<PopUp
			title={popUpTitle}
			onClose={handleCloseModal}
			type="approve"
			showCloseButton={false}
			showLoader={isPending}
			controls={{
				negativeControl: { onClick: handleCloseModal },
				positiveControl: { onClick: handleConfirm },
			}}
		>
			<Typography variant="m-400">{confirmMessage}</Typography>
		</PopUp>
	);
}
