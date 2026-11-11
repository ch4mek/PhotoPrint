import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	MICROFRONTEND_MESSAGES,
	NotificationType,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { createToast } from '@/helpers/createToast';
import { useResize } from '@/hooks/useResize';
import { useGetMicrofrontends } from '@/queries/microfrontends/useGetMicrofrontends';
import { useUpdateMicrofrontends } from '@/queries/microfrontends/useUpdateMicrofrontends';

import { MicrofrontendsFormContent } from './components/MicrofrontendsFormContent';
import {
	FORM_NAME,
	initialFormValues,
	MICROFRONTEND_FIELD_TITLE,
} from './constants';
import { microfrontendsValidationSchema } from './schema';
import { MicrofrontendsFormProps, MicrofrontendsFormValues } from './types';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';
import {
	addMicrofrontendToConfig,
	updateMicrofrontendInConfig,
} from './utils/microfrontendConfigUtils';

export function MicrofrontendsForm({
	id: microfrontendId,
	handleClose,
	data,
	handleSubmit,
}: MicrofrontendsFormProps) {
	const isEditMode = !!microfrontendId;

	const toast = useToast();
	const theme = useTheme();
	const [, windowHeight] = useResize();

	const { data: currentConfig } = useGetMicrofrontends();

	const { mutateAsync: updateMicrofrontends, isPending } =
		useUpdateMicrofrontends();

	const initialFormData: MicrofrontendsFormValues = useMemo(() => {
		if (isEditMode && data) {
			return {
				...data,
				routes: data.routes || [],
				claims: data.claims || [],
				remoteEntries: data.remoteEntries || '',
			};
		}

		return initialFormValues;
	}, [isEditMode, data]);

	const handleFormSubmit = useCallback(
		async (
			values: MicrofrontendsFormValues,
			{ setSubmitting }: FormikHelpers<MicrofrontendsFormValues>
		) => {
			setSubmitting(true);

			const existingMicrofrontends = currentConfig?.microfrontends || [];

			const updatedMicrofrontends = isEditMode
				? updateMicrofrontendInConfig(
						existingMicrofrontends,
						microfrontendId as string,
						values
				  )
				: addMicrofrontendToConfig(existingMicrofrontends, values);

			await updateMicrofrontends(
				{ microfrontends: updatedMicrofrontends },
				{
					onSuccess: () => {
						toast.open(
							createToast(
								NotificationType.SUCCESS,
								isEditMode
									? MICROFRONTEND_MESSAGES.UPDATED_SUCCESS
									: MICROFRONTEND_MESSAGES.CREATED_SUCCESS
							)
						);
						handleClose();

						if (handleSubmit) {
							handleSubmit(values);
						}
					},
					onError: () => {
						toast.open(
							createToast(
								NotificationType.ERROR,
								isEditMode
									? MICROFRONTEND_MESSAGES.UPDATE_ERROR
									: MICROFRONTEND_MESSAGES.CREATE_ERROR
							)
						);
					},
				}
			);
			setSubmitting(false);
		},
		[
			microfrontendId,
			isEditMode,
			currentConfig,
			toast,
			updateMicrofrontends,
			handleClose,
			handleSubmit,
		]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData}
			validationSchema={microfrontendsValidationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			name={FORM_NAME}
			id={microfrontendId}
			validateOnBlur={false}
		>
			{({ submitForm, isSubmitting, handleOutsideClose }) => (
				<PopUp
					type="form"
					stylesConfig={getPopUpStylesConfig(theme, windowHeight)}
					showLoader={isSubmitting || isPending}
					title={
						isEditMode
							? getEditingPopUpTitle(MICROFRONTEND_FIELD_TITLE)
							: getAddingPopUpTitle(MICROFRONTEND_FIELD_TITLE)
					}
					onClose={handleClose}
					onShadowClick={handleOutsideClose}
					controls={{
						positiveControl: { onClick: submitForm },
						negativeControl: { onClick: handleClose },
					}}
				>
					<MicrofrontendsFormContent />
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
