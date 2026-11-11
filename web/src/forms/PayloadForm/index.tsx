import { useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';

import {
	NotificationType,
	PAYLOAD_DETAILS_UPDATED,
} from '@/constants/notifications';
import { payloadQueriesKeys } from '@/constants/queriesKeys';
import { usePayloadContext } from '@/context/PayloadContext';
import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { usePayloadFormMutations } from '@/forms/PayloadForm/hooks/usePayloadFormMutations';
import { payloadValidationSchema } from '@/forms/PayloadForm/schema';
import { createToast } from '@/helpers/createToast';
import { removeExtraSpaces } from '@/helpers/removeExtraSpaces';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { getFullName } from '@/helpers/utils';
import { FormUserAutoselectOption } from '@/types/forms';

import { PayloadFormContent } from './components/PayloadFormContent';
import { PayloadFormTabs, POPUP_TITLE } from './constants';
import { PayloadFormProps, PayloadFormValues } from './types';

export function PayloadForm({ handleClose, id }: PayloadFormProps) {
	const [activeTab, setActiveTab] = useState(PayloadFormTabs.Details);
	const toast = useToast();
	const queryClient = useQueryClient();

	const { year } = usePayloadContext();

	const { isLoaderVisible, payloadData, mutateUpdateEmployeePayload } =
		usePayloadFormMutations({
			id,
			year,
		});

	const userPayload = payloadData?.users?.[0];

	const userData = {
		...userPayload,
		firstName: userPayload?.firstName || '',
		lastName: userPayload?.lastName || '',
		comment: userPayload?.comments || '',
		plan: userPayload?.plan || '',
		needProject: userPayload?.needProject || false,
	};

	const userFullName = getFullName(userData.firstName, userData.lastName);

	const onActiveTabChange = (newTab: PayloadFormTabs) => {
		setActiveTab(newTab);
	};

	const invalidateDataAfterProjectAccessUpdate = () => {
		queryClient.invalidateQueries({
			queryKey: [payloadQueriesKeys.getPayloadData],
		});
		queryClient.invalidateQueries({
			queryKey: [payloadQueriesKeys.getTotalPayloadData],
		});
	};

	const handleSubmit = async (values: PayloadFormValues) => {
		const castValues = removeExtraSpaces(values, payloadValidationSchema);

		await mutateUpdateEmployeePayload(
			castValues as unknown as PayloadFormValues,
			{
				onSuccess: () => {
					toast.open(
						createToast(NotificationType.SUCCESS, PAYLOAD_DETAILS_UPDATED)
					);

					invalidateDataAfterProjectAccessUpdate();
				},
				onError: createOnErrorHandler(toast),
				onSettled: () => handleClose(),
			}
		);
	};

	const user: FormUserAutoselectOption = {
		id,
		fullName: `${userFullName}`,
	};

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={userData}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validationSchema={payloadValidationSchema}
			validateOnBlur={false}
			name={POPUP_TITLE}
			id={id}
		>
			{({ submitForm, handleOutsideClose }) => (
				<PayloadFormContent
					{...{
						activeTab,
						isLoaderVisible,
						user,
						onActiveTabChange,
						handleClose,
						submitForm,
						handleOutsideClose,
					}}
				/>
			)}
		</FormikWithBackup>
	);
}
